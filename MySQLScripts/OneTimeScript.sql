CREATE DATABASE IF NOT EXISTS `food`;
USE `food`;

CREATE TABLE IF NOT EXISTS `tbl_UserDetails`(
    `UserId` INT NOT NULL AUTO_INCREMENT,
	PRIMARY KEY(UserId)
);

ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `UserName` VARCHAR(255) NOT NULL;
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `Password` VARCHAR(255) NOT NULL;
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `Role` VARCHAR(255) NOT NULL;
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `IsActive` BIT DEFAULT 0;
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `IsDeleted` BIT DEFAULT 1;
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `CreatedOn` DATE DEFAULT CURRENT_DATE();
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `FirstName` VARCHAR(255);
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `LastName` VARCHAR(255);
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `Gender` VARCHAR(20);
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `MobileNo` VARCHAR(12);
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `EmailId` VARCHAR(255);
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `Address` VARCHAR(255);
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `City` VARCHAR(255);
ALTER TABLE `tbl_UserDetails` ADD COLUMN  IF NOT EXISTS `PinCode` VARCHAR(10);
ALTER TABLE `tbl_UserDetails` ADD UNIQUE(UserName);
ALTER TABLE tbl_userdetails AUTO_INCREMENT=1;

INSERT INTO `tbl_UserDetails` (`UserName`, `Password`, `Role`, `IsActive`, `IsDeleted`, `CreatedOn`, `FirstName`)
SELECT * FROM (SELECT 'Admin' AS `UserName`, 'Pass@123' AS `Password`, 'Admin' AS `Role`, 1 AS `IsActive`, 0 AS `IsDeleted`, CURDATE() AS  `CreatedOn`, 'Admin' AS `FirstName`) AS tmp
WHERE NOT EXISTS (
    SELECT UserName FROM `tbl_UserDetails` WHERE `UserName` = 'Admin'
) LIMIT 1;



CREATE TABLE IF NOT EXISTS `tbl_ProductCategories`(
    `CategoryId` INT NOT NULL AUTO_INCREMENT,
	PRIMARY KEY(CategoryId)
);
ALTER TABLE `tbl_ProductCategories` ADD COLUMN  IF NOT EXISTS `CategoryName` VARCHAR(1000) NOT NULL;
ALTER TABLE `tbl_ProductCategories` ADD COLUMN  IF NOT EXISTS `Description` VARCHAR(5000) NOT NULL;
ALTER TABLE `tbl_ProductCategories` ADD COLUMN  IF NOT EXISTS `CreatedBy` INT NOT NULL;
ALTER TABLE `tbl_ProductCategories` ADD COLUMN  IF NOT EXISTS `CreatedOn` DATE DEFAULT CURRENT_DATE() NOT NULL;
ALTER TABLE `tbl_ProductCategories` ADD COLUMN  IF NOT EXISTS `IsDeleted` BIT DEFAULT 1;
ALTER TABLE `tbl_ProductCategories` ADD CONSTRAINT `FK_CategoriesUser` FOREIGN KEY IF NOT EXISTS (`CreatedBy`) REFERENCES `tbl_UserDetails`(`UserId`);
ALTER TABLE tbl_ProductCategories AUTO_INCREMENT=1;

INSERT INTO `tbl_ProductCategories`(`CategoryName`, `Description`, `CreatedBy`, `IsDeleted`, `CreatedOn`)
VALUES ('none','none',1,0,CURDATE());
SELECT * FROM (SELECT 'none' AS `CategoryName`, 'none' AS `Description`, (SELECT `UserId` FROM `tbl_UserDetails` WHERE `UserName` = 'Admin' LIMIT 1) AS `CreatedBy`, 0 AS `IsDeleted`, CURDATE() AS  `CreatedOn`) AS tmp
WHERE NOT EXISTS (
    SELECT CategoryName FROM `tbl_UserDetails` WHERE `CategoryName` = 'none'
) LIMIT 1;

INSERT INTO `tbl_ProductCategories`(`CategoryName`, `Description`, `CreatedBy`, `IsDeleted`, `CreatedOn`)
VALUES ('Vegetables','Vegetables',1,0,CURDATE());
SELECT * FROM (SELECT 'Vegetables' AS `CategoryName`, 'Vegetables' AS `Description`, (SELECT `UserId` FROM `tbl_UserDetails` WHERE `UserName` = 'Admin' LIMIT 1) AS `CreatedBy`, 0 AS `IsDeleted`, CURDATE() AS  `CreatedOn`) AS tmp
WHERE NOT EXISTS (
    SELECT CategoryName FROM `tbl_UserDetails` WHERE `CategoryName` = 'Vegetables'
) LIMIT 1;

INSERT INTO `tbl_ProductCategories`(`CategoryName`, `Description`, `CreatedBy`, `IsDeleted`, `CreatedOn`)
VALUES ('Fruits','Fruits',1,0,CURDATE());
SELECT * FROM (SELECT 'Fruits' AS `CategoryName`, 'Fruits' AS `Description`, (SELECT `UserId` FROM `tbl_UserDetails` WHERE `UserName` = 'Admin' LIMIT 1) AS `CreatedBy`, 0 AS `IsDeleted`, CURDATE() AS  `CreatedOn`) AS tmp
WHERE NOT EXISTS (
    SELECT CategoryName FROM `tbl_UserDetails` WHERE `CategoryName` = 'Fruits'
) LIMIT 1;




