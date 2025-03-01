using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FTB_Quests
{
    public class NewParser
    {
        private readonly MainForm form;
        ConfigManager configManager;
        public List<ProjectProperties> projectProperties;
        public List<Potions> potionProperties;
        private List<TempItems> tempItemsList;
        private List<(string name, string registry, int maxDamage, string oreDict)> oreDictionaryItems;
        private ConfigProperties configProperties;

        public NewParser(MainForm form, List<ProjectProperties> projectProperties, List<Potions> potionProperties)
        {
            this.form = form;
            this.projectProperties = projectProperties;
            this.potionProperties = potionProperties;
            configManager = ConfigManager.Instance;
            oreDictionaryItems = new List<(string name, string registry, int maxDamage, string oreDict)>();
            tempItemsList = new List<TempItems>();
            configProperties = new ConfigProperties();
        }

        public void ParseRecipeFile()
        {
            var recipeFilePath = configManager.Config.RecipeFile;
            var itemPanelFilePath = configManager.Config.ItemPanelFile;
            var oreDictFilePath = configManager.Config.OreDictionary;

            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar2.Value = 0;
            form.toolStripProgressBar2.Maximum = 9;

            if (string.IsNullOrWhiteSpace(recipeFilePath))
            {
                MessageBox.Show("The recipe file path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Loading Items";
            Application.DoEvents();

            if (string.IsNullOrWhiteSpace(itemPanelFilePath))
            {
                MessageBox.Show("The item panel file path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadItemPanelItems(itemPanelFilePath);
            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Compiling Ore Dictionary";
            Application.DoEvents();

            if (string.IsNullOrWhiteSpace(oreDictFilePath))
            {
                MessageBox.Show("The ore dictionary file path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadOreDictionary(oreDictFilePath);
            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Parsing Recipes";
            Application.DoEvents();

            var parsedRecipes = ParseRecipeGroups(recipeFilePath);
            form.toolStripProgressBar1.Maximum = parsedRecipes.Count;

            foreach (var recipe in parsedRecipes)
            {
                recipe.Ingredients = UpdateIngredientsWithDisplayNames(recipe.Ingredients);
            }

            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Updating Recipes";
            Application.DoEvents();
            UpdateRecipes();

            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Inserting Additional Items";
            Application.DoEvents();
            InsertNewItems();

            form.toolStripProgressBar2.Value++;
            form.toolStripStatusLabel1.Text = "Updating Ore Dictionary";
            Application.DoEvents();
            UpdateOreDictionary();

            form.toolStripProgressBar2.Value++;
            Application.DoEvents();

        }

        private ProjectProperties ParseIngredients(string inputIngredients)
        {
            var prefixes = new[] { "A:", "B:", "C:", "D:", "E:", "F:", "G:", "H:", "I:" };
            string suffix = ":*";

            var ingredients = new string[9];
            for (int i = 0; i < ingredients.Length; i++)
                ingredients[i] = "N/A";

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

            for (int i = 0; i < ingredientLines.Length; i++)
            {
                if (!string.IsNullOrEmpty(ingredientLines[i]))
                {
                    ingredients[i] = ingredientLines[i];
                }
            }

            return new ProjectProperties
            {
                Ingredients = ingredients
            };
        }




        public void PopulateRecipeText(List<ProjectProperties> projectProperties)
        {
            foreach (var recipe in projectProperties)
            {
                if (!string.IsNullOrEmpty(recipe.DisplayName))
                {
                    form.RecipeText.Items.Add(recipe.DisplayName);
                }
            }
        }

        private List<ProjectProperties> ParseRecipeGroups(string recipeFilePath)
        {
            var recipeGroups = new List<ProjectProperties>();

            if (string.IsNullOrWhiteSpace(recipeFilePath))
            {
                MessageBox.Show("The recipe file path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return recipeGroups;
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
                        var recipeData = ParseRecipe(currentRecipe.ToString().Trim());
                        recipeGroups.Add(recipeData);
                        projectProperties.Add(recipeData);
                        currentRecipe.Clear();
                    }
                    form.toolStripProgressBar1.Value++;
                }

                if (currentRecipe.Length > 0)
                {
                    var recipeData = ParseRecipe(currentRecipe.ToString().Trim());
                    recipeGroups.Add(recipeData);
                    projectProperties.Add(recipeData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading the recipe file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return recipeGroups;
        }

        private ProjectProperties ParseRecipe(string recipeText)
        {
            var parts = Regex.Split(recipeText, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Select(p => p.Trim('\"')).ToArray();
            string inputPattern = parts.ElementAtOrDefault(2)?.Trim() ?? "---------";
            var rows = inputPattern.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var cleanedPattern = string.Join("", rows.Select(row => row.PadRight(3, '-')));
            inputPattern = cleanedPattern.PadRight(9, '-');
            var ingredients = ParseIngredients(parts.ElementAtOrDefault(3)?.Trim() ?? "N/A").Ingredients;
            string outputItem = parts.ElementAtOrDefault(4)?.Trim() ?? "N/A";
            string[] outputParts = outputItem.Split(' ');
            string outputItemWithoutQuantity = outputParts[0];
            int quantity = (outputParts.Length > 1 && outputParts[1].StartsWith("x")) ? int.Parse(outputParts[1].Substring(1)) : 1;

            return new ProjectProperties
            {
                InputPattern = inputPattern,
                Ingredients = ingredients,
                OutputItem = outputItemWithoutQuantity,
                Quantity = quantity
            };
        }

        private void LoadItemPanelItems(string itempanelfile)
        {
            tempItemsList.Clear();

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
                        var tempItem = new TempItems
                        {
                            ItemMeta = columns[2],
                            ItemId = columns[1],
                            ItemNames = new List<string> { addItemName },
                            DisplayNames = new List<string> { columns[4] }
                        };

                        itemPanelItems[addItemName] = tempItem;
                        tempItemsList.Add(tempItem);
                    }
                    else
                    {
                        itemPanelItems[addItemName].ItemNames.Add(addItemName);
                        itemPanelItems[addItemName].DisplayNames.Add(columns[4]);
                    }
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }
        }

        private string[] UpdateIngredientsWithDisplayNames(string[] ingredients)
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
                        foreach (var item in tempItemsList)
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
                    foreach (var item in tempItemsList)
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

        private void LoadOreDictionary(string oreDictionaryFile)
        {
            oreDictionaryItems.Clear();

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
                    oreDictionaryItems.Add((name, registry, maxDamage, oreDict));
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }
        }

        private void UpdateRecipes()
        {
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = projectProperties.Count;

            foreach (var recipe in projectProperties)
            {
                if (tempItemsList.Any(item => item.ItemNames.Contains(recipe.OutputItem)))
                {
                    var itemPanel = tempItemsList.First(item => item.ItemNames.Contains(recipe.OutputItem));

                    recipe.ItemName = itemPanel.ItemNames.FirstOrDefault();
                    recipe.ItemId = itemPanel.ItemId;
                    recipe.ItemMeta = itemPanel.ItemMeta;
                    recipe.DisplayName = itemPanel.DisplayNames.FirstOrDefault();

                    form.RecipeText.Items.Add(recipe.DisplayName);
                    form.RecipeText.Refresh();
                }
                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }
        }

        private void InsertNewItems()
        {
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = tempItemsList.Count;

            foreach (var item in tempItemsList)
            {
                var newItem = new ProjectProperties
                {
                    ItemName = item.ItemNames.FirstOrDefault(),
                    ItemMeta = item.ItemMeta,
                    ItemId = item.ItemId,
                    DisplayName = item.DisplayNames.FirstOrDefault()
                };

                projectProperties.Add(newItem);

                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }
        }

        private void UpdateOreDictionary()
        {
            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = oreDictionaryItems.Count;

            foreach (var oreItem in oreDictionaryItems)
            {
                string name = oreItem.name;
                string oreDict = oreItem.oreDict;

                foreach (var recipe in projectProperties)
                {
                    if (recipe.DisplayName.Contains(name))
                    {
                        recipe.OreDict = oreDict;
                    }
                }

                form.toolStripProgressBar1.Value++;
                Application.DoEvents();
            }
        }
    }
}