using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class Configuration
    {

        public void LoadAndInitializeConfiguration()
        {
            string configFilePath = "Configuration.json";
            if (File.Exists(configFilePath))
            {
                try
                {
                    string json = File.ReadAllText(configFilePath);
                    config = JsonSerializer.Deserialize<ConfigProperties>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                config = new ConfigProperties();

                if (string.IsNullOrEmpty(config.ProjectFolder) || !Directory.Exists(config.ProjectFolder))
                {
                    MessageBox.Show("Please set a valid project folder before proceeding.", "Project Folder Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ProjectFolder_Click(null, null); // Prompt user to select a project folder
                }
                else
                {
                    UseCache.Checked = config.UseCache; // Set UseCache checkbox state
                }
            }

            PopulateTextBoxes();
        }

        public void PopulateTextBoxes()
        {
            projectFolderTextBox.Text = config.ProjectFolder;
            recipeFileTextBox.Text = config.RecipeFile;
            itemPanelFileTextBox.Text = config.ItemPanelFile;
            imageFolderTextBox.Text = config.ImageFolder;
            QuestFolderLocation.Text = config.QuestFolder;
            OreDictFileLocation.Text = config.OreDictionary;
            DatabaseFile.Text = config.DatabaseFile;
        }

        public void SaveConfig(string caller)
        {
            switch (caller)
            {
                case "ProjectFolder":
                    config.ProjectFolder = projectFolderTextBox.Text;
                    break;
                case "RecipeFile":
                    config.RecipeFile = recipeFileTextBox.Text;
                    break;
                case "ItemPanel":
                    config.ItemPanelFile = itemPanelFileTextBox.Text;
                    break;
                case "ImageFolder":
                    config.ImageFolder = imageFolderTextBox.Text;
                    break;
                case "QuestFolder":
                    config.QuestFolder = QuestFolderLocation.Text;
                    break;
                case "OreDictFile":
                    config.OreDictionary = OreDictFileLocation.Text;
                    break;
                case "DatabaseFile":
                    config.DatabaseFile = DatabaseFile.Text;
                    break;
                case "UseCache":
                    config.UseCache = UseCache.Checked;
                    break;
                    // Add cases for other properties as needed
            }

            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("Configuration.json", json);
        }

        private void UpdateTextBoxesAndConfigForCache(string baseCacheDir)
        {
            // Update text boxes to reflect cache folder
            projectFolderTextBox.Text = baseCacheDir;
            UpdateTextBoxes(baseCacheDir);

            // Update config to reflect cache folder
            config.ProjectFolder = baseCacheDir;
            config.RecipeFile = Path.Combine(baseCacheDir, Path.GetFileName(config.RecipeFile));
            config.ItemPanelFile = Path.Combine(baseCacheDir, Path.GetFileName(config.ItemPanelFile));
            config.ImageFolder = Path.Combine(baseCacheDir, Path.GetFileName(config.ImageFolder));
            config.QuestFolder = Path.Combine(baseCacheDir, Path.GetFileName(config.QuestFolder));
            config.OreDictionary = Path.Combine(baseCacheDir, Path.GetFileName(config.OreDictionary));
           // config.DatabaseFile = Path.Combine(baseCacheDir, Path.GetFileName(config.DatabaseFile));
        }

        public void UpdateTextBoxes(string projectFolderPath)
        {
            string mostRecentFile = GetMostRecentFile(projectFolderPath, "pmdumper\\shaped_recipes*.csv");
            if (mostRecentFile != null)
            {
                recipeFileTextBox.Text = mostRecentFile;
            }
            else
            {
                MessageBox.Show("No matching files found in the selected folder.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            config.RecipeFile = mostRecentFile;
            itemPanelFileTextBox.Text = Path.Combine(projectFolderPath, "dumps\\itempanel.csv");
            config.ItemPanelFile = itemPanelFileTextBox.Text;
            imageFolderTextBox.Text = Path.Combine(projectFolderPath, "dumps\\itempanel_icons\\");
            config.ImageFolder = imageFolderTextBox.Text;
            QuestFolderLocation.Text = Path.Combine(projectFolderPath, "config\\ftbquests\\normal\\chapters\\");
            config.QuestFolder = QuestFolderLocation.Text;
            OreDictFileLocation.Text = Path.Combine(projectFolderPath, "dumps\\itemdump.txt");
            config.OreDictionary = OreDictFileLocation.Text;
            DatabaseFile.Text = config.DatabaseFile;
        }

    }
}
