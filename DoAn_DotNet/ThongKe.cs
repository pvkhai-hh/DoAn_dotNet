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
using System.Windows.Forms.DataVisualization.Charting;
using static DoAn_DotNet.ChuoiKetNoi;

namespace DoAn_DotNet
{
    public partial class ThongKe : Form
    {
        public ThongKe()
        {
            InitializeComponent();
        }
        ChuoiKetNoi pro = new ChuoiKetNoi();

        private void ThongKe_Load(object sender, EventArgs e)
        {
            // Mặc định chọn từ đầu tháng đến hiện tại
            DateTime now = DateTime.Now;
            dtpTuNgay.Value = new DateTime(now.Year, now.Month, 1);
            dtpDenNgay.Value = now;

            // Cấu hình biểu đồ ban đầu
            CauHinhBieuDo();
        }
        private void CauHinhBieuDo()
        {
            // Xóa dữ liệu mẫu
            chartDoanhThu.Series.Clear();

            // Tạo Series mới (Cột)
            Series series = new Series("Doanh Thu");
            series.ChartType = SeriesChartType.Column; // Loại biểu đồ cột
            series.IsValueShownAsLabel = true; // Hiện số tiền trên đầu cột
            series.Color = Color.SteelBlue; // Màu cột

            chartDoanhThu.Series.Add(series);
            chartDoanhThu.ChartAreas[0].AxisX.Title = "Tên Sân";
            chartDoanhThu.ChartAreas[0].AxisY.Title = "Số Tiền (VNĐ)";
            chartDoanhThu.Titles.Add("BIỂU ĐỒ DOANH THU THEO SÂN");
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            LayDuLieuThongKe();
        }
        private void LayDuLieuThongKe()
        {
            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_ThongKeDoanhThu", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Xử lý ngày: Lấy trọn vẹn từ giây đầu tiên của TuNgay đến giây cuối cùng của DenNgay
                    DateTime tuNgay = dtpTuNgay.Value.Date;
                    // Ví dụ: Chọn 20/11 -> Lấy đến 23:59:59 của ngày 20/11
                    DateTime denNgay = dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1);

                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Đổ dữ liệu
                    dgvDoanhThu.DataSource = dt;

                    // [Quan Trọng] Tự động giãn cột cho đẹp (Thêm dòng này nếu chưa có)
                    dgvDoanhThu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Format tiền tệ
                    if (dgvDoanhThu.Columns["Thành Tiền"] != null)
                        dgvDoanhThu.Columns["Thành Tiền"].DefaultCellStyle.Format = "#,### VNĐ";

                    // Format ngày giờ (Giờ này bây giờ là Giờ Kết Thúc lấy từ DAT_SAN)
                    if (dgvDoanhThu.Columns["Ngày Thanh Toán"] != null)
                        dgvDoanhThu.Columns["Ngày Thanh Toán"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                    // Tính tổng tiền
                    decimal tongTien = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        // Kiểm tra null để tránh lỗi crash chương trình
                        if (row["Thành Tiền"] != DBNull.Value)
                        {
                            tongTien += Convert.ToDecimal(row["Thành Tiền"]);
                        }
                    }
                    lblTongDoanhThu.Text = $"Tổng doanh thu: {tongTien:#,###} VNĐ";

                    // Vẽ biểu đồ
                    VeBieuDo(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thống kê: " + ex.Message);
                }
            }
        }
        private void VeBieuDo(DataTable dt)
        {
            // 1. Xóa sạch các Series cũ đi để tránh lỗi trùng hoặc thiếu
            chartDoanhThu.Series.Clear();

            // 2. Tạo mới lại Series "Doanh Thu" ngay tại đây cho chắc ăn
            Series series = new Series("Doanh Thu");
            series.ChartType = SeriesChartType.Column; // Loại biểu đồ cột
            series.Color = Color.SteelBlue;
            series.IsValueShownAsLabel = true; // Hiện số tiền trên đầu cột

            // Thêm nó vào Chart
            chartDoanhThu.Series.Add(series);

            // --- PHẦN TÍNH TOÁN CŨ ---

            // Nhóm dữ liệu: Cộng dồn tiền của từng sân
            var thongKeSan = new System.Collections.Generic.Dictionary<string, decimal>();

            foreach (DataRow row in dt.Rows)
            {
                string tenSan = row["Tên Sân"].ToString();
                decimal tien = Convert.ToDecimal(row["Thành Tiền"]);

                if (thongKeSan.ContainsKey(tenSan))
                    thongKeSan[tenSan] += tien;
                else
                    thongKeSan.Add(tenSan, tien);
            }

            // Đổ dữ liệu từ Dictionary vào Chart
            foreach (var item in thongKeSan)
            {
                // X = Tên sân, Y = Tổng tiền sân đó
                chartDoanhThu.Series["Doanh Thu"].Points.AddXY(item.Key, item.Value);
            }

            // Đặt tiêu đề trục (nếu chưa có)
            chartDoanhThu.ChartAreas[0].AxisX.Title = "Tên Sân";
            chartDoanhThu.ChartAreas[0].AxisY.Title = "Số Tiền (VNĐ)";
        }

        private void ThongKe_Load_1(object sender, EventArgs e)
        {
                
        }
    }
}
