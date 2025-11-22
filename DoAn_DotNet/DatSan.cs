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
    public partial class DatSan : Form
    {
        public DatSan()
        {
            InitializeComponent();
        }
        public bool DaThucHien { get; set; } = false;

        ChuoiKetNoi pro = new ChuoiKetNoi();
        

        // Các biến ngầm (không nhìn thấy) những sẽ lưu SQL
        private string _tenSan;
        private DateTime _ngayDa;
        private string _gioBatDau;
        private string _gioKetThuc;
        private string _maSan; // mã sân -> tên sân

        public void SetThongTin(string tenSan, string khungGio, DateTime ngayDa)
        {
            // Lưu biến ngầm
            _tenSan = tenSan;
            _ngayDa = ngayDa;

            // Tách chuỗi "8:00 - 9:00" thành 2 biến riêng
            if (khungGio.Contains("-"))
            {
                string[] parts = khungGio.Split('-');
                _gioBatDau = parts[0].Trim();
                _gioKetThuc = parts[1].Trim();
            }

            // Tiêu đề label
            lblTieuDe.Text = $"ĐẶT: {tenSan.ToUpper()} | {khungGio} | {_ngayDa:dd/MM/yyyy}";

            // lấy tên sân từ mã sân
            LayMaSanTuTen(_tenSan);
        }
        private void LayMaSanTuTen(string tenSan)
        {
            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT MaSan FROM SAN WHERE TenSan = @TenSan";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@TenSan", tenSan);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        _maSan = result.ToString(); // Lưu được S001
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi lấy mã sân: " + ex.Message); }
            }
        }

        private void btnDatSan_Click(object sender, EventArgs e)
        {
            ThucHienDatSan(false); // false = Chỉ đặt
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult traloi;
            traloi = MessageBox.Show("Bạn không muốn đặt sân nữa?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question); 
            if (traloi == DialogResult.OK)
                return;

        }

        private void btnGiaoSan_Click(object sender, EventArgs e)
        {
            ThucHienDatSan(true); // true = Đặt xong giao luôn
        }
        private void ThucHienDatSan(bool giaoNgay)
        {
            if (txtTenKhach.Text == "" || txtSDT.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!", "Thiếu thông tin");
                return;
            }

            string maDat = "D" + DateTime.Now.ToString("ddHHmmss");
            string trangThai = giaoNgay ? "Đang sử dụng" : "Đã đặt"; // Nếu giao ngay thì set luôn trạng thái

            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
            {
                try
                {
                    conn.Open();
                    // B1: Insert vào DAT_SAN
                    string sql = @"INSERT INTO DAT_SAN (MaDat, TenKhach, SoDienThoai, MaSan, NgayDat, GioBatDau, GioKetThuc, TrangThai) 
                               VALUES (@MaDat, @TenKhach, @SDT, @MaSan, @NgayDat, @GioBatDau, @GioKetThuc, @TrangThai)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaDat", maDat);
                    cmd.Parameters.AddWithValue("@TenKhach", txtTenKhach.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaSan", _maSan);
                    cmd.Parameters.AddWithValue("@NgayDat", _ngayDa);
                    cmd.Parameters.AddWithValue("@GioBatDau", _gioBatDau);
                    cmd.Parameters.AddWithValue("@GioKetThuc", _gioKetThuc);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    cmd.ExecuteNonQuery();

                    // B2: Nếu là Giao Sân Luôn -> Insert thêm vào bảng GIAO_SAN
                    if (giaoNgay)
                    {
                        string sqlGiao = "INSERT INTO GIAO_SAN (MaGiao, MaDat, ThoiGianGiao) VALUES (@MaGiao, @MaDat, GETDATE())";
                        SqlCommand cmdGiao = new SqlCommand(sqlGiao, conn);
                        cmdGiao.Parameters.AddWithValue("@MaGiao", "G" + DateTime.Now.ToString("ddHHmmss"));
                        cmdGiao.Parameters.AddWithValue("@MaDat", maDat);
                        cmdGiao.ExecuteNonQuery();
                        MessageBox.Show("Đã đặt và giao sân thành công! Tính giờ ngay.", "Thành công");
                    }
                    else
                    {
                        MessageBox.Show("Đã đặt sân thành công!", "Thành công");
                    }

                    DaThucHien = true; // Đánh dấu đã làm xong
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
    }
}
