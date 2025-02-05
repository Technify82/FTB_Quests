using System;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    internal class CachingSystem
    {

        public static void CopyConfigLocationsToCache(ConfigProperties config, RichTextBox CacheInfoBox, ConfigManager configManager)
        {
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));
            EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);
            DirectoryInfo cacheDirInfo = new DirectoryInfo(baseCacheDir);
            CopyDirectory(new DirectoryInfo(File.Exists(configManager.Config.RecipeFile) ? Path.GetDirectoryName(configManager.Config.RecipeFile) : configManager.Config.RecipeFile), new DirectoryInfo(Path.Combine(baseCacheDir, "pmdumper")), CacheInfoBox);
            CopyDirectory(new DirectoryInfo(File.Exists(configManager.Config.ItemPanelFile) ? Path.GetDirectoryName(configManager.Config.ItemPanelFile) : configManager.Config.ItemPanelFile), new DirectoryInfo(Path.Combine(baseCacheDir, "dumps")), CacheInfoBox);
        }

        private static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination, RichTextBox CacheInfoBox)
        {
            if (!source.Exists)
            {
                CacheInfoBox.AppendText($"Source directory does not exist: {source.FullName}\n");
                return;
            }

            if (!destination.Exists)
            {
                destination.Create();
                CacheInfoBox.AppendText($"Created destination directory: {destination.FullName}\n");

                foreach (FileInfo file in source.GetFiles())
                {
                    string targetFilePath = Path.Combine(destination.FullName, file.Name);
                    file.CopyTo(targetFilePath, true);
                    CacheInfoBox.AppendText($"Copied file: {file.FullName} to {targetFilePath}\n");
                }

                foreach (DirectoryInfo dir in source.GetDirectories())
                {
                    string targetDirPath = Path.Combine(destination.FullName, dir.Name);
                    DirectoryInfo targetDir = new DirectoryInfo(targetDirPath);
                    CopyDirectory(dir, targetDir, CacheInfoBox);
                }
            }
        }

        public static void EnsureCacheFolderExists(string cacheDir, RichTextBox CacheInfoBox)
        {
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
                CacheInfoBox.AppendText($"Cache directory created: {cacheDir}\n");
            }
            else
            {
                CacheInfoBox.AppendText($"Cache directory already exists at {cacheDir}\n");
            }
        }
    }
}
