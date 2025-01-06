using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace FTB_Quests


{
    internal class OreDictLogic
    {
        public string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";
        private readonly MainForm form;


        public OreDictLogic(string connectionString, MainForm form)
        {
            this.connectionString = connectionString;
            this.form = form;
        }

        public void CompileOreDictInformation()
        {
            var oreIngredients = new HashSet<string>();
            var oreDictNames = new Dictionary<string, List<string>>();
            var displayNames = new Dictionary<string, string>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                try
                {
                    var ingredientQuery = "SELECT DISTINCT A, B, C, D, E, F, G, H, I FROM Recipes WHERE " +
                                          "A LIKE 'ore:%' OR B LIKE 'ore:%' OR C LIKE 'ore:%' OR D LIKE 'ore:%' OR " +
                                          "E LIKE 'ore:%' OR F LIKE 'ore:%' OR G LIKE 'ore:%' OR H LIKE 'ore:%' OR I LIKE 'ore:%'";
                    using (var command = new SQLiteCommand(ingredientQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    if (!reader.IsDBNull(i))
                                    {
                                        string ingredient = reader.GetString(i);
                                        if (!string.IsNullOrEmpty(ingredient) && ingredient.StartsWith("ore:"))
                                        {
                                            oreIngredients.Add(ingredient);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var oreDictQuery = "SELECT DISTINCT ItemName, DisplayName, OreDict FROM Recipes WHERE OreDict IS NOT NULL AND OreDict != ''";
                    using (var command = new SQLiteCommand(oreDictQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string itemName = reader.IsDBNull(0) ? null : reader.GetString(0);
                                string displayName = reader.IsDBNull(1) ? null : reader.GetString(1);
                                string oreDict = reader.IsDBNull(2) ? null : reader.GetString(2);

                                if (!string.IsNullOrEmpty(oreDict))
                                {
                                    if (!oreDictNames.ContainsKey(oreDict))
                                    {
                                        oreDictNames[oreDict] = new List<string>();
                                    }
                                    if (!string.IsNullOrEmpty(itemName))
                                    {
                                        oreDictNames[oreDict].Add(itemName);
                                    }
                                    if (!string.IsNullOrEmpty(displayName))
                                    {
                                        oreDictNames[oreDict].Add(displayName);
                                        displayNames[oreDict] = displayName;
                                    }
                                }
                            }
                        }
                    }

                    DisplayNameFinding(oreIngredients.ToDictionary(ingredient => ingredient, ingredient => ingredient), oreDictNames);
                    SwapOreItemsForDisplayNames(oreIngredients.ToDictionary(ingredient => ingredient, ingredient => ingredient), oreDictNames, displayNames);
                }
                finally
                {
                    connection.Close();
                }
            }
        }




        private readonly Dictionary<string, int> colorMapping = new Dictionary<string, int> {
            { "Black", 15 },
            { "Red", 14 },
            { "Green", 13 },
            { "Brown", 12 },
            { "Blue", 11 },
            { "Purple", 10 },
            { "Cyan", 9 },
            { "LightGray", 8 },
            { "Gray", 7 },
            { "Pink", 6 },
            { "Lime", 5 },
            { "Yellow", 4 },
            { "LightBlue", 3 },
            { "Magenta", 2 },
            { "Orange", 1 },
            { "White", 0 }
        }
 ;

        private int? GetColorCodeIfExists(string ingredientValue)
        {
            foreach (var color in colorMapping.Keys)
            {
                if (ingredientValue.Contains(color))
                {
                    return colorMapping[color];
                }
            }
            return null;
        }

        public void DisplayNameFinding(Dictionary<string, string> oreIngredients, Dictionary<string, List<string>> oreDictNames)
        {
            var ItemNameDictionary = new Dictionary<string, string>
    {
        {"dye", "minecraft:dye" },
        {"glass", "minecraft:stained_glass"},
        {"wool", "minecraft:wool" },
        { "ore:gemQuartzBlack", "actuallyadditions:item_misc:5" }
    };

            form.toolStripProgressBar1.Maximum = oreIngredients.Count;
            form.toolStripProgressBar1.Value = 0;

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                foreach (var ingredient in oreIngredients)
                {
                    string ingredientValue = ingredient.Value;
                    var containsColorMapping = GetColorCodeIfExists(ingredientValue);
                    if (ingredientValue.Contains("dye"))
                    {
                        containsColorMapping = containsColorMapping == 15 ? 0 : containsColorMapping == 0 ? 15 : containsColorMapping;
                    }

                    string itemName = GetItemName(ingredientValue, containsColorMapping, ItemNameDictionary);
                    bool displayNamesFound = false;

                    if (!string.IsNullOrEmpty(itemName))
                    {
                        displayNamesFound = ProcessIngredient(connection, ingredient, itemName, "ItemName");
                        displayNamesFound |= ProcessIngredient(connection, ingredient, itemName, "OreDict");
                    }

                    if (!displayNamesFound && oreDictNames.TryGetValue(ingredientValue.Replace("ore:", "").Trim(), out var items))
                    {
                        foreach (var item in items)
                        {
                            displayNamesFound = ProcessIngredient(connection, ingredient, item, "ItemName");
                            displayNamesFound |= ProcessIngredient(connection, ingredient, item, "OreDict");
                            if (displayNamesFound) break;
                        }
                    }

                    form.toolStripProgressBar1.Value++;
                }
            }
        }

        private string GetItemName(string ingredientValue, int? containsColorMapping, Dictionary<string, string> itemNameDictionary)
        {
            foreach (var item in itemNameDictionary)
            {
                if (ingredientValue.Contains(item.Key))
                {
                    return containsColorMapping.HasValue ? $"{item.Value}:{containsColorMapping}" : item.Value;
                }
            }
            return null;
        }

        private bool ProcessIngredient(SQLiteConnection connection, KeyValuePair<string, string> ingredient, string itemName, string type)
        {
            try
            {
                string query = type == "ItemName"
                    ? "SELECT DISTINCT DisplayName FROM Recipes WHERE ItemName = @Item"
                    : "SELECT DISTINCT DisplayName FROM Recipes WHERE OreDict = @Item";

                var displayNames = new List<string>();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Item", itemName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var displayName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            if (!string.IsNullOrEmpty(displayName))
                            {
                                displayNames.Add(displayName);
                            }
                        }
                    }
                }

                if (displayNames.Any())
                {
                    BatchUpdateDisplayNames(connection, ingredient.Key, displayNames);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching and swapping display names: {ex.Message}");
            }

            return false;
        }

        private void BatchUpdateDisplayNames(SQLiteConnection connection, string ingredient, List<string> displayNames)
        {
            foreach (var displayName in displayNames)
            {
                for (char col = 'A'; col <= 'I'; col++)
                {
                    var updateQuery = $"UPDATE Recipes SET {col} = @displayName WHERE {col} = @ingredient";
                    using (var updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@displayName", displayName);
                        updateCommand.Parameters.AddWithValue("@ingredient", ingredient);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SwapOreItemsForDisplayNames(Dictionary<string, string> oreIngredients, Dictionary<string, List<string>> oreDictNames, Dictionary<string, string> displayNames)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var preDictionaryMatches = new Dictionary<string, string>();

                foreach (var oreIngredient in oreIngredients.Keys)
                {
                    foreach (var oreDict in oreDictNames)
                    {
                        if (displayNames.ContainsKey(oreDict.Key) && oreIngredient.Contains(oreDict.Key))
                        {
                            string colorPart = oreIngredient.Replace(oreDict.Key, "").Replace("ore:", "");

                            if (colorMapping.TryGetValue(colorPart, out int colorCode))
                            {
                                string itemName = oreDict.Value.FirstOrDefault(name => name.Contains(colorCode.ToString()));
                                if (!string.IsNullOrEmpty(itemName))
                                {
                                    preDictionaryMatches[oreIngredient] = displayNames[oreDict.Key];
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (var match in preDictionaryMatches)
                {
                    string oreIngredient = match.Key;
                    string displayName = match.Value;

                    for (char col = 'A'; col <= 'I'; col++)
                    {
                        var updateQuery = $"UPDATE Recipes SET {col} = @displayName WHERE {col} = @oreIngredient";

                        using (var command = new SQLiteCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@displayName", displayName);
                            command.Parameters.AddWithValue("@oreIngredient", oreIngredient);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                connection.Close();
            }
        }
    }
}
