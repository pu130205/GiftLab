MERGE dbo.Roles AS t
USING (VALUES
    (1, N'Admin',    N'Quản trị hệ thống'),
    (2, N'Staff',    N'Nhân viên'),
    (3, N'Customer', N'Khách hàng')
) AS s(RoleID, RoleName, Description)
ON t.RoleID = s.RoleID
WHEN MATCHED THEN
    UPDATE SET
        t.RoleName = s.RoleName,
        t.Description = s.Description
WHEN NOT MATCHED THEN
    INSERT (RoleID, RoleName, Description)
    VALUES (s.RoleID, s.RoleName, s.Description);
GO

MERGE dbo.Accounts AS t
USING (VALUES
    (1, '0900000001', 'admin@giftlab.com', '123456', 1, N'Admin GiftLab', 1),
    (2, '0900000002', 'staff@giftlab.com', '123456', 1, N'Staff GiftLab', 2)
) AS s(AccountID, Phone, Email, Password, Active, Fullname, RoleID)
ON t.AccountID = s.AccountID
WHEN MATCHED THEN
    UPDATE SET
        t.Phone    = s.Phone,
        t.Email    = s.Email,
        t.Password = s.Password,
        t.Active   = s.Active,
        t.Fullname = s.Fullname,
        t.RoleID   = s.RoleID
WHEN NOT MATCHED THEN
    INSERT (AccountID, Phone, Email, Password, Active, Fullname, RoleID, CreateDate)
    VALUES (s.AccountID, s.Phone, s.Email, s.Password, s.Active, s.Fullname, s.RoleID, GETDATE());
GO

MERGE dbo.Categories AS t
USING (VALUES
    (1, N'Cupcake', NULL, NULL, 1, NULL, 'cupcake'),
    (2, N'Cookie', NULL, NULL, 1, NULL, 'cookie'),
    (3, N'Tart', NULL, NULL, 1, NULL, 'tart'),
    (4, N'Chocolate', NULL, NULL, 1, NULL, 'chocolate'),
    (5, N'Đất màu', NULL, NULL, 1, NULL, 'dat-mau'),
    (6, N'Len mềm', NULL, NULL, 1, NULL, 'len-mem'),
    (7, N'Hạt cườm', NULL, NULL, 1, NULL, 'hat-cuom')
) AS s(CatID, Catname, Description, ParentID, Published, Thumb, Alias)
ON t.CatID = s.CatID
WHEN MATCHED THEN
    UPDATE SET
        t.Catname = s.Catname,
        t.Description = s.Description,
        t.ParentID = s.ParentID,
        t.Published = s.Published,
        t.Thumb = s.Thumb,
        t.Alias = s.Alias
WHEN NOT MATCHED THEN
    INSERT (CatID, Catname, Description, ParentID, Published, Thumb, Alias)
    VALUES (s.CatID, s.Catname, s.Description, s.ParentID, s.Published, s.Thumb, s.Alias);
GO

----INSERT HAT CUOM
INSERT INTO Products
(ProductName, ShortDesc, Description, Price, Discount, Thumb, BestSellers, HomeFlag, Active, UnitsInStock, CatID)
VALUES
(
    N'Vòng tay Blue Shell Garden',
    N'🐚 Vòng tay dây rút với hạt xanh – trắng, điểm nhấn là charm vỏ sò biển.',
    N'🌊 Mix nhiều hạt thủy tinh xanh navy, xanh pastel, hình hoa và hạt tròn trong suốt.'
    + CHAR(13) + CHAR(10) +
    N'🐚 Ở trung tâm là vỏ sò trắng và một vài hạt xanh lá nhỏ, gợi cảm hứng “khu vườn bên bờ biển”.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Dây trắng có nút rút điều chỉnh, phù hợp nhiều size tay, dễ phối với outfit đi biển, picnic.',
    35000, 4000, '~/images/54.png', 1, 1, 1, 100, 7
),
(
    N'Vòng cổ Mermaid Dream',
    N'⭐ Dây hạt tông xanh biển – ngọc trai với charm đuôi cá và ngôi sao.',
    N'⭐ Kết hợp hạt tròn ánh ngọc trai, hạt trái tim trắng, hạt sao xanh và vài hạt trong suốt lấp lánh.'
    + CHAR(13) + CHAR(10) +
    N'🧜‍♀️ Điểm xuyết charm đuôi cá màu xanh lá và charm bướm/cánh thiên thần đầu dây, thích hợp đeo cổ, làm dây điện thoại hoặc dây đeo máy ảnh mini.',
    55000, 4000, '~/images/53.png', 1, 1, 1, 100, 7
),
(
    N'Vòng tay Deep Blue Dream',
    N'💙 Vòng tay tông xanh biển trong trẻo như mặt nước mùa hè.',
    N'💙 Gồm hạt tròn trắng, xanh pastel và hoa 5 cánh trong suốt, bố cục xen kẽ tạo cảm giác nhẹ nhàng.'
    + CHAR(13) + CHAR(10) +
    N'🌊 Phối trên nền dây trắng có thể kéo dãn, rất hợp với outfit trắng – xanh, phong cách “ocean girl”.',
    39000, 6000, '~/images/55.png', 1, 1, 1, 100, 7
),
(
    N'Vòng tay Misty Forest',
    N'🌿 Vòng tay xanh lá – xanh dương gợi nhớ khu rừng sương mờ.',
    N'🌿 Hạt hoa và lá nhỏ màu xanh mint, xanh ngọc, xen kẽ hạt trắng trong như giọt sương.'
    + CHAR(13) + CHAR(10) +
    N'🍃 Trung tâm là bông hoa lớn hơn, tạo điểm nhấn, phù hợp cho những bạn thích vibe thiên nhiên, rừng cây, camping.',
    39000, 6000, '~/images/56.png', 1, 1, 1, 100, 7
),
(
    N'Vòng tay Sakura Bloom',
    N'🌸 Vòng tay hạt tông hồng – kem lấy cảm hứng từ hoa anh đào.',
    N'🌿 Hạt hoa và lá nhỏ màu xanh mint, xanh ngọc, xen kẽ hạt trắng trong như giọt sương.'
    + CHAR(13) + CHAR(10) +
    N'💗 Dây kem ấm áp, tổng thể rất hợp với váy tiểu thư, outfit hồng pastel hoặc làm quà cho người yêu “vibe Nhật Bản dịu dàng”.',
    39000, 6000, '~/images/57.png', 1, 1, 1, 100, 7
);
GO

