using System;
using System.IO;
using System.Windows.Forms;
using System.Timers; // Explicitly include System.Timers

namespace FTB_Quests
{
    internal class CachingSystem
    {
        private static System.Timers.Timer cacheTimer;

        public static void InitializeCaching(string projectFolder, RichTextBox CacheInfoBox)
        {
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", projectFolder);
            EnsureCacheFolderExists(baseCacheDir, CacheInfoBox);
            PerformCaching(projectFolder, CacheInfoBox); // Perform initial caching
        }

        static void EnsureCacheFolderExists(string cacheDir, RichTextBox CacheInfoBox)
        {
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
                Console.WriteLine($"Cache directory created at {cacheDir}");

                // Append to RichTextBox
                CacheInfoBox.AppendText($"Cache directory created: {cacheDir}\n");
            }
            else
            {
                Console.WriteLine($"Cache directory already exists at {cacheDir}");
            }
        }

        public static void ScanProjectFilesForChanges(string projectFolder, RichTextBox CacheInfoBox)
        {
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", projectFolder);

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = baseCacheDir,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.*"
            };

            watcher.Changed += (source, e) => OnChanged(source, e, CacheInfoBox);
            watcher.Created += (source, e) => OnChanged(source, e, CacheInfoBox);
            watcher.Deleted += (source, e) => OnChanged(source, e, CacheInfoBox);
            watcher.Renamed += (source, e) => OnRenamed(source, e, CacheInfoBox);

            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e, RichTextBox CacheInfoBox)
        {
            CacheInfoBox.Invoke(new Action(() => CacheInfoBox.AppendText($"File cached: {e.FullPath}\n")));
        }

        private static void OnRenamed(object source, RenamedEventArgs e, RichTextBox CacheInfoBox)
        {
            CacheInfoBox.Invoke(new Action(() => CacheInfoBox.AppendText($"File cached: {e.FullPath}\n")));
        }

        public static void StartCacheTimer(string projectFolder, RichTextBox CacheInfoBox, double interval)
        {
            cacheTimer = new System.Timers.Timer(interval);
            cacheTimer.Elapsed += (sender, e) => PerformCaching(projectFolder, CacheInfoBox);
            cacheTimer.AutoReset = true;
            cacheTimer.Enabled = true;
        }

        private static void PerformCaching(string projectFolder, RichTextBox CacheInfoBox)
        {
            // Implement caching logic here
            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", projectFolder);
            CacheInfoBox.Invoke(new Action(() => CacheInfoBox.AppendText($"Performing caching for {projectFolder}...\n")));

            // Example caching logic: Copying files from project folder to cache folder
            string projectDir = ConfigManager.Config.ProjectFolder; // Assume projectFolder is the full path
            foreach (var file in Directory.GetFiles(projectDir))
            {
                string destFile = Path.Combine(baseCacheDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
                CacheInfoBox.Invoke(new Action(() => CacheInfoBox.AppendText($"Cached file: {file}\n")));
            }
        }
    }
}
