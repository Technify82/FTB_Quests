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
            LoadAndInitializeConfiguration();
            InitializeCache(config, CacheInfoBox, configManager);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            mainForm.UpdateConfiguration();
        }

        private void ProjectFolder_Click(object sender, EventArgs e)
        {
            string method = "ProjectFolder";
            FolderSelect(method);
            SaveConfig(method);
        }

        public void RecipeFile_Click(object sender, EventArgs e)
        {
            string method = "RecipeFile";
            FileSelect(method);
            SaveConfig(method);
        }

        public void ItemPanelFile_Click(object sender, EventArgs e)
        {
            string method = "ItemPanel";
            FileSelect(method);
            SaveConfig(method);
        }

        public void ItemImagesFolder_Click(object sender, EventArgs e)
        {

            string method = "ImageFolder";
            FolderSelect(method);
            SaveConfig(method);
        }

        public void FindQuestFolder_Click(object sender, EventArgs e)
        {
            string method = "QuestFolder";
            FolderSelect(method);
            SaveConfig(method);
        }

        private void OreDictButton_Click(object sender, EventArgs e)
        {
            string method = "OreDictFile";
            FileSelect(method);
            SaveConfig(method);
        }

        private void DatabaseFile_Click(object sender, EventArgs e)
        {
            string method = "DatabaseFile";
            FileSelect(method);
            SaveConfig(method);
        }

        private void UseCache_CheckedChanged(object sender, EventArgs e)
        {
            string method = "UseCache";

            if (UseCache.Checked)
            {
                string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));
                CachingSystem.EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);

                CacheInfoBox.AppendText("Copying config locations to cache...\n");
                CachingSystem.CopyConfigLocationsToCache(config, CacheInfoBox, configManager);

                // Call helper method to update text boxes and config
                UpdateTextBoxesAndConfigForCache(baseCacheDir);
            }
            else
            {
                CacheInfoBox.AppendText("Cache is turned off.\n");
            }

            configManager.Config.UseCache = UseCache.Checked;
            SaveConfig(method); // Save configuration after updates
        }

        private void PrintCurrentCache_Click(object sender, EventArgs e)
        {

        }
    }
}

