using System;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class DataDisplay : Form
    {
        ConfigManager configManager;

        public DataDisplay()
        {
            configManager = ConfigManager.Instance;
            InitializeComponent();
            textBox1.TextChanged += TextBox1_TextChanged;
        }

        public void DataDisplay_Load()
        {
            string connectionString = $"Data Source={configManager.Config.DatabaseFile};Version=3;";
            var tableGridViewPairs = new (string TableName, DataGridView GridView)[]
            {
                ("Recipes", RecipeDataGridView1),
                ("Potions", PotionDataGridView1)
            };

            foreach (var (TableName, GridView) in tableGridViewPairs)
            {
                LoadTable(connectionString, TableName, GridView);
            }
        }

        private void LoadTable(string connectionString, string tableName, DataGridView gridView)
        {
            var dataTable = new DataTable();
            string query = $"SELECT * FROM {tableName}";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                connection.Close();
            }
            gridView.DataSource = dataTable;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            string connectionString = $"Data Source={configManager.Config.DatabaseFile};Version=3;";
            string filterText = textBox1.Text.ToLower();
            var tableGridViewPairs = new (string TableName, DataGridView GridView)[]
            {
                ("Recipes", RecipeDataGridView1),
                ("Potions", PotionDataGridView1)
                // Add more pairs as needed
            };

            foreach (var (TableName, GridView) in tableGridViewPairs)
            {
                FilterTable(connectionString, TableName, filterText, GridView);
            }
        }

        private void FilterTable(string connectionString, string tableName, string filterText, DataGridView dataGridView)
        {
            var dataTable = new DataTable();
            string columnsQuery = $"PRAGMA table_info({tableName});";  // SQLite specific; adjust for other DBMS

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(columnsQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        StringBuilder queryBuilder = new StringBuilder($"SELECT * FROM {tableName} WHERE ");
                        bool firstColumn = true;

                        while (reader.Read())
                        {
                            if (!firstColumn)
                            {
                                queryBuilder.Append(" OR ");
                            }
                            firstColumn = false;
                            string columnName = reader["name"].ToString();
                            queryBuilder.Append($"LOWER({columnName}) LIKE @FilterText");
                        }

                        string query = queryBuilder.ToString();
                        using (SQLiteCommand filterCommand = new SQLiteCommand(query, connection))
                        {
                            filterCommand.Parameters.AddWithValue("@FilterText", "%" + filterText + "%");

                            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(filterCommand))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                    }
                }
                connection.Close();
            }
            dataGridView.DataSource = dataTable;
        }
    }
}
