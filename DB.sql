USE [master]
GO
/****** Object:  Database [HA.DigitalChangeoverDB]    Script Date: 3. 8. 2014 17:16:24 ******/
CREATE DATABASE [HA.DigitalChangeoverDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'HA.DigitalChangeoverDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\HA.DigitalChangeoverDB.mdf' , SIZE = 13312KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'HA.DigitalChangeoverDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\HA.DigitalChangeoverDB_log.ldf' , SIZE = 8384KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HA.DigitalChangeoverDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET RECOVERY FULL 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET  MULTI_USER 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'HA.DigitalChangeoverDB', N'ON'
GO
USE [HA.DigitalChangeoverDB]
GO
/****** Object:  FullTextCatalog [WorkCatalog]    Script Date: 3. 8. 2014 17:16:24 ******/
CREATE FULLTEXT CATALOG [WorkCatalog]WITH ACCENT_SENSITIVITY = ON

GO
/****** Object:  StoredProcedure [dbo].[Months]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Months] 
	@year int,
	@detachmentID int AS
BEGIN
    SELECT DISTINCT MONTH([DateDate])FROM [HA.DigitalChangeoverDB].[dbo].[Dates] WHERE YEAR([DateDate]) = @year AND DateDetachmentID = @detachmentID
END

GO
/****** Object:  StoredProcedure [dbo].[Years]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Years] 
	@detachmentID int AS
BEGIN
	SELECT DISTINCT YEAR([DateDate])FROM [HA.DigitalChangeoverDB].[dbo].[Dates] WHERE DateDetachmentID = @detachmentID
END


GO
/****** Object:  Table [dbo].[Attendances]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attendances](
	[AttendanceDateID] [bigint] NOT NULL,
	[AttendanceWorkerID] [int] NOT NULL,
	[AttendanceWorkerStateID] [int] NOT NULL,
	[AttendanceWorkerTourID] [int] NOT NULL,
	[AttendanceDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_Attendances_1] PRIMARY KEY CLUSTERED 
(
	[AttendanceDateID] ASC,
	[AttendanceWorkerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Dates]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dates](
	[DateID] [bigint] IDENTITY(1,1) NOT NULL,
	[DateDate] [date] NOT NULL,
	[DateIsNight] [bit] NOT NULL,
	[DateDescription] [nvarchar](max) NULL,
	[DateDetachmentID] [int] NOT NULL,
	[DateIsClosed] [bit] NOT NULL,
 CONSTRAINT [PK_Dates] PRIMARY KEY CLUSTERED 
(
	[DateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Dates] UNIQUE NONCLUSTERED 
(
	[DateDate] ASC,
	[DateIsNight] ASC,
	[DateDetachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Detachments]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Detachments](
	[DetachmentID] [int] IDENTITY(1,1) NOT NULL,
	[DetachmentName] [nvarchar](50) NOT NULL,
	[DetachmentDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_Detachments] PRIMARY KEY CLUSTERED 
(
	[DetachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Detachments] UNIQUE NONCLUSTERED 
(
	[DetachmentName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Changeovers]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Changeovers](
	[ChangeoverID] [int] IDENTITY(1,1) NOT NULL,
	[ChangeoverDateID] [bigint] NOT NULL,
	[ChangeoverVehicleID] [int] NOT NULL,
	[ChangeoverWorkTypeID] [int] NOT NULL,
	[ChangeoverDescription] [nvarchar](max) NOT NULL,
	[ChangeoverDetachmentID] [int] NOT NULL,
 CONSTRAINT [PK_Changeovers] PRIMARY KEY CLUSTERED 
(
	[ChangeoverID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Roles]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](30) NOT NULL,
	[RoleDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tours]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tours](
	[TourID] [int] IDENTITY(1,1) NOT NULL,
	[TourStartTime] [time](0) NOT NULL,
	[TourEndTime] [time](0) NOT NULL,
	[TourDescription] [nvarchar](max) NULL,
	[TourDetachmentID] [int] NOT NULL,
 CONSTRAINT [PK_Tours] PRIMARY KEY CLUSTERED 
(
	[TourID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Tours] UNIQUE NONCLUSTERED 
(
	[TourStartTime] ASC,
	[TourEndTime] ASC,
	[TourDetachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserInRole]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInRole](
	[UserInRoleRoleID] [int] NOT NULL,
	[UserInRoleUserID] [int] NOT NULL,
 CONSTRAINT [PK_UserInRole] PRIMARY KEY CLUSTERED 
(
	[UserInRoleRoleID] ASC,
	[UserInRoleUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserWorkerID] [int] NULL,
	[UserDetachmentID] [int] NOT NULL,
	[UserName] [nvarchar](30) NOT NULL,
	[UserPassword] [nvarchar](max) NOT NULL,
	[UserDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Users] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Vehicles]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vehicles](
	[VehicleID] [int] IDENTITY(1,1) NOT NULL,
	[VehicleNumber] [nvarchar](20) NOT NULL,
	[VehicleDescription] [nvarchar](max) NULL,
	[VehicleDetachmentID] [int] NOT NULL,
 CONSTRAINT [PK_Vehicles] PRIMARY KEY CLUSTERED 
(
	[VehicleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Vehicles] UNIQUE NONCLUSTERED 
(
	[VehicleNumber] ASC,
	[VehicleDetachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkerInTour]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkerInTour](
	[WorkerInTourWorkerID] [int] NOT NULL,
	[WorkerInTourTourID] [int] NOT NULL,
 CONSTRAINT [PK_WorkerInTour] PRIMARY KEY CLUSTERED 
(
	[WorkerInTourWorkerID] ASC,
	[WorkerInTourTourID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Workers]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Workers](
	[WorkerID] [int] IDENTITY(1,1) NOT NULL,
	[WorkerFirstName] [nvarchar](20) NOT NULL,
	[WorkerLastName] [nvarchar](30) NOT NULL,
	[WorkerServiceNumber] [nvarchar](20) NOT NULL,
	[WorkerSapNumber] [nvarchar](20) NOT NULL,
	[WorkerServiceEmail] [nvarchar](50) NULL,
	[WorkerPersonalEmail] [nvarchar](50) NULL,
	[WorkerServicePhone] [nvarchar](20) NULL,
	[WorkerPersonalPhone] [nvarchar](20) NULL,
	[WorkerDescription] [nvarchar](max) NULL,
	[WorkerPhoto] [image] NULL,
	[WorkerDetachmentID] [int] NOT NULL,
 CONSTRAINT [PK_Workers] PRIMARY KEY CLUSTERED 
(
	[WorkerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Workers] UNIQUE NONCLUSTERED 
(
	[WorkerSapNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Workers_1] UNIQUE NONCLUSTERED 
(
	[WorkerServiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkerStates]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkerStates](
	[WorkerStateID] [int] IDENTITY(1,1) NOT NULL,
	[WorkerStateName] [nvarchar](30) NOT NULL,
	[WorkerStateDescription] [nvarchar](max) NULL,
	[WorkerStateDetachmentID] [int] NOT NULL,
 CONSTRAINT [PK_WorkerStates] PRIMARY KEY CLUSTERED 
(
	[WorkerStateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_WorkerStates] UNIQUE NONCLUSTERED 
(
	[WorkerStateName] ASC,
	[WorkerStateDetachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Works]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Works](
	[WorkID] [bigint] IDENTITY(1,1) NOT NULL,
	[WorkVehicleID] [int] NOT NULL,
	[WorkWorkTypeID] [int] NOT NULL,
	[WorkDateID] [bigint] NOT NULL,
	[WorkFaultDescription] [nvarchar](max) NOT NULL,
	[WorkCauseDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_Works] PRIMARY KEY CLUSTERED 
(
	[WorkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkTypes]    Script Date: 3. 8. 2014 17:16:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkTypes](
	[WorkTypeID] [int] IDENTITY(1,1) NOT NULL,
	[WorkTypeName] [nvarchar](30) NOT NULL,
	[WorkTypeDescription] [nvarchar](max) NULL,
	[WorkTypeDetachmentID] [int] NOT NULL,
 CONSTRAINT [PK_WorkTypes] PRIMARY KEY CLUSTERED 
(
	[WorkTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_WorkTypes] UNIQUE NONCLUSTERED 
(
	[WorkTypeName] ASC,
	[WorkTypeDetachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Dates] ADD  DEFAULT ((1)) FOR [DateDetachmentID]
GO
ALTER TABLE [dbo].[Tours] ADD  DEFAULT ((1)) FOR [TourDetachmentID]
GO
ALTER TABLE [dbo].[Vehicles] ADD  DEFAULT ((1)) FOR [VehicleDetachmentID]
GO
ALTER TABLE [dbo].[WorkerStates] ADD  DEFAULT ((1)) FOR [WorkerStateDetachmentID]
GO
ALTER TABLE [dbo].[WorkTypes] ADD  DEFAULT ((1)) FOR [WorkTypeDetachmentID]
GO
ALTER TABLE [dbo].[Attendances]  WITH CHECK ADD  CONSTRAINT [FK_Attendances_Dates] FOREIGN KEY([AttendanceDateID])
REFERENCES [dbo].[Dates] ([DateID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Attendances] CHECK CONSTRAINT [FK_Attendances_Dates]
GO
ALTER TABLE [dbo].[Attendances]  WITH CHECK ADD  CONSTRAINT [FK_Attendances_Tours] FOREIGN KEY([AttendanceWorkerTourID])
REFERENCES [dbo].[Tours] ([TourID])
GO
ALTER TABLE [dbo].[Attendances] CHECK CONSTRAINT [FK_Attendances_Tours]
GO
ALTER TABLE [dbo].[Attendances]  WITH CHECK ADD  CONSTRAINT [FK_Attendances_Workers] FOREIGN KEY([AttendanceWorkerID])
REFERENCES [dbo].[Workers] ([WorkerID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Attendances] CHECK CONSTRAINT [FK_Attendances_Workers]
GO
ALTER TABLE [dbo].[Attendances]  WITH CHECK ADD  CONSTRAINT [FK_Attendances_WorkerStates] FOREIGN KEY([AttendanceWorkerStateID])
REFERENCES [dbo].[WorkerStates] ([WorkerStateID])
GO
ALTER TABLE [dbo].[Attendances] CHECK CONSTRAINT [FK_Attendances_WorkerStates]
GO
ALTER TABLE [dbo].[Dates]  WITH CHECK ADD  CONSTRAINT [FK_Dates_Detachments] FOREIGN KEY([DateDetachmentID])
REFERENCES [dbo].[Detachments] ([DetachmentID])
GO
ALTER TABLE [dbo].[Dates] CHECK CONSTRAINT [FK_Dates_Detachments]
GO
ALTER TABLE [dbo].[Changeovers]  WITH CHECK ADD  CONSTRAINT [FK_Changeovers_Dates] FOREIGN KEY([ChangeoverDateID])
REFERENCES [dbo].[Dates] ([DateID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Changeovers] CHECK CONSTRAINT [FK_Changeovers_Dates]
GO
ALTER TABLE [dbo].[Changeovers]  WITH CHECK ADD  CONSTRAINT [FK_Changeovers_Detachments] FOREIGN KEY([ChangeoverDetachmentID])
REFERENCES [dbo].[Detachments] ([DetachmentID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Changeovers] CHECK CONSTRAINT [FK_Changeovers_Detachments]
GO
ALTER TABLE [dbo].[Changeovers]  WITH CHECK ADD  CONSTRAINT [FK_Changeovers_Vehicles] FOREIGN KEY([ChangeoverVehicleID])
REFERENCES [dbo].[Vehicles] ([VehicleID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Changeovers] CHECK CONSTRAINT [FK_Changeovers_Vehicles]
GO
ALTER TABLE [dbo].[Changeovers]  WITH CHECK ADD  CONSTRAINT [FK_Changeovers_WorkTypes] FOREIGN KEY([ChangeoverWorkTypeID])
REFERENCES [dbo].[WorkTypes] ([WorkTypeID])
GO
ALTER TABLE [dbo].[Changeovers] CHECK CONSTRAINT [FK_Changeovers_WorkTypes]
GO
ALTER TABLE [dbo].[Tours]  WITH CHECK ADD  CONSTRAINT [FK_Tours_Detachments] FOREIGN KEY([TourDetachmentID])
REFERENCES [dbo].[Detachments] ([DetachmentID])
GO
ALTER TABLE [dbo].[Tours] CHECK CONSTRAINT [FK_Tours_Detachments]
GO
ALTER TABLE [dbo].[UserInRole]  WITH CHECK ADD  CONSTRAINT [FK_UserInRole_Roles] FOREIGN KEY([UserInRoleRoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[UserInRole] CHECK CONSTRAINT [FK_UserInRole_Roles]
GO
ALTER TABLE [dbo].[UserInRole]  WITH CHECK ADD  CONSTRAINT [FK_UserInRole_Users] FOREIGN KEY([UserInRoleUserID])
REFERENCES [dbo].[Users] ([UserID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserInRole] CHECK CONSTRAINT [FK_UserInRole_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Detachments] FOREIGN KEY([UserDetachmentID])
REFERENCES [dbo].[Detachments] ([DetachmentID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Detachments]
GO
ALTER TABLE [dbo].[Users]  WITH NOCHECK ADD  CONSTRAINT [FK_Users_Workers] FOREIGN KEY([UserWorkerID])
REFERENCES [dbo].[Workers] ([WorkerID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Workers]
GO
ALTER TABLE [dbo].[Vehicles]  WITH CHECK ADD  CONSTRAINT [FK_Vehicles_Detachments] FOREIGN KEY([VehicleDetachmentID])
REFERENCES [dbo].[Detachments] ([DetachmentID])
GO
ALTER TABLE [dbo].[Vehicles] CHECK CONSTRAINT [FK_Vehicles_Detachments]
GO
ALTER TABLE [dbo].[WorkerInTour]  WITH CHECK ADD  CONSTRAINT [FK_WorkerInTour_Tours] FOREIGN KEY([WorkerInTourTourID])
REFERENCES [dbo].[Tours] ([TourID])
GO
ALTER TABLE [dbo].[WorkerInTour] CHECK CONSTRAINT [FK_WorkerInTour_Tours]
GO
ALTER TABLE [dbo].[WorkerInTour]  WITH CHECK ADD  CONSTRAINT [FK_WorkerInTour_Workers] FOREIGN KEY([WorkerInTourWorkerID])
REFERENCES [dbo].[Workers] ([WorkerID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WorkerInTour] CHECK CONSTRAINT [FK_WorkerInTour_Workers]
GO
ALTER TABLE [dbo].[Workers]  WITH CHECK ADD  CONSTRAINT [FK_Workers_Detachments] FOREIGN KEY([WorkerDetachmentID])
REFERENCES [dbo].[Detachments] ([DetachmentID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Workers] CHECK CONSTRAINT [FK_Workers_Detachments]
GO
ALTER TABLE [dbo].[WorkerStates]  WITH CHECK ADD  CONSTRAINT [FK_WorkerStates_Detachments] FOREIGN KEY([WorkerStateDetachmentID])
REFERENCES [dbo].[Detachments] ([DetachmentID])
GO
ALTER TABLE [dbo].[WorkerStates] CHECK CONSTRAINT [FK_WorkerStates_Detachments]
GO
ALTER TABLE [dbo].[Works]  WITH CHECK ADD  CONSTRAINT [FK_Works_Dates] FOREIGN KEY([WorkDateID])
REFERENCES [dbo].[Dates] ([DateID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Works] CHECK CONSTRAINT [FK_Works_Dates]
GO
ALTER TABLE [dbo].[Works]  WITH CHECK ADD  CONSTRAINT [FK_Works_Vehicles] FOREIGN KEY([WorkVehicleID])
REFERENCES [dbo].[Vehicles] ([VehicleID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Works] CHECK CONSTRAINT [FK_Works_Vehicles]
GO
ALTER TABLE [dbo].[Works]  WITH CHECK ADD  CONSTRAINT [FK_Works_WorkTypes] FOREIGN KEY([WorkWorkTypeID])
REFERENCES [dbo].[WorkTypes] ([WorkTypeID])
GO
ALTER TABLE [dbo].[Works] CHECK CONSTRAINT [FK_Works_WorkTypes]
GO
ALTER TABLE [dbo].[WorkTypes]  WITH CHECK ADD  CONSTRAINT [FK_WorkTypes_Detachments] FOREIGN KEY([WorkTypeDetachmentID])
REFERENCES [dbo].[Detachments] ([DetachmentID])
GO
ALTER TABLE [dbo].[WorkTypes] CHECK CONSTRAINT [FK_WorkTypes_Detachments]
GO
USE [master]
GO
ALTER DATABASE [HA.DigitalChangeoverDB] SET  READ_WRITE 
GO
