using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelLibrary;
using ExcelLibrary.SpreadSheet;

namespace WindowsFormsApplication6
{
    public partial class Form1 : Form
    {

        private bool isProd = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //displayTable();
            DateTime lastReportDate = getLastReportDate();
            DataTable EnterTimeTable = devGetEnterTimeAlter(lastReportDate);
            EnterTimeTable.Columns.Add("OutTime");
            devUpdateReportTable(EnterTimeTable);

        }

        private void showAllButton_Click(object sender, EventArgs e)
        {
            displayTable();
        }

        private void mustPayButton_Click(object sender, EventArgs e)
        {
            if (this.dataSet2.Tables[0].Rows.Count > 0) {
                int hoursLimit = Convert.ToInt32(this.hoursBox.Text);
                decimal price = Convert.ToInt32(this.priceBox.Text);
                foreach (DataRow row in this.dataSet2.Tables[0].Rows) {
                    int hours = (row["Hours"] != DBNull.Value) ? Convert.ToInt32(row["Hours"]) : 0; 
                    if (hours <= hoursLimit) {
                        row.Delete();
                    }
                }
                this.dataSet2.Tables[0].AcceptChanges();

                dataGridView1.DataSource = this.dataSet2.Tables[0];
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //ExcelLibrary.DataSetHelper.CreateWorkbook("MyExcelFile.xls", this.dataSet2);
            string file = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("First Sheet");
            worksheet.Cells[0, 0] = new Cell(dataGridView1.Columns[0].HeaderText);
            worksheet.Cells[0, 1] = new Cell(dataGridView1.Columns[1].HeaderText);
            worksheet.Cells[0, 2] = new Cell(dataGridView1.Columns[2].HeaderText);
            worksheet.Cells[0, 3] = new Cell(dataGridView1.Columns[3].HeaderText);
            worksheet.Cells[0, 4] = new Cell(dataGridView1.Columns[4].HeaderText);
            worksheet.Cells[0, 5] = new Cell(dataGridView1.Columns[5].HeaderText);
            worksheet.Cells[0, 6] = new Cell(dataGridView1.Columns[6].HeaderText);

            int rowCnt = 1;
            foreach (DataRow row in this.dataSet2.Tables[0].Rows) {
                worksheet.Cells[rowCnt, 0] = new Cell(row[0].ToString());
                worksheet.Cells[rowCnt, 1] = new Cell(row[1].ToString());
                worksheet.Cells[rowCnt, 2] = new Cell(row[2].ToString());
                worksheet.Cells[rowCnt, 3] = new Cell(row[3].ToString());
                worksheet.Cells[rowCnt, 4] = new Cell(row[4].ToString());
                worksheet.Cells[rowCnt, 5] = new Cell(row[5].ToString());
                worksheet.Cells[rowCnt, 6] = new Cell(row[6].ToString());
                rowCnt++;
            }

            workbook.Worksheets.Add(worksheet);

            workbook.Save(file);
        }

        private void displayTable(int hoursLimit = 3)
        {
            getEnterTimes();

            this.progressBar1.Visible = true;
            this.progressBar1.Minimum = 1;
            this.progressBar1.Maximum = this.dataSet2.Tables[0].Rows.Count;
            this.progressBar1.Value = 1;
            this.progressBar1.Step = 1;

            DataColumn outTimeColumn = new DataColumn("OutTime", typeof(DateTime));
            DataColumn hoursColumn = new DataColumn("Hours", typeof(int));
            DataColumn amountColumn = new DataColumn("Amount", typeof(int));

            this.dataSet2.Tables[0].Columns.Add(outTimeColumn);
            this.dataSet2.Tables[0].Columns.Add(hoursColumn);
            this.dataSet2.Tables[0].Columns.Add(amountColumn);

            decimal price = Convert.ToInt32(this.priceBox.Text);
            foreach (DataRow row in this.dataSet2.Tables[0].Rows) {
                String number = (String) row["number"];
                DateTime enterTime = (DateTime) row["enterTime"];
                var outTime = getOutTime(enterTime, number);
                //DateTime outTime = (DateTime) row["outTime"];
                int hours = 0;

                if (!DBNull.Value.Equals(outTime)) {
                //if (!DBNull.Value.Equals(row["outTime"])) {
                    //var outTime = (DateTime)row["outTime"];
                    if (outTime < this.datePicker.Value) {
                        row.Delete();
                    } else {
                        row["OutTime"] = outTime;
                        TimeSpan timeSpan = outTime.Subtract(enterTime);
                        hours = (int) round(timeSpan).TotalHours;
                        row["Hours"] = hours;

                        if (hours > hoursLimit) {
                            row["Amount"] = (hours - hoursLimit) * price;
                        }
                    }
                }

                this.progressBar1.PerformStep();
            }

            this.dataSet2.Tables[0].AcceptChanges();

            dataGridView1.DataSource = this.dataSet2.Tables[0];
            dataGridView1.Columns[0].HeaderText = "Фамилия";
            dataGridView1.Columns[1].HeaderText = "Имя";
            dataGridView1.Columns[2].HeaderText = "Номер";
            dataGridView1.Columns[3].HeaderText = "Время заезда";
            dataGridView1.Columns[4].HeaderText = "Время Выезда";
            dataGridView1.Columns[5].HeaderText = "Стоянка, ч";
            dataGridView1.Columns[6].HeaderText = "Стоимость";
        }

        private void getEnterTimes()
        {
            if (isProd) {
                prodGetEnterTime();
            } else {
                devGetEnterTime();
            }
        }

        private dynamic getOutTime(DateTime enterTime, string number)
        {
            if (isProd) {
                return prodGetOutTime(enterTime, number);
            } else {
                return devGetOutTime(enterTime, number);
            }
        }

        private void prodGetEnterTime()
        {

            string onlyEnterTimequeryString = @"SELECT secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number, Data AS EnterTime
		        FROM ReportMessageMain
		        LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = ReportMessageMain.ID
		        LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = ReportMessageMain.ID
		        LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = ReportMessageMain.ID
		        WHERE Type = 201
                    AND Data < DATEADD (day, 1, @startDate)
		            AND Data > DATEADD (week, -1, @startDate)
                ORDER BY Data";

            string queryString = @"SELECT *
                FROM (
	                SELECT secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number, Data AS enterTime,
		                CASE WHEN (SELECT TOP 1 comeOut.TYpe FROM ReportMessageMain AS comeOut
			                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
			                WHERE comeOut.Type IN (201, 203)
				                AND comeOut.Data > comeIn.Data
				                AND numberComeOut.Arguments = numberComeIn.Arguments
				                AND comeOut.Data > DATEADD (week, -1, @startDate)
                            ) = 203
		                THEN 
			                (SELECT TOP 1 comeOut.Data from ReportMessageMain AS comeOut
			                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
			                WHERE comeOut.Type IN (201, 203)
				                AND comeOut.Data > comeIn.Data
				                AND numberComeOut.Arguments = numberComeIn.Arguments
				                AND comeOut.Data > DATEADD (week, -1, @startDate))
			            ELSE NULL END AS outTime
	                FROM ReportMessageMain AS comeIn
	                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = comeIn.ID
	                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = comeIn.ID
	                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = comeIn.ID
	                WHERE Type = 201
		                AND Data < DATEADD (day, 1, @startDate)
		                AND Data > DATEADD (week, -1, @startDate)) AS src1
                WHERE outTime >= @startDate OR OutTime IS NULL
                ORDER BY enterTime";

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Parking20ConnectionString)) {

                SqlCommand command = new SqlCommand(onlyEnterTimequeryString, connection);

                command.Parameters.Add("@startDate", SqlDbType.Date).Value = this.datePicker.Value;

                try {
                    connection.Open();
                }
                catch (Exception err) {
                    Debug.WriteLine(err.Message);
                }

                SqlDataReader reader = command.ExecuteReader(); 
                
                DataTable dataTable = new DataTable();

                this.dataSet2.Tables.Clear();
                this.dataSet2.Tables.Add(dataTable);
                dataTable.Load(reader);

                reader.Close();
                connection.Close();
            }
        }

        private void devGetEnterTime()
        {
            string onlyEnterTimequeryString = @"SELECT secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number, Data AS EnterTime
		        FROM ReportMessageMain
		        LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = ReportMessageMain.ID
		        LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = ReportMessageMain.ID
		        LEFT JOIN (SELECT ID, ReportMessageArguments.Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = ReportMessageMain.ID
		        WHERE Type = 201
                    AND Data < DATEADD (day, 1, ?)
		            AND Data > DATEADD (week, -1, ?)
                    --AND Data BETWEEN ? AND ?
                    --AND Data BETWEEN @startDate AND @endDate
                ORDER BY Data";

            string queryString = @"SELECT *
                FROM (
	                SELECT secondName.Arguments AS secondName, firstName.Arguments AS firstName, numberComeIn.Arguments AS number, Data AS enterTime,
		                CASE WHEN (SELECT TOP 1 comeOut.TYpe FROM ReportMessageMain AS comeOut
			                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
			                WHERE comeOut.Type IN (201, 203)
				                AND comeOut.Data > comeIn.Data
				                AND numberComeOut.Arguments = numberComeIn.Arguments
				                AND comeOut.Data > DATEADD (week, -1, ?)) = 203
		                THEN 
			                (SELECT TOP 1 comeOut.Data from ReportMessageMain AS comeOut
			                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = comeOut.ID
			                WHERE comeOut.Type IN (201, 203)
				                AND comeOut.Data > comeIn.Data
				                AND numberComeOut.Arguments = numberComeIn.Arguments
				                AND comeOut.Data > DATEADD (week, -1, ?))
			            ELSE NULL END AS outTime
	                FROM ReportMessageMain AS comeIn
	                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 10) AS secondName ON secondName.ID = comeIn.ID
	                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 16) AS firstName ON firstName.ID = comeIn.ID
	                LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeIn ON numberComeIn.ID = comeIn.ID
	                WHERE Type = 201
		                AND Data < DATEADD (day, 1, ?)
		                AND Data > DATEADD (week, -1, ?)) AS src1
                WHERE outTime >= ? OR OutTime IS NULL
                ORDER BY enterTime";

            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.ConnectionString1)) {

                OleDbCommand command = new OleDbCommand(onlyEnterTimequeryString, connection);

                command.Parameters.Add("startDate", OleDbType.DBDate).Value = this.datePicker.Value;
                command.Parameters.Add("startDate", OleDbType.DBDate).Value = this.datePicker.Value;
                //command.Parameters.Add("startDate", OleDbType.DBDate).Value = this.datePicker.Value;
                //command.Parameters.Add("startDate", OleDbType.DBDate).Value = this.datePicker.Value;
                //command.Parameters.Add("startDate", OleDbType.DBDate).Value = this.datePicker.Value;
                //command.Parameters.Add("endDate", OleDbType.DBDate).Value = this.datePicker.Value.AddDays(1);

                try {
                    connection.Open();
                }
                catch (Exception err) {
                    Debug.WriteLine(err.Message);
                }

                OleDbDataReader reader = command.ExecuteReader(); 

                DataTable dataTable = new DataTable();

                this.dataSet2.Tables.Clear();
                this.dataSet2.Tables.Add(dataTable);
                dataTable.Load(reader);

                reader.Close();
                connection.Close();
            }
        }

        private DateTime getLastReportDate()
        {
            DateTime lastReportDate = new DateTime(0);
            string queryString = "select top 1 EnterTime from Report order by EnterTime desc";

            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.ConnectionString1)) {

                OleDbCommand command = new OleDbCommand(queryString, connection);

                try {
                    connection.Open();
                    lastReportDate = (DateTime)command.ExecuteScalar();
                }
                catch (Exception err) {
                    Debug.WriteLine(err.Message);
                }
            }

            return lastReportDate;
        }

        private DataTable devGetEnterTimeAlter(DateTime lastReportDate)
        {
            DataTable dataTable = new DataTable();
            string queryString = @"select Data, Car.ID from ReportMessageMain
                join (select ID, Arguments from ReportMessageArguments where Keys = 12 and Arguments is not null and Arguments != '') as number on number.ID = ReportMessageMain.ID
                join Car on Car.Number = number.Arguments
                where type = 201 and Data > ?
                order by ReportMessageMain.Data";

            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.ConnectionString1)) {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.Add("@lastReportDate", SqlDbType.DateTime).Value = lastReportDate;

                try {
                    connection.Open();
                }
                catch (Exception err) {
                    Debug.WriteLine(err.Message);
                }

                OleDbDataReader reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            return dataTable;
        }

        private void devUpdateReportTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows) {
                //DateTime outTime = devGetOutTimeAlter((DateTime)row["Data"], (int)row["ID"]);
                row["OutTime"] = devGetOutTimeAlter((DateTime)row["Data"], (int)row["ID"]);
                //devAddReportItem((int) row["ID"], (DateTime) row["Data"], outTime);
            }
            dataTable.AcceptChanges();
        }

        private void devAddReportItem(int visitorId, DateTime enterTime, DateTime outTime)
        {
            string queryString = "insert into Report values (?, ?, ?)";

            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.ConnectionString1)) {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                //command.Parameters.Add("@visitorId", SqlDbType.Int).Value = visitorId;
                //command.Parameters.Add("@enterTime", SqlDbType.DateTime).Value = enterTime;
                //command.Parameters.Add("@outTime", SqlDbType.DateTime).Value = outTime;

                command.Parameters.AddWithValue("@visitorId", visitorId);
                command.Parameters.AddWithValue("@enterTime", enterTime);
                command.Parameters.AddWithValue("@outTime", outTime);

                try {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception err) {
                    Debug.WriteLine(err.Message);
                }
            }
        }

        private dynamic prodGetOutTime(DateTime enterTime, string number)
        {
            DateTime outTime;
            DateTime enterTimeInput = enterTime;

            string queryString = @"SELECT TOP 1 Data
                FROM ReportMessageMain
			    LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = ReportMessageMain.ID
			    WHERE Type = 203
				    AND Data BETWEEN @startDate AND @endDate
				    AND numberComeOut.Arguments = @number
                ORDER BY Data";

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Parking20ConnectionString)) {

                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = enterTimeInput;
                command.Parameters.Add("@endDate", SqlDbType.Date).Value = enterTime.AddDays(1);
                command.Parameters.Add("@number", SqlDbType.VarChar).Value = number;

                try {
                    connection.Open();
                }
                catch (Exception err) {
                    Debug.WriteLine(err.Message);
                }

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read()) {
                    outTime = (DateTime)reader.GetValue(0);
                    reader.Close();
                    connection.Close();

                    return outTime;
                } else {
                    reader.Close();
                    connection.Close();

                    return DBNull.Value;
                }
            }
        }

        private DateTime devGetOutTime(DateTime enterTime, string number)
        {
            DateTime outTime = new DateTime(0);
            DateTime enterTimeInput = enterTime;
            string queryString = @"SELECT TOP 1 Data
                FROM ReportMessageMain
			    LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = ReportMessageMain.ID
			    WHERE Type = 203
				    AND Data > ?
				    AND numberComeOut.Arguments = ?
                ORDER BY Data";

            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.ConnectionString1)) {

                OleDbCommand command = new OleDbCommand(queryString, connection);

                command.Parameters.Add("startDate", OleDbType.DBTimeStamp).Value = enterTimeInput;
                command.Parameters.Add("number", OleDbType.VarChar).Value = number;

                try {
                    connection.Open();
                    outTime = (DateTime)command.ExecuteScalar();
                }
                catch (Exception err) {
                    Debug.WriteLine(err.Message);
                }

                connection.Close();
            }

            return outTime;
        }

        private DateTime devGetOutTimeAlter(DateTime enterTime, int carId)
        {
            DateTime outTime = new DateTime(0);
            DateTime enterTimeInput = enterTime;
            string queryString = @"SELECT TOP 1 Data
                FROM ReportMessageMain
			    LEFT JOIN (SELECT ID, Arguments FROM ReportMessageArguments WHERE Keys = 12) AS numberComeOut ON numberComeOut.ID = ReportMessageMain.ID
                LEFT JOIN Car ON Car.Number = numberComeOut.Arguments
			    WHERE Type = 203
				    AND Data > ?
				    AND Car.ID = ?
                ORDER BY Data";

            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.ConnectionString1)) {

                OleDbCommand command = new OleDbCommand(queryString, connection);

                command.Parameters.Add("startDate", OleDbType.DBDate).Value = enterTimeInput;
                command.Parameters.Add("carId", OleDbType.Integer).Value = carId;

                try {
                    connection.Open();
                    outTime = (DateTime) command.ExecuteScalar();
                }
                catch (Exception err) {
                    Debug.WriteLine(err.Message);
                }
            }

            return outTime;
        }


        public static TimeSpan round(TimeSpan input)
        {
            if (input < TimeSpan.Zero) {
                return -round(-input);
            }
            double hours = (double)input.TotalHours;
            if (input.Minutes > 0) {
                hours++;
            }
            return TimeSpan.FromHours(hours);
        }

        private void applyPrice_Click(object sender, EventArgs e)
        {
            if (this.dataSet2.Tables[0].Rows.Count > 0) {
                int hoursLimit = Convert.ToInt32(this.hoursBox.Text);
                decimal price = Convert.ToInt32(this.priceBox.Text);
                foreach (DataRow row in this.dataSet2.Tables[0].Rows) {
                    int hours = (row["Hours"] != DBNull.Value) ? Convert.ToInt32(row["Hours"]) : 0;
                    if (hours > hoursLimit) {
                        row["Amount"] = (hours - hoursLimit) * price;
                    }
                }
                dataGridView1.DataSource = this.dataSet2.Tables[0];
            }
        }
    }
}
