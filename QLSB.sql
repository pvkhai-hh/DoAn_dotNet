CREATE DATABASE QLSANBONG;

USE QLSANBONG;

-- 2. TẠO CÁC BẢNG
-- Bảng SÂN
CREATE TABLE SAN (
    MaSan VARCHAR(10) PRIMARY KEY,
    TenSan NVARCHAR(100),
    LoaiSan NVARCHAR(50),
    DonGia DECIMAL(18,2)
);
GO

-- Bảng ĐẶT SÂN
CREATE TABLE DAT_SAN (
    MaDat VARCHAR(10) PRIMARY KEY,
    TenKhach NVARCHAR(100),
    SoDienThoai VARCHAR(15),
    MaSan VARCHAR(10),
    NgayDat DATE,
    GioBatDau TIME,
    GioKetThuc TIME,
    TrangThai NVARCHAR(50),
    FOREIGN KEY (MaSan) REFERENCES SAN(MaSan)
);
GO

-- Bảng GIAO SÂN
CREATE TABLE GIAO_SAN (
    MaGiao VARCHAR(10) PRIMARY KEY,
    MaDat VARCHAR(10),
    ThoiGianGiao DATETIME,
    ThoiGianTra DATETIME,
    FOREIGN KEY (MaDat) REFERENCES DAT_SAN(MaDat)
);
GO

-- Bảng THANH TOÁN
CREATE TABLE THANH_TOAN (
    MaTT VARCHAR(10) PRIMARY KEY,
    MaDat VARCHAR(10),
    TenKhach NVARCHAR(100),
    ThanhTien DECIMAL(18,2),
    FOREIGN KEY (MaDat) REFERENCES DAT_SAN(MaDat)
);
GO

-- 3. THÊM RÀNG BUỘC (CONSTRAINTS) QUAN TRỌNG
-- Ràng buộc 1: Giờ kết thúc phải lớn hơn giờ bắt đầu
ALTER TABLE DAT_SAN
ADD CONSTRAINT CK_GioHopLe CHECK (GioKetThuc > GioBatDau);
GO

-- Ràng buộc 2: Chỉ cho phép đặt từ 08:00 sáng đến 21:00 tối
ALTER TABLE DAT_SAN
ADD CONSTRAINT CK_GioHoatDong 
CHECK (GioBatDau >= '08:00:00' AND GioKetThuc <= '21:00:00');
GO

-- 4. NHẬP DỮ LIỆU MẪU
-- Dữ liệu Sân
INSERT INTO SAN VALUES
('S001', N'Sân A', N'Sân 7 người', 200000),
('S002', N'Sân B', N'Sân 7 người', 200000),
('S003', N'Sân C', N'Sân 10 người', 300000),
('S004', N'Sân D', N'Sân 10 người', 300000),
('S005', N'Sân E', N'Sân 7 người', 200000);
GO

-- Dữ liệu Đặt Sân (Đã sửa D003 thành 08:00 để không bị lỗi)
INSERT INTO DAT_SAN VALUES
('D001', N'Nguyễn Văn Tuấn', '0912345678', 'S001', '2025-11-20', '08:00', '09:00', N'Đã đặt'),
('D002', N'Trần Tuấn Anh', '0987654321', 'S002', '2025-11-20', '09:00', '10:00', N'Đang sử dụng'),
('D003', N'Lê Văn Minh', '0909123123', 'S003', '2025-11-21', '08:00', '09:00', N'Đã đặt'), -- Đã sửa giờ
('D004', N'Phạm Phú Thành', '0978234567', 'S004', '2025-11-21', '10:00', '11:00', N'Đã đặt'),
('D005', N'Ngô Văn Tuấn', '0934567890', 'S005', '2025-11-22', '17:00', '18:00', N'Đang sử dụng'),
('D006', N'Huỳnh Quốc Văn', '0921122334', 'S001', '2025-11-22', '19:00', '20:00', N'Đã đặt'),
('D007', N'Đỗ Văn Thịnh', '0919988776', 'S002', '2025-11-18', '16:00', '17:00', N'Đang sử dụng'),
('D008', N'Trần Văn Linh', '0967894321', 'S004', '2025-11-20', '18:00', '19:00', N'Đã đặt');
GO

-- Dữ liệu Giao Sân
INSERT INTO GIAO_SAN VALUES
('G001', 'D001', '2025-11-20 08:00', '2025-11-20 09:00'),
('G002', 'D002', '2025-11-20 09:00', NULL),
('G003', 'D004', '2025-11-21 10:00', NULL),
('G004', 'D005', '2025-11-22 17:00', NULL),
('G005', 'D008', '2025-11-20 18:00', NULL);
GO

-- Dữ liệu Thanh Toán
INSERT INTO THANH_TOAN VALUES
('T001', 'D001', N'Nguyễn Văn Tuấn', 200000),
('T002', 'D002', N'Trần Tuấn Anh', 200000),
('T003', 'D004', N'Phạm Phú Thành', 300000),
('T004', 'D005', N'Ngô Văn Tuấn', 200000),
('T005', 'D008', N'Trần Văn Linh', 300000);
GO

