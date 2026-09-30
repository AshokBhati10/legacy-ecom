/* ============================================================================
   Legacy eCommerce — database creation script
   Target : .\SQLEXPRESS
   Database : LegacyEcommerceDb

   Run with sqlcmd or SQL Server Management Studio:

       sqlcmd -S .\SQLEXPRESS -i CreateDatabase.sql

   IMPORTANT: replace __SET_YOUR_PASSWORD_HERE__ with a strong password for
   the legacy_app_user SQL login, and use the same value in the EcommerceDb
   connection string in Ecommerce.Web\Web.config. Never commit real passwords.
   ============================================================================ */

IF DB_ID(N'LegacyEcommerceDb') IS NULL
BEGIN
    CREATE DATABASE [LegacyEcommerceDb];
END
GO

USE [LegacyEcommerceDb];
GO

/* ------------------------- Catalog tables -------------------------------- */

CREATE TABLE [dbo].[Categories] (
    [Id]               INT            IDENTITY(1,1) NOT NULL,
    [Name]             NVARCHAR(200)  NOT NULL,
    [Slug]             NVARCHAR(200)  NOT NULL,
    [Description]      NVARCHAR(MAX)  NULL,
    [ParentCategoryId] INT            NULL,
    [DisplayOrder]     INT            NOT NULL,
    [IsActive]         BIT            NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Categories_Categories] FOREIGN KEY ([ParentCategoryId])
        REFERENCES [dbo].[Categories] ([Id])
);
GO

CREATE TABLE [dbo].[Products] (
    [Id]               INT            IDENTITY(1,1) NOT NULL,
    [Sku]              NVARCHAR(50)   NOT NULL,
    [Name]             NVARCHAR(200)  NOT NULL,
    [Slug]             NVARCHAR(200)  NOT NULL,
    [ShortDescription] NVARCHAR(500)  NULL,
    [Description]      NVARCHAR(MAX)  NULL,
    [Price]            DECIMAL(18,2)  NOT NULL,
    [SalePrice]        DECIMAL(18,2)  NULL,
    [CategoryId]       INT            NOT NULL,
    [ThumbnailUrl]     NVARCHAR(500)  NULL,
    [IsActive]         BIT            NOT NULL,
    [IsFeatured]       BIT            NOT NULL,
    [StockQuantity]    INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryId])
        REFERENCES [dbo].[Categories] ([Id])
);
GO

CREATE TABLE [dbo].[ProductImages] (
    [Id]           INT            IDENTITY(1,1) NOT NULL,
    [ProductId]    INT            NOT NULL,
    [Url]          NVARCHAR(500)  NOT NULL,
    [AltText]      NVARCHAR(200)  NULL,
    [DisplayOrder] INT            NOT NULL,
    [IsMain]       BIT            NOT NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY ([ProductId])
        REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[ProductVariants] (
    [Id]            INT            IDENTITY(1,1) NOT NULL,
    [ProductId]     INT            NOT NULL,
    [Name]          NVARCHAR(200)  NOT NULL,
    [Sku]           NVARCHAR(50)   NULL,
    [PriceAdjustment] DECIMAL(18,2) NOT NULL,
    [StockQuantity] INT            NOT NULL,
    [IsActive]      BIT            NOT NULL,
    CONSTRAINT [PK_ProductVariants] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductVariants_Products] FOREIGN KEY ([ProductId])
        REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE
);
GO

/* ------------------------- Customer / cart tables ------------------------ */

CREATE TABLE [dbo].[Customers] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [UserId]      NVARCHAR(128)  NULL,
    [FirstName]   NVARCHAR(100)  NOT NULL,
    [LastName]    NVARCHAR(100)  NOT NULL,
    [Email]       NVARCHAR(256)  NOT NULL,
    [Phone]       NVARCHAR(30)   NULL,
    [CreatedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[Addresses] (
    [Id]         INT            IDENTITY(1,1) NOT NULL,
    [CustomerId] INT            NOT NULL,
    [Label]      NVARCHAR(50)   NULL,
    [FirstName]  NVARCHAR(100)  NOT NULL,
    [LastName]   NVARCHAR(100)  NOT NULL,
    [Street]     NVARCHAR(250)  NOT NULL,
    [City]       NVARCHAR(100)  NOT NULL,
    [State]      NVARCHAR(100)  NOT NULL,
    [PostalCode] NVARCHAR(20)   NOT NULL,
    [Country]    NVARCHAR(100)  NOT NULL,
    [Phone]      NVARCHAR(30)   NULL,
    [IsDefault]  BIT            NOT NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Addresses_Customers] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[CartItems] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [UserId]      NVARCHAR(128)  NOT NULL,
    [ProductId]   INT            NOT NULL,
    [VariantId]   INT            NULL,
    [Quantity]    INT            NOT NULL,
    [DateCreated] DATETIME       NOT NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CartItems_Products] FOREIGN KEY ([ProductId])
        REFERENCES [dbo].[Products] ([Id])
);
GO

/* ------------------------- Order tables ---------------------------------- */