----INSERT 5 CHOCOLATE
INSERT INTO Products
(ProductName, ShortDesc, Description, Price, Discount, Thumb, BestSellers, HomeFlag, Active, UnitsInStock, CatID)
VALUES
(
    N'Dark Choco Truffle',
    N'🍫 Truffle chocolate đen bóng với nhân ganache cacao đậm vị.',
    N'🍫 Vỏ chocolate đen phủ ngoài, bề mặt láng mịn, tạo cảm giác sang trọng.'
    + CHAR(13) + CHAR(10) +
    N'💎 Bên trong là lớp ganache cacao đậm, hơi đăng đắng, tan chảy chậm trong miệng.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp cho khách yêu vị chocolate nguyên bản, ít ngọt, dùng kèm rượu vang đỏ hoặc espresso.',
    29000, 4000, '~/images/31.png', 1, 1, 1, 100, 4
),
(
    N'Oranges & Creamsicle',
    N'🍫 Viên chocolate cam – kem gợi nhớ vị kem que Cam Sữa.',
    N'🍊 Vỏ chocolate màu caramel/cam nhạt, vẽ đường swirl trắng vui mắt.'
    + CHAR(13) + CHAR(10) +
    N'🍦 Nhân kem ganache vị cam sữa, thơm mùi vỏ cam, béo nhưng tươi mát.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Rất hợp cho các hộp quà mùa hè, khách thích vị citrus dịu và không quá đắng.',
    29000, 4000, '~/images/32.png', 1, 1, 1, 100, 4
),
(
    N'Aztec Spice Truffle',
    N'🍫 Truffle cacao pha gia vị ấm kiểu Aztec',
    N'🍫 Lớp áo cacao/milk chocolate mịn, phủ bột cacao nhẹ bên ngoài.'
    + CHAR(13) + CHAR(10) +
    N'🌶 Nhân ganache chocolate đậm xen chút cay nhẹ (quế, ớt, tiêu) tạo hậu vị ấm nồng'
    + CHAR(13) + CHAR(10) +
    N'❤️ Rất hợp cho người thích trải nghiệm độc đáo, khác lạ so với chocolate truyền thống.',
    29000, 4000, '~/images/33.png', 1, 1, 1, 100, 4
),
(
    N'Strawberry Creme',
    N'🍫 Viên chocolate hồng vị dâu với nhân kem trắng và lớp chocolate đen bên trong.',
    N'🍓 Vỏ chocolate màu hồng dâu, có đường kẻ mảnh tạo hiệu ứng hiện đại.'
    + CHAR(13) + CHAR(10) +
    N'🍮 Bên trong là lớp kem trắng béo nhẹ nằm trên nền chocolate/dòng coulis dâu sẫm.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp cho set quà Valentine, 8/3 hoặc mọi dịp tặng người thích vị dâu ngọt ngào.',
    29000, 4000, '~/images/34.png', 1, 1, 1, 100, 4
),
(
    N'Double Choco Raspberry',
    N'🍫 Chocolate đen nhân raspberry chocolate kép chua ngọt.',
    N'🍫 Vỏ chocolate đen tròn, trang trí một đường ruy-băng raspberry đỏ trên bề mặt.'
    + CHAR(13) + CHAR(10) +
    N'🍇 Nhân ganache/ch Coulis raspberry hòa cùng chocolate tạo vị chua nhẹ cân bằng độ đắng.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Thích hợp cho người thích berry nhưng vẫn muốn giữ hương cacao mạnh mẽ.',
    29000, 4000, '~/images/35.png', 1, 1, 1, 100, 4
);
GO

