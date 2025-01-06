using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class DataDisplay : Form
    {
        public string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";
        public DataDisplay()
        {
            InitializeComponent();
            textBox1.TextChanged += TextBox1_TextChanged;
        }

        public void DataDisplay_Load()
        {
            var dataTable = new DataTable();
            string query = "SELECT * FROM Recipes";

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
            RecipeDataGridView1.DataSource = dataTable;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            string filterText = textBox1.Text.ToLower();
            var dataTable = new DataTable();
            string query = "SELECT * FROM Recipes WHERE " +
                "LOWER(InputPattern) LIKE @FilterText OR " +
                "LOWER(A) LIKE @FilterText OR " +
                "LOWER(B) LIKE @FilterText OR " +
                "LOWER(C) LIKE @FilterText OR " +
                "LOWER(D) LIKE @FilterText OR " +
                "LOWER(E) LIKE @FilterText OR " +
                "LOWER(F) LIKE @FilterText OR " +
                "LOWER(G) LIKE @FilterText OR " +
                "LOWER(H) LIKE @FilterText OR " +
                "LOWER(I) LIKE @FilterText OR " +
                "LOWER(OutputItem) LIKE @FilterText OR " +
                "LOWER(ItemName) LIKE @FilterText OR " +
                "LOWER(ItemId) LIKE @FilterText OR " +
                "LOWER(ItemMeta) LIKE @FilterText OR " +
                "LOWER(DisplayName) LIKE @FilterText OR " +
                "LOWER(OreDict) LIKE @FilterText OR " +
                "LOWER(Quests) LIKE @FilterText OR " +
                "LOWER(TaskUID) LIKE @FilterText";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FilterText", "%" + filterText + "%");

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                connection.Close();
            }
            RecipeDataGridView1.DataSource = dataTable;
        }
    }
}
