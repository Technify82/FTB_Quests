using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FTB_Quests
{

    public class QuestItem
    {
        public string FileName { get; set; }
        public bool IsBroken { get; set; }
        public Image QuestImage { get; set; }
    }

    public partial class QuestUI
    {
        private readonly Dictionary<string, QuestItem> questItemsCache = new Dictionary<string, QuestItem>();
        private readonly Dictionary<string, List<string>> displayNameCache = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, string> questFolderPaths = new Dictionary<string, string>();
        //ConfigManager configManager;

        private void DisplayQuestsInCanvas()
        {
            string connectionString = $"Data Source={configManager.Config.DatabaseFile};Version=3;";
            // configManager = ConfigManager.Instance;
            PreloadData(connectionString);
            var nonBrokenItems = FilterBrokenItems();
            QuestPanel.Controls.Clear();

            ToolTip toolTip = new ToolTip();

            var questItems = GetQuestsForSelectedIndex(connectionString, out int questItemCount);

            int maxGridItems = questItemCount + 10;

            int columnCount = 30;
            int totalItems = 0;
            var buffer = new List<Control>();

            foreach (var questItem in questItems)
            {
                var pictureBox = new PictureBox
                {
                    Image = questItem.QuestImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 32,
                    Height = 32,
                    Tag = questItem.FileName
                };

                if (displayNameCache.ContainsKey(questItem.FileName))
                {
                    var displayNames = displayNameCache[questItem.FileName];
                    if (displayNames.Count > 0)
                    {
                        toolTip.SetToolTip(pictureBox, displayNames[0]);
                    }
                }

                pictureBox.MouseDown += PictureBox_MouseDown;
                pictureBox.MouseMove += PictureBox_MouseMove;



                int row = totalItems / columnCount;
                int column = totalItems % columnCount;

                pictureBox.Location = new Point(column * 40, row * 40);
                buffer.Add(pictureBox);
                totalItems++;

                if (buffer.Count >= 100)
                {
                    AddControlsToCanvas(buffer);
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
            {
                AddControlsToCanvas(buffer);
            }

            while (totalItems < maxGridItems)
            {
                int row = totalItems / columnCount;
                int column = totalItems % columnCount;
                var pictureBox = new PictureBox
                {
                    Image = GetEmbeddedImage("Woops.png"),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 32,
                    Height = 32
                };

                pictureBox.MouseDown += PictureBox_MouseDown;
                pictureBox.MouseMove += PictureBox_MouseMove;


                pictureBox.Location = new Point(column * 40, row * 40);
                buffer.Add(pictureBox);
                totalItems++;

                if (buffer.Count >= 100)
                {
                    AddControlsToCanvas(buffer);
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
            {
                AddControlsToCanvas(buffer);
            }
        }

        private List<QuestItem> FilterBrokenItems()
        {
            return questItemsCache.Values.Where(item => !item.IsBroken).ToList();
        }

        public void PreloadData(string connectionString)
        {
            var allQuests = GetQuestsForSelectedIndex(connectionString, out _);

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

        public List<QuestItem> GetQuestsForSelectedIndex(string connectionString, out int questItemCount)
        {
            List<QuestItem> selectedNodeQuests = new List<QuestItem>();

            int selectedIndex = QuestList.SelectedIndex;
            if (selectedIndex >= 0)
            {
                var selectedFolderName = QuestList.SelectedItem.ToString();
                if (questFolderPaths.TryGetValue(selectedFolderName, out var selectedFolderPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(selectedFolderPath);

                    if (directoryInfo != null)
                    {
                        var snbtFiles = directoryInfo.GetFiles("*.snbt")
                                                    .Select(f => f.Name)
                                                    .ToList();
                        if (snbtFiles.Count == 0)
                        {
                            questItemCount = 0;
                            return selectedNodeQuests;
                        }

                        using (var connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            int batchSize = 100;
                            for (int batchStart = 0; batchStart < snbtFiles.Count; batchStart += batchSize)
                            {
                                using (var command = new SQLiteCommand(connection))
                                {
                                    StringBuilder queryBuilder = new StringBuilder();
                                    queryBuilder.Append("SELECT Quests, DisplayName FROM Recipes WHERE ");

                                    int batchEnd = Math.Min(batchStart + batchSize, snbtFiles.Count);
                                    for (int i = batchStart; i < batchEnd; i++)
                                    {
                                        string parameterName = "@QuestFile" + i;
                                        if (i > batchStart)
                                            queryBuilder.Append(" OR ");
                                        queryBuilder.Append($"Quests LIKE {parameterName}");
                                        command.Parameters.AddWithValue(parameterName, "%" + snbtFiles[i] + "%");
                                    }

                                    command.CommandText = queryBuilder.ToString();
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string questFile = reader["Quests"].ToString();
                                            string displayName = reader["DisplayName"].ToString();
                                            bool isBroken = string.IsNullOrEmpty(displayName);

                                            Image questImage = null;
                                            try
                                            {
                                                questImage = LoadQuestImage(questFile, connectionString);
                                            }
                                            catch (OutOfMemoryException)
                                            {
                                                Console.WriteLine($"Out of memory while loading image for quest: {questFile}");
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine($"Error loading image for quest: {ex.Message}");
                                            }

                                            QuestItem questItem = new QuestItem
                                            {
                                                FileName = questFile,
                                                IsBroken = isBroken,
                                                QuestImage = questImage
                                            };

                                            selectedNodeQuests.Add(questItem);

                                            if (questImage != null && questImage != questItem.QuestImage)
                                            {
                                                questImage.Dispose();
                                            }
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No valid selection in the ComboBox.");
            }

            questItemCount = selectedNodeQuests.Count;
            return selectedNodeQuests;
        }

        public Image LoadQuestImage(string questFileName, string connectionString)
        {
            List<string> displayNames = GetQuestDisplayNames(questFileName, connectionString);
            Image questImage = null;

            foreach (var displayName in displayNames)
            {
                string imagesDirectory = configManager.Config.ImageFolder;
                string imagePath = Path.Combine(imagesDirectory, Path.GetFileNameWithoutExtension(displayName) + ".png");

                if (File.Exists(imagePath))
                {
                    try
                    {
                        questImage = Image.FromFile(imagePath);
                        break;
                    }
                    catch (OutOfMemoryException)
                    {
                        Console.WriteLine($"Out of memory while loading image: {imagePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading image: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Image not found at: {imagePath}");
                }
            }

            if (questImage == null)
            {
                questImage = GetEmbeddedImage("Woops.png");
            }

            return questImage;
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

        private bool DirectoryContainsSNBTFiles(DirectoryInfo directoryInfo)
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

        private Image GetEmbeddedImage(string imageName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = $"FTB_Quests.{imageName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"Resource '{resourceName}' not found.");
                }
                return Image.FromStream(stream);
            }
        }
    }
}