----INSERT CUPCAKE
INSERT INTO Products
(ProductName, ShortDesc, Description, Price, Discount, Thumb, BestSellers, HomeFlag, Active, UnitsInStock, CatID)
VALUES
(
    N'Rocky Road',
    N'🧁 Cupcake chocolate phủ kem, marshmallow và sốt chocolate – trắng, lấy cảm hứng từ kem Rocky Road.',
    N'🧁 Cốt bánh chocolate ẩm, màu nâu đậm, làm nền cho lớp topping “siêu đầy”.'
    + CHAR(13) + CHAR(10) +
    N'🍫 Kem chocolate bông mịn, xen lẫn sốt chocolate và caramel tạo hiệu ứng xoáy.'
    + CHAR(13) + CHAR(10) +
    N'☁️ Phủ marshmallow mini, lát hạnh nhân/đậu phộng rang và rưới sốt chocolate trắng như “đá lở” trên đỉnh.'
    + CHAR(13) + CHAR(10) +
    N'❤️Thích hợp cho khách thích vị béo ngọt phong phú và phong cách trang trí cực kỳ nổi bật trên bàn tiệc.',
    35000, 5000, '~/images/1.png', 1, 1, 1, 100, 1
),
(
    N'Blush Cherry Whip',
    N'🧁 Cupcake vani phủ kem hồng trắng và anh đào tươi trên cùng',
    N'🧁 Cốt bánh vani mềm ẩm, xen lẫn vụn anh đào nhẹ nhàng.'
    + CHAR(13) + CHAR(10) +
    N'🍒 Topping kem bơ xoáy hai màu trắng – hồng, rắc đường màu và hạt trang trí.'
    + CHAR(13) + CHAR(10) +
    N'☁️ Phủ marshmallow mini, lát hạnh nhân/đậu phộng rang và rưới sốt chocolate trắng như “đá lở” trên đỉnh.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Thích hợp cho tiệc sinh nhật, lễ tình nhân hoặc những dịp muốn tạo cảm giác “ngọt ngào, dịu dàng',
    45000, 4000, '~/images/2.png', 1, 1, 1, 100, 1
),
(
    N'Red Velvet Heart',
    N'🧁 Cupcake red velvet với toppper trái tim đỏ.',
    N'🧁 Cốt bánh red velvet đỏ sẫm, mềm mịn, mùi cacao nhẹ.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Kem phô mai trắng mịn, rắc vụn bánh đỏ và một topper hình trái tim ở giữa.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Lựa chọn lý tưởng cho ngày Valentine, kỷ niệm, hoặc set quà “love theme”.',
    35000, 4000, '~/images/3.png', 1, 1, 1, 100, 1
),
(
    N'Raspberry Chocolate',
    N'🧁 Cupcake chocolate đậm phủ kem mâm xôi hồng.',
    N'🧁 Cốt bánh chocolate ẩm, màu nâu đậm sang trọng.'
    + CHAR(13) + CHAR(10) +
    N'🍓 Kem mâm xôi hồng phớt, điểm vài hạt chocolate hoặc kẹo tròn trên mặt.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Hoàn hảo cho khách thích vị chua nhẹ của berry trên nền chocolate béo.',
    36000, 4000, '~/images/4.png', 1, 1, 1, 100, 1
),
(
    N'Coconut Whisper',
    N'🧁 Cupcake dừa với kem trắng và dừa nạo.',
    N'🧁 Cốt bánh vị dừa, nhẹ nhàng, hơi béo nhưng không ngấy.'
    + CHAR(13) + CHAR(10) +
    N'🥥 Kem bơ trắng, rắc dừa sấy/nạo lên trên, tạo cảm giác “tuyết dừa” mềm.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp với concept nhiệt đới, biển đảo, hoặc các set quà summer-refresh.',
    36000, 4000, '~/images/5.png', 1, 1, 1, 100, 1
),
(
    N'Ginger Jolly',
    N'🧁 Cupcake gừng – gia vị ấm áp với trang trí người bánh gừng và tuyết.',
    N'🧁 Cốt bánh vị gừng, quế và mật đường, thơm đậm chất Giáng Sinh.'
    + CHAR(13) + CHAR(10) +
    N'🎄 Kem màu xanh nhạt, điểm xuyết hạt đường đỏ – trắng, bánh quy người tuyết gừng và bông tuyết chocolate.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Lý tưởng cho tiệc Noel, mùa đông hoặc các set quà lễ hội ấm cúng.',
    39000, 48000, '~/images/6.png', 1, 1, 1, 100, 1
),
(
    N'Chocolate Chips',
    N'🧁 Cupcake vani kem trắng rắc chip chocolate và cookie nhỏ.',
    N'🧁 Cốt bánh vani hoặc bơ sữa, nhẹ và thơm.'
    + CHAR(13) + CHAR(10) +
    N'🍪 Kem bơ vanilla, topping nhiều chip chocolate và một miếng cookie/viên dough trên đỉnh.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Rất hợp cho trẻ em và fan của cookie & cream.',
    36000, 4000, '~/images/7.png', 1, 1, 1, 100, 1
),
(
    N'Banana Caramel',
    N'🧁 Cupcake chuối phủ caramel và hạt giòn.',
    N'🧁 Cốt bánh vị chuối chín, mềm ẩm, thơm tự nhiên.'
    + CHAR(13) + CHAR(10) +
    N'🍌 Kem bơ vani, topping lát chuối tươi, sốt caramel và viền hạt caramel/bánh hạnh giòn.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Rất hợp với người thích hương vị trái cây kết hợp caramel béo ngọt.',
    36000, 4000, '~/images/8.png', 1, 1, 1, 100, 1
),
(
    N'Lemon Bar',
    N'🧁 Cupcake chanh vàng với kem chanh chua ngọt thanh.',
    N'🧁 Cốt bánh vị chanh nhẹ, màu vàng tươi sáng.'
    + CHAR(13) + CHAR(10) +
    N'🍋 Kem chanh bông xốp, trên cùng là miếng chanh candied/gel và lớp đường mịn tạo hiệu ứng “phủ sương”.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Thích hợp cho những ai thích vị chua mát, chống ngấy trong bàn tiệc ngọt.',
    36000, 4000, '~/images/9.png', 1, 1, 1, 100, 1
),
(
    N'Blueberry Pie',
    N'🧁 Cupcake việt quất với kem tím và trái việt quất tươi.',
    N'🧁 Cốt bánh vani trộn blueberry, tạo vân tím xanh bên trong.'
    + CHAR(13) + CHAR(10) +
    N'🧁 Kem bơ màu tím nhạt, trang trí 2–3 quả việt quất tươi trên cùng.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Gợi cảm giác như mini pie việt quất, hợp với concept “berry garden” hoặc tiệc trà.',
    35000, 5000, '~/images/10.png', 1, 1, 1, 100, 1
);
GO

