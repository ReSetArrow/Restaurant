USE [master]
GO
/****** Object:  Database [Restaurant]    Script Date: 2025/3/24 下午 02:03:23 ******/
CREATE DATABASE [Restaurant]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Restaurant', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Restaurant.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Restaurant_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Restaurant_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Restaurant] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Restaurant].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Restaurant] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Restaurant] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Restaurant] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Restaurant] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Restaurant] SET ARITHABORT OFF 
GO
ALTER DATABASE [Restaurant] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Restaurant] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Restaurant] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Restaurant] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Restaurant] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Restaurant] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Restaurant] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Restaurant] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Restaurant] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Restaurant] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Restaurant] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Restaurant] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Restaurant] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Restaurant] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Restaurant] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Restaurant] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [Restaurant] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Restaurant] SET RECOVERY FULL 
GO
ALTER DATABASE [Restaurant] SET  MULTI_USER 
GO
ALTER DATABASE [Restaurant] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Restaurant] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Restaurant] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Restaurant] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Restaurant] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Restaurant] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Restaurant', N'ON'
GO
ALTER DATABASE [Restaurant] SET QUERY_STORE = ON
GO
ALTER DATABASE [Restaurant] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Restaurant]
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetNewUserNumber]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fnGetNewUserNumber] (@RoleCode NVARCHAR(2))
RETURNS NVARCHAR(14)
AS
BEGIN
    DECLARE @newUserNumber NVARCHAR(14)
    DECLARE @datePart NVARCHAR(8)
    DECLARE @numberPart INT
    DECLARE @newNumberPart NVARCHAR(4)

    -- 取得當天日期部分
    SET @datePart = CONVERT(NVARCHAR(8), GETDATE(), 112) -- yyyyMMdd 格式

    -- 取得最新的 UserNumber
    SELECT TOP 1 @newUserNumber = UserNumber
    FROM UserLogin
    WHERE UserNumber LIKE @RoleCode + @datePart + '%'
    ORDER BY UserNumber DESC

    -- 提取數字部分，轉換為整數
    IF @newUserNumber IS NOT NULL
    BEGIN
        SET @numberPart = CAST(SUBSTRING(@newUserNumber, 11, 4) AS INT)
    END
    ELSE
    BEGIN
        SET @numberPart = 0
    END

    -- 加 1
    SET @numberPart = @numberPart + 1

    -- 將新的數字部分轉為字串，並確保補零至 4 位
    SET @newNumberPart = RIGHT('0000' + CAST(@numberPart AS NVARCHAR(4)), 4)

    -- 拼接回新的 UserNumber
    SET @newUserNumber = @RoleCode + @datePart + @newNumberPart

    RETURN @newUserNumber
END
GO
/****** Object:  UserDefinedFunction [dbo].[getOrderNumber]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 Create FUNCTION [dbo].[getOrderNumber]()

     RETURNS CHAR(12)
 AS
 BEGIN
     DECLARE @TodayDate VARCHAR(8);
 DECLARE @SerialNumber INT;
 DECLARE @NewOrderNumber VARCHAR(12);

 --取得今天的日期（格式: YYYYMMDD）
     SET @TodayDate = CONVERT(VARCHAR(8), GETDATE(), 112);

 --查詢今天已有的訂單數量，並計算下一個流水號
 SELECT @SerialNumber = ISNULL(MAX(CAST(SUBSTRING(OrderNumber, 9, 4) AS INT)), 0) + 1
     FROM [Order]
 WHERE OrderNumber LIKE @TodayDate + '%';

 --組合新的訂單編號，格式: YYYYMMDD + 4位數流水號
 SET @NewOrderNumber = @TodayDate + RIGHT('0000' + CAST(@SerialNumber AS VARCHAR(4)), 4);

 RETURN @NewOrderNumber;
 END
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[OrderNumber] [nchar](12) NOT NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[StatusCode] [nchar](2) NOT NULL,
	[UserNumber] [nvarchar](14) NOT NULL,
	[PaymentCode] [nchar](2) NOT NULL,
	[ExpectedArrivalDate] [nchar](12) NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[OrderNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderNumber] [nchar](12) NOT NULL,
	[TableID] [nchar](3) NOT NULL,
	[pricing] [money] NOT NULL,
	[Qty] [int] NOT NULL,
 CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	[OrderNumber] ASC,
	[TableID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[PaymentCode] [nchar](2) NOT NULL,
	[PaymentType] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[PaymentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleCode] [nchar](2) NOT NULL,
	[Title] [nvarchar](10) NOT NULL,
	[Supervisor] [bit] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[StatusCode] [nchar](2) NOT NULL,
	[StatusCategory] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Table]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Table](
	[TableID] [nchar](3) NOT NULL,
	[Limit] [int] NOT NULL,
	[Pricing] [money] NOT NULL,
	[Remark] [nvarchar](max) NOT NULL,
	[Picture] [nchar](10) NULL,
	[ImageType] [nvarchar](max) NULL,
 CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED 
(
	[TableID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogin]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogin](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Account] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](64) NOT NULL,
	[Name] [nvarchar](25) NOT NULL,
	[ID] [nchar](10) NOT NULL,
	[Birthday] [datetime2](7) NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[UserNumber] [nvarchar](14) NOT NULL,
	[ManagerNumber] [nvarchar](14) NULL,
	[RoleCode] [nchar](2) NOT NULL,
 CONSTRAINT [PK_UserLogin] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_UserLogin_Account] UNIQUE NONCLUSTERED 
(
	[Account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_UserLogin_UserNumber] UNIQUE NONCLUSTERED 
(
	[UserNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Order_MemberNumber]    Script Date: 2025/3/24 下午 02:03:23 ******/
