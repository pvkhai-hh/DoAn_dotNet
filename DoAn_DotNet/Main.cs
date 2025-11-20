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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        DataSet ds = new DataSet("dsQLSANBONG");
        SqlDataAdapter daSan;

        private void Main_Load(object sender, EventArgs e)
        {
            // Thêm vào combobox từ
            cboTu.Items.Add("8h00");
            cboTu.Items.Add("9h00");
            cboTu.Items.Add("10h00");
            cboTu.Items.Add("11h00");
            cboTu.Items.Add("12h00");
            cboTu.Items.Add("13h00");
            cboTu.Items.Add("14h00");
            cboTu.Items.Add("15h00");
            cboTu.Items.Add("16h00");
            cboTu.Items.Add("17h00");
            cboTu.Items.Add("18h00");
            cboTu.Items.Add("19h00");
            cboTu.Items.Add("20h00");
            cboTu.SelectedItem = "8h00";
            // Thêm vào combobox đến
            cboDen.Items.Add("9h00");
            cboDen.Items.Add("10h00");
            cboDen.Items.Add("11h00");
            cboDen.Items.Add("12h00");
            cboDen.Items.Add("13h00");
            cboDen.Items.Add("14h00");
            cboDen.Items.Add("15h00");
            cboDen.Items.Add("16h00");
            cboDen.Items.Add("17h00");
            cboDen.Items.Add("18h00");
            cboDen.Items.Add("19h00");
            cboDen.Items.Add("20h00");
            cboDen.Items.Add("21h00");
            cboDen.SelectedItem = "9h00";

            //Kết nối cơ sở dữ liệu
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=ADMIN\SQLEXPRESS;Initial Catalog=QLSANBONG;Integrated Security=True";

            // Tải dữ liệu vô combobox sân
            // Đổ dữ liệu vào combobox Sân
            string sQuerySan = @"SELECT * FROM SAN";
            daSan = new SqlDataAdapter(sQuerySan, conn);
            daSan.Fill(ds, "tblSan");

            cboChonSan.DataSource = ds.Tables["tblSan"];
            cboChonSan.DisplayMember = "TenSan";   // Hiển thị tên sân
            cboChonSan.ValueMember = "MaSan";      // Lưu giá trị thật là mã sân

        }
    }
}