----INSERT DAT MAU
INSERT INTO Products
(ProductName, ShortDesc, Description, Price, Discount, Thumb, BestSellers, HomeFlag, Active, UnitsInStock, CatID)
VALUES
(
    N'BST Đất Màu – Trái cây & Rau củ',
    N'✨ Bộ sưu tập đất màu hình trái cây – rau củ siêu dễ thương.',
    N'🍎 Gồm kiwi, táo, dâu, cà rốt, củ cải… với tông màu tươi sáng.'
    + CHAR(13) + CHAR(10) +
    N'✨ Bề mặt phủ bóng, bo tròn mép, an toàn khi dán sổ, case, khay trang trí.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp cho decor góc học tập, journal, làm quà nhỏ cho các “bé mê cute”.',
    89000, 9000, '~/images/39.png', 1, 1, 1, 100, 5
),
(
    N'Phone Hipper hình Smiski dễ thương',
    N'✨ Smiski đeo tai nghe ngáo ngơ đáng yêu.',
    N'💚 Tạo hình nhân vật tròn trịa, đeo “tai nghe”, biểu cảm hơi ngơ đáng yêu.'
    + CHAR(13) + CHAR(10) +
    N'🎨 Có thể đặt làm theo màu, thêm phụ kiện (nón, balo…) hoặc đổi biểu cảm.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Dùng để trưng bàn làm việc, chụp hình cùng đồ uống, làm quà tặng cá nhân hóa.',
    45000, 4000, '~/images/41.1.png', 1, 1, 1, 100, 5
),
(
    N'BST Đất Màu – Hoạt Hình Ghibli',
    N'✨ Nhân vật hoạt hình Ghibli như bồ hóng, vô diện,...',
    N'🔥 Nhiều biểu cảm ngọn lửa sinh động và đáng yêu.'
    + CHAR(13) + CHAR(10) +
    N'🎨 Nét vẽ thủ công, màu đậm, hợp fan phim/animation, dùng dán ốp, laptop hoặc làm charm'
    + CHAR(13) + CHAR(10) +
    N'❤️ Dùng để trưng bàn làm việc, chụp hình cùng đồ uống, làm quà tặng cá nhân hóa.',
    89000, 8000, '~/images/40.png', 1, 1, 1, 100, 5
),
(
    N'BST Đất Màu – Cún Mèo đáng yêu',
    N'✨ Bộ thú cưng đất màu gồm cún, mèo,... nhỏ nhắn và đáng yêu.',
    N'🐶 Các dáng thú dài, lùn, tròn với biểu cảm ngố, buồn, đáng yêu.'
    + CHAR(13) + CHAR(10) +
    N'🎨 Phong cách vẽ mềm, màu pastel, rất hợp decor journal, ốp điện thoại, hộp quà cho người yêu pet.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Dùng để trưng bàn làm việc, chụp hình cùng đồ uống, làm quà tặng cá nhân hóa.',
    92000, 7000, '~/images/38.png', 1, 1, 1, 100, 5
),
(
    N'Badge Đất Màu – Chibi Custom',
    N'✨ Pin cài nhân vật chibi thiết kế theo yêu cầu.',
    N'😺 Mỗi badge là một gương mặt chibi với tóc, phụ kiện (mèo đội đầu…), biểu cảm khác nhau.'
    + CHAR(13) + CHAR(10) +
	N'❤️ Phù hợp làm quà fanart, quà sinh nhật, quà lưu niệm.'
    + CHAR(13) + CHAR(10) +
    N'✍️ GifLab nhận vẽ theo nhân vật game/anime hoặc phong cách riêng của khách. Vui lòng liên hệ GiftLab để chúng mình tư vấn cho bạn nhé!',
    59000, 0, '~/images/43.1.png', 1, 1, 1, 100, 5
),
(
    N'Charm Đất Màu – Chibi Custom',
    N'✨ Charm chibi thiết kế theo yêu cầu.',
    N'😺 Mỗi charm là một gương mặt chibi với tóc, phụ kiện (mèo đội đầu…), biểu cảm khác nhau.'
    + CHAR(13) + CHAR(10) +
	 N'❤️ Phù hợp làm quà fanart, quà sinh nhật, quà lưu niệm.'
    + CHAR(13) + CHAR(10) +
    N'✍️ GifLab nhận vẽ theo nhân vật game/anime hoặc phong cách riêng của khách. Vui lòng liên hệ GiftLab để chúng mình tư vấn cho bạn nhé!',
    55000, 0, '~/images/42.png', 1, 1, 1, 100, 5
);
GO

