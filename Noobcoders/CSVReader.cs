using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Noobcoders
{
    internal class CSVReader
    {
        public static int readCSVandSaveToDataBase(string fileCSVPath) 
        {       
            int csvBufferSize = 5000;
            int bufferCounter = 0;
            
            DataTable csvData = new DataTable();

            string[] fields = null;
            try 
            {

                using (var reader = new StreamReader(fileCSVPath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {

                    bool firstRecord = true;
                    csv.Read();
                    csv.ReadHeader();
                    Post.namesOfFields = csv.HeaderRecord.ToList();
                    Post.FieldsCount = Post.namesOfFields.Count;
                    var recordTypes = new List<Type>();
                    while (csv.Read())
                    {
                       Post nextPost = new Post();
                        for (int i=0; i<Post.FieldsCount; i++)
                        {
                            var field = csv.GetField(i);
                            nextPost.Fields.Add(field);
                            if (firstRecord) recordTypes.Add(field.GetType());   // сделать распознавание типов полей таблицы
                        }
                        if (firstRecord) Post.typesOfFields = recordTypes;
                        firstRecord = false;    
                    }                   
                }                
            }
            catch { return 2; }

            // всё огонь
            return 0;
        }

        /*static private bool insertCurrentBunchOfRecs(DataTable csvData)
        {
            try
            {
                using (Form1.dBaseConnection)
                {
                    dBaseConnection.Open();
                    // вся соль вот в этом классе - SqlBulkCopy - он делает всю магию
                    using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                    {
                        // если таблица в какой-то схеме, то указать это
                        s.DestinationTableName = "ИМЯСХЕМЫ.ИМЯТАБЛИЦЫ";
                        foreach (var column in csvData.Columns)
                        {
                            s.ColumnMappings.Add(column.ToString(), column.ToString());
                        }
                        s.WriteToServer(csvData);
                    }
                }
            }
            catch { return false; }

            return true;
        }*/



    }
}
