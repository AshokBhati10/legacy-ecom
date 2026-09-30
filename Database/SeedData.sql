/* ============================================================================
   Legacy eCommerce — seed data
   Target : .\SQLEXPRESS / LegacyEcommerceDb

   Run AFTER CreateDatabase.sql:

       sqlcmd -S .\SQLEXPRESS -i SeedData.sql

   Seeds 6 categories, 12 products, product images and variants.
   Image URLs are site-relative placeholders — drop real product images into
   Ecommerce.Web\Content\images\products\ or update the URLs.
   ============================================================================ */

USE [LegacyEcommerceDb];
GO

/* ------------------------- Categories ------------------------------------ */

SET IDENTITY_INSERT [dbo].[Categories] ON;
GO

INSERT INTO [dbo].[Categories] ([Id], [Name], [Slug], [Description], [ParentCategoryId], [DisplayOrder], [IsActive]) VALUES
(1, N'Electronics', N'electronics', N'Phones, computers and gadgets.', NULL, 1, 1),
(2, N'Audio', N'audio', N'Headphones, speakers and sound.', 1, 2, 1),
(3, N'Home & Kitchen', N'home-kitchen', N'Appliances and kitchen essentials.', NULL, 3, 1),
(4, N'Sports & Outdoors', N'sports-outdoors', N'Fitness and outdoor gear.', NULL, 4, 1),
(5, N'Books', N'books', N'Books and collections.', NULL, 5, 1),
(6, N'Clothing', N'clothing', N'Apparel for every season.', NULL, 6, 1);
GO

SET IDENTITY_INSERT [dbo].[Categories] OFF;
GO

/* ------------------------- Products -------------------------------------- */

SET IDENTITY_INSERT [dbo].[Products] ON;
GO

INSERT INTO [dbo].[Products] ([Id], [Sku], [Name], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [CategoryId], [ThumbnailUrl], [IsActive], [IsFeatured], [StockQuantity], [CreatedDate]) VALUES
(1, N'AUD-HEAD-001', N'Wireless Headphones Pro', N'wireless-headphones-pro',
 N'Noise-cancelling over-ear headphones with 30-hour battery.',
 N'Premium over-ear wireless headphones with active noise cancellation, Bluetooth 5.0, 30-hour battery life and a folding design for travel.',
 149.99, 119.99, 2, N'/Content/images/products/headphones-pro-1.jpg', 1, 1, 42, GETDATE()),
(2, N'AUD-SPK-002', N'Portable Bluetooth Speaker', N'portable-bluetooth-speaker',
 N'Water-resistant portable speaker with deep bass.',
 N'Compact water-resistant Bluetooth speaker with 12-hour playtime, deep bass radiator and built-in microphone for calls.',
 59.99, NULL, 2, N'/Content/images/products/bluetooth-speaker-1.jpg', 1, 0, 85, GETDATE()),
(3, N'ELE-LAP-003', N'Ultrabook Laptop 14', N'ultrabook-laptop-14',
 N'14-inch ultrabook, 16GB RAM, 512GB SSD.',
 N'Thin-and-light 14-inch ultrabook with a full-HD display, 16GB RAM, 512GB SSD and all-day battery — built for work on the move.',
 999.99, NULL, 1, N'/Content/images/products/ultrabook-14-1.jpg', 1, 0, 18, GETDATE()),
(4, N'ELE-PHN-004', N'Smartphone X', N'smartphone-x',
 N'6.1-inch smartphone with dual camera.',
 N'Smartphone X with a 6.1-inch OLED display, dual rear camera, fast charging and 128GB of storage.',
 699.99, 649.99, 1, N'/Content/images/products/smartphone-x-1.jpg', 1, 1, 60, GETDATE()),
(5, N'HMK-CFM-005', N'Coffee Maker Deluxe', N'coffee-maker-deluxe',
 N'12-cup programmable coffee maker.',
 N'12-cup programmable coffee maker with thermal carafe, brew-strength control and auto-start timer.',
 89.99, NULL, 3, N'/Content/images/products/coffee-maker-1.jpg', 1, 0, 37, GETDATE()),
(6, N'HMK-KNF-006', N'Chef''s Knife Set (5 pc)', N'chefs-knife-set',
 N'Five-piece German steel knife set.',
 N'Five-piece chef''s knife set in high-carbon German steel with ergonomic handles and a wooden storage block.',
 129.99, NULL, 3, N'/Content/images/products/knife-set-1.jpg', 1, 0, 25, GETDATE()),
(7, N'SPO-YGA-007', N'Premium Yoga Mat', N'premium-yoga-mat',
 N'Extra-thick non-slip yoga mat.',
 N'6mm extra-thick non-slip yoga mat with alignment lines, carrying strap and sweat-resistant surface.',
 29.99, NULL, 4, N'/Content/images/products/yoga-mat-1.jpg', 1, 0, 120, GETDATE()),
(8, N'SPO-RUN-008', N'Road Running Shoes', N'road-running-shoes',
 N'Cushioned road running shoes.',
 N'Lightweight cushioned road running shoes with breathable mesh upper and responsive foam midsole.',
 119.99, 99.99, 4, N'/Content/images/products/running-shoes-1.jpg', 1, 1, 54, GETDATE()),
