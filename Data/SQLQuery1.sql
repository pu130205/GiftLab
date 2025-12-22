use GiftLab

INSERT INTO Roles (RoleID, RoleName, Description) VALUES
(1, N'Admin', N'Quản trị hệ thống'),
(2, N'Staff', N'Nhân viên'),
(3, N'Customer', N'Khách hàng');

INSERT INTO Accounts (AccountID, Phone, Email, Password, Active, Fullname, RoleID, CreateDate) VALUES
(1, '0900000001', 'admin@giftlab.com', '123456', 1, N'Admin GiftLab', 1, GETDATE()),
(2, '0900000002', 'staff@giftlab.com', '123456', 1, N'Staff GiftLab', 2, GETDATE());

INSERT INTO Categories (CatID, Catname, Description, ParentID, Published, Thumb, Alias) VALUES
(1, N'Cupcake', NULL, NULL, 1, NULL, 'cupcake'),
(2, N'Cookie', NULL, NULL, 1, NULL, 'cookie'),
(3, N'Tart', NULL, NULL, 1, NULL, 'tart'),
(4, N'Chocolate', NULL, NULL, 1, NULL, 'chocolate'),
(5, N'Đất màu', NULL, NULL, 1, NULL, 'dat-mau'),
(6, N'Len mềm', NULL, NULL, 1, NULL, 'len-mem'),
(7, N'Hạt cườm', NULL, NULL, 1, NULL, 'hat-cuom');

INSERT INTO Products
(ProductName, Price, Discount, Thumb, BestSellers, HomeFlag, Active, UnitsInStock, CatID)
VALUES
(N'Rocky Road', 35000, 5000, '~/images/1.png', 1, 1, 1, 100, 1),

(N'Melten Lava', 25000, 10000, '~/images/11.png', 1, 1, 1, 100, 2),

(N'Berry Puff', 35000, 0, '~/images/28.png', 1, 1, 1, 100, 3),

(N'Dark Choco Truffle', 35000, 0, '~/images/31.png', 1, 1, 1, 100, 4),

(N'Dây Đeo Điện Thoại Hình Nhân Vật Thiết Kế Theo Yêu Cầu',
 52000, 0, '~/images/42.png', 1, 1, 1, 100, 5),

(N'Túi Rút Bồ Hóng Ghibli', 65000, 10000, '~/images/47.png', 1, 1, 1, 100, 6),

(N'Rasberry Thumprint', 28000, 0, '~/images/13.png', 1, 1, 1, 100, 2),

(N'Pin Cài Hoạt Hình Ghibli', 72000, 0, '~/images/40.png', 1, 1, 1, 100, 5),

(N'Vòng Tay Misty Forest', 34000, 5000, '~/images/56.png', 1, 1, 1, 100, 7),

(N'Bó Hoa Mini Màu Pastel Xinh Xắn', 68000, 0, '~/images/58.png', 1, 1, 1, 100, 6);


INSERT INTO Attributes (AttributeID, Name) VALUES
(1, N'Kích thước'),
(2, N'Màu sắc'),
(3, N'Chất liệu');

INSERT INTO AttributesPrices (AttributesPriceID, AttributeID, ProductID, Price, Active) VALUES
(1, 1, 1, 0, 1),
(2, 2, 1, 2000, 1),
(3, 3, 5, 5000, 1);

INSERT INTO Customers (FullName, Email, Phone, Password, Active) VALUES
(N'Nguyễn Văn A', 'a@gmail.com', '0901111111', '123456', 1),
(N'Trần Thị B', 'b@gmail.com', '0902222222', '123456', 1);

-- Đồng nhất TransacStatus theo chuẩn 1..5 (an toàn, chạy nhiều lần không lỗi)

MERGE dbo.TransacStatus AS t
USING (VALUES
    (1, N'Pending',    N'Đang chờ xử lý'),
    (2, N'Processing', N'Đang xử lý'),
    (3, N'Shipped',    N'Đã giao hàng'),
    (4, N'Completed',  N'Hoàn thành'),
    (5, N'Cancelled',  N'Đã hủy')
) AS s(TransactStatusID, Status, Description)
ON t.TransactStatusID = s.TransactStatusID
WHEN MATCHED THEN
    UPDATE SET
        t.Status = s.Status,
        t.Description = s.Description
WHEN NOT MATCHED THEN
    INSERT (TransactStatusID, Status, Description)
    VALUES (s.TransactStatusID, s.Status, s.Description);

INSERT INTO OrderDetails (OrderDetailID, OrderID, ProductID, Quantity, Total) VALUES
(1, 1, 1, 2, 70000),
(2, 1, 3, 1, 35000),
(3, 2, 5, 1, 52000);
