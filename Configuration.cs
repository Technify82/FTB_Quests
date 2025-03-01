using System;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class Configuration : Form
    {
        public ConfigProperties config;
        readonly ConfigManager configManager;
        private readonly MainForm mainForm;
        private readonly Zip zip;

        public Configuration(MainForm mainForm, Zip zip)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.zip = zip;         
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

        private void UseCache_CheckedChanged(object sender, EventArgs e)
        {
            string method = "UseCache";

            if (UseCache.Checked)
            {
                string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));
                CachingSystem.EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);

                CacheInfoBox.AppendText("Copying config locations to cache...\n");
                CachingSystem.CopyConfigLocationsToCache(config, CacheInfoBox, configManager);

                UpdateTextBoxesAndConfigForCache(baseCacheDir);
            }
            else
            {
                CacheInfoBox.AppendText("Cache is turned off.\n");
            }

            configManager.Config.UseCache = UseCache.Checked;
            SaveConfig(method);     
        }

        private void SaveCache_Click(object sender, EventArgs e)
        {
            string cacheDir = Path.Combine(Environment.CurrentDirectory, "Cache");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string tempZipFilePath = Path.Combine(Environment.CurrentDirectory, $"TempCache_{timestamp}.zip");
            string finalZipFilePath = Path.Combine(cacheDir, $"Cache_{timestamp}.zip");

            zip.CompressDirectory(cacheDir, tempZipFilePath, CacheInfoBox);

            if (File.Exists(tempZipFilePath))
            {
                File.Move(tempZipFilePath, finalZipFilePath);
                CacheInfoBox.AppendText($"Cache directory zipped to: {finalZipFilePath}\n");
            }
        }
    }
}

