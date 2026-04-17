--CREATE DATABASE GKCentarDB2
--GO

--USE TestingSql
--GO

-- Create Schemas for naming Tables within same categories ProductMngt

CREATE SCHEMA CentarProductMngt;
GO

CREATE SCHEMA CentarOrderMngt;
GO

CREATE SCHEMA CentarForumMngt;
GO

--Create Tables

--ProductMngt
CREATE TABLE CentarProductMngt.Products
(
	ProductId INT IDENTITY (1,1) NOT NULL,
	ProductName VARCHAR(50) NOT NULL,
	ProductPrice DECIMAL(7,2) NOT NULL,
	ProductDescribtion VARCHAR(255),
	ProductImgPath VARCHAR(255),
	FK_CategoryId INT NOT NULL,
	FK_DiscountId INT NOT NULL,
	CONSTRAINT PK_ProductID PRIMARY KEY (ProductId)
);

CREATE TABLE CentarProductMngt.Categories
(
	CategoryId INT IDENTITY (1,1) NOT NULL,
	CategoryName VARCHAR(50) NOT NULL,
	FK_ParentCategoryId INT,
	CONSTRAINT PK_CategoryId PRIMARY KEY (CategoryId)
);

CREATE TABLE CentarProductMngt.Stocks
(
	StockId INT IDENTITY (1,1) NOT NULL,
	Quantity INT NOT NULL,
	FK_ProductId INT NOT NULL,
	CONSTRAINT PK_StockId PRIMARY KEY (StockId)
);

CREATE TABLE CentarProductMngt.Discounts
(
	DiscountId INT IDENTITY (1,1) NOT NULL,
	Discount INT NOT NULL,
	DiscountDescribtion VARCHAR(50) NOT NULL,
	StartDate DATE,
	EndDate DATE,
	DiscountType VARCHAR(50) NOT NULL,
	CONSTRAINT PK_DiscountId PRIMARY KEY (DiscountId)
);

CREATE TABLE CentarProductMngt.ProductReviews
(
	ProductReviewId INT IDENTITY (1,1) NOT NULL,
	ProductReviewContent VARCHAR(500) NOT NULL,
	FK_ProductId INT NOT NULL,
	FK_UserId NVARCHAR(450) NOT NULL,
	Rating INT NOT NULL,
	CONSTRAINT PK_ProductReviewId PRIMARY KEY (ProductReviewId)
);

--OrderMngt

CREATE TABLE CentarOrderMngt.Carts
(
	CartId INT IDENTITY (1,1) NOT NULL,
	TotalPrice DECIMAL(7,2) NOT NULL,
	CONSTRAINT PK_CartId PRIMARY KEY (CartId)
);

CREATE TABLE CentarOrderMngt.CartItems
(
	CartItemId INT IDENTITY (1,1) NOT NULL,
	Quantity INT NOT NULL,
	AddedDate DATE NOT NULL,
	FK_CartId INT NOT NULL,
	FK_ProductId INT NOT NULL,
	CONSTRAINT PK_CartItemId PRIMARY KEY (CartItemId)
);

CREATE TABLE CentarOrderMngt.Orders
(
	OrderId INT IDENTITY (1,1) NOT NULL,
	FK_CartId INT NOT NULL,
	FK_UserId NVARCHAR(450) NOT NULL,
	OrderStatus VARCHAR(50),
	CONSTRAINT PK_OrderId PRIMARY KEY (OrderId)
);

CREATE TABLE CentarOrderMngt.Invoices
(
	InvoiceId INT IDENTITY (1,1) NOT NULL,
	FK_UserId NVARCHAR(450) NOT NULL,
	FK_OrderId INT NOT NULL,
	OrderDateTime DATETIME NOT NULL,
	PaymentDateTime DATETIME,
	FK_PaymentId INT,
	FK_TaxId INT NOT NULL,
	CONSTRAINT PK_InvoiceId PRIMARY KEY (InvoiceId)
);

CREATE TABLE CentarOrderMngt.InvoiceItems
(
	InvoiceItemId INT IDENTITY(1,1) NOT NULL,
	FK_InvoiceId INT NOT NULL,
	FK_ProductId INT NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(7,2) NOT NULL,
	FK_DiscountId INT,
	SubTotal DECIMAL(7,2) NOT NULL,
	CONSTRAINT PK_InvoiceItemId PRIMARY KEY (InvoiceItemId)
);

CREATE TABLE CentarOrderMngt.Taxes
(
	TaxId INT IDENTITY(1,1) NOT NULL,
	TaxIndex INT NOT NULL,
	TaxDescribtion VARCHAR(50) NOT NULL,
	CONSTRAINT PK_TaxId PRIMARY KEY (TaxId)
);

CREATE TABLE CentarOrderMngt.Histories
(
	OrderHistoryId INT IDENTITY(1,1) NOT NULL,
	FK_InvoiceId INT NOT NULL,
	FK_UserId NVARCHAR(450) NOT NULL,
	CONSTRAINT PK_OrderHistoryId PRIMARY KEY (OrderHistoryId)
);

CREATE TABLE CentarOrderMngt.Payments
(
	PaymentId INT IDENTITY(1,1) NOT NULL,
	FK_OrderId INT NOT NULL,
	PaymentMethod VARCHAR(50),
	PaymentStatus BIT,
	TransactionId INT,
	Amount DECIMAL(7,2),
	PaymentDate DATETIME,
	CONSTRAINT PK_PaymentId PRIMARY KEY (PaymentId)
)