----INSERT 8 COOKIE
INSERT INTO Products
(ProductName, ShortDesc, Description, Price, Discount, Thumb, BestSellers, HomeFlag, Active, UnitsInStock, CatID)
VALUES
(
    N'Melten Lava',
    N'🍪 Cookie chocolate nhân caramel lỏng tràn như dung nham.',
    N'🍪 Vỏ bánh chocolate mềm, phủ nhẹ đường hạt tạo hiệu ứng lấp lánh.'
    + CHAR(13) + CHAR(10) +
    N'🍯 Nhân caramel muối ở giữa, khi bẻ đôi caramel chảy xuống đầy hấp dẫn.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Lý tưởng làm “signature cookie” cho set quà cao cấp hoặc dessert đặc biệt.',
    25000, 4000, '~/images/11.png', 1, 1, 1, 100, 2
),
(
    N'Chocolate Chunk',
    N'🍪 Cookie bơ mềm với những khối chocolate chảy ở giữa.',
    N'🍪 Bánh quy bơ vàng, rìa hơi giòn nhưng tâm mềm ẩm.'
    + CHAR(13) + CHAR(10) +
    N'🍫 Trộn các khối chocolate đen to, khi bẻ đôi sẽ chảy thành lớp nhân sánh mịn.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp cho người thích kiểu “chewy cookie” Mỹ, ăn cùng sữa hoặc cà phê.',
    25000, 4000, '~/images/12.png', 1, 1, 1, 100, 2
),
(
    N'Raspberry Thumbprint',
    N'🍪 Cookie bơ với mứt mâm xôi đỏ ở giữa.',
    N'🍪 Bánh bơ tròn nhỏ, kết cấu giòn nhẹ, bề mặt hơi nứt tự nhiên.'
    + CHAR(13) + CHAR(10) +
    N'🍓 Giữa bánh là mứt raspberry đỏ óng, chua ngọt, nhìn như viên ngọc.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp set trà chiều, tiệc Giáng Sinh hoặc các hộp quà handmade.',
    25000, 4000, '~/images/13.png', 1, 1, 1, 100, 2
),
(
    N'Matcha Red Bean',
    N'🍪 Cookie trà xanh nhân đậu đỏ dẻo, phong cách Nhật.',
    N'🍪 Cốt cookie mềm màu xanh matcha, hơi ẩm, vị trà xanh nhẹ đắng.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Bên trong là lớp nhân đậu đỏ ngọt dẻo, thêm vài hạt đậu đỏ nguyên hạt trên mặt.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Rất hợp cho khách yêu hương vị Á Đông, muốn cookie bớt ngọt và thơm trà.',
    25000, 4000, '~/images/14.png', 1, 1, 1, 100, 2
),
(
    N'Florentine Almond',
    N'🍪 Cookie bơ giòn thơm, phủ đầy hạnh nhân nướng vàng kiểu Florentine.',
    N'🍪 Đế cookie bơ vàng óng, giòn rụm bên ngoài nhưng vẫn hơi mềm ở giữa.'
    + CHAR(13) + CHAR(10) +
    N'🌰 Topping hạnh nhân lát và nguyên hạt được rang tới khi vàng thơm, phủ kín bề mặt bánh.'
    + CHAR(13) + CHAR(10) +
	N'🍯 Áo nhẹ lớp caramel/đường nâu giúp hạnh nhân bám chặt, tạo vị ngọt bùi và bóng đẹp.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp cho các set quà cao cấp, teabreak hoặc khách thích vị bùi béo của các loại hạt.',
    25000, 4000, '~/images/15.png', 1, 1, 1, 100, 2
),
(
    N'Vanilla Cream Sandwich',
    N'🍪 Bánh quy kẹp kem vanilla trắng mềm mịn.',
    N'🍪 Hai lớp cookie bơ tròn, giòn nhẹ, khứa họa tiết hoa cổ điển.'
    + CHAR(13) + CHAR(10) +
    N'🍦 Nhân kem vanilla béo nhẹ, bắt tràn ra viền tạo hiệu ứng “sandwich kem” hấp dẫn.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp cho set trà chiều.',
    25000, 4000, '~/images/17.png', 1, 1, 1, 100, 2
),
(
    N'Levain-Style Cookie',
    N'🍪 Cookie dày kiểu New York với chocolate chunk và hạt.',
    N'🍪 Bánh dày, bề mặt nứt tự nhiên, bên trong ẩm mềm, kiểu “chewy gooey”.'
    + CHAR(13) + CHAR(10) +
    N'🍫 Trộn nhiều miếng chocolate đen to cùng hạt (óc chó/hạnh nhân) cho cảm giác nhai “đã miệng”.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp cho set trà chiều.',
    25000, 4000, '~/images/18.png', 1, 1, 1, 100, 2
),
(
    N'Triple Chocolate Cookie',
    N'🍪 Cookie ba loại chocolate trên nền cacao đậm.',
    N'🍪 Cốt cookie chocolate sẫm, dày và mềm, xen vân cacao như brownie.'
    + CHAR(13) + CHAR(10) +
    N'🍫 Mix chocolate trắng, sữa và đen thành nhiều khối vuông lớn, rắc thêm chút muối hạt để cân vị ngọt.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Phù hợp cho set trà chiều.',
    25000, 4000, '~/images/19.png', 1, 1, 1, 100, 2
);
GO

