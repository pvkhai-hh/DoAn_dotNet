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
                if (txtTaiKhoan.Text == "admin" && txtMatKhau.Text == "123")
                {
                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
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

        private void Login_Load(object sender, EventArgs e)
        {
                
        }
    }
}
