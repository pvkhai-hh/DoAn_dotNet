CREATE DATABASE QLSB;

USE QLSB;

CREATE TABLE SAN (
    MaSan VARCHAR(10) PRIMARY KEY,
    TenSan NVARCHAR(100),
    LoaiSan NVARCHAR(50),
    DonGia DECIMAL(18,2),
    TinhTrang NVARCHAR(50)
);

CREATE TABLE DAT_SAN (
    MaDat VARCHAR(10) PRIMARY KEY,
    TenKhach NVARCHAR(100),
    SoDienThoai VARCHAR(15),
    MaSan VARCHAR(10),
    NgayDat DATE,
    GioBatDau TIME,
    GioKetThuc TIME,
    FOREIGN KEY (MaSan) REFERENCES SAN(MaSan)
);

CREATE TABLE GIAO_SAN (
    MaGiao VARCHAR(10) PRIMARY KEY,
    MaDat VARCHAR(10),
    ThoiGianGiao DATETIME,
    ThoiGianTra DATETIME,
    FOREIGN KEY (MaDat) REFERENCES DAT_SAN(MaDat)
);

CREATE TABLE THANH_TOAN (
    MaTT VARCHAR(10) PRIMARY KEY,
    MaDat VARCHAR(10),
    TenKhach NVARCHAR(100),
    ThoiGianSuDung DECIMAL(5,2),
    ThanhTien DECIMAL(18,2),
    FOREIGN KEY (MaDat) REFERENCES DAT_SAN(MaDat)
);

INSERT INTO SAN (MaSan, TenSan, LoaiSan, DonGia, TinhTrang)
VALUES 
('S001', N'Sân A', N'Sân 7 người', 200000, N'Trống'),
('S002', N'Sân B', N'Sân 7 người', 220000, N'Trống'),
('S003', N'Sân C', N'Sân 10 người', 300000, N'Trống'),
('S004', N'Sân D', N'Sân 10 người', 320000, N'Trống'),
('S005', N'Sân E', N'Sân 7 người', 210000, N'Trống');

INSERT INTO DAT_SAN (MaDat, TenKhach, SoDienThoai, MaSan, NgayDat, GioBatDau, GioKetThuc)
VALUES
('D001', N'Nguyễn Văn Tuấn', '0912345678', 'S001', '2025-11-19', '08:00', '09:00'),
('D002', N'Trần Tuấn Anh', '0987654321', 'S002', '2025-11-20', '09:00', '10:00'),
('D003', N'Lê Văn Minh', '0909123123', 'S003', '2025-11-21', '07:00', '08:00'),
('D004', N'Phạm Phú Thành', '0978234567', 'S004', '2025-11-21', '10:00', '11:00'),
('D005', N'Ngô Văn Tuấn', '0934567890', 'S001', '2025-11-22', '17:00', '18:00'),
('D006', N'Huỳnh Quốc Văn', '0921122334', 'S005', '2025-11-22', '19:00', '20:00'),
('D007', N'Đỗ Văn Thịnh', '0919988776', 'S002', '2025-11-18', '16:00', '17:00'),
('D008', N'Trần Văn Linh', '0967894321', 'S004', '2025-11-20', '18:00', '19:00');

INSERT INTO GIAO_SAN (MaGiao, MaDat, ThoiGianGiao, ThoiGianTra)
VALUES
('G001', 'D001', '2025-11-19 08:00', '2025-11-19 09:00'),
('G002', 'D002', '2025-11-20 09:00', NULL),
('G003', 'D006', '2025-11-22 19:00', '2025-11-22 20:00'),
('G004', 'D008', '2025-11-20 18:00', NULL),
('G005', 'D003', '2025-11-21 07:00', NULL);

INSERT INTO THANH_TOAN (MaTT, MaDat, TenKhach, ThoiGianSuDung, ThanhTien)
VALUES
('TT001', 'D001', N'Nguyễn Văn Tuấn', 1.0, 200000),
('TT002', 'D002', N'Trần Tuấn Anh', 1.0, 220000),
('TT003', 'D006', N'Huỳnh Quốc Văn', 1.0, 210000),
('TT004', 'D008', N'Trần Văn Linh', 1.0, 320000),
('TT005', 'D004', N'Phạm Phú Thành', 1.0, 320000);

SELECT * FROM SAN;
SELECT * FROM DAT_SAN;
SELECT * FROM GIAO_SAN;
SELECT * FROM THANH_TOAN;