--ForumMngt
CREATE TABLE CentarForumMngt.Posts
(
	PostId INT IDENTITY(1,1) NOT NULL,
	PostTitle VARCHAR(50) NOT NULL,
	PostContent VARCHAR(1000) NOT NULL,
	PostCreateDate DATETIME NOT NULL,
	FK_UserId NVARCHAR(450) NOT NULL,
	CONSTRAINT PK_PostId PRIMARY KEY (PostId)
);

CREATE TABLE CentarForumMngt.Comments
(
	CommentId INT IDENTITY(1,1) NOT NULL,
	CommentContent VARCHAR(500) NOT NULL,
	CommentDateTime DATETIME NOT NULL,
	FK_UserId NVARCHAR(450) NOT NULL,
	FK_PostId INT NOT NULL,
	CONSTRAINT PK_CommentId PRIMARY KEY (CommentId)
);

--Foreign Keys Constraints 

--Products FK
ALTER TABLE CentarProductMngt.Products
ADD CONSTRAINT FK_Products_CategoryId
FOREIGN KEY (FK_CategoryId) REFERENCES CentarProductMngt.Categories(CategoryId);

ALTER TABLE CentarProductMngt.Products
ADD CONSTRAINT FK_Products_DiscountId
FOREIGN KEY (FK_DiscountId) REFERENCES CentarProductMngt.Discounts(DiscountId);

ALTER TABLE CentarProductMngt.Categories
ADD CONSTRAINT FK_Categories_ParentCategoryId
FOREIGN KEY (FK_ParentCategoryId) REFERENCES CentarProductMngt.Categories(CategoryId);

ALTER TABLE CentarProductMngt.Stocks
ADD CONSTRAINT FK_Stocks_ProductId
FOREIGN KEY (FK_ProductId) REFERENCES CentarProductMngt.Products(ProductId);

ALTER TABLE CentarProductMngt.ProductReviews
ADD CONSTRAINT FK_ProductReviews_ProductId 
FOREIGN KEY (FK_ProductId) REFERENCES CentarProductMngt.Products(ProductId);

--Order FK
ALTER TABLE CentarOrderMngt.CartItems
ADD CONSTRAINT FK_CartItems_ProductId
FOREIGN KEY (FK_ProductId) REFERENCES CentarProductMngt.Products(ProductId);

ALTER TABLE CentarOrderMngt.CartItems
ADD CONSTRAINT FK_CartItems_CartId
FOREIGN KEY (FK_CartId) REFERENCES CentarOrderMngt.Carts(CartId);

ALTER TABLE CentarOrderMngt.Orders
ADD CONSTRAINT FK_Orders_CartId 
FOREIGN KEY (FK_CartId) REFERENCES CentarOrderMngt.Carts(CartId);

ALTER TABLE CentarOrderMngt.Invoices
ADD CONSTRAINT FK_Invoices_TaxId
FOREIGN KEY (FK_TaxId) REFERENCES CentarOrderMngt.Taxes(TaxId);

ALTER TABLE CentarOrderMngt.Invoices
ADD CONSTRAINT FK_Invoices_PaymentId
FOREIGN KEY (FK_PaymentId) REFERENCES CentarOrderMngt.Payments(PaymentId);

ALTER TABLE CentarOrderMngt.Invoices
ADD CONSTRAINT FK_Invoices_OrderId
FOREIGN KEY (FK_OrderId) REFERENCES CentarOrderMngt.Orders(OrderId);

ALTER TABLE CentarOrderMngt.InvoiceItems
ADD CONSTRAINT FK_InvoiceItems_InvoiceId
FOREIGN KEY (FK_InvoiceId) REFERENCES CentarOrderMngt.Invoices(InvoiceId);

ALTER TABLE CentarOrderMngt.InvoiceItems
ADD CONSTRAINT FK_InvoiceItems_ProductId
FOREIGN KEY (FK_ProductId) REFERENCES CentarProductMngt.Products(ProductId);

ALTER TABLE CentarOrderMngt.Payments
ADD CONSTRAINT FK_Payments_OrderId
FOREIGN KEY (FK_OrderId) REFERENCES CentarOrderMngt.Orders(OrderId);

ALTER TABLE CentarOrderMngt.Histories
ADD CONSTRAINT FK_Histories_InvoiceId
FOREIGN KEY (FK_InvoiceId) REFERENCES CentarOrderMngt.Invoices(InvoiceId);

--Forum FK
ALTER TABLE CentarForumMngt.Comments
ADD CONSTRAINT FK_Comments_PostId
FOREIGN KEY (FK_PostId) REFERENCES CentarForumMngt.Posts(PostId);

--User FK
ALTER TABLE CentarProductMngt.ProductReviews
ADD CONSTRAINT FK_ProductReviews_UserId
FOREIGN KEY (FK_UserId) REFERENCES CentarUserMngt.Users(Id);

ALTER TABLE CentarOrderMngt.Invoices
ADD CONSTRAINT FK_Invoices_UserId
FOREIGN KEY (FK_UserId) REFERENCES CentarUserMngt.Users(Id);

ALTER TABLE CentarOrderMngt.Orders
ADD CONSTRAINT FK_Orders_UserId
FOREIGN KEY (FK_UserId) REFERENCES CentarUserMngt.Users(Id);

ALTER TABLE CentarOrderMngt.Histories
ADD CONSTRAINT FK_Histories_UserId
FOREIGN KEY (FK_UserId) REFERENCES CentarUserMngt.Users(Id);

ALTER TABLE CentarForumMngt.Posts
ADD CONSTRAINT FK_Posts_UserId
FOREIGN KEY (FK_UserId) REFERENCES CentarUserMngt.Users(Id);

ALTER TABLE CentarForumMngt.Comments
ADD CONSTRAINT FK_Comments_UserId
FOREIGN KEY (FK_UserId) REFERENCES CentarUserMngt.Users(Id);