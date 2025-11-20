CREATE DATABASE QLSANBONG;
GO
USE QLSANBONG;
GO

-- BẢNG SÂN
CREATE TABLE SAN (
    MaSan VARCHAR(10) PRIMARY KEY,
    TenSan NVARCHAR(100),
    LoaiSan NVARCHAR(50),
    DonGia DECIMAL(18,2)
);
GO

-- BẢNG ĐẶT SÂN
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

-- BẢNG GIAO SÂN
CREATE TABLE GIAO_SAN (
    MaGiao VARCHAR(10) PRIMARY KEY,
    MaDat VARCHAR(10),
    ThoiGianGiao DATETIME,
    ThoiGianTra DATETIME,
    FOREIGN KEY (MaDat) REFERENCES DAT_SAN(MaDat)
);
GO

-- BẢNG THANH TOÁN
CREATE TABLE THANH_TOAN (
    MaTT VARCHAR(10) PRIMARY KEY,
    MaDat VARCHAR(10),
    TenKhach NVARCHAR(100),
    ThanhTien DECIMAL(18,2),
    FOREIGN KEY (MaDat) REFERENCES DAT_SAN(MaDat)
);
GO

-- ========================
-- DỮ LIỆU MẪU CHO BẢNG SÂN
-- ========================
INSERT INTO SAN VALUES
('S001', N'Sân A', N'Sân 7 người', 200000),
('S002', N'Sân B', N'Sân 7 người', 200000),
('S003', N'Sân C', N'Sân 10 người', 300000),
('S004', N'Sân D', N'Sân 10 người', 300000),
('S005', N'Sân E', N'Sân 7 người', 200000);
GO

-- ==========================
-- DỮ LIỆU MẪU CHO BẢNG ĐẶT SÂN
-- ==========================
INSERT INTO DAT_SAN VALUES
('D001', N'Nguyễn Văn Tuấn', '0912345678', 'S001', '2025-11-20', '08:00', '09:00', N'Đã đặt'),
('D002', N'Trần Tuấn Anh', '0987654321', 'S002', '2025-11-20', '09:00', '10:00', N'Đang sử dụng'),
('D003', N'Lê Văn Minh', '0909123123', 'S003', '2025-11-21', '07:00', '08:00', N'Trống'),
('D004', N'Phạm Phú Thành', '0978234567', 'S004', '2025-11-21', '10:00', '11:00', N'Đã đặt'),
('D005', N'Ngô Văn Tuấn', '0934567890', 'S005', '2025-11-22', '17:00', '18:00', N'Đang sử dụng'),
('D006', N'Huỳnh Quốc Văn', '0921122334', 'S001', '2025-11-22', '19:00', '20:00', N'Trống'),
('D007', N'Đỗ Văn Thịnh', '0919988776', 'S002', '2025-11-18', '16:00', '17:00', N'Trống'),
('D008', N'Trần Văn Linh', '0967894321', 'S004', '2025-11-20', '18:00', '19:00', N'Đã đặt');
GO

-- ==========================
-- DỮ LIỆU MẪU CHO BẢNG GIAO SÂN
-- ==========================
INSERT INTO GIAO_SAN VALUES
('G001', 'D001', '2025-11-20 08:00', '2025-11-20 09:00'),
('G002', 'D002', '2025-11-20 09:00', NULL),
('G003', 'D004', '2025-11-21 10:00', NULL),
('G004', 'D005', '2025-11-22 17:00', NULL),
('G005', 'D008', '2025-11-20 18:00', NULL);
GO

-- ==========================
-- DỮ LIỆU MẪU CHO BẢNG THANH TOÁN
-- ==========================
INSERT INTO THANH_TOAN VALUES
('T001', 'D001', N'Nguyễn Văn Tuấn', 200000),
('T002', 'D002', N'Trần Tuấn Anh', 200000),
('T003', 'D004', N'Phạm Phú Thành', 300000),
('T004', 'D005', N'Ngô Văn Tuấn', 200000),
('T005', 'D008', N'Trần Văn Linh', 300000);
GO

SELECT * FROM SAN;
SELECT * FROM DAT_SAN;
SELECT * FROM GIAO_SAN;
SELECT * FROM THANH_TOAN;