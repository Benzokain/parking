/*select Data, Type, Source, FullCode, Arguments from ReportMessageMain
left join ReportMessageArguments on ReportMessageArguments.id = ReportMessageMain.id
where Data >= '2014-01-01' and Data <= '2015-08-31'
--and Arguments like '%Прохорова%'
and 1 = 1
and Type not in (201, 203, 205, 206)
Order by Data;*/


/*DECLARE @date date = '2015-01-04';
DECLARE @hours int = 0;
SELECT *,
	CASE WHEN Hours > 3 AND NOT number LIKE '%S%' THEN Hours * 50 ELSE 0 END AS Amount
FROM (
	SELECT *, 
		CASE WHEN OutTime IS NULL THEN DATEDIFF(hour, EnterTime, GETDATE()) ELSE DATEDIFF(hour, EnterTime, OutTime) END AS Hours
	FROM (
		SELECT comeIn.ID, comeIn.Data AS EnterTime, comeIn.Type, secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number,
			(SELECT TOP 1 comeOut.Data from ReportMessageMain AS comeOut
			LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
			WHERE comeOut.Type = 203
				AND comeOut.Data > comeIn.Data
				AND numberComeOut.Arguments = numberComeIn.Arguments) AS OutTime
		FROM ReportMessageMain AS comeIn
		LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = comeIn.ID
		LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = comeIn.ID
		LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = comeIn.ID
		WHERE comeIn.Type = 201) AS src2) AS src1
WHERE EnterTime >= @date AND EnterTime < DATEADD (day, 1, @date)
	AND OutTime >= @date AND OutTime < DATEADD (day, 1, @date)
	AND (EnterTime >= @date OR OutTime IS NULL)
	AND Hours > @hours
	AND EnterTime > CAST('2015-01-01' AS DATE)
ORDER BY EnterTime;*/


/*select * from ReportMessageArguments where ID = 3575;*/

/*SELECT comeIn.Data AS EnterTime, comeIn.Type, secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number
FROM ReportMessageMain AS comeIn
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = comeIn.ID
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = comeIn.ID
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = comeIn.ID
WHERE comeIn.Data >= '2015-08-06' AND comeIn.Data <= '2015-08-07'
AND comeIn.Type IN (201, 203)
AND numberComeIn.Arguments = '00 4335'


select TOP 1 ReportMessageMain.ID, ReportMessageMain.Data, ReportMessageMain.Type, number.Arguments from ReportMessageMain
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS number ON number.ID = ReportMessageMain.ID
WHERE ReportMessageMain.Type = 203
AND Data > '2015-08-06 06:42:38.000'
AND number.Arguments LIKE '00 5078'
ORDER BY Data


DECLARE @date date;
SELECT comeIn.Data AS EnterTime, comeIn.Type, secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number
FROM ReportMessageMain AS comeIn
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = comeIn.ID
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = comeIn.ID
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = comeIn.ID
WHERE comeIn.Type = 201
	AND comeIn.Data >= @date AND comeIn.Data < DATEADD (day, 1, @date)
ORDER BY comeIn.Data*/

/*SELECT Convert(char(8), Data, 112), Data, secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number
FROM (
	SELECT Convert(char(8), Data, 112) AS DataDay, ReportMessageMain.Type AS Type, number.Arguments AS Number, count (ReportMessageMain.Type) AS CominCount from ReportMessageMain
	LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS number ON number.ID = ReportMessageMain.ID
	WHERE Type = 201
	GROUP BY Convert(char(8), Data, 112), number.Arguments, ReportMessageMain.Type
	HAVING count (number.Arguments) > 1) As src1
LEFT JOIN (
	SELECT Convert(char(8), Data, 112) AS DataDay, ReportMessageMain.Type AS Type, number.Arguments AS Number, count (ReportMessageMain.Type) AS ComeoutCount from ReportMessageMain
	LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS number ON number.ID = ReportMessageMain.ID
	WHERE Type = 203
	GROUP BY Convert(char(8), Data, 112), number.Arguments, ReportMessageMain.Type
	--HAVING count (number.Arguments) > 1) AS src2 ON CAST(REPLACE(src1.Number, ' ', '') AS INT) = CAST(REPLACE(src2.Number, ' ', '') AS INT)
	HAVING count (number.Arguments) > 1) AS src2 ON CONCAT(src1.DataDay, src1.Number) = CONCAT(src2.DataDay, src2.Number)
WHERE 1 = 1
	--src1.CominCount != src2.ComeoutCount 
	--AND src1.DataDay ='20150906'
	--AND src1.Number = '00 01964'
ORDER BY src1.DataDay*/

DECLARE @date date = '2015-08-08';
SELECT *
FROM (
	SELECT Data AS EnterTime, secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number,
		CASE WHEN (SELECT TOP 1 comeOut.TYpe FROM ReportMessageMain AS comeOut
			LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
			WHERE comeOut.Type IN (201, 203)
				AND comeOut.Data > comeIn.Data
				AND numberComeOut.Arguments = numberComeIn.Arguments
				AND comeOut.Data > DATEADD (week, -1, @date)) = 203
		THEN 
			(SELECT TOP 1 comeOut.Data from ReportMessageMain AS comeOut
			LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
			WHERE comeOut.Type IN (201, 203)
				AND comeOut.Data > comeIn.Data
				AND numberComeOut.Arguments = numberComeIn.Arguments
				AND comeOut.Data > DATEADD (week, -1, @date))
			ELSE NULL END AS OutTime
	FROM ReportMessageMain AS comeIn
	LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = comeIn.ID
	LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = comeIn.ID
	LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = comeIn.ID
	WHERE Type = 201
		AND Data < DATEADD (day, 1, @date)
		AND Data > DATEADD (week, -1, @date)) AS src1
WHERE OutTime >= @date OR OutTime IS NULL
ORDER BY EnterTime



/*SELECT comeIn.Data AS EnterTime, comeIn.Type, secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number
FROM ReportMessageMain AS comeIn
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = comeIn.ID
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = comeIn.ID
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = comeIn.ID
WHERE type IN (201, 203) AND numberComeIn.Arguments = '00 3142' AND comeIn.Data >= '2015-07-22' AND comeIn.Data <='2015-09-03'*/

--DECLARE @date date = '2015-02-01';
/*SELECT Data AS EnterTime, secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number,
	CASE WHEN (SELECT TOP 1 comeOut.Type FROM ReportMessageMain AS comeOut
		LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
		WHERE comeOut.Type in (201, 203)
			AND comeOut.Data > comeIn.Data
			AND numberComeOut.Arguments = numberComeIn.Arguments
			AND comeOut.Data > '2015-01-01') = 203
	THEN 
		(SELECT TOP 1 comeOut.Data from ReportMessageMain AS comeOut
		LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
		WHERE comeOut.Type in (201, 203)
			AND comeOut.Data > comeIn.Data
			AND numberComeOut.Arguments = numberComeIn.Arguments
			AND comeOut.Data > '2015-01-01')
		ELSE NULL END AS OutTime
FROM ReportMessageMain AS comeIn
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = comeIn.ID
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = comeIn.ID
LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = comeIn.ID
WHERE Type = 201
	AND Data < DATEADD (day, 1, '2015-02-01')*/