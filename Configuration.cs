using System;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class Configuration : Form
    {
        public ConfigProperties config;
        ConfigManager configManager;
        private MainForm mainForm;

        public Configuration(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            configManager = ConfigManager.Instance;
            LoadConfiguration();
            Load += Configuration_Load;
            InitializeCache(config, CacheInfoBox, configManager);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            mainForm.UpdateConfiguration();
        }

        private void ProjectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a project folder";
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    projectFolderTextBox.Text = folderBrowserDialog.SelectedPath;
                    config.ProjectFolder = folderBrowserDialog.SelectedPath;

                    UpdateTextBoxes(config.ProjectFolder);
                    SaveConfig();
                    EnableControls();
                }
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
                UpdateConfiguration();
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
                UpdateConfiguration();
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
                UpdateConfiguration();
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
                UpdateConfiguration();
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
                UpdateConfiguration();
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
                    config.DatabaseFile = openFileDialog.FileName;
                    DatabaseFile.Text = openFileDialog.FileName;
                }
                UpdateConfiguration();
            }
        }

        private void UseCache_CheckedChanged(object sender, EventArgs e)
        {
            if (UseCache.Checked)
            {
                string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));
                CachingSystem.EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);

                CacheInfoBox.AppendText("Copying config locations to cache...\n");
                CachingSystem.CopyConfigLocationsToCache(config, CacheInfoBox, configManager);
            }
            else
            {
                CacheInfoBox.AppendText("Cache is turned off.\n");
            }

            configManager.Config.UseCache = UseCache.Checked;
            UpdateConfiguration();
        }



    }
}
