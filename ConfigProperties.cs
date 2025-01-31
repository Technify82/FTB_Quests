using System;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    public class ConfigProperties
    {
        public string ProjectFolder { get; set; }
        public string RecipeFile { get; set; }
        public string ItemPanelFile { get; set; }
        public string ImageFolder { get; set; }
        public string QuestFolder { get; set; }
        public string OreDictionary { get; set; }
        public string DatabaseFile { get; set; }

        public string OriginalProjectFolder { get; set; }
        public string OriginalRecipeFile { get; set; }
        public string OriginalItemPanelFile { get; set; }
        public string OriginalImageFolder { get; set; }
        public string OriginalQuestFolder { get; set; }
        public string OriginalOreDictionary { get; set; }
        public string OriginalDatabaseFile { get; set; }

        public void SwitchToCachedPaths()
        {
            OriginalProjectFolder = ProjectFolder;
            OriginalRecipeFile = RecipeFile;
            OriginalItemPanelFile = ItemPanelFile;
            OriginalImageFolder = ImageFolder;
            OriginalQuestFolder = QuestFolder;
            OriginalOreDictionary = OreDictionary;
            OriginalDatabaseFile = DatabaseFile;

            RecipeFile = GetCachedPath(RecipeFile);
            ItemPanelFile = GetCachedPath(ItemPanelFile);
            ImageFolder = GetCachedPath(ImageFolder);
            QuestFolder = GetCachedPath(QuestFolder);
            OreDictionary = GetCachedPath(OreDictionary);
            DatabaseFile = GetCachedPath(DatabaseFile);
        }

        private string GetCachedPath(string originalPath)
        {
            if (string.IsNullOrWhiteSpace(originalPath))
            {
                return originalPath;
            }

            string baseCacheDir = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(ProjectFolder));
            string cachedPath = Path.Combine(baseCacheDir, Path.GetFileName(originalPath));
            return File.Exists(cachedPath) || Directory.Exists(cachedPath) ? cachedPath : originalPath;
        }

    }
}
