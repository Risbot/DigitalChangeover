USE [HA.DigitalChangeoverDB]
GO

INSERT INTO [dbo].[Detachments]([DetachmentName])VALUES('Zkušebna 471 Praha ONJ')
INSERT INTO [dbo].[Tours]([TourStartTime],[TourEndTime])VALUES('18:15','06:15')
INSERT INTO [dbo].[Tours]([TourStartTime],[TourEndTime])VALUES('06:15','18:15')
INSERT INTO [dbo].[WorkerStates]([WorkerStateName])VALUES('Pøítomen')
INSERT INTO [dbo].[WorkerStates]([WorkerStateName])VALUES('Dovolená')
INSERT INTO [dbo].[WorkerStates]([WorkerStateName])VALUES('Nemocen')
INSERT INTO [dbo].[WorkerStates]([WorkerStateName])VALUES('Nepøítomen')

USE [HA.DigitalChangeoverDB]
GO

DECLARE @count INT = 0
DECLARE @motorak INT = 471000
DECLARE @ridicak INT = 971000
DECLARE @vlozak INT = 071000

WHILE @count < 50
BEGIN
  INSERT INTO [dbo].[Vehicles]([VehicleNumber],[VehicleDetachmentID])VALUES(@motorak, 1)
  INSERT INTO [dbo].[Vehicles]([VehicleNumber],[VehicleDetachmentID])VALUES(Right('000000' + convert(nvarchar(10), @vlozak), 6), 1)
  INSERT INTO [dbo].[Vehicles]([VehicleNumber],[VehicleDetachmentID])VALUES(@ridicak, 1)
  SET @motorak = @motorak + 1
  SET @ridicak = @ridicak + 1
  SET @vlozak = @vlozak + 1
  SET @count = @count+1
END

USE [HA.DigitalChangeoverDB]
GO

DECLARE @startdt DATE, @enddt DATE

SET @startdt = '2007-12-24'
SET @enddt = '2050-02-27'

WHILE @startdt < @enddt
BEGIN
  INSERT INTO [dbo].[Dates]
           ([DateDate]
           ,[DateIsNight]
           ,[DateIsClosed])
     VALUES
           (@startdt
           ,0
           ,0)

  set @startdt = DATEADD(day, 1, @startdt)
END
 

GO


