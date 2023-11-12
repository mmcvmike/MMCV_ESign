USE [MMCV_ESign]
GO
/****** Object:  User [mmcv]    Script Date: 11/1/2023 10:35:10 ******/
CREATE USER [mmcv] FOR LOGIN [mmcv] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [pim]    Script Date: 11/1/2023 10:35:10 ******/
CREATE USER [pim] FOR LOGIN [pim] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [mmcv]
GO
ALTER ROLE [db_owner] ADD MEMBER [pim]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Department](
	[DepartmentID] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentName] [nvarchar](100) NOT NULL,
	[Active] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[DepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Document]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Document](
	[DocumentID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentName] [nvarchar](50) NOT NULL,
	[DocumentTypeID] [int] NULL,
	[Issuer] [nvarchar](50) NOT NULL,
	[IssuerEmpId] [nvarchar](20) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Status] [int] NULL,
	[Link] [nvarchar](100) NULL,
	[ReferenceCode] [varchar](100) NULL,
	[Note] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[Active] [int] NULL,
	[EmailCC] [nvarchar](600) NULL,
 CONSTRAINT [PK__Document__1ABEEF6FE207ECF0] PRIMARY KEY CLUSTERED 
(
	[DocumentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentSign]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentSign](
	[DocumentSignID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentID] [int] NOT NULL,
	[DocumentReferenceCode] [varchar](20) NULL,
	[Fullname] [nvarchar](30) NULL,
	[Email] [varchar](500) NOT NULL,
	[SignIndex] [int] NULL,
	[SignDate] [datetime] NULL,
	[UserSignatureID] [int] NULL,
	[UserEmpID] [varchar](20) NULL,
	[Status] [int] NULL,
	[Note] [nvarchar](200) NULL,
	[X] [int] NULL,
	[Y] [int] NULL,
	[Width] [int] NULL,
	[Height] [int] NULL,
	[Type] [int] NULL,
	[Page] [int] NULL,
 CONSTRAINT [PK__Document__B48ECF39B8C740DA] PRIMARY KEY CLUSTERED 
(
	[DocumentSignID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentType]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentType](
	[DocumentTypeID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentTypeName] [nvarchar](50) NOT NULL,
	[EmployeeID] [varchar](15) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[Active] [int] NULL,
 CONSTRAINT [PK__Document__DBA390C101413DB5] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[MenuID] [int] IDENTITY(1,1) NOT NULL,
	[MenuName] [nvarchar](50) NULL,
	[MenuParent] [int] NULL,
	[MenuIndex] [int] NULL,
	[Url] [nvarchar](100) NULL,
	[Active] [int] NULL,
 CONSTRAINT [PK__Menu__C99ED25056F65410] PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[PermissionID] [int] IDENTITY(1,1) NOT NULL,
	[PermissionName] [nvarchar](50) NOT NULL,
	[Active] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Position]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Position](
	[PositionID] [int] IDENTITY(1,1) NOT NULL,
	[PositionName] [nvarchar](100) NOT NULL,
	[Active] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[Active] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleMenu]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleMenu](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[MenuID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermission]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermission](
	[RolePermissionID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[PermissionID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[RolePermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tag]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tag](
	[TagID] [int] IDENTITY(1,1) NOT NULL,
	[TagName] [nvarchar](50) NULL,
	[EmployeeID] [nvarchar](20) NULL,
	[Active] [int] NULL,
	[Order] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TagDoc]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TagDoc](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TagID] [int] NULL,
	[DocID] [int] NULL,
	[EmpID] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](20) NOT NULL,
	[Fullname] [nvarchar](30) NULL,
	[Email] [varchar](50) NOT NULL,
	[Password] [varchar](30) NOT NULL,
	[EmployeeID] [varchar](15) NULL,
	[DepartmentID] [int] NULL,
	[PositionID] [int] NULL,
	[RoleID] [int] NULL,
	[StampBase64] [text] NULL,
	[Active] [int] NULL,
 CONSTRAINT [PK__User__1788CCAC0D0CA13F] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMenu]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMenu](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[MenuID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPermission]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPermission](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[PermissionID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[RoleID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSignature]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSignature](
	[UserSignatureID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Base64Signature] [text] NOT NULL,
	[IsDefault] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[Active] [int] NULL,
 CONSTRAINT [PK__UserSign__CD15C26FEAA5C7FC] PRIMARY KEY CLUSTERED 
(
	[UserSignatureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Department] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Document] ADD  CONSTRAINT [DF__Document__Status__276EDEB3]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Document] ADD  CONSTRAINT [DF__Document__Active__286302EC]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[DocumentSign] ADD  CONSTRAINT [DF__DocumentS__Statu__2B3F6F97]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[DocumentType] ADD  CONSTRAINT [DF__DocumentT__Activ__24927208]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF__Menu__Active__3A81B327]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Permission] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Position] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__Active__2E1BDC42]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[UserSignature] ADD  CONSTRAINT [DF__UserSigna__Activ__3F466844]  DEFAULT ((0)) FOR [Active]
GO
/****** Object:  StoredProcedure [dbo].[USP_Dashboard_Count]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_Dashboard_Count]
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
GO
/****** Object:  StoredProcedure [dbo].[USP_Documents_Get]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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

GO
/****** Object:  StoredProcedure [dbo].[USP_Documents_Get_Cancel]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  StoredProcedure [dbo].[USP_Documents_Get_Draft]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  StoredProcedure [dbo].[USP_Documents_Get_MeSign]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_Documents_Get_MeSign] 
	@Signer NVARCHAR(50),
	@Status VARCHAR(2),
	@Title NVARCHAR(100),
	@ReferenceCode VARCHAR(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	SELECT d.*, ds.[Status] as SignerStatus
	FROM Document d
	JOIN DocumentSign ds ON ds.DocumentID = d.DocumentID
	WHERE ds.Email = @Signer
	AND d.[Status] NOT IN(3, 4)
	AND (ds.Status = @Status OR @Status IS NULL OR @Status = '')
	AND (d.Title = @Title OR @Title IS NULL OR @Title = '')
	AND (d.ReferenceCode = @ReferenceCode OR @ReferenceCode IS NULL OR @ReferenceCode = '');

END
GO
/****** Object:  StoredProcedure [dbo].[USP_Documents_Get_Sent]    Script Date: 11/1/2023 10:35:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
