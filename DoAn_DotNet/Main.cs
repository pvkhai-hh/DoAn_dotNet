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
        string strKetNoi = @"Data Source=ADMIN\SQLEXPRESS;Initial Catalog=QLSANBONG;Integrated Security=True";

        private void Main_Load(object sender, EventArgs e)
        {
            cboThoiGian.Items.Clear();
            cboThoiGian.Items.Add("8:00 - 9:00");
            cboThoiGian.Items.Add("9:00 - 10:00");
            cboThoiGian.Items.Add("10:00 - 11:00");
            cboThoiGian.Items.Add("11:00 - 12:00");
            cboThoiGian.Items.Add("12:00 - 13:00");
            cboThoiGian.Items.Add("13:00 - 14:00");
            cboThoiGian.Items.Add("14:00 - 15:00");
            cboThoiGian.Items.Add("15:00 - 16:00");
            cboThoiGian.Items.Add("16:00 - 17:00");
            cboThoiGian.Items.Add("17:00 - 18:00");
            cboThoiGian.Items.Add("18:00 - 19:00");
            cboThoiGian.Items.Add("19:00 - 20:00");
            cboThoiGian.Items.Add("20:00 - 21:00");

            // Mặc định chọn khung đầu tiên
            cboThoiGian.SelectedIndex = 0;

            LoadTrangThaiSan();
        }
        private void LoadTrangThaiSan(string tenSanLoc = "")
        {
            // --- PHẦN 1: CÀI ĐẶT GIAO DIỆN & ẨN HIỆN ---
            Label[] labels = { lblSanA, lblSanB, lblSanC, lblSanD, lblSanE };
            var mapSan = new Dictionary<string, Label>
    {
        { "Sân A", lblSanA }, { "Sân B", lblSanB },
        { "Sân C", lblSanC }, { "Sân D", lblSanD }, { "Sân E", lblSanE }
    };

            foreach (var kvp in mapSan)
            {
                Label lbl = kvp.Value;
                string tenSanTrongList = kvp.Key;

                lbl.Text = "";
                lbl.BackColor = Color.LightGray;
                lbl.TextAlign = ContentAlignment.TopLeft; //căn trái
                lbl.Padding = new Padding(5, 10, 0, 0); // cách lề

                lbl.Tag = null;

                // Logic ẩn hiện khi tìm kiếm
                if (string.IsNullOrEmpty(tenSanLoc))
                {
                    lbl.Visible = true;
                }
                else
                {
                    lbl.Visible = (tenSanTrongList == tenSanLoc);
                }
            }

            // --- PHẦN 2: XỬ LÝ THỜI GIAN ĐỂ LỌC ---
            string gioTu = "";
            string gioDen = "";
            bool coLocGio = false;

            // Cắt chuỗi dựa vào dấu gạch ngang
            if (!string.IsNullOrEmpty(cboThoiGian.Text) && cboThoiGian.Text.Contains("-"))
            {
                string[] parts = cboThoiGian.Text.Split('-');
                if (parts.Length == 2)
                {
                    // Vì bạn đã nhập chuẩn "08:00" nên chỉ cần Trim() cắt khoảng trắng thừa là xong
                    gioTu = parts[0].Trim();
                    gioDen = parts[1].Trim();

                    coLocGio = true;
                }
            }


            // --- PHẦN 3: LẤY DỮ LIỆU SQL ---
            string ngayChon = dtpChonNgay.Value.ToString("yyyy-MM-dd");

            // [MỚI] Lấy chuỗi ngày để hiển thị (VD: 20/11/2025)
            string hienThiNgay = dtpChonNgay.Value.ToString("dd/MM/yyyy");

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT s.TenSan, s.LoaiSan, d.MaDat, d.TenKhach, d.SoDienThoai, d.TrangThai, d.GioBatDau, d.GioKetThuc
                           FROM SAN s
                           LEFT JOIN DAT_SAN d ON s.MaSan = d.MaSan AND d.NgayDat = @NgayDat";

                    if (coLocGio)
                    {
                        sql += " AND (d.GioBatDau < @DenGio AND d.GioKetThuc > @TuGio) ";
                    }

                    if (!string.IsNullOrEmpty(tenSanLoc))
                    {
                        sql += " WHERE s.TenSan = @TenSanLoc ";
                    }

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@NgayDat", ngayChon);

                    if (coLocGio)
                    {
                        cmd.Parameters.AddWithValue("@TuGio", gioTu);
                        cmd.Parameters.AddWithValue("@DenGio", gioDen);
                    }

                    if (!string.IsNullOrEmpty(tenSanLoc))
                    {
                        cmd.Parameters.AddWithValue("@TenSanLoc", tenSanLoc);
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string tenSan = reader["TenSan"].ToString();
                        string loaiSan = reader["LoaiSan"].ToString();

                        if (!mapSan.ContainsKey(tenSan)) continue;
                        Label lbl = mapSan[tenSan];

                        // Dữ liệu mặc định
                        string maDat = "---";
                        string tenKhach = "---";
                        string sdt = "---";
                        string trangThai = "Trống";
                        string hienThiGio = "---"; // [MỚI] Biến chứa giờ

                        if (!reader.IsDBNull(reader.GetOrdinal("MaDat")))
                        {
                            maDat = reader["MaDat"].ToString();
                            tenKhach = reader["TenKhach"].ToString();
                            sdt = reader["SoDienThoai"].ToString();
                            trangThai = reader["TrangThai"].ToString();

                            TimeSpan tsDa = (TimeSpan)reader["GioBatDau"];
                            TimeSpan tsKet = (TimeSpan)reader["GioKetThuc"];
                            hienThiGio = $"{tsDa.ToString(@"hh\:mm")} - {tsKet.ToString(@"hh\:mm")}";

                            if (trangThai == "Đang sử dụng") lbl.BackColor = Color.LightGreen;
                            else lbl.BackColor = Color.Gold;

                            //// Tô màu cam nếu đang tìm đúng mã này
                            //if (!string.IsNullOrEmpty(txtTraCuu.Text) && maDat == txtTraCuu.Text.Trim())
                            //    lbl.BackColor = Color.OrangeRed;
                            lbl.Tag = $"{maDat}|{tenSan}|{trangThai}"; // Lưu tạm thông tin vào Tag
                        }
                        else
                        {
                            lbl.BackColor = Color.LightGray;
                            lbl.Tag = $"|{tenSan}|Trống"; // Lưu tạm thông tin vào Tag
                        }

                        // --- SỬ DỤNG HÀM CĂN GIỮA TỰ ĐỘNG ---
                        string dong1 = CanGiuaTuDong($"{tenSan.ToUpper()}", lbl);
                        string dong2 = CanGiuaTuDong($"({loaiSan})", lbl);
                        string line = CanGiuaTuDong("_____________________", lbl);

                        // [MỚI] Nội dung căn trái (Đã thêm Ngày và Giờ)
                        // Dùng các khoảng trắng sau dấu : để căn thẳng hàng cho đẹp
                        string body = $"Mã đặt: {maDat}\n" +
                                      $"Ngày: {hienThiNgay}\n" +   // <-- Thêm dòng Ngày
                                      $"Giờ: {hienThiGio}\n" +    // <-- Thêm dòng Giờ
                                      $"Tên: {tenKhach}\n" +
                                      $"SĐT: {sdt}\n" +
                                      $"Trạng thái: {trangThai}";

                        lbl.Text = dong1 + "\n" + dong2 + "\n" + line + "\n\n" + body;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        // Hàm tự động thêm dấu cách vào trước chữ để đẩy nó ra giữa
        private string CanGiuaTuDong(string text, Label lbl)
        {
            //return new string(' ', soDauCach) + text;
            if (string.IsNullOrEmpty(text) || lbl == null) return "";

            // 1. Đo độ rộng của chữ gốc
            int doRongChu = TextRenderer.MeasureText(text, lbl.Font).Width;

            // 2. Tính khoảng trống cần thiết ở bên trái để chữ ra giữa
            // Công thức: (Chiều rộng khung - Chiều rộng chữ) / 2
            int doRongKhung = lbl.Width - lbl.Padding.Left - lbl.Padding.Right;
            int khoangTrongBenTrai = (doRongKhung - doRongChu) / 2;

            if (khoangTrongBenTrai <= 0) return text; // Chữ dài quá thì khỏi căn

            // 3. VÒNG LẶP CHÈN DẤU CÁCH (CHÍNH XÁC HƠN PHÉP CHIA)
            string dauCach = "";
            int doRongHienTai = 0;

            // Cứ thêm dấu cách vào cho đến khi nào độ rộng của đám dấu cách gần bằng khoảng trống bên trái
            while (doRongHienTai < khoangTrongBenTrai)
            {
                dauCach += " ";
                doRongHienTai = TextRenderer.MeasureText(dauCach, lbl.Font).Width;
            }

            // Trả về: Đám dấu cách + Chữ gốc
            return dauCach + text;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboThoiGian.Text))
            {
                MessageBox.Show("Vui lòng chọn khung thời gian!", "Thông báo");
                return;
            }
            LoadTrangThaiSan("");
        }

        private void btnTraCuu_Click(object sender, EventArgs e)
        {
            string maCanTim = txtTraCuu.Text.Trim();

            // Nếu rỗng -> Hiện lại tất cả
            if (string.IsNullOrEmpty(maCanTim))
            {
                LoadTrangThaiSan("");
                return;
            }

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    // Tìm Ngày và Tên sân của mã đó
                    string sql = @"SELECT d.NgayDat, s.TenSan 
                           FROM DAT_SAN d 
                           JOIN SAN s ON d.MaSan = s.MaSan 
                           WHERE d.MaDat = @MaDat";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaDat", maCanTim);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        DateTime ngay = (DateTime)reader["NgayDat"];
                        string tenSan = reader["TenSan"].ToString();

                        // 1. Đổi ngày
                        dtpChonNgay.Value = ngay;

                        // 2. Xóa lọc giờ
                        cboThoiGian.SelectedIndex = -1; 
                        cboThoiGian.Text = "";

                        // 3. GỌI HÀM LOAD ĐỂ CHỈ HIỆN ĐÚNG SÂN ĐÓ
                        LoadTrangThaiSan(tenSan);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy mã đơn này!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        // Hàm Giao Sân (Check-in)
        private void XuLyGiaoSan(string maDat, DateTime gioNhan)
        {
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    // Gọi Stored Procedure đã tạo ở Phần 1
                    SqlCommand cmd = new SqlCommand("sp_GiaoSan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaDat", maDat);
                    cmd.Parameters.AddWithValue("@ThoiGianGiao", gioNhan);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Đã giao sân! Khách nhận lúc: {gioNhan:HH:mm}");
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        // Hàm Trả Sân & Thanh Toán (Thường sẽ nằm trong frmThanhToan khi bấm nút 'Thanh Toán')
        // Nhưng nếu bạn muốn xử lý nhanh ở Main thì dùng hàm này:
        private void XuLyTraSan(string maDat, DateTime gioTra)
        {
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_TraSan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaDat", maDat);
                    cmd.Parameters.AddWithValue("@ThoiGianTra", gioTra);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Đã trả sân lúc: {gioTra:HH:mm}. Trạng thái chuyển về Hoàn thành.");
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
        private void lblSanA_Click(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl.Tag == null) return;

            string tagData = lbl.Tag.ToString();
            string[] parts = tagData.Split('|');

            string maDat = parts[0];
            string tenSan = parts[1];
            string trangThai = parts[2];

            // 1. SÂN TRỐNG -> ĐẶT
            if (trangThai == "Trống" || trangThai == "Hoàn thành")
            {
                DatSan f = new DatSan();
                f.SetThongTin(tenSan, cboThoiGian.Text, dtpChonNgay.Value);
                f.ShowDialog();
                LoadTrangThaiSan();
            }
            // 2. ĐÃ ĐẶT -> GIAO SÂN (NHẬP GIỜ NHẬN)
            else if (trangThai == "Đã đặt")
            {
                // Mở form chọn giờ nhận (Mặc định là giờ hiện tại)
                GiaoSan f = new GiaoSan("Xác nhận Giờ Nhận Sân", DateTime.Now);

                if (f.ShowDialog() == DialogResult.OK)
                {
                    DateTime gioNhanThucTe = f.ThoiGianChon;
                    XuLyGiaoSan(maDat, gioNhanThucTe); // Gọi hàm xử lý
                    LoadTrangThaiSan();
                }
            }
            // 3. ĐANG SỬ DỤNG -> TRẢ SÂN (NHẬP GIỜ TRẢ)
            else if (trangThai == "Đang sử dụng")
            {
                // Mở form chọn giờ trả
                GiaoSan f = new GiaoSan("Xác nhận Giờ Trả Sân", DateTime.Now);

                if (f.ShowDialog() == DialogResult.OK)
                {
                    DateTime gioTraThucTe = f.ThoiGianChon;

                    // Mở form Tính tiền và truyền giờ thực tế sang
                    ThanhToan fTT = new ThanhToan();
                    fTT.SetThongTinThanhToan(maDat, gioTraThucTe);
                    fTT.ShowDialog();

                    LoadTrangThaiSan();
                }
            }
        }
    }
}
