using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class DataDisplay : Form
    {
        MainForm form;
        ConfigManager configManager;
        public List<ProjectProperties> projectProperties;
        public List<Potions> potionProperties;

        public DataDisplay(MainForm form, List<ProjectProperties> projectProperties, List<Potions> potionProperties)
        {
            configManager = ConfigManager.Instance;
            this.form = form;
            this.potionProperties = potionProperties;
            this.projectProperties = projectProperties;
            InitializeComponent();
            textBox1.TextChanged += TextBox1_TextChanged;
        }

        public void DataDisplay_Load()
        {
            LoadTable("Recipes", RecipeDataGridView1);
            LoadTable("Potions", PotionDataGridView1);
        }


        private void LoadTable(string tableName, DataGridView gridView)
        {            
            DataTable dataTable = null;

            if (tableName == "Recipes")
            {
                dataTable = CreateDataTable(tableName, projectProperties, null);
            }
            else if (tableName == "Potions")
            {
                dataTable = CreateDataTable(tableName, null, potionProperties);
            }

            if (dataTable != null)
            {
                gridView.DataSource = dataTable;
            }
        }

        private void FilterTable(string tableName, string filterText, DataGridView dataGridView)
        {
            DataTable dataTable = null;

            if (tableName == "Recipes")
            {
                dataTable = CreateDataTable(tableName, projectProperties, null, filterText);
            }
            else if (tableName == "Potions")
            {
                dataTable = CreateDataTable(tableName, null, potionProperties, filterText);
            }

            if (dataTable != null)
            {
                dataGridView.DataSource = dataTable;
            }
        }

        private DataTable CreateDataTable(string tableName, List<ProjectProperties> projectProperties, List<Potions> potionProperties, string filterText = null)
        {
            DataTable dataTable = new DataTable();

            if (tableName == "Recipes" && projectProperties != null && projectProperties.Count > 0)
            {
                dataTable.Columns.Add("RecipeID");
                dataTable.Columns.Add("InputPattern");
                dataTable.Columns.Add("A");
                dataTable.Columns.Add("B");
                dataTable.Columns.Add("C");
                dataTable.Columns.Add("D");
                dataTable.Columns.Add("E");
                dataTable.Columns.Add("F");
                dataTable.Columns.Add("G");
                dataTable.Columns.Add("H");
                dataTable.Columns.Add("I");
                dataTable.Columns.Add("OutputItem");
                dataTable.Columns.Add("DisplayName");
                dataTable.Columns.Add("OreDict");
                dataTable.Columns.Add("Quests");
                dataTable.Columns.Add("TaskUID");

                foreach (var recipe in projectProperties)
                {
                    if (recipe?.Ingredients == null || recipe.Ingredients.All(i => i == "N/A" || i == null))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(filterText) ||
                        recipe.DisplayName.ToLower().Contains(filterText) ||
                        recipe.OutputItem.ToLower().Contains(filterText))
                    {
                        var row = dataTable.NewRow();
                        row["RecipeID"] = recipe.RecipeID;
                        row["InputPattern"] = recipe.InputPattern;

                        var ingredientMap = SplitIngredients(recipe.Ingredients);
                        row["A"] = ingredientMap["A"];
                        row["B"] = ingredientMap["B"];
                        row["C"] = ingredientMap["C"];
                        row["D"] = ingredientMap["D"];
                        row["E"] = ingredientMap["E"];
                        row["F"] = ingredientMap["F"];
                        row["G"] = ingredientMap["G"];
                        row["H"] = ingredientMap["H"];
                        row["I"] = ingredientMap["I"];

                        row["OutputItem"] = recipe.OutputItem;
                        row["DisplayName"] = recipe.DisplayName;
                        row["OreDict"] = recipe.OreDict;
                        row["Quests"] = recipe.Quests;
                        row["TaskUID"] = recipe.TaskUID;
                        dataTable.Rows.Add(row);
                    }
                }
            }
            else if (tableName == "Potions" && potionProperties != null && potionProperties.Count > 0)
            {
                dataTable.Columns.Add("RecipeID");
                dataTable.Columns.Add("InputPattern");
                dataTable.Columns.Add("PotionName");
                dataTable.Columns.Add("PotionDisplayName");
                dataTable.Columns.Add("OreDict");
                dataTable.Columns.Add("Quests");
                dataTable.Columns.Add("TaskUID");

                foreach (var potion in potionProperties)
                {
                    if (string.IsNullOrEmpty(filterText) ||
                        potion.PotionDisplayName.ToLower().Contains(filterText) ||
                        potion.PotionName.ToLower().Contains(filterText))
                    {
                        var row = dataTable.NewRow();
                        row["RecipeID"] = potion.RecipeID;
                        row["InputPattern"] = potion.InputPattern;
                        row["PotionName"] = potion.PotionName;
                        row["PotionDisplayName"] = potion.PotionDisplayName;
                        row["OreDict"] = potion.OreDict;
                        row["Quests"] = potion.Quests;
                        row["TaskUID"] = potion.TaskUID;
                        dataTable.Rows.Add(row);
                    }
                }
            }

            return dataTable;
        }




        private Dictionary<string, string> SplitIngredients(string[] ingredients)
        {
            var ingredientMap = new Dictionary<string, string>
    {
        { "A", "N/A" },
        { "B", "N/A" },
        { "C", "N/A" },
        { "D", "N/A" },
        { "E", "N/A" },
        { "F", "N/A" },
        { "G", "N/A" },
        { "H", "N/A" },
        { "I", "N/A" }
    };

            if (ingredients == null || ingredients.All(i => i == "N/A" || i == null))
            {
                return ingredientMap;
            }

            for (int i = 0; i < ingredients.Length; i++)
            {
                if (i < ingredientMap.Count && !string.IsNullOrEmpty(ingredients[i]) && ingredients[i] != "N/A")
                {
                    ingredientMap[ingredientMap.ElementAt(i).Key] = ingredients[i];
                }
            }

            return ingredientMap;
        }






        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            string filterText = textBox1.Text.ToLower();
            FilterTable("Recipes", filterText, RecipeDataGridView1);
            FilterTable("Potions", filterText, PotionDataGridView1);
        }
    }

}
