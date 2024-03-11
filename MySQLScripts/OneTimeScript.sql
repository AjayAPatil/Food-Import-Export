--DECLARE @DatabaseName nvarchar(50)
--SET @DatabaseName = N'food'

--DECLARE @SQL varchar(max)

--SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
--FROM MASTER..SysProcesses
--WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

----SELECT @SQL 
--EXEC(@SQL)

--DROP DATABASE food;
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'food')
BEGIN
	CREATE DATABASE [food]
END
GO

USE [food]
GO

--TABLE tbl_UserDetails
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbl_UserDetails' and xtype='U')
BEGIN
    CREATE TABLE tbl_UserDetails(UserId INT PRIMARY KEY IDENTITY (1, 1))
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'UserName')
BEGIN
	ALTER TABLE tbl_UserDetails ADD UserName VARCHAR(255) UNIQUE NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'Password')
BEGIN
	ALTER TABLE tbl_UserDetails ADD Password VARCHAR(255) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'Role')
BEGIN
	ALTER TABLE tbl_UserDetails ADD Role VARCHAR(255) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'IsActive')
BEGIN
	ALTER TABLE tbl_UserDetails ADD IsActive  BIT DEFAULT 0
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE tbl_UserDetails ADD IsDeleted  BIT DEFAULT 1
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'CreatedOn')
BEGIN
	ALTER TABLE tbl_UserDetails ADD CreatedOn DATETIME DEFAULT GETDATE()
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'FirstName')
BEGIN
	ALTER TABLE tbl_UserDetails ADD FirstName VARCHAR(255)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'LastName')
BEGIN
	ALTER TABLE tbl_UserDetails ADD LastName VARCHAR(255)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'Gender')
BEGIN
	ALTER TABLE tbl_UserDetails ADD Gender VARCHAR(20)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'MobileNo')
BEGIN
	ALTER TABLE tbl_UserDetails ADD MobileNo VARCHAR(12)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'EmailId')
BEGIN
	ALTER TABLE tbl_UserDetails ADD EmailId VARCHAR(255)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'Address')
BEGIN
	ALTER TABLE tbl_UserDetails ADD Address VARCHAR(255)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'City')
BEGIN
	ALTER TABLE tbl_UserDetails ADD City VARCHAR(255)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserDetails]') AND name = 'PinCode')
BEGIN
	ALTER TABLE tbl_UserDetails ADD PinCode VARCHAR(10)
END
--ADDING ADMIN USER
IF NOT EXISTS (SELECT * FROM tbl_UserDetails WHERE UserName = 'Admin')
BEGIN
	INSERT INTO tbl_UserDetails (UserName, Password, Role, IsActive, IsDeleted, CreatedOn, FirstName)
	VALUES ('Admin', 'Pass@123', 'Admin', 1, 0, GETDATE(), 'Admin')
END



--TABLE tbl_ProductCategories
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbl_ProductCategories' and xtype='U')
BEGIN
    CREATE TABLE tbl_ProductCategories(CategoryId INT PRIMARY KEY IDENTITY (1, 1))
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductCategories]') AND name = 'CategoryName')
BEGIN
	ALTER TABLE tbl_ProductCategories ADD CategoryName VARCHAR(255) UNIQUE NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductCategories]') AND name = 'Description')
BEGIN
	ALTER TABLE tbl_ProductCategories ADD Description VARCHAR(MAX) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductCategories]') AND name = 'CreatedBy')
BEGIN
	ALTER TABLE tbl_ProductCategories ADD CreatedBy INT FOREIGN KEY REFERENCES tbl_UserDetails(UserId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductCategories]') AND name = 'CreatedOn')
BEGIN
	ALTER TABLE tbl_ProductCategories ADD CreatedOn DATETIME DEFAULT GETDATE()
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductCategories]') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE tbl_ProductCategories ADD IsDeleted  BIT DEFAULT 1
END

IF NOT EXISTS (SELECT CategoryName FROM tbl_ProductCategories WHERE CategoryName = 'none')
BEGIN
	INSERT INTO tbl_ProductCategories(CategoryName, Description, CreatedBy, IsDeleted, CreatedOn)
	VALUES ('none', 'none', (SELECT TOP 1 UserId FROM tbl_UserDetails WHERE UserName = 'Admin'), 0, GETDATE());
END

IF NOT EXISTS (SELECT CategoryName FROM tbl_ProductCategories WHERE CategoryName = 'Vegetables')
BEGIN
	INSERT INTO tbl_ProductCategories(CategoryName, Description, CreatedBy, IsDeleted, CreatedOn)
	VALUES ('Vegetables', 'Vegetables', (SELECT TOP 1 UserId FROM tbl_UserDetails WHERE UserName = 'Admin'), 0, GETDATE());
END

IF NOT EXISTS (SELECT CategoryName FROM tbl_ProductCategories WHERE CategoryName = 'Fruits')
BEGIN
	INSERT INTO tbl_ProductCategories(CategoryName, Description, CreatedBy, IsDeleted, CreatedOn)
	VALUES ('Fruits', 'Fruits', (SELECT TOP 1 UserId FROM tbl_UserDetails WHERE UserName = 'Admin'), 0, GETDATE());
END


--TABLE tbl_Products
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbl_Products' and xtype='U')
BEGIN
    CREATE TABLE tbl_Products(ProductId INT PRIMARY KEY IDENTITY (1, 1))
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'ProductName')
BEGIN
	ALTER TABLE tbl_Products ADD ProductName VARCHAR(1000) UNIQUE NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'Description')
BEGIN
	ALTER TABLE tbl_Products ADD Description VARCHAR(MAX) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'Images')
