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
    public partial class Main : Form
    {
        private string _quyenHan;
        public Main(string quyenHan)
        {
            InitializeComponent();
            _quyenHan = quyenHan; // Lưu lại quyền hạn
        }


        ChuoiKetNoi pro = new ChuoiKetNoi();

        private void Main_Load(object sender, EventArgs e)
        {
            cboThoiGian.Items.Clear();
            cboThoiGian.Items.Add("08:00 - 09:00");
            cboThoiGian.Items.Add("09:00 - 10:00");
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
            //if (_quyenHan == "Admin")
            //{
            //    // Nếu là Admin thì cho phép dùng Thống kê
            //    MenuThongKe.Enabled = true;
            //    this.Text = "Quản lý sân bóng - QUẢN TRỊ VIÊN"; // Đổi tên form cho oách
            //}
            if (_quyenHan == "NhanVien")
            {
                // Nếu không phải Admin (là Nhân viên) thì khóa lại
                MenuThongKe.Enabled = false;
                // MenuThongKe.Visible = false; // Hoặc dùng lệnh này để ẩn luôn
                this.Text = "Quản lý sân bóng - NHÂN VIÊN";
            }
            LoadTrangThaiSan();
        }



        private void LoadTrangThaiSan(string tenSanLoc = "")
        {
            // --- PHẦN 1: CÀI ĐẶT GIAO DIỆN & ẨN HIỆN ---
            Label[] labels = { lblSanA, lblSanB, lblSanC, lblSanD, lblSanE };
            var mapSan = new Dictionary<string, Label>{
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

            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
            {
                try
                {
                    conn.Open();
                    //string sql = @"SELECT s.TenSan, s.LoaiSan, d.MaDat, d.TenKhach, d.SoDienThoai, d.TrangThai, d.GioBatDau, d.GioKetThuc
                    //       FROM SAN s
                    //       LEFT JOIN DAT_SAN d ON s.MaSan = d.MaSan AND d.NgayDat = @NgayDat";
                    // [QUAN TRỌNG] Thêm điều kiện: Chỉ lấy đơn Đã đặt hoặc Đang sử dụng. 
                    // Các đơn 'Hoàn thành' hoặc 'Hủy' sẽ bị lọc bỏ -> Trả về NULL -> Giao diện sẽ hiện ---
                    //string sql = @"SELECT s.TenSan, s.LoaiSan, d.MaDat, d.TenKhach, d.SoDienThoai, d.TrangThai, d.GioBatDau, d.GioKetThuc
                    //       FROM SAN s
                    //       LEFT JOIN DAT_SAN d ON s.MaSan = d.MaSan 
                    //            AND d.NgayDat = @NgayDat 
                    //            AND d.TrangThai IN (N'Đã đặt', N'Đang sử dụng')";
                    // [SỬA ĐỔI] Thêm N'Hoàn thành' vào danh sách lấy dữ liệu
                    string sql = @"SELECT s.TenSan, s.LoaiSan, d.MaDat, d.TenKhach, d.SoDienThoai, d.TrangThai, d.GioBatDau, d.GioKetThuc
                           FROM SAN s
                           LEFT JOIN DAT_SAN d ON s.MaSan = d.MaSan 
                                AND d.NgayDat = @NgayDat 
                                AND d.TrangThai IN (N'Đã đặt', N'Đang sử dụng', N'Hoàn thành')";
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

                            //if (trangThai == "Đang sử dụng") lbl.BackColor = Color.LightGreen;
                            //else lbl.BackColor = Color.Gold;
                            if (trangThai == "Đang sử dụng")
                            {
                                lbl.BackColor = Color.LightGreen;
                            }
                            else if (trangThai == "Đã đặt")
                            {
                                lbl.BackColor = Color.Gold;
                            }
                            else if (trangThai == "Hoàn thành")
                            {

                                //lbl.BackColor = Color.LightGray;
                                lbl.BackColor = Color.DeepSkyBlue;
                            }
                            else                             
                            {
                                lbl.BackColor = Color.LightGray;
                            }
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
                        string dong1 = CanGiuaTuDong($"   {tenSan.ToUpper()}", lbl);
                        string dong2 = CanGiuaTuDong($"   ({loaiSan})", lbl);
                        string line = CanGiuaTuDong("_____________________", lbl);

                        // [MỚI] Nội dung căn trái (Đã thêm Ngày và Giờ)
                        // Dùng các khoảng trắng sau dấu : để căn thẳng hàng cho đẹp
                        string body = $"Mã đặt: {maDat}\n" +
                                      $"Ngày: {hienThiNgay}\n" +   // <-- Thêm dòng Ngày
                                      $"Giờ: {hienThiGio}\n" +    // <-- Thêm dòng Giờ
                                      $"Tên: {tenKhach}\n" +
                                      $"SĐT: {sdt}\n" +
                                      $"Trạng thái: {trangThai}";

                        lbl.Text = "\n\n\n\n\n" + dong1 + "\n" + dong2 + "\n" + line + "\n\n" + body;
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

            if (dtpChonNgay.Value.Date < DateTime.Now.Date)
            {
                lblSanA.Enabled = false;
                lblSanB.Enabled = false;
                lblSanC.Enabled = false;
                lblSanD.Enabled = false;
                lblSanE.Enabled = false;


            }
            else
            {
                if (!string.IsNullOrEmpty(cboThoiGian.Text))
                {
                    lblSanA.Enabled = true;
                    lblSanB.Enabled = true;
                    lblSanC.Enabled = true;
                    lblSanD.Enabled = true;
                    lblSanE.Enabled = true;

                }
                else
                {
                    MessageBox.Show("Vui lòng chọn khung thời gian!", "Thông báo");
                    return;
                }
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

            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
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
            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
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
                    MessageBox.Show($"Đã giao sân!");
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        // Hàm Trả Sân & Thanh Toán (Thường sẽ nằm trong frmThanhToan khi bấm nút 'Thanh Toán')
        // Nhưng nếu bạn muốn xử lý nhanh ở Main thì dùng hàm này:
        private void XuLyTraSan(string maDat, DateTime gioTra)
        {
            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_TraSan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaDat", maDat);
                    cmd.Parameters.AddWithValue("@ThoiGianTra", gioTra);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Đã trả sân!");
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
        private void lblSanA_Click(object sender, EventArgs e)
        {
            XuLySKClickSan(sender);
        }


        private void lblSanB_Click(object sender, EventArgs e)
        {
            XuLySKClickSan(sender);
        }

        private void lblSanC_Click(object sender, EventArgs e)
        {
            XuLySKClickSan(sender);
        }

        private void lblSanD_Click(object sender, EventArgs e)
        {
            XuLySKClickSan(sender);
        }

        private void lblSanE_Click(object sender, EventArgs e)
        {
            XuLySKClickSan(sender);
        }
        private void XuLySKClickSan(object sender)
        {
            Label lbl = sender as Label;
            if (lbl.Tag == null) return;

            string tagData = lbl.Tag.ToString();
            string[] parts = tagData.Split('|');

            string maDat = parts[0];
            string tenSan = parts[1];
            string trangThai = parts[2];

            // 1. SÂN TRỐNG -> ĐẶT
            if (trangThai == "Trống")
            {
                var luachon = MessageBox.Show($"{tenSan} đang trống. Bạn có muốn đặt sân không?", "Đặt sân", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (luachon == DialogResult.Yes)
                {
                    DatSan ds = new DatSan();
                    ds.SetThongTin(tenSan, cboThoiGian.Text, dtpChonNgay.Value);
                    ds.ShowDialog();
                    LoadTrangThaiSan();
                    // Nếu họ đã bấm nút Đặt hoặc Giao luôn thì load lại màu
                    if (ds.DaThucHien)
                        LoadTrangThaiSan();
                }
            }
            // 2. ĐÃ ĐẶT -> GIAO SÂN (NHẬP GIỜ NHẬN)
            else if (trangThai == "Đã đặt")
            {
                // Logic: Yes -> Nhận sân | No -> Hủy khách này
                DialogResult hoi = MessageBox.Show($"Sân này đã được đặt. Bạn có muốn nhận sân không?", "Xử lý đặt sân", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (hoi == DialogResult.Yes) // NHẬN SÂN
                {
                    // Gọi code check-in (Giao sân)
                    XuLyGiaoSan(maDat, DateTime.Now);
                    LoadTrangThaiSan(); // Refresh lại màu xanh
                }
                else if (hoi == DialogResult.No) // HỦY ĐẶT
                {
                    if (MessageBox.Show("Bạn chắc chắn muốn hủy lịch đặt này và chuyển sân về trạng thái trống?",
                                        "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        XuLyHuyDat(maDat);
                        LoadTrangThaiSan(); // Refresh lại màu xám
                    }
                }
            }
            // 3. ĐANG SỬ DỤNG -> TRẢ SÂN (NHẬP GIỜ TRẢ)
            else if (trangThai == "Đang sử dụng")
            {
                var hoi = MessageBox.Show("Bạn có muốn thanh toán và trả sân không?", "Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (hoi == DialogResult.Yes)
                {
                    // Mở form Thanh Toán, truyền Mã Đặt qua để tính tiền
                    ThanhToan tt = new ThanhToan(maDat);
                    tt.ShowDialog();

                    if (tt.DaThanhToan)
                        LoadTrangThaiSan(); // Refresh lại thành trống
                }
            }
        }
        private void XuLyHuyDat(string maDat)
        {
            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_HuyDatSan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaDat", maDat);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Đã hủy lịch đặt. Sân đã trống.");
                }
                catch (Exception ex) { MessageBox.Show("Lỗi hủy: " + ex.Message); }
            }
        }

        private void MenuThongKe_Click(object sender, EventArgs e)
        {
            // 1. Khởi tạo form Thống kê
            ThongKe tk = new ThongKe();

            // 2. Ẩn form Main hiện tại đi
            this.Hide();

            // 3. Hiển thị form Thống kê dưới dạng Dialog (Chương trình sẽ dừng tại dòng này chờ bạn tắt form Thống kê)
            tk.ShowDialog();

            // 4. [QUAN TRỌNG] Sau khi tắt form Thống kê, dòng này sẽ chạy để hiện lại form Main
            this.Show();
        }

        private void MenuTroGiup_Click(object sender, EventArgs e)
        {
            TroGiup tg = new TroGiup();
            tg.ShowDialog();
        }
    }
}
