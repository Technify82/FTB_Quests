using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FTB_Quests
{

    public class RecipeData
    {
        public string InputPattern { get; set; }
        public string[] Ingredients { get; set; }
        public string OutputItem { get; set; }
        public int Quantity { get; set; }
    }


    public class TempItems
    {
        public List<string> ItemNames { get; set; } = new List<string>();
        public List<string> DisplayNames { get; set; } = new List<string>();
        public string ItemMeta { get; set; }
        public string ItemId { get; set; }
    }

    public class NewParser
    {
        private readonly MainForm form;
        ConfigManager configManager;
        // readonly string itempanelfile = ConfigManager.Config.ItemPanelFile;
        // public string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";
        readonly DatabaseIO databaseIO;
        private readonly List<RecipeData> parsedRecipes = new List<RecipeData>();
        public NewParser(MainForm form)
        {
            this.form = form;
            configManager = ConfigManager.Instance;
            databaseIO = new DatabaseIO();
        }

        private string[] ParseIngredients(string inputIngredients)
        {
            var prefixes = new[] { "A:", "B:", "C:", "D:", "E:", "F:", "G:", "H:", "I:" };
            string suffix = ":*";

            var ingredientLines = inputIngredients.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(line =>
                                                   {
                                                       foreach (var prefix in prefixes)
                                                       {
                                                           if (line.StartsWith(prefix))
                                                           {
                                                               line = line.Substring(prefix.Length).Trim();
                                                           }
                                                       }
                                                       if (line.EndsWith(suffix))
                                                       {
                                                           line = line.Substring(0, line.Length - suffix.Length).Trim();
                                                       }
                                                       return line.Trim();
                                                   })
                                                   .ToArray();
            return new string[]
                {
        ingredientLines.ElementAtOrDefault(0) ?? "N/A",
        ingredientLines.ElementAtOrDefault(1) ?? "N/A",
        ingredientLines.ElementAtOrDefault(2) ?? "N/A",
        ingredientLines.ElementAtOrDefault(3) ?? "N/A",
        ingredientLines.ElementAtOrDefault(4) ?? "N/A",
        ingredientLines.ElementAtOrDefault(5) ?? "N/A",
        ingredientLines.ElementAtOrDefault(6) ?? "N/A",
        ingredientLines.ElementAtOrDefault(7) ?? "N/A",
        ingredientLines.ElementAtOrDefault(8) ?? "N/A"
                };
        }

        public void CheckDatabaseAndPopulateRecipeText()
        {
            string dbPath = configManager.Config.DatabaseFile.ToString();

            if (File.Exists(dbPath))
            {
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    PopulateRecipeText(connection);

                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Database file not found.");
            }
        }

        public void PopulateRecipeText(SQLiteConnection connection)
        {
            var query = "SELECT DisplayName FROM Recipes";

            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string recipeName = reader["DisplayName"].ToString();
                        if (recipeName != "")
                        {
                            form.RecipeText.Items.Add(recipeName);
                        }
                    }
                }
            }
        }

        private bool CheckDatabaseData(SQLiteConnection connection)
        {
            string checkDataQuery = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND (name='IngredientLookup' OR name='Recipes')";
            using (SQLiteCommand command = new SQLiteCommand(checkDataQuery, connection))
            {
                int tableCount = Convert.ToInt32(command.ExecuteScalar());
                return tableCount > 0;
            }
        }

        private bool PromptDatabasePurge()
        {
            DialogResult result = MessageBox.Show("The database is already populated. Would you like to purge it?", "Database Check", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private List<string> ParseRecipeGroups(string recipeFilePath)
        {
            var recipeGroups = new List<string>();

            if (string.IsNullOrWhiteSpace(recipeFilePath))
            {
                MessageBox.Show("The recipe file path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return recipeGroups; // Early return on invalid path
            }

            try
            {
                var lines = File.ReadAllLines(recipeFilePath);
                var currentRecipe = new StringBuilder();
                bool inQuotes = false;
                form.toolStripProgressBar1.Value = 0;
                form.toolStripProgressBar1.Maximum = lines.Length;

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    foreach (char c in line)
                    {
                        if (c == '\"')
                        {
                            inQuotes = !inQuotes;
                        }
                    }
                    currentRecipe.AppendLine(line);
                    if (!inQuotes && Regex.IsMatch(line, @"\b[1-3],[1-3]\b"))
                    {
                        recipeGroups.Add(currentRecipe.ToString().Trim());
                        currentRecipe.Clear();
                    }
                    form.toolStripProgressBar1.Value++;
                }

                if (currentRecipe.Length > 0)
                {
                    recipeGroups.Add(currentRecipe.ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading the recipe file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return recipeGroups;
        }


        private Dictionary<string, TempItems> LoadItemPanelItems(string itempanelfile)
        {
            var itemPanelItems = new Dictionary<string, TempItems>();
            var itemPanelLines = File.ReadAllLines(itempanelfile);
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = itemPanelLines.Length;

            foreach (var line in itemPanelLines.Skip(1))
            {
                var columns = line.Split(',');
                if (columns.Length >= 5)
                {
                    string v = columns[0];
                    string addItemName = columns[2] == "0" ? v : v + ":" + columns[2];

                    if (!itemPanelItems.ContainsKey(addItemName))
                    {
                        itemPanelItems[addItemName] = new TempItems
                        {
                            ItemMeta = columns[2],
                            ItemId = columns[1],
                            ItemNames = new List<string>(),
                            DisplayNames = new List<string>()
                        };
                    }

                    itemPanelItems[addItemName].ItemNames.Add(addItemName);
                    itemPanelItems[addItemName].DisplayNames.Add(columns[4]);
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }

            return itemPanelItems;
        }

        private string[] UpdateIngredientsWithDisplayNames(string[] ingredients, Dictionary<string, TempItems> itemPanelItems)
        {
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = ingredients.Length;
            for (int i = 0; i < ingredients.Length; i++)
            {
                string ingredient = ingredients[i];

                if (ingredient.Contains("N/A"))
                {
                    form.toolStripProgressBar1.Value++;
                    continue;
                }

                if (ingredient.Contains("|"))
                {
                    var possibleIngredients = ingredient.Split('|')
                                     .Select(item => item.Trim())
                                     .ToList();

                    var displayNames = new List<string>();

                    foreach (var possibleIngredient in possibleIngredients)
                    {
                        foreach (var item in itemPanelItems.Values)
                        {
                            if (item.ItemNames.Contains(possibleIngredient))
                            {
                                displayNames.Add(item.DisplayNames.FirstOrDefault());
                                break;
                            }
                        }
                    }

                    ingredients[i] = string.Join(", ", displayNames.Count > 0 ? displayNames : possibleIngredients);

                }
                else
                {
                    foreach (var item in itemPanelItems.Values)
                    {
                        if (item.ItemNames.Contains(ingredient))
                        {
                            ingredients[i] = item.DisplayNames.FirstOrDefault();
                            break;
                        }
                    }
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }

            return ingredients;
        }

        private Dictionary<string, List<(string registry, int maxDamage, string oreDict)>> LoadOreDictionary(string oreDictionaryFile)
        {
            var oreDictItems = new Dictionary<string, List<(string registry, int maxDamage, string oreDict)>>();
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = File.ReadLines(oreDictionaryFile).Count();
            foreach (var line in File.ReadLines(oreDictionaryFile).Skip(1))
            {
                var parts = line.Split(new[] { " - " }, StringSplitOptions.None);
                if (parts.Length == 4)
                {
                    string name = parts[0];
                    string registry = parts[1] == "null" ? "N/A" : parts[1];
                    string maxDamageStr = parts[2] == "null" ? "N/A" : parts[2];
                    string oreDict = parts[3] == "null" ? "N/A" : parts[3];

                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(registry) || string.IsNullOrEmpty(maxDamageStr) || oreDict == "N/A")
                    {
                        continue;
                    }

                    int maxDamage = int.Parse(maxDamageStr);
                    if (!oreDictItems.ContainsKey(name))
                    {
                        oreDictItems[name] = new List<(string registry, int maxDamage, string oreDict)>();
                    }
                    oreDictItems[name].Add((registry, maxDamage, oreDict));
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }

            return oreDictItems;
        }

        private void InsertRecipes(SQLiteConnection connection, SQLiteTransaction transaction, List<RecipeData> parsedRecipes)
        {
            const int maxBatchSize = 80;
            int totalRecipes = parsedRecipes.Count;

            for (int batchStart = 0; batchStart < totalRecipes; batchStart += maxBatchSize)
            {
                var batch = parsedRecipes.Skip(batchStart).Take(maxBatchSize).ToList();

                var bulkInsertQuery = new StringBuilder("INSERT INTO Recipes (InputPattern, A, B, C, D, E, F, G, H, I, OutputItem, Quantity) VALUES ");

                for (int i = 0; i < batch.Count; i++)
                {
                    bulkInsertQuery.Append($"(@InputPattern{i}, @A{i}, @B{i}, @C{i}, @D{i}, @E{i}, @F{i}, @G{i}, @H{i}, @I{i}, @OutputItem{i}, @Quantity{i}),");
                }

                bulkInsertQuery.Length--;
                bulkInsertQuery.Append(";");

                using (SQLiteCommand command = new SQLiteCommand(bulkInsertQuery.ToString(), connection, transaction))
                {
                    for (int i = 0; i < batch.Count; i++)
                    {
                        var recipe = batch[i];
                        command.Parameters.AddWithValue($"@InputPattern{i}", recipe.InputPattern);
                        command.Parameters.AddWithValue($"@A{i}", recipe.Ingredients.ElementAtOrDefault(0)?.Trim());
                        command.Parameters.AddWithValue($"@B{i}", recipe.Ingredients.ElementAtOrDefault(1)?.Trim());
                        command.Parameters.AddWithValue($"@C{i}", recipe.Ingredients.ElementAtOrDefault(2)?.Trim());
                        command.Parameters.AddWithValue($"@D{i}", recipe.Ingredients.ElementAtOrDefault(3)?.Trim());
                        command.Parameters.AddWithValue($"@E{i}", recipe.Ingredients.ElementAtOrDefault(4)?.Trim());
                        command.Parameters.AddWithValue($"@F{i}", recipe.Ingredients.ElementAtOrDefault(5)?.Trim());
                        command.Parameters.AddWithValue($"@G{i}", recipe.Ingredients.ElementAtOrDefault(6)?.Trim());
                        command.Parameters.AddWithValue($"@H{i}", recipe.Ingredients.ElementAtOrDefault(7)?.Trim());
                        command.Parameters.AddWithValue($"@I{i}", recipe.Ingredients.ElementAtOrDefault(8)?.Trim());
                        command.Parameters.AddWithValue($"@OutputItem{i}", recipe.OutputItem.Trim());
                        command.Parameters.AddWithValue($"@Quantity{i}", recipe.Quantity);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }



        private Dictionary<int, string> LoadRecipes(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            string loadRecipesQuery = "SELECT * FROM Recipes";
            var recipes = new Dictionary<int, string>();

            using (SQLiteCommand loadCommand = new SQLiteCommand(loadRecipesQuery, connection, transaction))
            {
                using (SQLiteDataReader reader = loadCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int recipeID = Convert.ToInt32(reader["RecipeID"]);
                        string outputItem = reader["OutputItem"].ToString();
                        recipes[recipeID] = outputItem;
                    }
                }
            }

            return recipes;
        }

        private void UpdateRecipes(SQLiteConnection connection, SQLiteTransaction transaction, Dictionary<int, string> recipes, Dictionary<string, TempItems> itemPanelItems)
        {
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = recipes.Count;


            foreach (var recipe in recipes)
            {
                int recipeID = recipe.Key;
                string outputItem = recipe.Value;

                if (itemPanelItems.TryGetValue(outputItem, out var itemPanel))
                {
                    string updateQuery = @"UPDATE Recipes SET
                                 ItemName = @ItemName,
                                 ItemId = @ItemId,
                                 ItemMeta = @ItemMeta,
                                 DisplayName = @DisplayName
                               WHERE RecipeID = @RecipeID";
                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@ItemName", itemPanel.ItemNames.FirstOrDefault());
                        updateCommand.Parameters.AddWithValue("@ItemId", itemPanel.ItemId);
                        updateCommand.Parameters.AddWithValue("@ItemMeta", itemPanel.ItemMeta);
                        updateCommand.Parameters.AddWithValue("@DisplayName", itemPanel.DisplayNames.FirstOrDefault());
                        updateCommand.Parameters.AddWithValue("@RecipeID", recipeID);
                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        form.RecipeText.Items.Add(itemPanel.DisplayNames.FirstOrDefault());
                        form.RecipeText.Refresh();
                    }
                    itemPanelItems.Remove(outputItem);
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }
        }

        private void InsertNewItems(SQLiteConnection connection, SQLiteTransaction transaction, Dictionary<string, TempItems> itemPanelItems)
        {
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = itemPanelItems.Count;

            foreach (var kvp in itemPanelItems)
            {
                var item = kvp.Value;
                string insertQuery = @"INSERT INTO Recipes (ItemName, ItemMeta, ItemId, DisplayName)
                              VALUES (@ItemName, @ItemMeta, @ItemId, @DisplayName)";
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection, transaction))
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ItemName", item.ItemNames.FirstOrDefault());
                    command.Parameters.AddWithValue("@ItemMeta", item.ItemMeta);
                    command.Parameters.AddWithValue("@ItemId", item.ItemId);
                    command.Parameters.AddWithValue("@DisplayName", item.DisplayNames.FirstOrDefault());
                    command.ExecuteNonQuery();
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }
        }

        private void UpdateOreDictionary(SQLiteConnection connection, SQLiteTransaction transaction, Dictionary<string, List<(string registry, int maxDamage, string oreDict)>> oreDictItems)
        {
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = oreDictItems.Count;

            foreach (var oreItem in oreDictItems)
            {
                string name = oreItem.Key;
                var (registry, maxDamage, oreDict) = oreItem.Value.FirstOrDefault();

                string query = "UPDATE Recipes SET OreDict = @OreDict WHERE DisplayName LIKE @DisplayName";
                using (SQLiteCommand command = new SQLiteCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@OreDict", oreDict);
                    command.Parameters.AddWithValue("@DisplayName", name);
                    command.ExecuteNonQuery();
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }
        }

        public void ParseRecipeFile()
        {
            string connectionString = $"Data Source={configManager.Config.DatabaseFile};Version=3;";

            try
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentException("Database connection string cannot be empty.");
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    bool hasData = CheckDatabaseData(connection);
                    if (hasData)
                    {
                        if (PromptDatabasePurge())
                        {
                            databaseIO.PurgeDatabase();
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar2.Value = 0;
            form.toolStripProgressBar2.Maximum = 9;

            var recipeFilePath = configManager.Config.RecipeFile;
            if (string.IsNullOrWhiteSpace(recipeFilePath))
            {
                MessageBox.Show("The recipe file path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var recipeGroups = ParseRecipeGroups(recipeFilePath);
            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Loading Items";
            Application.DoEvents();

            string itemPanelFilePath = configManager.Config.ItemPanelFile;
            if (string.IsNullOrWhiteSpace(itemPanelFilePath))
            {
                MessageBox.Show("The item panel file path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var itemPanelItems = LoadItemPanelItems(itemPanelFilePath);
            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Compiling Ore Dictionary";
            Application.DoEvents();

            var oreDictFilePath = configManager.Config.OreDictionary;
            if (string.IsNullOrWhiteSpace(oreDictFilePath))
            {
                MessageBox.Show("The ore dictionary file path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var oreDictItems = LoadOreDictionary(oreDictFilePath);
            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Parsing Recipes";
            Application.DoEvents();

            var parsedRecipes = ParseRecipes(recipeGroups);
            form.toolStripProgressBar1.Maximum = parsedRecipes.Count;

            foreach (var recipe in parsedRecipes)
            {
                recipe.Ingredients = UpdateIngredientsWithDisplayNames(recipe.Ingredients, itemPanelItems);
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        form.toolStripStatusLabel1.Text = "Inserting Recipes";
                        Application.DoEvents();
                        InsertRecipes(connection, transaction, parsedRecipes);
                        form.toolStripProgressBar2.Value++;
                        Application.DoEvents();

                        var recipes = LoadRecipes(connection, transaction);
                        form.toolStripProgressBar2.Value++;
                        form.toolStripStatusLabel1.Text = "Updating Recipes";
                        Application.DoEvents();
                        UpdateRecipes(connection, transaction, recipes, itemPanelItems);
                        form.toolStripProgressBar2.Value++;
                        form.toolStripStatusLabel1.Text = "Inserting Additional Items";
                        Application.DoEvents();
                        InsertNewItems(connection, transaction, itemPanelItems);
                        form.toolStripProgressBar2.Value++;
                        form.toolStripStatusLabel1.Text = "Updating Ore Dictionary";
                        Application.DoEvents();
                        UpdateOreDictionary(connection, transaction, oreDictItems);
                        form.toolStripProgressBar2.Value++;
                        Application.DoEvents();
                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show($"SQLite Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        transaction.Rollback();
                    }
                }
            }
        }

        private List<RecipeData> ParseRecipes(List<string> recipeGroups)
        {
            var parsedRecipes = new List<RecipeData>();

            foreach (var recipeLine in recipeGroups)
            {
                var parts = Regex.Split(recipeLine, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Select(p => p.Trim('\"')).ToArray();
                string inputPattern = parts.ElementAtOrDefault(2)?.Trim() ?? "---------";
                var rows = inputPattern.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var cleanedPattern = string.Join("", rows.Select(row => row.PadRight(3, '-')));
                inputPattern = cleanedPattern.PadRight(9, '-');
                var ingredients = ParseIngredients(parts.ElementAtOrDefault(3)?.Trim() ?? "N/A").Select(ingredient => ingredient.Trim()).ToArray();
                string outputItem = parts.ElementAtOrDefault(4)?.Trim() ?? "N/A";
                string[] outputParts = outputItem.Split(' ');
                string outputItemWithoutQuantity = outputParts[0];
                int quantity = (outputParts.Length > 1 && outputParts[1].StartsWith("x")) ? int.Parse(outputParts[1].Substring(1)) : 1;

                parsedRecipes.Add(new RecipeData
                {
                    InputPattern = inputPattern,
                    Ingredients = ingredients,
                    OutputItem = outputItemWithoutQuantity,
                    Quantity = quantity
                });
            }
            return parsedRecipes;
        }
    }
}