----INSERT 6 TART
INSERT INTO Products
(ProductName, ShortDesc, Description, Price, Discount, Thumb, BestSellers, HomeFlag, Active, UnitsInStock, CatID)
VALUES
(
    N'Cherry Lychee Cream',
    N'❤️ Tart vuông mỏng với kem vải – cherry hồng nhẹ và hai quả cherry đỏ mọng trên mặt.',
    N'🥧 Đế tart/bánh quy bơ hình chữ nhật, nướng vàng giòn, dày vừa phải để dễ cầm ăn.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Lớp kem trắng vani bắt thành chóp nhỏ, phía trên là kem hồng vị vải – cherry mềm mịn.'
    + CHAR(13) + CHAR(10) +
    N'🍒 Trên cùng là hai quả cherry đỏ bóng nổi bật, tạo điểm nhấn “ngọt ngào – lãng mạn”, rất hợp cho set quà hoặc tiệc trà phong cách nữ tính.',
    59000, 9000, '~/images/25.png', 1, 1, 1, 100, 3
),
(
    N'Grape Mint Garden',
    N'❤️ Tart nho xanh mát lạnh với kem phô mai nhẹ và lá bạc hà.',
    N'🥧 Đế tart bơ nướng vàng, giòn nhẹ.'
    + CHAR(13) + CHAR(10) +
    N'🍇 Nhân kem phô mai hoặc custard vani, phủ đầy nho xanh giòn ngọt.'
    + CHAR(13) + CHAR(10) +
    N'🌿 Trang trí lá bạc hà tươi và rắc đường bột tạo hiệu ứng “vườn nho phủ sương”.',
    59000, 9000, '~/images/26.png', 1, 1, 1, 100, 3
),
(
    N'Strawberry Rose Cream',
    N'❤️ Tart dâu tây với kem hoa hồng sang trọng, điểm vàng lá.',
    N'🥧 Đế tart bơ mỏng, giòn tan.'
    + CHAR(13) + CHAR(10) +
    N'🌹 Lớp kem mousse/whipped cream hương hoa hồng tạo hình xoáy như bông hồng.'
    + CHAR(13) + CHAR(10) +
    N'🍓 Viền dâu tươi cắt lát cùng vài mảnh vàng lá ăn được, rất hợp concept cao cấp – quà tặng.',
    59000, 9000, '~/images/27.png', 1, 1, 1, 100, 3
),
(
    N'Berry Puff',
    N'❤️ Bánh su kem phủ berry tươi rực rỡ.',
    N'💨 Vỏ su phồng vàng, bề mặt hơi nứt, phủ lớp đường bột mỏng.'
    + CHAR(13) + CHAR(10) +
    N'🍦 Nhân kem lạnh/whipped cream béo nhẹ, bắt trên mặt như “chiếc gối” cho trái cây.'
    + CHAR(13) + CHAR(10) +
    N'🍓 Trang trí dâu, việt quất và lá xanh tươi, tạo cảm giác tươi mới, hợp tiệc ngoài trời.',
    59000, 9000, '~/images/28.png', 1, 1, 1, 100, 3
),
(
    N'Matcha Choux Blossom',
    N'❤️ Tart matcha với kem choux tạo hình bông hoa, điểm cánh hoa khô',
	N'🥧 Đế tart vàng cùng lớp nhân matcha custard hoặc ganache xanh nhạt.'
    + CHAR(13) + CHAR(10) +
    N'💨 Vỏ su phồng vàng, bề mặt hơi nứt, phủ lớp đường bột mỏng.'
    + CHAR(13) + CHAR(10) +
    N'🌸 Kem choux hai tông trắng – xanh, bắt xoắn như cánh hoa nở trên mặt bánh.'
    + CHAR(13) + CHAR(10) +
    N'🌸 Rắc cánh hoa hồng khô/hạt vụn, rất hợp concept Nhật – fusion, dùng cho các box quà tinh tế.',
    59000, 9000, '~/images/29.png', 1, 1, 1, 100, 3
),
(
    N'Vanilla Choux',
    N'❤️ Tart choux vani với kem trắng mịn và topping hạt – kẹo vàng.',
    N'🥧 Đế tart tròn nhỏ, phía trên là lớp caramel/mứt mỏng tạo độ bóng.'
    + CHAR(13) + CHAR(10) +
    N'🧁 Kem vani đánh bông, bắt xoáy cao, kèm viên choux mini hoặc kẹo cầu vàng trên đỉnh.'
    + CHAR(13) + CHAR(10) +
    N'❤️ Rắc thêm hạt giòn/nhân hạnh để tăng texture, phù hợp set trà chiều thanh lịch.',
    59000, 4000, '~/images/30.png', 1, 1, 1, 100, 3
);
GO