BEGIN
	ALTER TABLE tbl_Products ADD Images VARBINARY(MAX) NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'MRPrice')
BEGIN
	ALTER TABLE tbl_Products ADD MRPrice DECIMAL(10, 2) DEFAULT 0 NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'SalePrice')
BEGIN
	ALTER TABLE tbl_Products ADD SalePrice DECIMAL(10, 2) DEFAULT 0 NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'DiscountPercent')
BEGIN
	ALTER TABLE tbl_Products ADD DiscountPercent DECIMAL(10, 2) DEFAULT 0 NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'AvailableQuantity')
BEGIN
	ALTER TABLE tbl_Products ADD AvailableQuantity INT DEFAULT 0 NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'AvailableQuantityUnit')
BEGIN
	ALTER TABLE tbl_Products ADD AvailableQuantityUnit VARCHAR(255) DEFAULT 'none' NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'CategoryId')
BEGIN
	ALTER TABLE tbl_Products ADD CategoryId INT FOREIGN KEY REFERENCES tbl_ProductCategories(CategoryId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'CreatedBy')
BEGIN
	ALTER TABLE tbl_Products ADD CreatedBy INT FOREIGN KEY REFERENCES tbl_UserDetails(UserId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'CreatedOn')
BEGIN
	ALTER TABLE tbl_Products ADD CreatedOn DATETIME DEFAULT GETDATE()
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Products]') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE tbl_Products ADD IsDeleted  BIT DEFAULT 1
END


--TABLE tbl_UserProducts
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbl_UserProducts' and xtype='U')
BEGIN
    CREATE TABLE tbl_UserProducts(UserProductId INT PRIMARY KEY IDENTITY (1, 1))
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserProducts]') AND name = 'ProductId')
BEGIN
	ALTER TABLE tbl_UserProducts ADD ProductId INT FOREIGN KEY REFERENCES tbl_Products(ProductId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserProducts]') AND name = 'SavedAs')
BEGIN
	ALTER TABLE tbl_UserProducts ADD SavedAs VARCHAR(MAX) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserProducts]') AND name = 'CreatedBy')
BEGIN
	ALTER TABLE tbl_UserProducts ADD CreatedBy INT FOREIGN KEY REFERENCES tbl_UserDetails(UserId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserProducts]') AND name = 'CreatedOn')
BEGIN
	ALTER TABLE tbl_UserProducts ADD CreatedOn DATETIME DEFAULT GETDATE()
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_UserProducts]') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE tbl_UserProducts ADD IsDeleted  BIT DEFAULT 1
END


--TABLE tbl_Orders
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbl_Orders' and xtype='U')
BEGIN
    CREATE TABLE tbl_Orders(OrderId INT PRIMARY KEY IDENTITY (1, 1))
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Orders]') AND name = 'MobileNo')
BEGIN
	ALTER TABLE tbl_Orders ADD MobileNo VARCHAR(12)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Orders]') AND name = 'EmailId')
BEGIN
	ALTER TABLE tbl_Orders ADD EmailId VARCHAR(255)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Orders]') AND name = 'Address')
BEGIN
	ALTER TABLE tbl_Orders ADD Address VARCHAR(1000)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Orders]') AND name = 'City')
BEGIN
	ALTER TABLE tbl_Orders ADD City VARCHAR(255)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Orders]') AND name = 'PinCode')
BEGIN
	ALTER TABLE tbl_Orders ADD PinCode VARCHAR(10)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Orders]') AND name = 'CreatedBy')
BEGIN
	ALTER TABLE tbl_Orders ADD CreatedBy INT FOREIGN KEY REFERENCES tbl_UserDetails(UserId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Orders]') AND name = 'CreatedOn')
BEGIN
	ALTER TABLE tbl_Orders ADD CreatedOn DATETIME DEFAULT GETDATE()
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_Orders]') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE tbl_Orders ADD IsDeleted  BIT DEFAULT 1
END

--TABLE tbl_ProductOrders
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbl_ProductOrders' and xtype='U')
BEGIN
    CREATE TABLE tbl_ProductOrders(ProductOrderId INT PRIMARY KEY IDENTITY (1, 1))
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductOrders]') AND name = 'OrderId')
BEGIN
	ALTER TABLE tbl_ProductOrders ADD OrderId INT FOREIGN KEY REFERENCES tbl_Orders(OrderId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductOrders]') AND name = 'ProductId')
BEGIN
	ALTER TABLE tbl_ProductOrders ADD ProductId INT FOREIGN KEY REFERENCES tbl_Products(ProductId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductOrders]') AND name = 'Quantity')
BEGIN
	ALTER TABLE tbl_ProductOrders ADD Quantity INT
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductOrders]') AND name = 'QuantityUnit')
BEGIN
	ALTER TABLE tbl_ProductOrders ADD QuantityUnit VARCHAR(255)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductOrders]') AND name = 'Amount')
BEGIN
	ALTER TABLE tbl_ProductOrders ADD Amount DECIMAL(10, 2) DEFAULT 0 NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductOrders]') AND name = 'CreatedBy')
BEGIN
	ALTER TABLE tbl_ProductOrders ADD CreatedBy INT FOREIGN KEY REFERENCES tbl_UserDetails(UserId) NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductOrders]') AND name = 'CreatedOn')
BEGIN
	ALTER TABLE tbl_ProductOrders ADD CreatedOn DATETIME DEFAULT GETDATE()
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[tbl_ProductOrders]') AND name = 'IsDeleted')
BEGIN
	ALTER TABLE tbl_ProductOrders ADD IsDeleted  BIT DEFAULT 1
END