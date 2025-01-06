using System.Reflection;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace FTB_Quests
{
    public class QuestItem
    {
        public string FileName { get; set; }
        public bool IsBroken { get; set; }
        public Image QuestImage { get; set; }
    }

    public class QuestTreeViewManager
    {
        private TreeView questTreeView;
        private Dictionary<string, List<string>> displayNameCache;
        private Dictionary<string, Image> imageCache;
        public Dictionary<string, QuestItem> questItemsCache = new Dictionary<string, QuestItem>();

        public QuestTreeViewManager(TreeView treeView)
        {
            questTreeView = treeView;
            displayNameCache = new Dictionary<string, List<string>>();
            imageCache = new Dictionary<string, Image>();
            questItemsCache = new Dictionary<string, QuestItem>();
        }

        public void InitializeTreeView(EventHandler doubleClickHandler)
        {
            questTreeView.DoubleClick += doubleClickHandler;
        }

        public Dictionary<string, List<string>> DisplayNameCache
        {
            get { return displayNameCache; }
        }

        public void LoadFolderTree(string rootPath)
        {
            DirectoryInfo rootDirectory = new DirectoryInfo(rootPath);
            TreeNode rootNode = new TreeNode(rootDirectory.Name)
            {
                Tag = rootDirectory
            };
            GetDirectories(rootDirectory, rootNode);
            questTreeView.Nodes.Add(rootNode);
        }

        public void LoadFileTree(string rootPath)
        {
            DirectoryInfo rootDirectory = new DirectoryInfo(rootPath);
            TreeNode rootNode = new TreeNode(rootDirectory.Name)
            {
                Tag = rootDirectory
            };
            GetFiles(rootDirectory, rootNode);
            questTreeView.Nodes.Add(rootNode);
        }

        private void GetDirectories(DirectoryInfo directoryInfo, TreeNode parentNode)
        {
            int maxDepth = 1;

            if (maxDepth < 1)
                return;

            foreach (var directory in directoryInfo.GetDirectories())
            {
                TreeNode directoryNode = new TreeNode(directory.Name)
                {
                    Tag = directory
                };
                parentNode.Nodes.Add(directoryNode);

                if (maxDepth - 1 > 1)
                {
                    GetDirectories(directory, directoryNode);
                }
            }
        }

        private void GetFiles(DirectoryInfo directoryInfo, TreeNode parentNode)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                TreeNode directoryNode = new TreeNode(directory.Name)
                {
                    Tag = directory
                };
                parentNode.Nodes.Add(directoryNode);

                GetFiles(directory, directoryNode);      
            }

            foreach (var file in directoryInfo.GetFiles("*.snbt"))
            {
                TreeNode fileNode = new TreeNode(file.Name)
                {
                    Tag = file
                };
                parentNode.Nodes.Add(fileNode);
            }
        }


        public void FilterNodes(TreeNodeCollection nodes, Action<TreeNode> action)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.EndsWith(".snbt") && !node.Text.Contains("index"))
                {
                    action(node);
                }

                if (node.Nodes.Count > 0)
                {
                    FilterNodes(node.Nodes, action);
                }
            }
        }

        public async Task FilterNodesAsync(TreeNodeCollection nodes, Func<TreeNode, Task> action)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.EndsWith(".snbt") && !node.Text.Contains("index"))
                {
                    await action(node);
                }

                if (node.Nodes.Count > 0)
                {
                    await FilterNodesAsync(node.Nodes, action);
                }
            }
        }

        public void PreloadData(string connectionString)
        {
            var allQuests = GetAllQuests(connectionString);

            foreach (var questItem in allQuests)
            {
                if (!questItemsCache.ContainsKey(questItem.FileName))
                {
                    questItemsCache[questItem.FileName] = questItem;
                }

                if (!displayNameCache.ContainsKey(questItem.FileName))
                {
                    displayNameCache[questItem.FileName] = new List<string> { questItem.FileName };
                }
            }
        }

        public Image LoadQuestImage(string questFileName, string connectionString)
        {
            List<string> displayNames = GetQuestDisplayNames(questFileName, connectionString);

            foreach (var displayName in displayNames)
            {
                string imagesDirectory = ConfigManager.Config.ImageFolder;
                string imagePath = Path.Combine(imagesDirectory, Path.GetFileNameWithoutExtension(displayName) + ".png");

                if (File.Exists(imagePath))
                {
                    return Image.FromFile(imagePath);
                }
            }

            return GetEmbeddedImage("Woops.png");
        }

        public List<string> GetQuestDisplayNames(string questFileName, string connectionString)
        {
            if (displayNameCache.ContainsKey(questFileName))
            {
                return displayNameCache[questFileName];
            }

            List<string> displayNames = new List<string>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = @"
                SELECT DisplayName
                FROM Recipes
                WHERE Quests LIKE @QuestFile";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QuestFile", "%" + questFileName + "%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string displayName = reader["DisplayName"].ToString();
                            displayNames.Add(displayName);
                        }
                    }
                }

                connection.Close();
            }

            displayNameCache[questFileName] = displayNames;
            return displayNames;
        }


        public Image GetEmbeddedImage(string imageName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"FTB_Quests.{imageName}";

            var resourceNames = assembly.GetManifestResourceNames();
            Console.WriteLine("Available resources:");
            foreach (var name in resourceNames)
            {
                Console.WriteLine(name);
            }

            if (Array.Exists(resourceNames, element => element.Equals(resourceName)))
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    return Image.FromStream(stream);
                }
            }

            throw new ArgumentException($"Resource '{resourceName}' not found.");
        }

        public List<QuestItem> GetAllQuests(string connectionString)
        {
            List<QuestItem> allQuests = new List<QuestItem>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT Quests, DisplayName
            FROM Recipes";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string questFile = reader["Quests"].ToString();
                            string displayName = reader["DisplayName"].ToString();

                            bool isBroken = string.IsNullOrEmpty(displayName);

                            QuestItem questItem = new QuestItem
                            {
                                FileName = questFile,
                                IsBroken = isBroken,
                                QuestImage = LoadQuestImage(questFile, connectionString)
                            };

                            allQuests.Add(questItem);
                        }
                    }
                }

                connection.Close();
            }

            return allQuests;
        }

    }
}
