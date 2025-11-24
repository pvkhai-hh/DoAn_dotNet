using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAn_DotNet
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
           
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if ((txtTaiKhoan.Text == "") || (txtMatKhau.Text == ""))
            {
                MessageBox.Show("Bạn chưa nhập tên đăng nhập hoặc mật khẩu", "Thông báo");
                txtTaiKhoan.Focus();
            }
            else
            {
                if ((txtTaiKhoan.Text == "") || (txtMatKhau.Text == ""))
                {
                    MessageBox.Show("Bạn chưa nhập tên đăng nhập hoặc mật khẩu", "Thông báo");
                    txtTaiKhoan.Focus();
                }
                else
                {
                    // TÀI KHOẢN 1: QUẢN LÝ (Admin) - Được xem tất cả
                    if (txtTaiKhoan.Text == "1" && txtMatKhau.Text == "1")
                    {
                        MessageBox.Show("Đăng nhập admin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Truyền chữ "Admin" sang Main
                        Main main = new Main("Admin");

                        this.Hide();
                        main.ShowDialog();
                        this.Show(); // Hiện lại form login khi tắt Main (tùy chọn)
                    }
                    // TÀI KHOẢN 2: NHÂN VIÊN - Bị khóa thống kê
                    else if (txtTaiKhoan.Text == "2" && txtMatKhau.Text == "1")
                    {
                        MessageBox.Show("Đăng nhập nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Truyền chữ "NhanVien" sang Main
                        Main main = new Main("NhanVien");

                        this.Hide();
                        main.ShowDialog();
                        this.Show();
                    }
                    else
                    {
                        MessageBox.Show("Tài khoản hoặc mật khẩu không đúng!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTaiKhoan.Clear();
                        txtMatKhau.Clear();
                        txtTaiKhoan.Focus();
                    }
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            
            this.txtMatKhau.UseSystemPasswordChar = true;
        }
    }
}
