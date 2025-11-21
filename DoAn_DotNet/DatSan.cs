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

namespace DoAn_DotNet
{
    public partial class DatSan : Form
    {
        public DatSan()
        {
            InitializeComponent();
        }
        string strKetNoi = @"Data Source=ADMIN\SQLEXPRESS;Initial Catalog=QLSANBONG;Integrated Security=True";

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
            using (SqlConnection conn = new SqlConnection(strKetNoi))
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
            if (txtTenKhach.Text == "" || txtSDT.Text == "")
            {
                MessageBox.Show("Vui lòng nhập Tên khách và Số điện thoại!", "Thiếu thông tin");
                return;
            }

            // Đặt mã sân tự động: D2511200830 -> Đặt ngày 20/11/25 lúc 08:30
            string maDat = "D" + DateTime.Now.ToString("yyMMddHHmmss");

            //Lưu vào CSDL
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO DAT_SAN (MaDat, TenKhach, SoDienThoai, MaSan, NgayDat, GioBatDau, GioKetThuc, TrangThai) 
                                   VALUES (@MaDat, @TenKhach, @SDT, @MaSan, @NgayDat, @GioBatDau, @GioKetThuc, N'Đã đặt')";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // Các tham số lấy từ ô nhập liệu
                    cmd.Parameters.AddWithValue("@MaDat", maDat);
                    cmd.Parameters.AddWithValue("@TenKhach", txtTenKhach.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());

                    // Các tham số lấy từ biến ngầm (đã lưu ở trên)
                    cmd.Parameters.AddWithValue("@MaSan", _maSan);
                    cmd.Parameters.AddWithValue("@NgayDat", _ngayDa);
                    cmd.Parameters.AddWithValue("@GioBatDau", _gioBatDau);
                    cmd.Parameters.AddWithValue("@GioKetThuc", _gioKetThuc);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Đặt sân thành công!", "Thông báo");
                    this.Close(); // Đóng form để quay về màn hình chính
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lưu đặt sân: " + ex.Message);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult traloi;
            traloi = MessageBox.Show("Bạn không muốn đặt sân nữa?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question); ;
            if (traloi == DialogResult.OK)
                Application.Exit();

        }
    }
}
