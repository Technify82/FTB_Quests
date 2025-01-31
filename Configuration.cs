using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class Configuration : Form
    {
        public ConfigProperties config;

        public Configuration()
        {
            InitializeComponent();
            LoadConfiguration();
            PopulateTextBoxes();
            Load += Configuration_Load;
        }

        private void Configuration_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
            PopulateTextBoxes();

            if (string.IsNullOrEmpty(config.ProjectFolder) || !Directory.Exists(config.ProjectFolder))
            {
                MessageBox.Show("Please set a valid project folder before proceeding.", "Project Folder Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                projectFolder.PerformClick();
            }
            else
            {
                CachingSystem.InitializeCaching(config, CacheInfoBox);
                UpdateTextBoxes(config.ProjectFolder);
            }
        }

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

        public void ProjectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a project folder";
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    projectFolderTextBox.Text = folderBrowserDialog.SelectedPath;
                    config.ProjectFolder = folderBrowserDialog.SelectedPath;


                    CachingSystem.InitializeCaching(config, CacheInfoBox);
                    UpdateTextBoxes(config.ProjectFolder);
                    SaveConfig();
                    EnableControls();
                }
            }
        }


        private void DisableControls()
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Name != "projectFolderTextBox" && ctrl.Name != "ProjectFolderButton")
                {
                    ctrl.Enabled = false;
                }
            }
        }

        private void EnableControls()
        {
            foreach (Control ctrl in Controls)
            {
                ctrl.Enabled = true;
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
                openFileDialog.Filter = "DB files (*.db)|*.db";
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


        private void PerformCachingButton_Click(object sender, EventArgs e)
        {
        }

        private void SwitchToCachedPathsButton_Click(object sender, EventArgs e)
        {
            config.SwitchToCachedPaths();
            MessageBox.Show("Switched to cached paths.");
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));
            CachingSystem.EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);

            CacheInfoBox.AppendText("Copying config locations to cache...\n");
            CachingSystem.CopyConfigLocationsToCache(config, CacheInfoBox);
            MessageBox.Show("Caching completed.");
        }
    }
}
