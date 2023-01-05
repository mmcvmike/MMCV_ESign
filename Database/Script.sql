create database MMCV_ESign

go

use MMCV_ESign

go

create table DocumentType(
	DocumentTypeID int Identity Primary key,
	DocumentTypeName nvarchar(50) not null,
	[Description] nvarchar(200),
	Active int default(1), -- 1: Active, 0: Inactive
)

go

create table Document(
	DocumentID int identity primary key,
	DocumentName nvarchar(50) not null,
	DocumentTypeID int,
	Issuer nvarchar(50) not null,
	IssuerEmpId nvarchar(20) not null,
	Title nvarchar(100) not null,
	[Status] int default(0), -- 0: Initial, 1: Completed, 2: Decline, 3: Cancel, 4: Draft
	Link varchar(100),
	ReferenceCode varchar(100),
	Note nvarchar(500),
	CreatedBy nvarchar(20),
	CreatedDate datetime,
	Active int default(1), -- 1: Active, 0: Inactive
)

go

create table DocumentSign(
	DocumentSignID int identity primary key,
	DocumentID int not null,
	DocumentReferenceCode varchar(20) not null,
	Fullname nvarchar(30),
	Email varchar(50) not null,
	SignIndex int,
	SignDate datetime,
	UserSignatureID int,
	UserEmpID varchar(20),
	[Status] int default(0), -- 0: Initial, 1: Signed, 2: Declined
	Note nvarchar(200)
)

go

create table [User](
	UserID int identity primary key,
	Username varchar(20) not null,
	Fullname nvarchar(30),
	Email varchar(50) not null,
	[Password] varchar(30) not null,
	EmployeeID varchar(15),
	DepartmentID int,
	PositionID int,
	StampBase64 text,
	Active int default(1), -- 1: Active, 0: Inactive
)

go

create table [Permission](
	PermissionID int identity primary key,
	PermissionName nvarchar(50) not null,
	Active int default(1), -- 1: Active, 0: Inactive
)

go

create table [Role](
	RoleID int identity primary key,
	RoleName nvarchar(50) not null,
	Active int default(1), -- 1: Active, 0: Inactive
)

go

create table RolePermission(
	RolePermissionID int identity primary key,
	RoleID int,
	PermissionID int
)

go

create table UserPermission(
	ID int identity primary key,
	UserID int,
	PermissionID int
)

go

create table UserRole(
	ID int identity primary key,
	UserID int,
	RoleID int
)

go

create table Menu(
	MenuID int identity primary key,
	MenuName nvarchar(50),
	MenuParent int,
	MenuIndex int,
	Active int default(1), -- 1: Active, 0: Inactive
)

go

create table UserMenu(
	ID int identity primary key,
	UserID int,
	MenuID int
)

go

create table UserSignature(
	UserSignatureID int identity primary key,
	UserID int not null,
	Base64Signature text not null,
	IsDefault int, -- 1: Default, 0: Undefault
	CreatedDate datetime,
	Active int default(0), -- 1: Active, 0: Inactive
)

go

create table Department(
	DepartmentID int identity primary key,
	DepartmentName nvarchar(100) not null,
	Active int default(1), -- 1: Active, 0: Inactive
)

go

create table Position(
	PositionID int identity primary key,
	PositionName nvarchar(100) not null,
	Active int default(1), -- 1: Active, 0: Inactive
)

go

CREATE PROCEDURE [dbo].[USP_Documents_Get] 
	@Issuer NVARCHAR(50),
	@Signer NVARCHAR(50),
	@Status VARCHAR(2)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT *
	FROM Document d
	JOIN DocumentSign ds ON ds.DocumentID = d.DocumentID
	WHERE Issuer = @Issuer OR ds.Email = @Signer;

END

go

CREATE PROCEDURE [dbo].[USP_Documents_Get_MeSign] 
	@Signer NVARCHAR(50),
	@Status VARCHAR(2)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT d.*, ds.[Status] as SignerStatus
	FROM Document d
	JOIN DocumentSign ds ON ds.DocumentID = d.DocumentID
	WHERE ds.Email = @Signer
	AND d.[Status] NOT IN(3, 4);
	--AND ds.[SignDate] IS NULL
	--AND ds.[Status] = @Status;

END

go

CREATE PROCEDURE [dbo].[USP_Documents_Get_Sent] 
	@Issuer NVARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT *
	FROM Document
	WHERE Issuer = @Issuer AND [Status] NOT IN (3, 4);

END

go

go

CREATE PROCEDURE [dbo].[USP_Documents_Get_Draft] 
	@Issuer NVARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT *
	FROM Document
	WHERE Issuer = @Issuer AND [Status] = 4;

END

go

CREATE PROCEDURE [dbo].[USP_Documents_Get_Cancel] 
	@Issuer NVARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT *
	FROM Document
	WHERE Issuer = @Issuer AND [Status] = 3;

END

go

CREATE PROCEDURE USP_Dashboard_Count
	@Email nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @meSign int = 0;
	declare @sent int = 0;
	declare @completed int = 0;

	SELECT @meSign = count(1)
	FROM Document d
	JOIN DocumentSign ds ON ds.DocumentID = d.DocumentID
	WHERE ds.Email = @Email
	AND d.[Status] IN (0, 1);

	SELECT @sent = count(1)
	FROM Document d
	WHERE d.Issuer = @Email
	AND d.[Status] != 4;

	SELECT @completed = count(1)
	FROM Document d
	JOIN DocumentSign ds ON ds.DocumentID = d.DocumentID
	WHERE ds.Email = @Email
	AND d.[Status] = 1 
	AND ds.[Status] = 1;

	SELECT @meSign as MeSign, @sent as Sent, @completed as Completed;

END

go

insert into [User](Username, Fullname, Email, Password, EmployeeID, DepartmentID, PositionID)
values ('tunguyen', 'Nguyen Van Tuyen', 'tuyen.nguyenvan@mmcv.mektec.com', 'anhtuyen1997', 'MV220770', 1, 1);
insert into [User](Username, Fullname, Email, Password, EmployeeID, DepartmentID, PositionID)
values ('phamvithanh', 'Pham Vi Thanh', 'thanh.phamvi@mmcv.mektec.com', 'test', 'MV220829', 1, 1);

go

insert into DocumentType (DocumentTypeName, Description)
values ('Labor Contract', 'Labor Contract');
insert into DocumentType (DocumentTypeName, Description)
values ('Other', 'Other');

go

insert into [Role](RoleName) values ('Admin');
insert into [Role](RoleName) values ('User');

go

insert into Permission(PermissionName) values ('');
insert into Permission(PermissionName) values ('');

go

insert into Menu(MenuName, MenuIndex) values ('Dashboard', 1);
insert into Menu(MenuName, MenuIndex) values ('Document Management', 2);
insert into Menu(MenuName, MenuIndex) values ('Account Information', 3);
insert into Menu(MenuName, MenuIndex) values ('User Management', 4);

go

insert into UserMenu (MenuID, UserID) values (1, 1);
insert into UserMenu (MenuID, UserID) values (1, 2);
insert into UserMenu (MenuID, UserID) values (1, 3);
insert into UserMenu (MenuID, UserID) values (1, 4);

