using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace Noobcoders
{
    public partial class Form1 : Form
    {
        private String CSVFileName;
        private String dBaseFileName;
        public static SQLiteConnection dBaseConnection;
        public static SQLiteCommand sQLCommand;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV files(*.csv)|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            CSVFileName = openFileDialog1.FileName;
            dBaseFileName = $"{Path.GetDirectoryName(CSVFileName)}\\{Path.GetFileNameWithoutExtension(CSVFileName)}.db";
            toolStripStatusLabel2.Text = DBase.CreateDBASE(dBaseFileName)? "Connected" : "Disconnected";
            CSVReader.readCSVandSaveToDataBase(CSVFileName);
            MessageBox.Show("Файл открыт");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Database files(*.db)|*.db";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            dBaseFileName = openFileDialog1.FileName;
            toolStripStatusLabel2.Text = DBase.ConnectDBASE(dBaseFileName) ? "Connected" : "Disconnected";
            MessageBox.Show("Файл открыт");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            dBaseConnection = new SQLiteConnection();
            sQLCommand = new SQLiteCommand();
            dBaseFileName = "sampleDB.db";
            toolStripStatusLabel2.Text = "Disconnected";
        }

    }
}
