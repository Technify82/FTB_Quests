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

        public void LoadConfiguration()
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
                MessageBox.Show("Configuration file not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                config = new ConfigProperties();
            }
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


        public void SaveConfig()
        {
            config.ProjectFolder = projectFolderTextBox.Text;
            config.RecipeFile = recipeFileTextBox.Text;
            config.ItemPanelFile = itemPanelFileTextBox.Text;
            config.ImageFolder = imageFolderTextBox.Text;
            config.QuestFolder = QuestFolderLocation.Text;
            config.OreDictionary = OreDictFileLocation.Text;
            config.DatabaseFile = DatabaseFile.Text;
            config.UseCache = UseCache.Checked;

            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("Configuration.json", json);
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
            itemPanelFileTextBox.Text = Path.Combine(projectFolderPath, "dumps\\itempanel.csv");
            imageFolderTextBox.Text = Path.Combine(projectFolderPath, "dumps\\itempanel_icons\\");
            QuestFolderLocation.Text = Path.Combine(projectFolderPath, "config\\ftbquests\\normal\\chapters\\");
            OreDictFileLocation.Text = Path.Combine(projectFolderPath, "dumps\\itemdump.txt");
            DatabaseFile.Text = config.DatabaseFile;
        }

    }
}
