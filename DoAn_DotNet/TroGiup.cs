using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DoAn_DotNet.ChuoiKetNoi;

namespace DoAn_DotNet
{
    public partial class TroGiup : Form
    {
        public TroGiup()
        {
            InitializeComponent();
        }
        ChuoiKetNoi pro = new ChuoiKetNoi();

        private void BtnGui_Click(object sender, EventArgs e)
        {
            string cauHoi = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(cauHoi)) return;

            // 1. Hiện câu hỏi của người dùng lên khung chat (màu xanh/đen)
            ThemTinNhan("Bạn: " + cauHoi, Color.Black);
            txtInput.Clear();

            // 2. Xử lý câu hỏi
            XuLyCauHoi(cauHoi);
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnGui.PerformClick();
                e.SuppressKeyPress = true; // Chặn tiếng 'bíp'
            }
        }

        // Hàm thêm tin nhắn vào khung chat với màu sắc
        private void ThemTinNhan(string noiDung, Color mauChu)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = mauChu;
            rtbChat.AppendText(noiDung + "\n\n");
            rtbChat.ScrollToCaret();
        }

        private void XuLyCauHoi(string cauHoi)
        {
            // Mẫu câu chuẩn: "Sân A lúc 10:00 - 11:00 ngày 23/11/2025..."
            // Dùng Regex để bắt các nhóm dữ liệu:
            // Nhóm 1: Tên sân (Sân A)
            // Nhóm 2: Giờ bắt đầu (10:00)
            // Nhóm 3: Giờ kết thúc (11:00)
            // Nhóm 4: Ngày (23/11/2025)

            string pattern = @"(Sân [A-Za-z0-9]+).*?(\d{1,2}:\d{2})\s*-\s*(\d{1,2}:\d{2}).*?ngày\s*(\d{1,2}/\d{1,2}/\d{4})";

            Match match = Regex.Match(cauHoi, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string tenSan = match.Groups[1].Value;  // Sân A
                string gioBatDau = match.Groups[2].Value; // 10:00
                string gioKetThuc = match.Groups[3].Value; // 11:00
                string ngayStr = match.Groups[4].Value; // 23/11/2025

                // Gọi hàm kiểm tra Database
                KiemTraTrangThaiSan(tenSan, gioBatDau, gioKetThuc, ngayStr);
            }
            else
            {
                // Nếu người dùng hỏi linh tinh hoặc sai cú pháp
                ThemTinNhan("Bot: Xin lỗi, tôi chưa hiểu. Hãy hỏi theo mẫu: 'Sân A lúc 10:00 - 11:00 ngày dd/mm/yyyy...'", Color.Red);
            }
        }

        private void KiemTraTrangThaiSan(string tenSan, string gioBD, string gioKT, string ngayStr)
        {
            using (SqlConnection conn = new SqlConnection(pro.strKetNoi))
            {
                try
                {
                    conn.Open();

                    // Chuyển đổi ngày sang định dạng SQL (yyyy-MM-dd)
                    DateTime ngayDat = DateTime.ParseExact(ngayStr, "dd/MM/yyyy", null);
                    string strNgaySQL = ngayDat.ToString("yyyy-MM-dd");

                    // Câu lệnh SQL: Tìm xem có đơn đặt nào trùng khớp không
                    // Ta nối bảng SAN và DAT_SAN, kiểm tra Tên Sân + Ngày + Giờ Bắt Đầu
                    string sql = @"
                        SELECT TOP 1 d.TrangThai 
                        FROM DAT_SAN d
                        JOIN SAN s ON d.MaSan = s.MaSan
                        WHERE s.TenSan = @TenSan 
                          AND d.NgayDat = @NgayDat
                          AND d.GioBatDau = @GioBatDau";
                    // Lưu ý: Logic ở đây là kiểm tra chính xác khung giờ (theo combo box của bạn)

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@TenSan", tenSan);
                    cmd.Parameters.AddWithValue("@NgayDat", strNgaySQL);
                    cmd.Parameters.AddWithValue("@GioBatDau", TimeSpan.Parse(gioBD));

                    object result = cmd.ExecuteScalar();

                    string phanHoi = "";

                    if (result != null)
                    {
                        // Tìm thấy dữ liệu -> Tức là đã có người đặt
                        string trangThai = result.ToString();
                        phanHoi = $"Bot: {tenSan} lúc {gioBD} - {gioKT} ngày {ngayStr} đang '{trangThai}'. Bạn hãy chọn sân khác hoặc giờ khác nhé!";
                        ThemTinNhan(phanHoi, Color.OrangeRed);
                    }
                    else
                    {
                        // Không tìm thấy -> Tức là trống
                        phanHoi = $"Bot: {tenSan} lúc {gioBD} - {gioKT} ngày {ngayStr} đang TRỐNG. Bạn có thể đặt ngay!";
                        ThemTinNhan(phanHoi, Color.Green);
                    }
                }
                catch (Exception ex)
                {
                    ThemTinNhan("Bot: Có lỗi xảy ra khi tra cứu: " + ex.Message, Color.Red);
                }
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