CREATE NONCLUSTERED INDEX [IX_Order_MemberNumber] ON [dbo].[Order]
(
	[UserNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Order_PaymentCode]    Script Date: 2025/3/24 下午 02:03:23 ******/
CREATE NONCLUSTERED INDEX [IX_Order_PaymentCode] ON [dbo].[Order]
(
	[PaymentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Order_StatusCode]    Script Date: 2025/3/24 下午 02:03:23 ******/
CREATE NONCLUSTERED INDEX [IX_Order_StatusCode] ON [dbo].[Order]
(
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderDetails_TableID]    Script Date: 2025/3/24 下午 02:03:23 ******/
CREATE NONCLUSTERED INDEX [IX_OrderDetails_TableID] ON [dbo].[OrderDetails]
(
	[TableID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_OrderNumber]  DEFAULT ([dbo].[getOrderNumber]()) FOR [OrderNumber]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_RoleCode]  DEFAULT ('MB') FOR [RoleCode]
GO
ALTER TABLE [dbo].[UserLogin] ADD  CONSTRAINT [DF_UserLogin_UserNumber]  DEFAULT (N'dbo.fnGetNewUserNumber()') FOR [UserNumber]
GO
ALTER TABLE [dbo].[UserLogin] ADD  CONSTRAINT [DF_UserLogin_ManagerNumber]  DEFAULT (N'dbo.fnGetNewUserNumber()') FOR [ManagerNumber]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Payment] FOREIGN KEY([PaymentCode])
REFERENCES [dbo].[Payment] ([PaymentCode])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Payment]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Status] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[Status] ([StatusCode])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Status]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_UserLogin] FOREIGN KEY([UserNumber])
REFERENCES [dbo].[UserLogin] ([UserNumber])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_UserLogin]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Order] FOREIGN KEY([OrderNumber])
REFERENCES [dbo].[Order] ([OrderNumber])
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Order]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Table] FOREIGN KEY([TableID])
REFERENCES [dbo].[Table] ([TableID])
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Table]
GO
ALTER TABLE [dbo].[UserLogin]  WITH CHECK ADD  CONSTRAINT [FK_UserLogin_RoleCode] FOREIGN KEY([RoleCode])
REFERENCES [dbo].[Role] ([RoleCode])
GO
ALTER TABLE [dbo].[UserLogin] CHECK CONSTRAINT [FK_UserLogin_RoleCode]
GO
ALTER TABLE [dbo].[Table]  WITH CHECK ADD  CONSTRAINT [CK_Table_TableID_Format] CHECK  (([TableID] like '[A-Z][1-3][1-9]'))
GO
ALTER TABLE [dbo].[Table] CHECK CONSTRAINT [CK_Table_TableID_Format]
GO
/****** Object:  StoredProcedure [dbo].[AddNewOrder]    Script Date: 2025/3/24 下午 02:03:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE proc [dbo].[AddNewOrder]
     @StatusCode nchar(2),
     @UserNumber nchar(14),
     @PaymentCode nchar(2),
     @ExpectedArrivalDate nchar(12),
     @Cart nvarchar(max) --訂單JSON 資訊放進來
 as
 begin
     begin tran --開始交易處理程序

     declare @OrderNumber nchar(12) = dbo.getOrderNumber()
     
     -- 插入訂單
     insert into [Order] 
     values(@OrderNumber, getdate(), @StatusCode, @UserNumber, @PaymentCode, @ExpectedArrivalDate)

     if @@ERROR = 0 --上面程序沒有出錯繼續執行
     begin
         insert into OrderDetails (OrderNumber, TableID, Pricing, Qty)
         select 
             @OrderNumber, 
             TableID, 
             Pricing,
             Qty
         from openjson(@Cart) 
         with(
             TableID nchar(3),
             Pricing money,
             Qty int
         )

         if @@ERROR = 0 --第二道程序沒有出錯
             commit --執行
         else
         begin
             rollback
             raiserror('Error inserting into OrderDetails', 16, 1)
         end
     end
     else
     begin
         rollback
         raiserror('Error inserting into Order', 16, 1)
     end
 end
GO
USE [master]
GO
ALTER DATABASE [Restaurant] SET  READ_WRITE 
GO
