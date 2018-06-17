--DROP TABLE Report;
--SELECT * FROM information_schema.tables;
--create table Parking20.dbo.Report (ID int IDENTITY(1, 1), VisitorId int unique not null, EnterTime datetime null, OutTime datetime null);

--ALTER TABLE Report add VisitorId int unique not null, EnterTime datetime null, OutTime datetime null

--EXEC sp_RENAME 'Report.id' , 'ID', 'COLUMN';

--ALTER TABLE Report DROP COLUMN ID;
--ALTER TABLE Report alter column id int IDENTITY(1, 1);



/*SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' AND TABLE_NAME = 'Report' 
AND TABLE_SCHEMA ='dbo';*/

--SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'Report';


/*select *, Data, Car.ID from ReportMessageMain
    join (select ID, Arguments from ReportMessageArguments where Keys = 12 and Arguments is not null and Arguments != '') as number on number.ID = ReportMessageMain.ID
    join Car on Car.Number = number.Arguments
    where type = 201 and Data >= '2015-08-10' order by ReportMessageMain.Data*/

--select top 1 EnterTime from Report order by EnterTime desc;

--select top 10 * from ReportMessageArguments where ReportMessageArguments.ID = 1511;

--SELECT top 10 ID, Arguments FROM ReportMessageArguments WHERE Keys = 12 AND Arguments is not null AND Arguments != ''


/*select top 1 * from ReportMessageArguments where Arguments = '00 4335';


select top 1 * from Visitor;*/
--select top 1 * from Car;

/*select * from Report
join Visitor on Visitor.ID = Report.VisitorId
join Car on Car.ID = Report.VisitorId;*/

/*SELECT TOP 1 Data
    FROM ReportMessageMain
	LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = ReportMessageMain.ID
	LEFT JOIN Car ON  Car.Number = numberComeOut.Arguments
	WHERE Type = 203
		AND Data > '01.01.01'
		AND Car.ID = 110
    ORDER BY Data*/

--SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12

/*SELECT TOP 1 Data
    FROM ReportMessageMain
	WHERE Type = 203
		AND Data > '2015-08-01'
    ORDER BY Data*/

	--insert into Report values (110, '2013-12-20', '2013-12-21');

	--update Report set EnterTime = '2013-01-01' where ID = 0;

	--select * from Report order by EnterTime;

	--truncate table Report;