CREATE TABLE IF NOT EXISTS `tbl_Products`(
    `ProductId` INT NOT NULL AUTO_INCREMENT,
	PRIMARY KEY(`ProductId`)
);

ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `ProductName` VARCHAR(1000) NOT NULL;
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `Description` VARCHAR(5000) NOT NULL;
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `Images` LONGBLOB;
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `MRPrice` DECIMAL(10, 2) DEFAULT 0 NOT NULL;
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `SalePrice` DECIMAL(10, 2) DEFAULT 0 NOT NULL;
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `DiscountPercent` DECIMAL(10, 2) DEFAULT 0 NOT NULL;
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `CategoryId` INT NOT NULL;
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `CreatedBy` INT NOT NULL;
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `CreatedOn` DATE DEFAULT CURRENT_DATE();
ALTER TABLE `tbl_Products` ADD COLUMN  IF NOT EXISTS `IsDeleted` BIT DEFAULT 1;
ALTER TABLE `tbl_Products` AUTO_INCREMENT=1;
ALTER TABLE `tbl_Products` ADD CONSTRAINT `FK_ProductsUser` FOREIGN KEY IF NOT EXISTS (`CreatedBy`) REFERENCES `tbl_UserDetails`(`UserId`);
ALTER TABLE `tbl_Products` ADD CONSTRAINT `FK_ProductsCategories` FOREIGN KEY IF NOT EXISTS (`CategoryId`) REFERENCES `tbl_ProductCategories`(`CategoryId`);



CREATE TABLE IF NOT EXISTS `tbl_UserProducts`(
    `UserProductId` INT NOT NULL AUTO_INCREMENT,
	PRIMARY KEY(`UserProductId`)
);
ALTER TABLE `tbl_UserProducts` ADD COLUMN  IF NOT EXISTS `ProductId` INT NOT NULL REFERENCES `tbl_Products`(`ProductId`);
ALTER TABLE `tbl_UserProducts` ADD COLUMN  IF NOT EXISTS `SavedAs` VARCHAR(5000) NOT NULL;
ALTER TABLE `tbl_UserProducts` ADD COLUMN  IF NOT EXISTS `CreatedBy` INT NOT NULL REFERENCES `tbl_UserDetails`(`UserId`);
ALTER TABLE `tbl_UserProducts` ADD COLUMN  IF NOT EXISTS `CreatedOn` DATE DEFAULT CURRENT_DATE();
ALTER TABLE `tbl_UserProducts` ADD COLUMN  IF NOT EXISTS `IsDeleted` BIT DEFAULT 1;
ALTER TABLE `tbl_UserProducts` AUTO_INCREMENT=1;


CREATE TABLE IF NOT EXISTS `tbl_Orders`(
    `OrderId` INT NOT NULL AUTO_INCREMENT,
	PRIMARY KEY(`OrderId`)
);
ALTER TABLE `tbl_Orders` ADD COLUMN  IF NOT EXISTS `MobileNo` VARCHAR(12);
ALTER TABLE `tbl_Orders` ADD COLUMN  IF NOT EXISTS `EmailId` VARCHAR(255);
ALTER TABLE `tbl_Orders` ADD COLUMN  IF NOT EXISTS `Address` VARCHAR(255);
ALTER TABLE `tbl_Orders` ADD COLUMN  IF NOT EXISTS `City` VARCHAR(255);
ALTER TABLE `tbl_Orders` ADD COLUMN  IF NOT EXISTS `PinCode` VARCHAR(10);
ALTER TABLE `tbl_Orders` ADD COLUMN  IF NOT EXISTS `CreatedBy` INT NOT NULL  REFERENCES `tbl_UserDetails`(`UserId`);
ALTER TABLE `tbl_Orders` ADD COLUMN  IF NOT EXISTS `CreatedOn` DATE DEFAULT CURRENT_DATE() NOT NULL;
ALTER TABLE `tbl_Orders` ADD COLUMN  IF NOT EXISTS `IsDeleted` BIT DEFAULT 1;
ALTER TABLE `tbl_Orders` AUTO_INCREMENT=1;


CREATE TABLE IF NOT EXISTS `tbl_ProductOrders`(
    `ProductOrderId` INT NOT NULL AUTO_INCREMENT,
	PRIMARY KEY(`ProductOrderId`)
);
ALTER TABLE `tbl_ProductOrders` ADD COLUMN  IF NOT EXISTS `OrderId` INT REFERENCES `tbl_Orders`(`OrderId`);
ALTER TABLE `tbl_ProductOrders` ADD COLUMN  IF NOT EXISTS `ProductId` INT REFERENCES `tbl_Products`(`ProductId`);
ALTER TABLE `tbl_ProductOrders` ADD COLUMN  IF NOT EXISTS `Quantity` INT;
ALTER TABLE `tbl_ProductOrders` ADD COLUMN  IF NOT EXISTS `QuantityUnit` VARCHAR(255);
ALTER TABLE `tbl_ProductOrders` ADD COLUMN  IF NOT EXISTS `CreatedBy` INT NOT NULL  REFERENCES `tbl_UserDetails`(`UserId`);
ALTER TABLE `tbl_ProductOrders` ADD COLUMN  IF NOT EXISTS `CreatedOn` DATE DEFAULT CURRENT_DATE() NOT NULL;
ALTER TABLE `tbl_ProductOrders` ADD COLUMN  IF NOT EXISTS `IsDeleted` BIT DEFAULT 1;
ALTER TABLE `tbl_ProductOrders` AUTO_INCREMENT=1;