(9, N'SPO-DMB-009', N'Dumbbell Set 20kg', N'dumbbell-set-20kg',
 N'Adjustable 20kg dumbbell pair.',
 N'Pair of adjustable dumbbells up to 20kg each with knurled steel handles and quick-change plates.',
 79.99, NULL, 4, N'/Content/images/products/dumbbell-set-1.jpg', 1, 0, 30, GETDATE()),
(10, N'BOK-CLS-010', N'Classic Novel Collection', N'classic-novel-collection',
 N'Ten timeless classics in hardcover.',
 N'Boxed set of ten timeless classic novels in hardcover with ribbon markers — a shelf essential.',
 39.99, NULL, 5, N'/Content/images/products/novel-collection-1.jpg', 1, 0, 66, GETDATE()),
(11, N'CLT-DJK-011', N'Denim Jacket', N'denim-jacket',
 N'Classic fit denim jacket.',
 N'Classic fit denim jacket in washed indigo with button front and chest pockets.',
 89.99, NULL, 6, N'/Content/images/products/denim-jacket-1.jpg', 1, 0, 40, GETDATE()),
(12, N'CLT-TEE-012', N'Cotton T-Shirt', N'cotton-t-shirt',
 N'100% cotton crew-neck t-shirt.',
 N'Soft 100% cotton crew-neck t-shirt, pre-shrunk and available in three sizes.',
 19.99, NULL, 6, N'/Content/images/products/cotton-tshirt-1.jpg', 1, 0, 200, GETDATE());
GO

SET IDENTITY_INSERT [dbo].[Products] OFF;
GO

/* ------------------------- Product images ------------------------------- */

SET IDENTITY_INSERT [dbo].[ProductImages] ON;
GO

INSERT INTO [dbo].[ProductImages] ([Id], [ProductId], [Url], [AltText], [DisplayOrder], [IsMain]) VALUES
(1, 1, N'/Content/images/products/headphones-pro-1.jpg', N'Wireless Headphones Pro - front', 1, 1),
(2, 1, N'/Content/images/products/headphones-pro-2.jpg', N'Wireless Headphones Pro - side', 2, 0),
(3, 1, N'/Content/images/products/headphones-pro-3.jpg', N'Wireless Headphones Pro - case', 3, 0),
(4, 2, N'/Content/images/products/bluetooth-speaker-1.jpg', N'Portable Bluetooth Speaker', 1, 1),
(5, 3, N'/Content/images/products/ultrabook-14-1.jpg', N'Ultrabook Laptop 14 - open', 1, 1),
(6, 3, N'/Content/images/products/ultrabook-14-2.jpg', N'Ultrabook Laptop 14 - side', 2, 0),
(7, 4, N'/Content/images/products/smartphone-x-1.jpg', N'Smartphone X - front', 1, 1),
(8, 4, N'/Content/images/products/smartphone-x-2.jpg', N'Smartphone X - back', 2, 0),
(9, 5, N'/Content/images/products/coffee-maker-1.jpg', N'Coffee Maker Deluxe', 1, 1),
(10, 6, N'/Content/images/products/knife-set-1.jpg', N'Chef''s Knife Set', 1, 1),
(11, 7, N'/Content/images/products/yoga-mat-1.jpg', N'Premium Yoga Mat', 1, 1),
(12, 8, N'/Content/images/products/running-shoes-1.jpg', N'Road Running Shoes - pair', 1, 1),
(13, 8, N'/Content/images/products/running-shoes-2.jpg', N'Road Running Shoes - sole', 2, 0),
(14, 9, N'/Content/images/products/dumbbell-set-1.jpg', N'Dumbbell Set 20kg', 1, 1),
(15, 10, N'/Content/images/products/novel-collection-1.jpg', N'Classic Novel Collection', 1, 1),
(16, 11, N'/Content/images/products/denim-jacket-1.jpg', N'Denim Jacket', 1, 1),
(17, 12, N'/Content/images/products/cotton-tshirt-1.jpg', N'Cotton T-Shirt', 1, 1);
GO

SET IDENTITY_INSERT [dbo].[ProductImages] OFF;
GO

/* ------------------------- Product variants ----------------------------- */

SET IDENTITY_INSERT [dbo].[ProductVariants] ON;
GO

INSERT INTO [dbo].[ProductVariants] ([Id], [ProductId], [Name], [Sku], [PriceAdjustment], [StockQuantity], [IsActive]) VALUES
(1, 1, N'Midnight Black', N'AUD-HEAD-001-BLK', 0.00, 25, 1),
(2, 1, N'Arctic Silver', N'AUD-HEAD-001-SLV', 10.00, 17, 1),
(3, 12, N'Small', N'CLT-TEE-012-S', 0.00, 80, 1),
(4, 12, N'Medium', N'CLT-TEE-012-M', 0.00, 70, 1),
(5, 12, N'Large', N'CLT-TEE-012-L', 2.00, 50, 1);
GO

SET IDENTITY_INSERT [dbo].[ProductVariants] OFF;
GO

PRINT N'Seed data inserted.';
GO
