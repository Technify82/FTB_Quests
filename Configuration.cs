using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class Configuration : Form
    {
        public Config config;

        public Configuration()
        {
            InitializeComponent();
            LoadConfiguration();
            PopulateTextBoxes();

        }
        public void LoadConfiguration()
        {
            string configFilePath = "Configuration.json";
            if (File.Exists(configFilePath))
            {
                try
                {
                    string json = File.ReadAllText(configFilePath);
                    config = JsonSerializer.Deserialize<Config>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Configuration file not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                config = new Config();
            }
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
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("Configuration.json", json);
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

        public void ProjectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a project folder";
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    projectFolderTextBox.Text = folderBrowserDialog.SelectedPath;
                    UpdateTextBoxes(folderBrowserDialog.SelectedPath);
                }
                SaveConfig();
            }
        }

        public void RecipeFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a recipe file";
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                openFileDialog.DefaultExt = "csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    recipeFileTextBox.Text = openFileDialog.FileName;
                }
                SaveConfig();
            }
        }
        public void ItemPanelFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select an item panel file";
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                openFileDialog.DefaultExt = "csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    itemPanelFileTextBox.Text = openFileDialog.FileName;
                }
                SaveConfig();
            }
        }
        public void ItemImagesFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder containing recipe images.";
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    imageFolderTextBox.Text = folderBrowserDialog.SelectedPath;
                }
                SaveConfig();
            }
        }
        public void FindQuestFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder containing quests";
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    QuestFolderLocation.Text = folderBrowserDialog.SelectedPath;
                }
                SaveConfig();
            }
        }

        private void OreDictButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select an Ore Dictionary file";
                openFileDialog.Filter = "TXT files (*.txt)|*.txt";
                openFileDialog.DefaultExt = "txt";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    OreDictFileLocation.Text = openFileDialog.FileName;
                }
                SaveConfig();
            }

        }

        private void DatabaseFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select an SQL Database";
                openFileDialog.Filter = "TXT files (*.db)|*.db";
                openFileDialog.DefaultExt = "db";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DatabaseFile.Text = openFileDialog.FileName;
                }
                SaveConfig();
            }

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
        public string GetMostRecentFile(string folderPath, string searchPattern)
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            var file = directoryInfo.GetFiles(searchPattern)
                                    .OrderByDescending(f => f.LastWriteTime)
                                    .FirstOrDefault();
            return file?.FullName;
        }
    }

    public class Config
    {
        public string ProjectFolder { get; set; }
        public string RecipeFile { get; set; }
        public string ItemPanelFile { get; set; }
        public string ImageFolder { get; set; }
        public string QuestFolder { get; set; }
        public string OreDictionary { get; set; }
        public string DatabaseFile { get; set; }
    }

    public static class ConfigManager
    {
        public static Config Config { get; set; }

        static ConfigManager()
        {
            LoadConfiguration();
        }

        public static void LoadConfiguration()
        {
            if (File.Exists("Configuration.json"))
            {
                string json = File.ReadAllText("Configuration.json");
                Config = JsonSerializer.Deserialize<Config>(json);
            }
            else
            {
                Config = new Config();
            }
        }
    }
}