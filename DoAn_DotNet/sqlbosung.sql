
USE QLSANBONG;
GO

-- 1. Tạo thủ tục Giao Sân (Check-in)
CREATE PROCEDURE sp_GiaoSan
    @MaDat VARCHAR(10),
    @ThoiGianGiao DATETIME
AS
BEGIN
    -- Tự động sinh MaGiao theo thời gian (Ví dụ: G + chuỗi số ngày giờ)
    -- Lưu ý: Cần đảm bảo độ dài không quá 10 ký tự của cột MaGiao
    DECLARE @MaGiao VARCHAR(10);
    SET @MaGiao = 'G' + FORMAT(GETDATE(), 'ddHHmmss'); 

    -- Thêm dòng mới vào bảng GIAO_SAN
    INSERT INTO GIAO_SAN (MaGiao, MaDat, ThoiGianGiao)
    VALUES (@MaGiao, @MaDat, @ThoiGianGiao);

    -- Cập nhật trạng thái sân trong bảng DAT_SAN
    UPDATE DAT_SAN
    SET TrangThai = N'Đang sử dụng'
    WHERE MaDat = @MaDat;
END;
GO

-- 2. Tạo thủ tục Trả Sân (Check-out)
-- Bạn nên tạo luôn cái này vì lát nữa bấm "Trả sân" sẽ bị lỗi tương tự
CREATE PROCEDURE sp_TraSan
    @MaDat VARCHAR(10),
    @ThoiGianTra DATETIME
AS
BEGIN
    -- Cập nhật giờ trả vào bảng GIAO_SAN
    UPDATE GIAO_SAN
    SET ThoiGianTra = @ThoiGianTra
    WHERE MaDat = @MaDat AND ThoiGianTra IS NULL;

    -- Cập nhật trạng thái trong bảng DAT_SAN
    UPDATE DAT_SAN
    SET TrangThai = N'Hoàn thành' -- Hoặc N'Trống' tùy logic của bạn sau khi thanh toán
    WHERE MaDat = @MaDat;
END;
GO
USE QLSANBONG;
GO

CREATE PROCEDURE sp_LayThongTinThanhToan
    @MaDat VARCHAR(10)
AS
BEGIN
    SELECT 
        d.MaDat,
        d.TenKhach,
        d.SoDienThoai,
        s.TenSan,
        s.LoaiSan,
        s.DonGia,
        g.ThoiGianGiao,
        g.ThoiGianTra
    FROM DAT_SAN d
    JOIN SAN s ON d.MaSan = s.MaSan
    LEFT JOIN GIAO_SAN g ON d.MaDat = g.MaDat
    WHERE d.MaDat = @MaDat;
END;
GO

CREATE PROCEDURE sp_LuuThanhToan
    @MaDat VARCHAR(10),
    @ThanhTien DECIMAL(18,2)
AS
BEGIN
    -- Tạo mã thanh toán tự động (Ví dụ: T + ngày giờ)
    DECLARE @MaTT VARCHAR(10);
    SET @MaTT = 'T' + FORMAT(GETDATE(), 'ddHHmmss');

    -- Lấy tên khách từ bảng đặt sân để lưu vào bảng thanh toán (hoặc truyền tham số tùy logic code)
    DECLARE @TenKhach NVARCHAR(100);
    SELECT @TenKhach = TenKhach FROM DAT_SAN WHERE MaDat = @MaDat;

    -- Thêm vào bảng THANH_TOAN
    INSERT INTO THANH_TOAN (MaTT, MaDat, TenKhach, ThanhTien)
    VALUES (@MaTT, @MaDat, @TenKhach, @ThanhTien);
END;
GO




USE QLSANBONG;
GO

-- 1. Cập nhật thủ tục Giao Sân (Check-in)
-- Sử dụng CREATE OR ALTER để tự động tạo mới nếu chưa có, hoặc cập nhật nếu đã có
CREATE OR ALTER PROCEDURE sp_GiaoSan
    @MaDat VARCHAR(10),
    @ThoiGianGiao DATETIME
AS
BEGIN
    -- Tự động sinh MaGiao theo thời gian (Ví dụ: G + chuỗi số ngày giờ)
    DECLARE @MaGiao VARCHAR(10);
    SET @MaGiao = 'G' + FORMAT(GETDATE(), 'ddHHmmss'); 

    -- Thêm dòng mới vào bảng GIAO_SAN
    INSERT INTO GIAO_SAN (MaGiao, MaDat, ThoiGianGiao)
    VALUES (@MaGiao, @MaDat, @ThoiGianGiao);

    -- Cập nhật trạng thái sân trong bảng DAT_SAN thành 'Đang sử dụng'
    UPDATE DAT_SAN
    SET TrangThai = N'Đang sử dụng'
    WHERE MaDat = @MaDat;
