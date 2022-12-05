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
            else
            {
                DialogResult dialogResult = MessageBox.Show("ОК - добавить записи в существующую базу\n Отмена - создать базу заново", "База уже существует", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel)
                {
                    Form1.dBaseConnection.Close();
                    File.Delete(dBaseName);
                    SQLiteConnection.CreateFile(dBaseName);
                    CreateNewDBASE(dBaseName);
                }
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
                commandLine.Append("id INTEGER PRIMARY KEY AUTOINCREMENT, ");
                for (int i = 0; i < Post.FieldsCount; i++)
                {
                    commandLine.Append(Post.namesOfFields[i]);
                    /*switch (Post.typesOfFields[i])                                      // TO DO: создание полей в соответствии с опозныннми при парсинге типами
                    {
                        case GetType(String): commandLine.Append(" TEXT"); break;
                        case int: commandLine.Append(" INTEGER"); break;
                        case DateTime: commandLine.Append(" DATETIME"); break;
                        case int: commandLine.Append(" INTEGER"); break;
                        case double: commandLine.Append(" float"); break;
                        case decimal: commandLine.Append(" money"); break;
                        default: commandLine.Append(" TEXT"); break;
                    }*/
                    commandLine.Append(" TEXT");
                    if (i < Post.FieldsCount - 1) commandLine.Append(", ");
                }
                commandLine.Append(")");

                Form1.sQLCommand.CommandText = commandLine.ToString();
                Form1.sQLCommand.ExecuteNonQuery();
                commandLine.Clear();
                commandLine.Append($"CREATE UNIQUE INDEX record ON {Path.GetFileNameWithoutExtension(dBaseName)}(");
                for (int i = 0; i < Post.FieldsCount; i++)
                {
                    commandLine.Append(Post.namesOfFields[i]);
                    if (i < Post.FieldsCount - 1) commandLine.Append(", ");
                }
                commandLine.Append(")");
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

        public static void AddDataToBase(string dBaseName, Post record)
        {
            if (Form1.dBaseConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Open connection with database");
                return;
            }
                                      var commandLine = new StringBuilder();
            try
            {
                commandLine.Append($"INSERT OR IGNORE INTO {Path.GetFileNameWithoutExtension(dBaseName)} (");
                for (int i = 0; i < Post.FieldsCount; i++)
                {
                    commandLine.Append($"'{Post.namesOfFields[i]}'");
                    if (i < Post.FieldsCount - 1) commandLine.Append(", ");
                }


                commandLine.Append(") Values (");
                for (int i = 0; i < Post.FieldsCount; i++)
                {
                    commandLine.Append($"@{Post.namesOfFields[i]}");
                    if (i < Post.FieldsCount - 1) commandLine.Append(", ");
                }
                commandLine.Append(")");
                for (int i = 0; i < Post.FieldsCount; i++)
                {
                    SQLiteParameter sqlParameter = new SQLiteParameter();
                    sqlParameter.ParameterName = $"@{Post.namesOfFields[i]}";
                    sqlParameter.Value = record.Fields[i];
                    sqlParameter.DbType = DbType.String;    //  TO DO: назначать в зависимости от типа данных поля
                    Form1.sQLCommand.Parameters.Add(sqlParameter);
                }

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
