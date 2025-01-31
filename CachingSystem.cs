using System;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    internal class CachingSystem
    {


        public static void InitializeCaching(ConfigProperties config, RichTextBox CacheInfoBox)
        {
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));

        }

        public static void CopyConfigLocationsToCache(ConfigProperties config, RichTextBox CacheInfoBox)
        {
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));
            EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);
            DirectoryInfo cacheDirInfo = new DirectoryInfo(baseCacheDir);
            CacheInfoBox.AppendText($"Cleared cache folder: {baseCacheDir}\n");
            CopyDirectory(new DirectoryInfo(File.Exists(ConfigManager.Config.RecipeFile) ? Path.GetDirectoryName(ConfigManager.Config.RecipeFile) : ConfigManager.Config.RecipeFile), new DirectoryInfo(Path.Combine(baseCacheDir, "pmdumper")), CacheInfoBox);
            CopyDirectory(new DirectoryInfo(File.Exists(ConfigManager.Config.ItemPanelFile) ? Path.GetDirectoryName(ConfigManager.Config.ItemPanelFile) : ConfigManager.Config.ItemPanelFile), new DirectoryInfo(Path.Combine(baseCacheDir, "Dump")), CacheInfoBox);
            CacheInfoBox.AppendText("All config locations copied to cache.\n");
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
            }

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

        public static void DeleteCacheFolders(ConfigProperties config, RichTextBox CacheInfoBox)
        {
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(config.ProjectFolder));

            try
            {
                if (Directory.Exists(baseCacheDir))
                {
                    Directory.Delete(baseCacheDir, true);
                    CacheInfoBox.AppendText($"Cache directory deleted: {baseCacheDir}\n");
                }
                else
                {
                    CacheInfoBox.AppendText($"Cache directory does not exist: {baseCacheDir}\n");
                }
            }
            catch (Exception ex)
            {
                CacheInfoBox.AppendText($"Error deleting cache directory: {ex.Message}\n");
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