-- Kiểm tra lại kết quả
SELECT * FROM SAN;
SELECT * FROM DAT_SAN;
SELECT * FROM SAN;
SELECT * FROM DAT_SAN;
SELECT * FROM GIAO_SAN;
SELECT * FROM THANH_TOAN;

GO

-- Thủ tục Giao Sân (Check-in)
CREATE PROCEDURE sp_GiaoSan
    @MaDat VARCHAR(10),
    @ThoiGianGiao DATETIME
AS
BEGIN
    -- Thêm vào bảng Giao Sân
    INSERT INTO GIAO_SAN (MaGiao, MaDat, ThoiGianGiao)
    VALUES ('G' + FORMAT(GETDATE(), 'ddMMyyHHmm'), @MaDat, @ThoiGianGiao);

    -- Cập nhật trạng thái Đặt Sân thành 'Đang sử dụng'
    UPDATE DAT_SAN SET TrangThai = N'Đang sử dụng' WHERE MaDat = @MaDat;
END;
GO

-- Thủ tục Trả Sân (Check-out) - Đơn giản hóa logic update
CREATE PROCEDURE sp_TraSan
    @MaDat VARCHAR(10),
    @ThoiGianTra DATETIME
AS
BEGIN
    -- Cập nhật giờ trả vào bảng Giao Sân
    UPDATE GIAO_SAN SET ThoiGianTra = @ThoiGianTra WHERE MaDat = @MaDat;
    
    -- Cập nhật trạng thái Đặt Sân về lại Trống hoặc Hoàn thành (Tùy logic của bạn, ở đây tôi để Hoàn thành)
    UPDATE DAT_SAN SET TrangThai = N'Hoàn thành' WHERE MaDat = @MaDat;
END;
GO

-- 1. Thủ tục Hủy Đặt Sân (Xóa dòng đặt sân để trả về trạng thái Trống)
CREATE PROCEDURE sp_HuyDatSan
    @MaDat VARCHAR(10)
AS
BEGIN
    DELETE FROM DAT_SAN WHERE MaDat = @MaDat;
END;
GO

-- 2. Thủ tục lấy Đơn Giá sân theo Mã Đặt (Dùng cho Thanh Toán)
CREATE PROCEDURE sp_LayThongTinThanhToan
    @MaDat VARCHAR(10)
AS
BEGIN
    SELECT s.TenSan, s.DonGia, d.GioBatDau, d.GioKetThuc, d.TenKhach
    FROM DAT_SAN d
    JOIN SAN s ON d.MaSan = s.MaSan
    WHERE d.MaDat = @MaDat;
END;
GO
USE QLSANBONG;
GO

ALTER PROCEDURE sp_TraSan
    @MaDat VARCHAR(20), -- Đã sửa thành 20 cho khớp với mã mới
    @ThoiGianTra DATETIME
AS
BEGIN
    -- 1. Cập nhật giờ trả vào bảng Giao Sân
    UPDATE GIAO_SAN 
    SET ThoiGianTra = @ThoiGianTra 
    WHERE MaDat = @MaDat;
    
    -- 2. QUAN TRỌNG: Trả trạng thái về 'Trống' (thay vì Hoàn thành)
    UPDATE DAT_SAN 
    SET TrangThai = N'Trống' 
    WHERE MaDat = @MaDat;
END;
GO

ALTER PROCEDURE sp_TraSan
    @MaDat VARCHAR(20),
    @ThoiGianTra DATETIME
AS
BEGIN
    -- Cập nhật giờ trả
    UPDATE GIAO_SAN SET ThoiGianTra = @ThoiGianTra WHERE MaDat = @MaDat;
    
    -- Cập nhật trạng thái thành 'Hoàn thành' (Để phân biệt với 'Trống' và 'Đang sử dụng')
    UPDATE DAT_SAN 
    SET TrangThai = N'Hoàn thành' 
    WHERE MaDat = @MaDat;
END;
GO
ALTER PROCEDURE sp_GiaoSan
    @MaDat VARCHAR(20), -- Lưu ý: Đảm bảo kiểu dữ liệu khớp với bảng cha
    @ThoiGianGiao DATETIME
AS
BEGIN
    -- [SỬA LỖI] Rút ngắn mã Giao còn 9 ký tự (G + Ngày + Giờ + Phút + Giây)
    -- Ví dụ: G22013045 (Ngày 22, 01h30p45s) -> 9 ký tự (Vừa khít ô 10)
    DECLARE @MaGiao VARCHAR(10) = 'G' + FORMAT(GETDATE(), 'ddHHmmss');

    -- 1. Thêm vào bảng Giao Sân
    INSERT INTO GIAO_SAN (MaGiao, MaDat, ThoiGianGiao)
    VALUES (@MaGiao, @MaDat, @ThoiGianGiao);

    -- 2. Cập nhật trạng thái Đặt Sân
    UPDATE DAT_SAN 
    SET TrangThai = N'Đang sử dụng' 
    WHERE MaDat = @MaDat;
END;
GO