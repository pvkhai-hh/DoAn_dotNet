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
            // --- THÊM ĐOẠN NÀY ĐỂ ÉP KÍCH THƯỚC ---
            //this.AutoScaleMode = AutoScaleMode.None; // Tắt tự phóng to
            //this.AutoSize = false; // Tắt tự giãn
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
            // 1. Tạo mã hóa đơn ngẫu nhiên (hoặc theo quy tắc)
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

                        //// Xử lý thời gian
                        //TimeSpan batDau = (TimeSpan)reader["GioBatDau"];
                        //TimeSpan ketThuc = (TimeSpan)reader["GioKetThuc"];

                        //// Nếu chưa có giờ kết thúc (lỗi dữ liệu) thì lấy giờ hiện tại để tính tạm
                        //if (ketThuc == TimeSpan.Zero) ketThuc = DateTime.Now.TimeOfDay;

                        //double soGio = (ketThuc - batDau).TotalHours;
                        //// Làm tròn số giờ (VD: 1.5 giờ)
                        //soGio = Math.Round(soGio, 1);
                        //if (soGio < 1) soGio = 1; // Tối thiểu tính 1 giờ

                        //decimal thanhTien = (decimal)soGio * donGia;
                        //_tongTienCuoi = thanhTien;
                        double soGio = 1;

                        decimal thanhTien = donGia; // Vì nhân với 1 nên thành tiền = đơn giá luôn
                        _tongTienCuoi = thanhTien;

                        // --- ĐỔ DỮ LIỆU LÊN FORM ---
                        lblTenKhach.Text = "Tên KH: " + tenKhach;

                        // Format dòng chi tiết: Căn chỉnh bằng khoảng trắng hoặc Tab
                        // Lưu ý: Để căn thẳng tắp, nên dùng Font Monospace (Consolas) hoặc chia Label ra
                        // Ở đây mình dùng chuỗi format đơn giản:
                        string maSanNgan = tenSan.Replace("Sân ", "S"); // Rút gọn tên sân cho đẹp

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
                        // 1. Lưu vào bảng THANH_TOAN (Lịch sử)
                        string sqlTT = @"INSERT INTO THANH_TOAN (MaTT, MaDat, TenKhach, ThanhTien) 
                                         VALUES (@MaTT, @MaDat, @TenKhach, @ThanhTien)";

                        string maTT = "TT" + DateTime.Now.ToString("ddHHmmss");
                        // Tách tên khách từ label (bỏ chữ "Tên khách hàng: ")
                        string tenKhach = lblTenKhach.Text.Replace("Tên khách hàng: ", "");

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
