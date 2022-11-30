using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoElastic
{
    internal class CSVReader
    {

        public static bool readCSVHeader(string fileCSVPath, string fileDBasePath)
        {
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
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        
        
        
        public static bool readCSVandSaveToDataBase(string fileCSVPath, string fileDBasePath)
        {
            string[] fields = null;
            try
            {

                using (var reader = new StreamReader(fileCSVPath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {

                    bool firstRecord = true;
                    csv.Read();
                    var recordTypes = new List<Type>();
                    var fieldsToIndex = new List<bool>();
                    while (csv.Read())
                    {
                        Post nextPost = new Post();
                        for (int i = 0; i < Post.FieldsCount; i++)
                        {
                            var field = csv.GetField(i);
                            nextPost.Fields.Add(field);
                            if (firstRecord) recordTypes.Add(field.GetType());   // сделать распознавание типов полей таблицы
                            if (i<=0) fieldsToIndex.Add(true);
                            else fieldsToIndex.Add(false);
                        }
                        if (firstRecord)
                        {
                            Post.typesOfFields = recordTypes;
                            Post.FieldsToIndex = fieldsToIndex;
                        }
                        firstRecord = false;
                        DBase.AddDataToBase(fileDBasePath, nextPost);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
            Form1.dBaseConnection.Close();
            return true;
        }

    }
}