----INSERT LEN MEM
INSERT INTO Products
(ProductName, ShortDesc, Description, Price, Discount, Thumb, BestSellers, HomeFlag, Active, UnitsInStock, CatID)
VALUES
(
    N'Phụ kiện len trang trí tai nghe mầm lá xanh nhỏ xinh',
    N'🎧 Chiếc lá len mini gắn vào tai nghe của bạn',
    N'🌿 Dây len gắn 2 chiếc lá nhỏ xinh vào tai nghe.'
    + CHAR(13) + CHAR(10) +
    N'🎧 Phù hợp cho các dòng tai nghe chụp tai, giúp tạo điểm nhấn dễ thương khi chụp ảnh, đi cà phê, học online.',
    19000,0, '~/images/36.png', 1, 1, 1, 100, 6
),
(
    N'Bồ Hóng Ngồi Xích Đu',
    N'✨ Nhân vật bồ hóng Ghibli ngồi xích đu dây leo màu xanh đáng yêu.',
    N'🌱 Dây đeo là hai sợi “dây leo” có lá, có thể treo trên balo, tay nắm tủ, gương, xe máy.',
    39000, 6000, '~/images/48.png', 1, 1, 1, 100, 6
),
(
    N'Túi Rút Bồ Hóng',
    N'✨ Túi rút len hình bồ hóng mắt to, đính sao nhỏ.',
    N'🎒 Kích thước vừa cho son, tiền lẻ, cáp sạc; dây rút hai bên kết thúc bằng chùm len nhỏ.'
    + CHAR(13) + CHAR(10) +
    N'✨ Mặt túi thêu 2 mắt trắng và vài ngôi sao pastel, rất hợp fan phong cách Ghibli/cute.',
    79000,0, '~/images/47.png', 1, 1, 1, 100, 6
),
(
    N'Bồ Hóng Cài Hoa',
    N'✨ Móc khóa len bồ hóng mắt to cài hoa đáng yêu.',
    N'🌼 Bồ hóng nhỏ xinh, đội bông hoa cúc trắng nhỏ xinh trên đầu.'
    + CHAR(13) + CHAR(10) +
    N'📎 Có khoen treo phía trên, dễ gắn lên balo, túi tote, chìa khóa hoặc decor trong xe.',
    34000,5000, '~/images/46.png', 1, 1, 1, 100, 6
),
(
    N'Móc Khóa Hoa Tulip',
    N'✨ Hoa Tulip nhỏ xinh làm bằng len',
    N'✨ Hoa Tulip nhỏ xinh làm bằng len'
    + CHAR(13) + CHAR(10) +
    N'🔗 Có vòng len có thể dùng làm charm túi tote, móc khóa, trang trí móc khóa ví.',
    25000,4000, '~/images/52.png', 1, 1, 1, 100, 6
),
(
    N'Móc Khóa Hoa Anh Đào',
    N'🌸 Cặp móc khóa hoa sakura pastel, combo 2 chiếc',
    N'🌸 Mỗi bông 5 cánh, một bông hồng nhạt, một hồng đậm, đính lá móc màu xanh.'
    + CHAR(13) + CHAR(10) +
    N'🔗 Có vòng len có thể dùng làm charm túi tote, móc khóa, trang trí móc khóa ví.',
    39000,7000, '~/images/50.png', 1, 1, 1, 100, 6
),
(
    N'Gấu Trắng Ôm Trái Tim',
    N'✨ Bộ gấu len trắng mũm mĩm ôm trái tim màu hồng và đỏ.',
    N'🐻 Thân gấu tròn, má hồng phúng phính, mỗi bé ôm một trái tim (hồng pastel, đỏ, hồng đậm) tạo cảm giác ngọt ngào.'
    + CHAR(13) + CHAR(10) +
    N'🔗 Có khoen kim loại để gắn vào chìa khóa, balo, túi xách; thích hợp làm quà tặng cặp đôi, bạn thân hoặc set 3 bé “gấu gia đình”.',
    69000,6000, '~/images/37.png', 1, 1, 1, 100, 6
),
(
    N'Móc Khóa Hoa Linh Lan',
    N'✨ Hoa Tulip nhỏ xinh làm bằng len',
    N'✨ Hoa Linh Lan nhỏ xinh làm bằng len'
    + CHAR(13) + CHAR(10) +
    N'🔗 Có vòng len có thể dùng làm charm túi tote, móc khóa, trang trí móc khóa ví.',
    25000,4000, '~/images/51.png', 1, 1, 1, 100, 6
),
(
    N'Bó Hoa Len mini màu pastel xinh xắn',
    N'✨ Bó hoa len pastel với nhiều bông nhỏ xinh, bền đẹp và không bao giờ héo.',
    N'💐 Gồm nhiều bông hoa 5 cánh màu trắng, vàng, hồng… đi kèm lá xanh và phần “giấy gói” cũng móc bằng len, buộc nơ hồng ở giữa.'
    + CHAR(13) + CHAR(10) +
    N'🌸 Phù hợp làm quà sinh nhật, tốt nghiệp, kỷ niệm. Có thể trưng trên bàn làm việc, kệ sách như một món decor mềm mại, ấm áp',
    55000,4000, '~/images/58.png', 1, 1, 1, 100, 6
),
(
    N'Hộp quà tặng ngẫu nhiên GiftLab',
    N'🎁 Hộp quà màu hồng – trắng kèm nơ lụa. Đóng gói các quà tặng ngẫu nhiên của GiftLab.',
    N'🎁 Thiết kế hộp vuông cứng cáp, tông hồng pastel, cột nơ ruy băng trắng phía trước, tạo cảm giác cao cấp nhưng vẫn dễ thương.'
    + CHAR(13) + CHAR(10) +
    N'✨ Dịch vụ đóng gói gồm lót giấy, sắp xếp sản phẩm len thành set quà hoàn chỉnh, sẵn sàng tặng ngay không cần gói thêm.',
    189000,4000, '~/images/59.png', 1, 1, 1, 100, 6
);
GO