END;
GO

-- 2. Cập nhật thủ tục Trả Sân (Check-out)
CREATE OR ALTER PROCEDURE sp_TraSan
    @MaDat VARCHAR(10),
    @ThoiGianTra DATETIME
AS
BEGIN
    -- Cập nhật giờ trả vào bảng GIAO_SAN tại dòng tương ứng chưa có giờ trả
    UPDATE GIAO_SAN
    SET ThoiGianTra = @ThoiGianTra
    WHERE MaDat = @MaDat AND ThoiGianTra IS NULL;

    -- Cập nhật trạng thái trong bảng DAT_SAN thành 'Hoàn thành'
    UPDATE DAT_SAN
    SET TrangThai = N'Hoàn thành'
    WHERE MaDat = @MaDat;
END;
GO

 USE QLSANBONG;
GO

ALTER PROCEDURE sp_ThongKeDoanhThu
    @TuNgay DATETIME,
    @DenNgay DATETIME
AS
BEGIN
    SELECT 
        s.TenSan AS [Tên Sân],
        ds.TenKhach AS [Tên Khách],
        tt.ThanhTien AS [Thành Tiền],
        -- Logic Mới: Ghép Ngày Đặt + Giờ Kết Thúc để ra thời điểm thanh toán dự kiến
        -- Lưu ý: Kiểm tra kiểu dữ liệu trong DB của bạn. Nếu GioKetThuc là TIME và NgayDat là DATE:
        CAST(ds.NgayDat AS DATETIME) + CAST(ds.GioKetThuc AS DATETIME) AS [Ngày Thanh Toán]
    FROM DAT_SAN ds
    -- 1. Kết nối bảng THANH_TOAN để lấy tiền (Chỉ đơn nào đã thanh toán mới hiện)
    JOIN THANH_TOAN tt ON ds.MaDat = tt.MaDat
    -- 2. Kết nối bảng SAN để lấy tên sân
    JOIN SAN s ON ds.MaSan = s.MaSan
    WHERE 
        -- 3. Chỉ lấy đơn có trạng thái 'Hoàn thành'
        ds.TrangThai = N'Hoàn thành' 
        -- 4. Lọc theo khoảng thời gian dựa trên (Ngày Đặt + Giờ Kết Thúc)
        AND (CAST(ds.NgayDat AS DATETIME) + CAST(ds.GioKetThuc AS DATETIME)) >= @TuNgay
        AND (CAST(ds.NgayDat AS DATETIME) + CAST(ds.GioKetThuc AS DATETIME)) <= @DenNgay
    ORDER BY [Ngày Thanh Toán] DESC;
END;
GO

CREATE PROCEDURE sp_HuyDatSan
    @MaDat VARCHAR(20) -- Kiểu dữ liệu phải khớp với cột MaDat trong bảng
AS
BEGIN
    -- Xóa thông tin đặt sân để sân trống trở lại
    DELETE FROM DAT_SAN 
    WHERE MaDat = @MaDat
END
GO




-- dữ liệu mẫu!

USE QLSANBONG
GO

-- =============================================
-- PHẦN 1: XÓA DỮ LIỆU CŨ (THEO ĐÚNG TRÌNH TỰ)
-- =============================================
PRINT N'--- Đang xóa dữ liệu cũ ---'

-- 1. Xóa bảng con (Phụ thuộc) trước
DELETE FROM THANH_TOAN;
DELETE FROM GIAO_SAN;
DELETE FROM CTDS;

-- 2. Xóa bảng cha (Chứa khóa chính) sau
DELETE FROM DAT_SAN;
DELETE FROM SAN;

PRINT N'--- Đã xóa sạch dữ liệu ---'
GO

-- =============================================
-- PHẦN 2: TẠO DỮ LIỆU MẪU MỚI
-- =============================================
PRINT N'--- Đang thêm dữ liệu mẫu ---'

