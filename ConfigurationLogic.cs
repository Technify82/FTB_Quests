using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class Configuration
    {
        
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
                UpdateTextBoxes(config.ProjectFolder);
                UseCache.Checked = config.UseCache;
            }
        }



        private void EnableControls()
        {
            foreach (Control ctrl in Controls)
            {
                ctrl.Enabled = true;
            }
        }


        public string GetMostRecentFile(string folderPath, string searchPattern)
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            var file = directoryInfo.GetFiles(searchPattern)
                                    .OrderByDescending(f => f.LastWriteTime)
                                    .FirstOrDefault();
            return file?.FullName;
        }

        public static string GetPath(string projectPath, string baseCacheDir, CheckBox UseCache)
        {
            if (UseCache.Checked)
            {
                return baseCacheDir;
            }
            else
            {
                return projectPath;
            }

        }

        private static void InitializeCache(ConfigProperties config, RichTextBox CacheInfoBox, ConfigManager configManager)
        {
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));

            if (Directory.Exists(baseCacheDir))
            {
                if (IsCacheStale(baseCacheDir))
                {
                    DialogResult result = MessageBox.Show("Cache is outdated. Overwrite?", "Cache", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        CachingSystem.EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);
                        CacheInfoBox.AppendText("Copying config locations to cache...\n");
                        CachingSystem.CopyConfigLocationsToCache(config, CacheInfoBox, configManager);
                        CacheInfoBox.AppendText("Caching completed.\n");
                    }
                }
            }
            else
            {
                CachingSystem.EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);
                CacheInfoBox.AppendText("Copying config locations to cache...\n");
                CachingSystem.CopyConfigLocationsToCache(config, CacheInfoBox, configManager);
                CacheInfoBox.AppendText("Caching completed.\n");
            }
        }

        private static bool IsCacheStale(string cachePath)
        {
            DirectoryInfo cacheDirInfo = new DirectoryInfo(cachePath);
            foreach (FileInfo file in cacheDirInfo.GetFiles())
            {
                if (file.LastWriteTime < DateTime.Now.AddDays(-7))
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateConfiguration()
        {
            SaveConfig(); // Save current settings
            PopulateTextBoxes(); // Reflect changes in the UI
            InitializeCache(config, CacheInfoBox, configManager); // Reinitialize cache if needed
            //RefreshComponents(); // Recreate or refresh components
        }
    }
}
