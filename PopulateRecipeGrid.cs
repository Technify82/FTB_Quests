using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTB_Quests
{
    public class PopulateRecipeGrid
    {
        public MainForm form;
        public string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";
        public string ImageFolder = ConfigManager.Config.ImageFolder;
        private readonly QuestLinker questLinker;

        public PopulateRecipeGrid(MainForm form)
        {
            this.form = form;
            questLinker = new QuestLinker(form);
        }

        public void GridParser(ComboBox comboBox, Form parentForm)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string is invalid");
                Console.WriteLine("Connection string is null or empty.");
                return;
            }

            if (comboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an item from the list.");
                return;
            }

            string selectedRecipe = comboBox.SelectedItem.ToString();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                ClearAllPictureBoxes(parentForm);

                var (displayName, inputPattern, ingredients) = GetRecipeDetails(selectedRecipe, connection);
                if (!string.IsNullOrEmpty(displayName))
                {
                    SetOutputImage(displayName, parentForm);
                }

                PopulateIngredientPictureBoxes(inputPattern, ingredients, parentForm);


                connection.Close();
            }
        }

        private void PopulateIngredientPictureBoxes(string inputPattern, Dictionary<string, string> ingredients, Form parentForm)
        {
            var pictureBoxes = parentForm.Controls.OfType<PictureBox>()
                                                   .Where(pb => System.Text.RegularExpressions.Regex.IsMatch(pb.Name, @"pictureBox[1-9]$"))
                                                   .OrderBy(pb => pb.Name)
                                                   .ToList();

            for (int index = 0; index < pictureBoxes.Count; index++)
            {
                if (index >= inputPattern.Length) break;

                string ingredientKey = inputPattern[index].ToString();
                if (ingredients.TryGetValue(ingredientKey, out string ingredient) && !string.IsNullOrEmpty(ingredient))
                {
                    try
                    {
                        string imagePath = Path.Combine(ImageFolder, ingredient + ".png");

                        if (File.Exists(imagePath))
                        {
                            pictureBoxes[index].Image = Image.FromFile(imagePath);
                            pictureBoxes[index].SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                        {
                            Console.WriteLine($"Image not found for ingredient: {ingredient}");
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Invalid path for ingredient: {ingredient}, Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Ingredient key not found or empty: {ingredientKey}");
                }
            }
        }

        private (string DisplayName, string InputPattern, Dictionary<string, string> Ingredients) GetRecipeDetails(string recipe, SQLiteConnection connection)
        {
            string displayName = null;
            string inputPattern = string.Empty;
            var ingredients = new Dictionary<string, string>();

            var query = @"SELECT DisplayName, InputPattern, A, B, C, D, E, F, G, H, I 
                  FROM Recipes WHERE DisplayName = @RecipeName";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@RecipeName", recipe);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        displayName = reader.IsDBNull(0) ? null : reader.GetString(0);
                        inputPattern = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                        for (char c = 'A'; c <= 'I'; c++)
                        {
                            string ingredient = reader.IsDBNull(c - 'A' + 2) ? string.Empty : reader.GetString(c - 'A' + 2);
                            ingredients[c.ToString()] = ingredient;
                        }
                    }
                }
            }
            OutputFullRecipeRow(recipe, connection);

            return (displayName, inputPattern, ingredients);
        }

        public void OutputFullRecipeRow(string recipe, SQLiteConnection connection)
        {
            var query = @"SELECT RecipeID, InputPattern, A, B, C, D, E, F, G, H, I, OutputItem, ItemName, ItemId, ItemMeta, DisplayName, OreDict, Quantity, Quests 
                  FROM Recipes WHERE DisplayName = @RecipeName";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@RecipeName", recipe);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        form.RecipeTextDetails.Clear();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            string columnValue = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i).ToString();
                            form.RecipeTextDetails.AppendText($"{columnName}: {columnValue}{Environment.NewLine}");
                            Console.WriteLine($"{columnName}: {columnValue}");
                        }
                    }
                }
            }
        }


        public void SetOutputImage(string displayName, Form parentForm)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                Console.WriteLine($"Invalid display name: {displayName}");
                return;
            }

            string invalidChars = new string(Path.GetInvalidPathChars());
            foreach (char c in invalidChars)
            {
                displayName = displayName.Replace(c.ToString(), "");
            }

            string imageFilePath = Path.Combine(ImageFolder, displayName + ".png");

            if (!File.Exists(imageFilePath))
            {
                Console.WriteLine($"Image not found: {imageFilePath}");
                return;
            }

            try
            {
                if (parentForm.Controls.Find("pictureBoxOutput", true).FirstOrDefault() is PictureBox pictureBoxOutput)
                {
                    pictureBoxOutput.Image = Image.FromFile(imageFilePath);
                    pictureBoxOutput.SizeMode = PictureBoxSizeMode.StretchImage;
                    Console.WriteLine($"Set PictureBoxOutput Image Path: {imageFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        public void ClearAllPictureBoxes(Form parentForm)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (parentForm.Controls.Find($"pictureBox{i}", true).FirstOrDefault() is PictureBox pictureBox)
                {
                    pictureBox.Image = null;
                    Console.WriteLine($"Cleared Image for pictureBox{i}");
                }
            }

            if (parentForm.Controls.Find("pictureBoxOutput", true).FirstOrDefault() is PictureBox pictureBoxOutput)
            {
                pictureBoxOutput.Image = null;
                Console.WriteLine($"Cleared Image for pictureBoxOutput");
            }
        }

    }
}

