using CsvHelper;
using Nest;
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

        public static ElasticClient elasticSearchClient;
        /*public static bool readCSVHeader(string fileCSVPath, string fileDBasePath)
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
        }*/



        public static bool readCSVandSaveToDataBase(string fileCSVPath, string fileDBasePath)
        {
            try
            {
                using (var reader = new StreamReader(fileCSVPath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {

                    var postsTable = new List<Posts>();
                    bool firstRecord = true;
                    csv.Read();
                    while (csv.Read())
                    {
                        postsTable.Add(new Posts());
                        postsTable[postsTable.Count - 1].Text = csv.GetField<string>(0);
                        postsTable[postsTable.Count - 1].CreatedDate = csv.GetField<DateTime>(1);
                        postsTable[postsTable.Count - 1].Rubrics = csv.GetField<string>(2);
                        DBase.AddDataToBase(fileDBasePath, postsTable[postsTable.Count - 1]);
                        
                    }
                    ElasticsearchHelper.CreateDocument(Form1.elasticSearchClient, "posts", postsTable);
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
