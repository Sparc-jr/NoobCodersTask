using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Noobcoders
{
    internal class DBase
    {
        public static bool CreateDBASE(string dBaseName)
        {
            if (!File.Exists(dBaseName))
                SQLiteConnection.CreateFile(dBaseName);
            ConnectDBASE(dBaseName);
            return true;
        }
        public static bool ConnectDBASE(string dBaseName)
        {
            try
            {
                Form1.dBaseConnection = new SQLiteConnection("Data Source=" + dBaseName + ";Version=3;");
                Form1.dBaseConnection.Open();
                Form1.sQLCommand.Connection = Form1.dBaseConnection;
                var commandLine = new StringBuilder();
                commandLine.Append("CREATE TABLE IF NOT EXISTS ");
                commandLine.Append(Path.GetFileNameWithoutExtension(dBaseName));
                commandLine.Append(" ");
                commandLine.Append("id INTEGER PRIMARY KEY AUTOINCREMENT, ");
                for(int i = 0; i < Post.FieldsCount; i++)
                {
                    commandLine.Append(Post.namesOfFields[i]);
                    switch(Post.typesOfFields[i])
                    {
                        case string: commandLine.Append(" TEXT");break;
                        case int: commandLine.Append(" INTEGER"); break;
                        case DateTime: commandLine.Append(" DATETIME"); break;
                        case int: commandLine.Append(" INTEGER"); break;
                        case double: commandLine.Append(" float"); break;
                        case decimal: commandLine.Append(" money"); break;
                        default: commandLine.Append(" TEXT"); break;
                    }
                    if(i<Post.FieldsCount-1) commandLine.Append(", ")
                }
                Form1.sQLCommand.CommandText = commandLine.ToString();
                Form1.sQLCommand.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

    }
}
