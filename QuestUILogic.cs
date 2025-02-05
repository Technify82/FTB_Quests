using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FTB_Quests
{
    public partial class QuestUI
    {

        private void LoadQuestlines()
        {
            string questFolder = configManager.Config.QuestFolder;
            DirectoryInfo directoryInfo = new DirectoryInfo(questFolder);

            if (directoryInfo.Exists)
            {
                var validQuestFolders = directoryInfo.GetDirectories()
                                                     .Where(dir => DirectoryContainsValidSNBTFiles(dir))
                                                     .ToList();

                QuestList.Items.Clear();
                questFolderPaths.Clear();

                foreach (var dir in validQuestFolders)
                {
                    QuestList.Items.Add(dir.Name);
                    questFolderPaths[dir.Name] = dir.FullName;
                }
            }
        }

        private bool DirectoryContainsValidSNBTFiles(DirectoryInfo directoryInfo)
        {
            var excludedFiles = new HashSet<string> { "file.snbt", "index.snbt" };

            var snbtFiles = directoryInfo.GetFiles("*.snbt");

            if (snbtFiles.Length == 0)
            {
                return false;
            }

            foreach (var file in snbtFiles)
            {
                if (!excludedFiles.Contains(file.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