-- 1. THÊM DANH SÁCH SÂN
-- Giả lập 5 sân: 3 sân 5 người, 2 sân 7 người
INSERT INTO SAN (MaSan, TenSan, LoaiSan, DonGia) VALUES (N'S001', N'Sân A', N'Sân 5 người', 150000);
INSERT INTO SAN (MaSan, TenSan, LoaiSan, DonGia) VALUES (N'S002', N'Sân B', N'Sân 5 người', 150000);
INSERT INTO SAN (MaSan, TenSan, LoaiSan, DonGia) VALUES (N'S003', N'Sân C', N'Sân 5 người', 150000);
INSERT INTO SAN (MaSan, TenSan, LoaiSan, DonGia) VALUES (N'S004', N'Sân D', N'Sân 7 người', 250000);
INSERT INTO SAN (MaSan, TenSan, LoaiSan, DonGia) VALUES (N'S005', N'Sân E', N'Sân 7 người', 250000);

-- Biến lấy ngày hôm nay để dữ liệu luôn "tươi" khi bạn chạy script này
DECLARE @HomNay DATE = GETDATE(); 
DECLARE @NgayMai DATE = DATEADD(day, 1, GETDATE());

-- 2. THÊM DỮ LIỆU ĐẶT SÂN (DAT_SAN)
-- Tình huống 1: Đã hoàn thành (Đá xong, đã trả tiền) - Sân A
INSERT INTO DAT_SAN (MaDat, TenKhach, SoDienThoai, MaSan, NgayDat, GioBatDau, GioKetThuc, TrangThai)
VALUES (N'D_TEST_01', N'Nguyễn Văn An', N'0901234567', N'S001', @HomNay, '07:00:00', '08:00:00', N'Hoàn thành');

-- Tình huống 2: Đang sử dụng (Khách đang đá) - Sân B
-- Giả sử đang đá khung 16h-17h (Bạn có thể sửa giờ này cho khớp giờ hiện tại để test màu XANH)
INSERT INTO DAT_SAN (MaDat, TenKhach, SoDienThoai, MaSan, NgayDat, GioBatDau, GioKetThuc, TrangThai)
VALUES (N'D_TEST_02', N'Trần Thị Bích', N'0909888777', N'S002', @HomNay, '16:00:00', '17:00:00', N'Đang sử dụng');

-- Tình huống 3: Đã đặt (Sắp đá, chưa nhận sân) - Sân C
INSERT INTO DAT_SAN (MaDat, TenKhach, SoDienThoai, MaSan, NgayDat, GioBatDau, GioKetThuc, TrangThai)
VALUES (N'D_TEST_03', N'Lê Hoàng Cường', N'0912333444', N'S003', @HomNay, '18:00:00', '19:00:00', N'Đã đặt');

-- Tình huống 4: Đặt ngày mai - Sân A
INSERT INTO DAT_SAN (MaDat, TenKhach, SoDienThoai, MaSan, NgayDat, GioBatDau, GioKetThuc, TrangThai)
VALUES (N'D_TEST_04', N'Phạm Văn Dũng', N'0988777666', N'S001', @NgayMai, '08:00:00', '09:00:00', N'Đã đặt');

-- 3. THÊM DỮ LIỆU GIAO SÂN (GIAO_SAN) - Check-in log
-- Tương ứng D_TEST_01: Đã nhận lúc 7h, Trả lúc 8h
INSERT INTO GIAO_SAN (MaGiao, MaDat, ThoiGianGiao, ThoiGianTra)
VALUES (N'G_TEST_01', N'D_TEST_01', CAST(CONCAT(@HomNay, ' 07:00:00') AS DATETIME), CAST(CONCAT(@HomNay, ' 08:00:00') AS DATETIME));

-- Tương ứng D_TEST_02: Đã nhận lúc 16h, Chưa trả (ThoiGianTra = NULL)
INSERT INTO GIAO_SAN (MaGiao, MaDat, ThoiGianGiao, ThoiGianTra)
VALUES (N'G_TEST_02', N'D_TEST_02', CAST(CONCAT(@HomNay, ' 16:05:00') AS DATETIME), NULL);

-- 4. THÊM DỮ LIỆU THANH TOÁN (THANH_TOAN) - Bill
-- Chỉ có D_TEST_01 là đã xong và thanh toán
INSERT INTO THANH_TOAN (MaTT, MaDat, TenKhach, ThanhTien)
VALUES (N'TT_001', N'D_TEST_01', N'Nguyễn Văn An', 150000);

PRINT N'--- Hoàn tất tạo dữ liệu mẫu! ---'
GO