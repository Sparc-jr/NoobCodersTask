using Nest;
using System.Data;
using System.Data.SQLite;

namespace CSVtoElastic
{
    public partial class Form1 : Form
    {
        internal String CSVFileName;
        private String dBaseFileName;
        public static SQLiteConnection dBaseConnection;
        public static SQLiteCommand sQLCommand;
        public static ElasticClient elasticSearchClient = ElasticsearchHelper.GetESClient();
        List<CheckBox> checkBoxColumnToIndex = new List<CheckBox>();


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
            RefreshDataGridView();
            MessageBox.Show("Файл открыт");
        }

        private void RefreshDataGridView()
        {
            SQLiteDataAdapter sQLiteDataAdapter = new SQLiteDataAdapter($"SELECT * FROM {Path.GetFileNameWithoutExtension(dBaseFileName)}", dBaseConnection);
            DataSet dataSet = new DataSet();
            sQLiteDataAdapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
            DrawCheckBoxesInColumns();
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
            RefreshDataGridView();
            MessageBox.Show("Файл открыт");

            //ElasticsearchHelper.CreateDocument(Form1.elasticSearchClient, "posts", postsTable);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            dBaseConnection.Close();
        }

        private void DrawCheckBoxesInColumns()       // добавляем чекбоксы в заголовки столбцов для выбора что индексировать
        {                                            // на данный момент индексация осуществляется по умолчанию по первому столбцу           
            foreach (CheckBox chkBox in checkBoxColumnToIndex)
            {
                chkBox.Dispose();
            }
            checkBoxColumnToIndex.Clear();
            for (int i = 0; i < Post.FieldsCount; i++)
            {
                var ckBox = new CheckBox();
                checkBoxColumnToIndex.Add(ckBox);
                Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(i+1, -1, true);
                checkBoxColumnToIndex[i].Size = new Size(18, 18);
                checkBoxColumnToIndex[i].Top = rect.Top+1;
                checkBoxColumnToIndex[i].Left = rect.Left + rect.Width - checkBoxColumnToIndex[i].Width-1;
                checkBoxColumnToIndex[i].CheckedChanged += (sender, eventArgs) => {
                    CheckBox senderCheckbox = (CheckBox)sender; 
                    ckBox_CheckedChanged(i);
                };
                  this.dataGridView1.Controls.Add(checkBoxColumnToIndex[i]);
            }
        }
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            for (int i = 0; i < Post.FieldsCount; i++)
            {
                Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(i + 1, -1, true);
                checkBoxColumnToIndex[i].Size = new Size(18, 18);
                checkBoxColumnToIndex[i].Top = rect.Top + 1;
                checkBoxColumnToIndex[i].Left = rect.Left + rect.Width - checkBoxColumnToIndex[i].Width - 1;
            };
        }
        private void ckBox_CheckedChanged(int checkBoxIndex)
        {
            Post.FieldsToIndex[checkBoxIndex] = !Post.FieldsToIndex[checkBoxIndex];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var searchResult = ElasticsearchHelper.SearchDocument(elasticSearchClient, "posts", textBox1.Text);
            dataGridView2.DataSource = searchResult;
            dataGridView2.MultiSelect = true;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void dataGridView2_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            int rowIndex = dgv.HitTest(e.X, e.Y).RowIndex;
            if (rowIndex == -1) { return; }
            dgv.Rows[rowIndex].Selected = !dgv.Rows[rowIndex].Selected;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            dataGridView1.Width = Form1.ActiveForm.Width - 48;
            dataGridView2.Width = Form1.ActiveForm.Width - 48;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dBaseConnection.Close();
            Form1.dBaseConnection.Open();
            Form1.sQLCommand.Connection = Form1.dBaseConnection;
            var selectedRows = dataGridView2.SelectedRows
                .OfType<DataGridViewRow>()
                .ToArray();

            foreach (var row in selectedRows)
            {
                ElasticsearchHelper.DeleteDocument(elasticSearchClient, "posts", new Record(int.Parse(row.Cells[0].Value.ToString()), row.Cells[1].Value.ToString()));
                sQLCommand.CommandText = $"DELETE FROM {Path.GetFileNameWithoutExtension(dBaseFileName)} WHERE id like {int.Parse(row.Cells[0].Value.ToString())}";
                Form1.sQLCommand.ExecuteNonQuery();
            }
            RefreshDataGridView();
            dataGridView2.ClearSelection();
            button3_Click(sender, e);
            Form1.dBaseConnection.Close();
        }
    }
}