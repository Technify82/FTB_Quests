using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTB_Quests
{

    public class QuestTreeViewManager
    {
        private TreeView questTreeView;
        //private Dictionary<string, List<string>> displayNameCache;
        //private Dictionary<string, Image> imageCache;
        //public Dictionary<string, QuestItem> questItemsCache = new Dictionary<string, QuestItem>();

        public QuestTreeViewManager(TreeView treeView)
        {
            questTreeView = treeView;
            // displayNameCache = new Dictionary<string, List<string>>();
            //imageCache = new Dictionary<string, Image>();
            //questItemsCache = new Dictionary<string, QuestItem>();
        }

        public void InitializeTreeView(EventHandler doubleClickHandler)
        {
            questTreeView.DoubleClick += doubleClickHandler;
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

        //public Dictionary<string, List<string>> DisplayNameCache
        //{
        //    get { return displayNameCache; }
        //}

        //public void LoadFolderTree(string rootPath)
        //{
        //    DirectoryInfo rootDirectory = new DirectoryInfo(rootPath);
        //    TreeNode rootNode = new TreeNode(rootDirectory.Name)
        //    {
        //        Tag = rootDirectory
        //    };
        //    GetDirectories(rootDirectory, rootNode);
        //    questTreeView.Nodes.Add(rootNode);
        //}

        //private void GetDirectories(DirectoryInfo directoryInfo, TreeNode parentNode)
        //{
        //    int maxDepth = 1;

        //    if (maxDepth < 1)
        //        return;

        //    foreach (var directory in directoryInfo.GetDirectories())
        //    {
        //        TreeNode directoryNode = new TreeNode(directory.Name)
        //        {
        //            Tag = directory
        //        };
        //        parentNode.Nodes.Add(directoryNode);

        //        if (maxDepth - 1 > 1)
        //        {
        //            GetDirectories(directory, directoryNode);
        //        }
        //    }
        //}

        //public void PreloadData(string connectionString)
        //{
        //    var allQuests = GetQuestsForSelectedNode(connectionString);

        //    foreach (var questItem in allQuests)
        //    {
        //        if (!questItemsCache.ContainsKey(questItem.FileName))
        //        {
        //            questItemsCache[questItem.FileName] = questItem;
        //        }

        //        if (!displayNameCache.ContainsKey(questItem.FileName))
        //        {
        //            displayNameCache[questItem.FileName] = new List<string> { questItem.FileName };
        //        }
        //    }
        //}

        //public Image LoadQuestImage(string questFileName, string connectionString)
        //{
        //    Console.WriteLine($"LoadQuestImage called with questFileName: {questFileName}");

        //    List<string> displayNames = GetQuestDisplayNames(questFileName, connectionString);
        //    Image questImage = null;

        //    foreach (var displayName in displayNames)
        //    {
        //        string imagesDirectory = ConfigManager.ConfigProperties.ImageFolder;
        //        string imagePath = Path.Combine(imagesDirectory, Path.GetFileNameWithoutExtension(displayName) + ".png");

        //        Console.WriteLine($"Checking for image at: {imagePath}");

        //        if (File.Exists(imagePath))
        //        {
        //            try
        //            {
        //                Console.WriteLine($"Loading image from: {imagePath}");
        //                questImage = Image.FromFile(imagePath);
        //                Console.WriteLine("Image loaded successfully.");
        //                break; // Stop after loading the first valid image
        //            }
        //            catch (OutOfMemoryException)
        //            {
        //                Console.WriteLine($"Out of memory while loading image: {imagePath}");
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error loading image: {ex.Message}");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Image not found at: {imagePath}");
        //        }
        //    }

        //    if (questImage == null)
        //    {
        //        Console.WriteLine("No valid image found, loading default image.");
        //        questImage = GetEmbeddedImage("Woops.png");
        //    }

        //    return questImage;
        //}



        //public List<string> GetQuestDisplayNames(string questFileName, string connectionString)
        //{
        //    Console.WriteLine($"GetQuestDisplayNames called with questFileName: {questFileName}");

        //    if (displayNameCache.ContainsKey(questFileName))
        //    {
        //        Console.WriteLine($"Cache hit for questFileName: {questFileName}");
        //        return displayNameCache[questFileName];
        //    }

        //    List<string> displayNames = new List<string>();

        //    Console.WriteLine("Opening database connection...");
        //    using (var connection = new SQLiteConnection(connectionString))
        //    {
        //        connection.Open();
        //        Console.WriteLine("Database connection opened.");

        //        string query = @"
        //SELECT DisplayName
        //FROM Recipes
        //WHERE Quests LIKE @QuestFile";

        //        using (var command = new SQLiteCommand(query, connection))
        //        {
        //            Console.WriteLine("Executing query...");
        //            command.Parameters.AddWithValue("@QuestFile", "%" + questFileName + "%");

        //            using (var reader = command.ExecuteReader())
        //            {
        //                Console.WriteLine("Reading query results...");
        //                while (reader.Read())
        //                {
        //                    string displayName = reader["DisplayName"].ToString();
        //                    displayNames.Add(displayName);
        //                    Console.WriteLine($"Found displayName: {displayName}");
        //                }
        //            }
        //        }

        //        connection.Close();
        //        Console.WriteLine("Database connection closed.");
        //    }

        //    displayNameCache[questFileName] = displayNames;
        //    Console.WriteLine($"Added {displayNames.Count} display names to cache for questFileName: {questFileName}");
        //    return displayNames;
        //}



        //public Image GetEmbeddedImage(string imageName)
        //{
        //    var assembly = Assembly.GetExecutingAssembly();
        //    var resourceName = $"FTB_Quests.{imageName}";

        //    var resourceNames = assembly.GetManifestResourceNames();
        //    Console.WriteLine("Available resources:");
        //    foreach (var name in resourceNames)
        //    {
        //        Console.WriteLine(name);
        //    }

        //    if (Array.Exists(resourceNames, element => element.Equals(resourceName)))
        //    {
        //        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        //        {
        //            return Image.FromStream(stream);
        //        }
        //    }

        //    throw new ArgumentException($"Resource '{resourceName}' not found.");
        //}

        //public List<QuestItem> GetQuestsForSelectedNode(string connectionString)
        //{
        //    // Ensure the list is cleared each time the method is called
        //    List<QuestItem> selectedNodeQuests = new List<QuestItem>();

        //    TreeNode selectedNode = questTreeView.SelectedNode;
        //    if (selectedNode?.Tag is DirectoryInfo directoryInfo)
        //    {
        //        var snbtFiles = directoryInfo.GetFiles("*.snbt")
        //                                    .Select(f => f.Name)
        //                                    .ToList();

        //        if (snbtFiles.Count == 0)
        //        {
        //            Console.WriteLine("No .snbt files found in the selected directory.");
        //            return selectedNodeQuests;
        //        }

        //        using (var connection = new SQLiteConnection(connectionString))
        //        {
        //            connection.Open();

        //            // Batch size to avoid exceeding SQLite expression tree limit
        //            int batchSize = 100;
        //            for (int batchStart = 0; batchStart < snbtFiles.Count; batchStart += batchSize)
        //            {
        //                using (var command = new SQLiteCommand(connection))
        //                {
        //                    StringBuilder queryBuilder = new StringBuilder();
        //                    queryBuilder.Append("SELECT Quests, DisplayName FROM Recipes WHERE ");

        //                    int batchEnd = Math.Min(batchStart + batchSize, snbtFiles.Count);
        //                    for (int i = batchStart; i < batchEnd; i++)
        //                    {
        //                        string parameterName = "@QuestFile" + i;
        //                        if (i > batchStart)
        //                            queryBuilder.Append(" OR ");
        //                        queryBuilder.Append($"Quests LIKE {parameterName}");
        //                        command.Parameters.AddWithValue(parameterName, "%" + snbtFiles[i] + "%");
        //                    }

        //                    command.CommandText = queryBuilder.ToString();

        //                    using (var reader = command.ExecuteReader())
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            string questFile = reader["Quests"].ToString();
        //                            string displayName = reader["DisplayName"].ToString();

        //                            bool isBroken = string.IsNullOrEmpty(displayName);

        //                            Image questImage = null;
        //                            try
        //                            {
        //                                questImage = LoadQuestImage(questFile, connectionString);
        //                            }
        //                            catch (OutOfMemoryException)
        //                            {
        //                                Console.WriteLine($"Out of memory while loading image for quest: {questFile}");
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                Console.WriteLine($"Error loading image for quest: {ex.Message}");
        //                            }

        //                            QuestItem questItem = new QuestItem
        //                            {
        //                                FileName = questFile,
        //                                IsBroken = isBroken,
        //                                QuestImage = questImage
        //                            };

        //                            selectedNodeQuests.Add(questItem);

        //                            // Dispose image if it's no longer needed
        //                            if (questImage != null && questImage != questItem.QuestImage)
        //                            {
        //                                questImage.Dispose();
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            connection.Close();
        //        }
        //    }

        //    return selectedNodeQuests;
        //}





    }
}
