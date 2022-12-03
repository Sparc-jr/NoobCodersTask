using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;

namespace CSVtoElastic
{
    internal class DBase
    {
        public static bool CreateDBASE(string dBaseName)
        {
            if (!File.Exists(dBaseName))
            {
                SQLiteConnection.CreateFile(dBaseName);
                CreateNewDBASE(dBaseName);
            }
            return ConnectDBASE(dBaseName);
        }
        public static bool CreateNewDBASE(string dBaseName)
        {
            try
            {
                Form1.dBaseConnection = new SQLiteConnection("Data Source=" + dBaseName + ";Version=3;");
                Form1.dBaseConnection.Open();
                Form1.sQLCommand.Connection = Form1.dBaseConnection;
                var commandLine = new StringBuilder();
                commandLine.Append("CREATE TABLE IF NOT EXISTS ");
                commandLine.Append(Path.GetFileNameWithoutExtension(dBaseName));
                commandLine.Append(" (");
                commandLine.Append("id INTEGER PRIMARY KEY AUTOINCREMENT, text TEXT, created_date DATETIME, rubrics TEXT)");

                Form1.sQLCommand.CommandText = commandLine.ToString();
                Form1.sQLCommand.ExecuteNonQuery();
                commandLine.Clear();
                commandLine.Append($"CREATE UNIQUE INDEX record ON {Path.GetFileNameWithoutExtension(dBaseName)}(text, created_date, rubrics)");
                Form1.sQLCommand.CommandText = commandLine.ToString();
                Form1.sQLCommand.ExecuteNonQuery();
                Form1.dBaseConnection.Close();
                return true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static bool ConnectDBASE(string dBaseName)
        {
            if (!File.Exists(dBaseName))
                MessageBox.Show("Please, create DB and blank table (Push \"Create\" button)");

            try
            {
                Form1.dBaseConnection = new SQLiteConnection("Data Source=" + dBaseName + ";Version=3;New=False;Compress=True;");
                Form1.dBaseConnection.Open();
                Form1.sQLCommand.Connection = Form1.dBaseConnection;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
           return true;
        }

        public static void AddDataToBase(string dBaseName, Posts record)
        {
            var namesOfFields = new { "text", "created_date", "rubrics" };
            if (Form1.dBaseConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Open connection with database");
                return;
            }
            var commandLine = new StringBuilder();
            try
            {
                commandLine.Append($"INSERT OR IGNORE INTO {Path.GetFileNameWithoutExtension(dBaseName)} ('text', 'created_date', 'rubrics'");
                commandLine.Append(") Values ('@text', '@created_date', '@rubrics')");

                for (int i = 0; i < 3; i++)
                {
                    SQLiteParameter sqlParameter = new SQLiteParameter();
                    sqlParameter.ParameterName = $"@{namesOfFields[i]}";
                    sqlParameter.Value = record.Fields[i];
                    sqlParameter.DbType = DbType.String;    // сделать автоподбираемым в зависимости от типа данных поля
                    Form1.sQLCommand.Parameters.Add(sqlParameter);
                }


                //Form1.sQLCommand.Parameters.Add(new SQLiteParameter("@text", record.Text));
                //Form1.sQLCommand.Parameters.Add(new SQLiteParameter("@created_date", record.CreatedDate));
                //Form1.sQLCommand.Parameters.Add(new SQLiteParameter("@rubrics", record.Rubrics));
                Form1.sQLCommand.CommandText = commandLine.ToString();
                Form1.sQLCommand.CommandType = CommandType.Text;
                Form1.sQLCommand.ExecuteNonQuery();
                
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
