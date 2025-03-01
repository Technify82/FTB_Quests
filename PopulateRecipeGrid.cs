

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTB_Quests
{
    public class PopulateRecipeGrid
    {
        public MainForm form;
        ConfigManager configManager;
        private List<ProjectProperties> projectProperties;
        List<Potions> potions;

        public PopulateRecipeGrid(MainForm form, List<ProjectProperties> projectProperties)
        {
            this.form = form;
            configManager = ConfigManager.Instance;
            this.projectProperties = projectProperties;
        }

        public void GridParser(ComboBox comboBox, Form parentForm)
        {
            if (comboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an item from the list.");
                return;
            }

            string selectedRecipe = comboBox.SelectedItem.ToString();
            ClearAllPictureBoxes(parentForm);

            var (displayName, inputPattern, ingredients) = GetRecipeDetails(selectedRecipe);
            if (!string.IsNullOrEmpty(displayName))
            {
                SetOutputImage(displayName, parentForm);
            }

            PopulateIngredientPictureBoxes(inputPattern, ingredients, parentForm);
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
                        string imagePath = Path.Combine(configManager.Config.ImageFolder, ingredient + ".png");

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

        private (string DisplayName, string InputPattern, Dictionary<string, string> Ingredients) GetRecipeDetails(string recipe)
        {
            string displayName = null;
            string inputPattern = string.Empty;
            var ingredients = new Dictionary<string, string>();

            var recipeDetails = projectProperties.FirstOrDefault(r => r.DisplayName == recipe);
            if (recipeDetails != null)
            {
                displayName = recipeDetails.DisplayName;
                inputPattern = recipeDetails.InputPattern;

                for (int i = 0; i < recipeDetails.Ingredients.Length; i++)
                {
                    char ingredientKey = (char)('A' + i); // Convert index to corresponding letter A-I
                    ingredients[ingredientKey.ToString()] = recipeDetails.Ingredients[i];
                }
            }

            OutputFullRecipeRow(recipe);

            return (displayName, inputPattern, ingredients);
        }


        public void OutputFullRecipeRow(string recipe)
        {
            var recipeDetails = projectProperties.FirstOrDefault(r => r.DisplayName == recipe);
            if (recipeDetails != null)
            {
                form.RecipeTextDetails.Clear();
                foreach (var prop in typeof(ProjectProperties).GetProperties())
                {
                    string columnName = prop.Name;
                    string columnValue = prop.GetValue(recipeDetails)?.ToString() ?? "NULL";
                    form.RecipeTextDetails.AppendText($"{columnName}: {columnValue}{Environment.NewLine}");
                    Console.WriteLine($"{columnName}: {columnValue}");
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

            string imageFilePath = Path.Combine(configManager.Config.ImageFolder, displayName + ".png");

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