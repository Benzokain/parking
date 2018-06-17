/*DECLARE @date date = '2015-8-28';
DECLARE @hours int = 0;
SELECT *,
	CASE WHEN Hours > 3 AND NOT Number LIKE '%S%' THEN Hours * 50 ELSE 0 END AS Amount
FROM (
	SELECT *,
		CASE WHEN OutTime IS NULL THEN DATEDIFF(hour, EnterTime, GETDATE()) ELSE DATEDIFF(hour, EnterTime, OutTime) END AS Hours
	FROM (
		SELECT Visitor.LastName, Visitor.FirstName, Car.Number, Car.Brand, History.EnterTime,
			(SELECT _History.EnterTime
			FROM History AS _History
			WHERE _History.ID = History.ID
				AND _History.ListID = History.ListID + 1) AS OutTime
		FROM History 
		INNER JOIN Visitor ON Visitor.ID = History.ID
		INNER JOIN Car ON Car.ID = History.ID
		WHERE (History.ListID % 2) = 0) AS src2) AS src1
WHERE EnterTime <= DATEADD (day, 1, @date)
	AND (OutTime >= @date OR OutTime IS NULL)
	AND Hours > @hours
	AND EnterTime > CAST('2015-01-01' AS DATE)
ORDER BY EnterTime;
*/

select history.id, LastName, FirstName, EnterTime, History.ListID, Car.Number, Account.*, Registration.*, ReportMessageArguments.*, ReportMessageIndex.*, ReportMessageMain.*, Tag.* from History
LEFT JOIN Visitor ON Visitor.ID = History.ID
LEFT JOIN Car ON Car.ID = History.ID
LEFT JOIN Registration ON Registration.ID = History.ID
LEFT JOIN Arguments ON Arguments.ID = History.ID
LEFT JOIN Account ON Account.ID = History.ID
LEFT JOIN ReportMessageArguments ON ReportMessageArguments.ID = History.ID
LEFT JOIN ReportMessageIndex ON ReportMessageIndex.ID = History.ID
LEFT JOIN ReportMessageMain ON ReportMessageMain.ID = History.ID
LEFT JOIN Tag ON Tag.ID = History.ID
WHERE 
--EnterTime <= CAST('2015-08-28' AS DATE)
--AND FirstName LIKE '%прох%'
--LastName LIKE '%прох%'
History.ID = 1387
--History.ID = 1429
--1 = 1
ORDER BY History.ListID DESC;

/*DECLARE @date date = '2015-8-28';
DECLARE @hours int = 0;
SELECT *,
	CASE WHEN Hours > 3 AND NOT Number LIKE '%S%' THEN Hours * 50 ELSE 0 END AS Amount
FROM (
	SELECT *,
		CASE WHEN OutTime IS NULL THEN DATEDIFF(hour, EnterTime, GETDATE()) ELSE DATEDIFF(hour, EnterTime, OutTime) END AS Hours
	FROM (
		SELECT Visitor.LastName, Visitor.FirstName, Car.Number, Car.Brand, History.EnterTime,
			(SELECT _History.EnterTime
			FROM History AS _History
			WHERE _History.ID = History.ID
				AND _History.ListID = History.ListID + 1) AS OutTime
		FROM History 
		INNER JOIN Visitor ON Visitor.ID = History.ID
		INNER JOIN Car ON Car.ID = History.ID
		WHERE (History.ListID % 2) = 0) AS src2) AS src1
WHERE EnterTime <= @date
	AND (OutTime >= @date OR OutTime IS NULL)
	AND Hours > @hours
	AND EnterTime > CAST('2015-01-01' AS DATE)
ORDER BY EnterTime;*/


/*RESTORE FILELISTONLY
FROM DISK = 'D:\Parking20\Parking20.bak'
GO*/

/*RESTORE HEADERONLY
FROM DISK = 'D:\Parking20\Parking20.bak'*/

/*ALTER DATABASE Parking20 SET OFFLINE WITH ROLLBACK IMMEDIATE;
ALTER DATABASE Parking20 SET ONLINE;
DROP DATABASE Parking20;*/

/*RESTORE DATABASE Parking20
FROM DISK = 'D:\Parking20\Parking20.bak'
WITH MOVE 'Parking20' TO 'D:\Parking20\Parking20.mdf',
MOVE 'Parking20_log' TO 'D:\Parking20\Parking20.ldf'*/

/*RESTORE DATABASE Parking20
FROM DISK = 'D:\Parking20\Parking20.bak'
WITH FILE = 2,
MOVE 'Parking20' TO 'D:\Parking20\Parking20.mdf',
MOVE 'Parking20_log' TO 'D:\Parking20\Parking20.ldf'*/