--------------------------------------------------------------

MERGE dbo.Attributes AS t
USING (VALUES
    (1, N'Kích thước'),
    (2, N'Màu sắc'),
    (3, N'Chất liệu')
) AS s(AttributeID, Name)
ON t.AttributeID = s.AttributeID
WHEN MATCHED THEN
    UPDATE SET t.Name = s.Name
WHEN NOT MATCHED THEN
    INSERT (AttributeID, Name)
    VALUES (s.AttributeID, s.Name);
GO

-- ===== AttributesPrices (an toàn, không hardcode ProductID) =====
-- ===== AttributesPrices (an toàn, tự sinh ID) =====

DECLARE @p1 INT = (SELECT TOP 1 ProductID FROM dbo.Products WHERE ProductName = N'Rocky Road');
DECLARE @p5 INT = (SELECT TOP 1 ProductID FROM dbo.Products WHERE ProductName = N'Coconut Whisper');

-- Attribute: Kích thước + Màu sắc cho Rocky Road
IF @p1 IS NOT NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM dbo.AttributesPrices 
        WHERE AttributeID = 1 AND ProductID = @p1
    )
    BEGIN
        INSERT INTO dbo.AttributesPrices
        (AttributesPriceID, AttributeID, ProductID, Price, Active)
        VALUES
        (
            (SELECT ISNULL(MAX(AttributesPriceID), 0) + 1 FROM dbo.AttributesPrices),
            1, @p1, 0, 1
        );
    END

    IF NOT EXISTS (
        SELECT 1 FROM dbo.AttributesPrices 
        WHERE AttributeID = 2 AND ProductID = @p1
    )
    BEGIN
        INSERT INTO dbo.AttributesPrices
        (AttributesPriceID, AttributeID, ProductID, Price, Active)
        VALUES
        (
            (SELECT ISNULL(MAX(AttributesPriceID), 0) + 1 FROM dbo.AttributesPrices),
            2, @p1, 2000, 1
        );
    END
END

-- Attribute: Chất liệu cho Coconut Whisper
IF @p5 IS NOT NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM dbo.AttributesPrices 
        WHERE AttributeID = 3 AND ProductID = @p5
    )
    BEGIN
        INSERT INTO dbo.AttributesPrices
        (AttributesPriceID, AttributeID, ProductID, Price, Active)
        VALUES
        (
            (SELECT ISNULL(MAX(AttributesPriceID), 0) + 1 FROM dbo.AttributesPrices),
            3, @p5, 5000, 1
        );
    END
END
GO

MERGE dbo.Customers AS t
USING (VALUES
    (N'Nguyễn Văn A', 'a@gmail.com', '0901111111', '123456', 1),
    (N'Trần Thị B', 'b@gmail.com', '0902222222', '123456', 1),
    (N'uyen phuong', 'parkjiuyen@gmail.com', '0123456789', '123456', 1)
) AS s(FullName, Email, Phone, Password, Active)
ON t.Email = s.Email
WHEN MATCHED THEN
    UPDATE SET
        t.FullName = s.FullName,
        t.Phone = s.Phone,
        t.Password = s.Password,
        t.Active = s.Active
WHEN NOT MATCHED THEN
    INSERT (FullName, Email, Phone, Password, Active)
    VALUES (s.FullName, s.Email, s.Phone, s.Password, s.Active);
GO

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
GO