CREATE TABLE [dbo].[Orders] (
    [Id]             INT            IDENTITY(1,1) NOT NULL,
    [OrderNumber]    NVARCHAR(50)   NOT NULL,
    [CustomerId]     INT            NULL,
    [UserId]         NVARCHAR(128)  NULL,
    [OrderDate]      DATETIME       NOT NULL,
    [Status]         NVARCHAR(50)   NOT NULL,
    [SubTotal]       DECIMAL(18,2)  NOT NULL,
    [ShippingCost]   DECIMAL(18,2)  NOT NULL,
    [TaxAmount]      DECIMAL(18,2)  NOT NULL,
    [Total]          DECIMAL(18,2)   NOT NULL,
    [ShippingMethod] NVARCHAR(50)   NOT NULL,
    [PaymentMethod]  NVARCHAR(50)   NOT NULL,
    [ShipFirstName]  NVARCHAR(100)  NOT NULL,
    [ShipLastName]   NVARCHAR(100)  NOT NULL,
    [ShipEmail]      NVARCHAR(256)  NOT NULL,
    [ShipPhone]      NVARCHAR(30)   NULL,
    [ShipStreet]     NVARCHAR(250)  NOT NULL,
    [ShipCity]       NVARCHAR(100)  NOT NULL,
    [ShipState]      NVARCHAR(100)  NOT NULL,
    [ShipPostalCode] NVARCHAR(20)   NOT NULL,
    [ShipCountry]    NVARCHAR(100)  NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders_Customers] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customers] ([Id])
);
GO

CREATE TABLE [dbo].[OrderLines] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [OrderId]     INT            NOT NULL,
    [ProductId]   INT            NOT NULL,
    [VariantId]   INT            NULL,
    [ProductName] NVARCHAR(200)  NOT NULL,
    [VariantName] NVARCHAR(200)  NULL,
    [Sku]         NVARCHAR(50)   NULL,
    [UnitPrice]   DECIMAL(18,2)  NOT NULL,
    [Quantity]    INT            NOT NULL,
    [LineTotal]   DECIMAL(18,2)  NOT NULL,
    CONSTRAINT [PK_OrderLines] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderLines_Orders] FOREIGN KEY ([OrderId])
        REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderLines_Products] FOREIGN KEY ([ProductId])
        REFERENCES [dbo].[Products] ([Id])
);
GO

/* ------------------------- Indexes --------------------------------------- */

CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories_Slug] ON [dbo].[Categories] ([Slug] ASC);
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products] ([CategoryId] ASC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_Slug] ON [dbo].[Products] ([Slug] ASC);
CREATE NONCLUSTERED INDEX [IX_Products_IsActive] ON [dbo].[Products] ([IsActive] ASC);
CREATE NONCLUSTERED INDEX [IX_ProductImages_ProductId] ON [dbo].[ProductImages] ([ProductId] ASC);
CREATE NONCLUSTERED INDEX [IX_ProductVariants_ProductId] ON [dbo].[ProductVariants] ([ProductId] ASC);
CREATE NONCLUSTERED INDEX [IX_Customers_UserId] ON [dbo].[Customers] ([UserId] ASC);
CREATE NONCLUSTERED INDEX [IX_Customers_Email] ON [dbo].[Customers] ([Email] ASC);
CREATE NONCLUSTERED INDEX [IX_Addresses_CustomerId] ON [dbo].[Addresses] ([CustomerId] ASC);
CREATE NONCLUSTERED INDEX [IX_CartItems_UserId] ON [dbo].[CartItems] ([UserId] ASC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_Orders_OrderNumber] ON [dbo].[Orders] ([OrderNumber] ASC);
CREATE NONCLUSTERED INDEX [IX_Orders_UserId] ON [dbo].[Orders] ([UserId] ASC);
CREATE NONCLUSTERED INDEX [IX_Orders_CustomerId] ON [dbo].[Orders] ([CustomerId] ASC);
CREATE NONCLUSTERED INDEX [IX_OrderLines_OrderId] ON [dbo].[OrderLines] ([OrderId] ASC);
GO

/* ------------------------- ASP.NET Identity 2.0 tables -------------------- */
/* Membership lives in the same LegacyEcommerceDb database.                 */

CREATE TABLE [dbo].[AspNetRoles] (
    [Id]   NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(256) NOT NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR(128) NOT NULL,
    [Email]                NVARCHAR(256) NULL,
    [EmailConfirmed]       BIT NOT NULL,
    [PasswordHash]         NVARCHAR(MAX) NULL,
    [SecurityStamp]        NVARCHAR(MAX) NULL,
    [PhoneNumber]          NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled]     BIT NOT NULL,
    [LockoutEndDateUtc]    DATETIME NULL,
    [LockoutEnabled]       BIT NOT NULL,
    [AccessFailedCount]    INT NOT NULL,
    [UserName]             NVARCHAR(256) NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR(128) NOT NULL,
    [RoleId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles] FOREIGN KEY ([RoleId])
        REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]        INT IDENTITY(1,1) NOT NULL,
    [UserId]    NVARCHAR(128) NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [ProviderKey]   NVARCHAR(128) NOT NULL,
    [UserId]        NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetRoles_Name] ON [dbo].[AspNetRoles] ([Name] ASC);
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers_UserName] ON [dbo].[AspNetUsers] ([UserName] ASC);
GO

/* ------------------------- Application SQL login ------------------------- */
/* Least-privilege login used by the EcommerceDb connection string.          */

USE [master];
GO

IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name = N'legacy_app_user')
BEGIN
    CREATE LOGIN [legacy_app_user] WITH PASSWORD = N'__SET_YOUR_PASSWORD_HERE__',
        CHECK_EXPIRATION = OFF, CHECK_POLICY = ON;
END
GO

USE [LegacyEcommerceDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'legacy_app_user')
BEGIN
    CREATE USER [legacy_app_user] FOR LOGIN [legacy_app_user];
    ALTER ROLE [db_datareader] ADD MEMBER [legacy_app_user];
    ALTER ROLE [db_datawriter] ADD MEMBER [legacy_app_user];
END
GO
