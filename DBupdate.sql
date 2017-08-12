ALTER TABLE dbo.Tours 
	ADD	TourDetachmentID int NOT NULL DEFAULT 1
ALTER TABLE dbo.Tours 
	DROP CONSTRAINT IX_Tours
ALTER TABLE dbo.Tours 
	ADD CONSTRAINT
	IX_Tours UNIQUE NONCLUSTERED 
	(
	TourStartTime,
	TourEndTime,
	TourDetachmentID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];


ALTER TABLE dbo.Vehicles ADD
	VehicleDetachmentID int NOT NULL DEFAULT 1
ALTER TABLE dbo.Vehicles
	DROP CONSTRAINT IX_Vehicles
ALTER TABLE dbo.Vehicles ADD CONSTRAINT
	IX_Vehicles UNIQUE NONCLUSTERED 
	(
	VehicleNumber,
	VehicleDetachmentID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	

ALTER TABLE dbo.WorkerStates ADD
	WorkerStateDetachmentID int NOT NULL DEFAULT 1
ALTER TABLE dbo.WorkerStates
	DROP CONSTRAINT IX_WorkerStates
ALTER TABLE dbo.WorkerStates ADD CONSTRAINT
	IX_WorkerStates UNIQUE NONCLUSTERED 
	(
	WorkerStateName,
	WorkerStateDetachmentID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


ALTER TABLE dbo.WorkTypes ADD
	WorkTypeDetachmentID int NOT NULL DEFAULT 1
ALTER TABLE dbo.WorkTypes
	DROP CONSTRAINT IX_WorkTypes
ALTER TABLE dbo.WorkTypes ADD CONSTRAINT
	IX_WorkTypes UNIQUE NONCLUSTERED 
	(
	WorkTypeName,
	WorkTypeDetachmentID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


ALTER TABLE dbo.Works
	DROP CONSTRAINT FK_Works_Detachments
ALTER TABLE dbo.Works
	DROP COLUMN WorkDetachmentID


ALTER TABLE dbo.Tours 
ADD CONSTRAINT FK_Tours_Detachments FOREIGN KEY (TourDetachmentID) 
    REFERENCES dbo.Detachments (DetachmentID) 
    ON DELETE NO ACTION
    ON UPDATE NO ACTION


ALTER TABLE dbo.Vehicles 
ADD CONSTRAINT FK_Vehicles_Detachments FOREIGN KEY (VehicleDetachmentID) 
    REFERENCES dbo.Detachments (DetachmentID) 
    ON DELETE NO ACTION
    ON UPDATE NO ACTION


ALTER TABLE dbo.WorkerStates 
ADD CONSTRAINT FK_WorkerStates_Detachments FOREIGN KEY (WorkerStateDetachmentID) 
    REFERENCES dbo.Detachments (DetachmentID) 
    ON DELETE NO ACTION
    ON UPDATE NO ACTION


ALTER TABLE dbo.WorkTypes 
ADD CONSTRAINT FK_WorkTypes_Detachments FOREIGN KEY (WorkTypeDetachmentID) 
    REFERENCES dbo.Detachments (DetachmentID) 
    ON DELETE NO ACTION
    ON UPDATE NO ACTION


ALTER TABLE dbo.Dates ADD
	DateDetachmentID int  NOT NULL DEFAULT 1
ALTER TABLE dbo.Dates
	DROP CONSTRAINT IX_Dates
ALTER TABLE dbo.Dates ADD CONSTRAINT
	IX_Dates UNIQUE NONCLUSTERED 
	(
	DateDate,
	DateIsNight,
	DateDetachmentID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
ALTER TABLE dbo.Dates
ADD CONSTRAINT FK_Dates_Detachments FOREIGN KEY (DateDetachmentID) 
    REFERENCES dbo.Detachments (DetachmentID) 
    ON DELETE NO ACTION
    ON UPDATE NO ACTION


ALTER TABLE dbo.Users
	DROP CONSTRAINT FK_Users_Detachments 
ALTER TABLE dbo.Users
ADD CONSTRAINT FK_Users_Detachments FOREIGN KEY (UserDetachmentID) 
    REFERENCES dbo.Detachments (DetachmentID) 
    ON DELETE NO ACTION
    ON UPDATE NO ACTION


CREATE PROCEDURE Years 
	@detachmentID int AS
BEGIN
	SELECT DISTINCT YEAR([DateDate])FROM [HA.DigitalChangeoverDB].[dbo].[Dates] WHERE DateDetachmentID = @detachmentID
END



CREATE PROCEDURE Months 
	@year int,
	@detachmentID int AS
BEGIN
    SELECT DISTINCT MONTH([DateDate])FROM [HA.DigitalChangeoverDB].[dbo].[Dates] WHERE YEAR([DateDate]) = @year AND DateDetachmentID = @detachmentID
END



CREATE TRIGGER dbo.CascadeDeleteTrigger
ON dbo.Detachments
INSTEAD OF DELETE
AS 
BEGIN
declare @id int
select @id =  DetachmentID from deleted;
DELETE  FROM [dbo].[Vehicles] WHERE [VehicleDetachmentID] = @id;
DELETE  FROM [dbo].[Dates] WHERE [DateDetachmentID] = @id;
DELETE  FROM [dbo].[Changeovers] WHERE [ChangeoverDetachmentID] = @id;
DELETE  FROM [dbo].[Tours] WHERE [TourDetachmentID] = @id;
DELETE  FROM [dbo].[Users] WHERE [UserDetachmentID] = @id;
DELETE  FROM [dbo].[Users] WHERE [UserDetachmentID] = @id;
DELETE  FROM [dbo].[Workers] WHERE [WorkerDetachmentID] = @id;
DELETE  FROM [dbo].[WorkerStates] WHERE [WorkerStateDetachmentID] = @id;
DELETE  FROM [dbo].[WorkTypes] WHERE [WorkTypeDetachmentID] = @id;
DELETE  FROM [dbo].[Detachments] WHERE [DetachmentID] = @id;
END