using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DoAn_DotNet.ChuoiKetNoi;

namespace DoAn_DotNet
{
    public partial class ThanhToan : Form
    {
        // Biến nhận dữ liệu từ Main
        private string _maDat;
        private decimal _tongTienCuoi = 0;

        public ThanhToan(string maDat)
        {
            _maDat = maDat;
            InitializeComponent();
            this.Size = new Size(400, 550); // Ép kích thước cố định (Rộng, Cao)
        }
       
        // Biến báo trạng thái cho Main biết
        public bool DaThanhToan { get; set; } = false;

        ChuoiKetNoi pro = new ChuoiKetNoi();

        private void ThanhToan_Load(object sender, EventArgs e)
        {
            HienThiHoaDon();
        }
        private void HienThiHoaDon()
        {
            string maHD = "HD" + DateTime.Now.ToString("ddHHmm");
            lblMaHD.Text = "Mã HD: " + maHD;
            lblNgay.Text = "Ngày: " + DateTime.Now.ToString("dd/MM/yyyy");
            lblMaDat.Text = "Mã đặt: " + _maDat;

            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
            {
                try
                {
                    conn.Open();
                    // Gọi Procedure lấy thông tin (đã tạo ở bước trước)
                    SqlCommand cmd = new SqlCommand("sp_LayThongTinThanhToan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaDat", _maDat);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string tenSan = reader["TenSan"].ToString();
                        string tenKhach = reader["TenKhach"].ToString();
                        decimal donGia = Convert.ToDecimal(reader["DonGia"]);

                        double soGio = 1;

                        decimal thanhTien = donGia;
                        _tongTienCuoi = thanhTien;

                        lblTenKhach.Text = "Tên KH: " + tenKhach;

                        string maSanNgan = tenSan.Replace("Sân ", "S"); // Lấy mã sân ngắn gọn

                        // Format tiền tệ VNĐ
                        string strDonGia = string.Format("{0:0,0}", donGia);
                        string strThanhTien = string.Format("{0:0,0}", thanhTien);

                        // Gán vào Label chi tiết (Dùng padRight để căn cột)
                        lblChiTiet.Text = $"{maSanNgan.PadRight(10)}         1               {strDonGia.PadRight(12)}           {strThanhTien}";

                        // Gán tổng
                        lblTongGio.Text = $"Tổng:     {soGio}";
                        lblTongTien.Text = $" {strThanhTien}";
                        lblKhachTT.Text = $"Khách thanh toán:                                    {strThanhTien}";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải hóa đơn: " + ex.Message);
                }
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            DialogResult traloi = MessageBox.Show("Xác nhận thanh toán và in hóa đơn?", "Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (traloi == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
                {
                    try
                    {
                        conn.Open();
                        string sqlTT = @"INSERT INTO THANH_TOAN (MaTT, MaDat, TenKhach, ThanhTien) 
                                         VALUES (@MaTT, @MaDat, @TenKhach, @ThanhTien)";

                        string maTT = "TT" + DateTime.Now.ToString("ddHHmmss");
                        string tenKhach = lblTenKhach.Text.Replace("Tên KH: ", "").Trim();

                        SqlCommand cmd = new SqlCommand(sqlTT, conn);
                        cmd.Parameters.AddWithValue("@MaTT", maTT);
                        cmd.Parameters.AddWithValue("@MaDat", _maDat);
                        cmd.Parameters.AddWithValue("@TenKhach", tenKhach);
                        cmd.Parameters.AddWithValue("@ThanhTien", _tongTienCuoi);
                        cmd.ExecuteNonQuery();

                        // 2. Trả sân (Set trạng thái về Hoàn thành/Trống)
                        SqlCommand cmdTra = new SqlCommand("sp_TraSan", conn);
                        cmdTra.CommandType = CommandType.StoredProcedure;
                        cmdTra.Parameters.AddWithValue("@MaDat", _maDat);
                        cmdTra.Parameters.AddWithValue("@ThoiGianTra", DateTime.Now);
                        cmdTra.ExecuteNonQuery();

                        MessageBox.Show("Thanh toán thành công!", "Thông báo");

                        DaThanhToan = true; // Báo cho Main biết
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi thanh toán: " + ex.Message);
                    }
                }
            }
        }
        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
