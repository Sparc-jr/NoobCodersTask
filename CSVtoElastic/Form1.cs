using System.Formats.Asn1;
using System.IO;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace CSVtoElastic
{
    public partial class Form1 : Form
    {
        internal String CSVFileName;
        private String dBaseFileName;
        public static SQLiteConnection dBaseConnection;
        public static SQLiteCommand sQLCommand;
        public static ElasticClient elasticSearchClient = ElasticsearchHelper.GetESClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dBaseConnection.Close();
            openFileDialog1.Filter = "CSV files(*.csv)|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            CSVFileName = openFileDialog1.FileName;
            dBaseFileName = $"{Path.GetDirectoryName(CSVFileName)}\\{Path.GetFileNameWithoutExtension(CSVFileName)}.db";
            CSVReader.readCSVHeader(CSVFileName, dBaseFileName);
            toolStripStatusLabel2.Text = DBase.CreateDBASE(dBaseFileName) ? "Connected" : "Disconnected";
            CSVReader.readCSVandSaveToDataBase(CSVFileName, dBaseFileName);
            dataGridView1.DataSource = dBaseFileName;
            SQLiteDataAdapter sQLiteDataAdapter = new SQLiteDataAdapter($"SELECT * FROM {Path.GetFileNameWithoutExtension(dBaseFileName)}", dBaseConnection);
            DataSet dataSet = new DataSet();
            sQLiteDataAdapter.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
            DrawCheckBoxes();
            MessageBox.Show("Файл открыт");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dBaseConnection = new SQLiteConnection();
            sQLCommand = new SQLiteCommand();
            dBaseFileName = "sampleDB.db";
            toolStripStatusLabel2.Text = "Disconnected";
            ElasticsearchHelper.GetESClient();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Database files(*.db)|*.db";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            dBaseConnection.Close();
            dBaseFileName = openFileDialog1.FileName;
            toolStripStatusLabel2.Text = DBase.ConnectDBASE(dBaseFileName) ? "Connected" : "Disconnected";
            MessageBox.Show("Файл открыт");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            dBaseConnection.Close();
        }

        private void DrawCheckBoxes()
        {
            List<CheckBox> checkBoxColumnToIndex = new List<CheckBox>();
            for (int i = 0; i < Post.FieldsCount; i++)
            {
                var ckBox = new CheckBox();
                checkBoxColumnToIndex.Add(ckBox);
                //Get the column header cell bounds
                Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(i+1, -1, true);
                checkBoxColumnToIndex[i].Size = new Size(18, 18);
                //Change the location of the CheckBox to make it stay on the header
                checkBoxColumnToIndex[i].Top = rect.Top+1;
                checkBoxColumnToIndex[i].Left = rect.Left + rect.Width - checkBoxColumnToIndex[i].Width-1;
                checkBoxColumnToIndex[i].CheckedChanged += (sender, eventArgs) => {
                    CheckBox senderCheckbox = (CheckBox)sender; 
                    ckBox_CheckedChanged(i);
                };
                  this.dataGridView1.Controls.Add(checkBoxColumnToIndex[i]);
            }
        }
        private void ckBox_CheckedChanged(int checkBoxIndex)
        {
            Post.FieldsToIndex[checkBoxIndex] = !Post.FieldsToIndex[checkBoxIndex];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var searchResult = ElasticsearchHelper.SearchDocument(elasticSearchClient, "posts", textBox1.Text);
            dataGridView2.DataSource = searchResult;
        }
    }
}