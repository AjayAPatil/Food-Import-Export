DROP TABLE IF EXISTS `tbl_UserProducts`;
DROP TABLE IF EXISTS `tbl_Products`;
DROP TABLE IF EXISTS `tbl_ProductCategories`;
DROP TABLE IF EXISTS `tbl_userdetails`;


CREATE TABLE tbl_UserDetails(
    UserId INT NOT NULL AUTO_INCREMENT,
    UserName VARCHAR(255) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Role VARCHAR(255) NOT NULL,
    IsActive BIT DEFAULT 0,
    IsDeleted BIT DEFAULT 1,
    CreatedOn DATE DEFAULT CURRENT_DATE(),
	FirstName VARCHAR(255),
	LastName VARCHAR(255),
	Gender VARCHAR(20),
	MobileNo VARCHAR(12),
	EmailId VARCHAR(255),
	Address VARCHAR(255),
	City VARCHAR(255),
	PinCode VARCHAR(10),
	PRIMARY KEY(UserId),
	UNIQUE(UserName)
);
ALTER TABLE tbl_userdetails AUTO_INCREMENT=1;

INSERT INTO `tbl_userdetails`(`UserName`, `Password`, `Role`, `IsActive`, `IsDeleted`, `CreatedOn`, `FirstName`)
VALUES ('Admin','Pass@123','Admin',1,0,CURDATE(),'Admin');


CREATE TABLE tbl_ProductCategories(
    CategoryId INT NOT NULL AUTO_INCREMENT,
    CategoryName VARCHAR(1000) NOT NULL,
    Description VARCHAR(5000),
    CreatedBy INT NOT NULL,
    CreatedOn DATE DEFAULT CURRENT_DATE() NOT NULL,
    IsDeleted BIT DEFAULT 1 NOT NULL,
	PRIMARY KEY(CategoryId),
	FOREIGN KEY (CreatedBy) REFERENCES tbl_UserDetails(UserId)
);

ALTER TABLE tbl_ProductCategories AUTO_INCREMENT=1;

INSERT INTO `tbl_ProductCategories`(`CategoryName`, `Description`, `CreatedBy`, `IsDeleted`, `CreatedOn`)
VALUES ('none','none',1,0,CURDATE());
INSERT INTO `tbl_ProductCategories`(`CategoryName`, `Description`, `CreatedBy`, `IsDeleted`, `CreatedOn`)
VALUES ('Vegetables','Vegetables',1,0,CURDATE());
INSERT INTO `tbl_ProductCategories`(`CategoryName`, `Description`, `CreatedBy`, `IsDeleted`, `CreatedOn`)
VALUES ('Fruits','Fruits',1,0,CURDATE());

CREATE TABLE tbl_Products(
    ProductId INT NOT NULL AUTO_INCREMENT,
    ProductName VARCHAR(1000) NOT NULL,
    Description VARCHAR(5000),
    Images LONGBLOB,
    MRPrice DECIMAL(10, 2) DEFAULT 0 NOT NULL,
    SalePrice DECIMAL(10, 2) DEFAULT 0 NOT NULL,
    DiscountPercent DECIMAL(10, 2) DEFAULT 0 NOT NULL,
    CategoryId INT NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedOn DATE DEFAULT CURRENT_DATE() NOT NULL,
    IsDeleted BIT DEFAULT 1 NOT NULL,
	PRIMARY KEY(ProductId),
	FOREIGN KEY (CreatedBy) REFERENCES tbl_UserDetails(UserId),
	FOREIGN KEY (CategoryId) REFERENCES tbl_ProductCategories(CategoryId)
);

ALTER TABLE tbl_Products AUTO_INCREMENT=1;

CREATE TABLE tbl_UserProducts(
    UserProductId INT NOT NULL AUTO_INCREMENT,
    ProductId INT NOT NULL REFERENCES tbl_Products(ProductId),
    SavedAs VARCHAR(5000) NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedOn DATE DEFAULT CURRENT_DATE() NOT NULL,
    IsDeleted BIT DEFAULT 1 NOT NULL,
	PRIMARY KEY(UserProductId),
	FOREIGN KEY (CreatedBy) REFERENCES tbl_UserDetails(UserId)
);

ALTER TABLE tbl_UserProducts AUTO_INCREMENT=1;