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

    public partial class QuestUI
    {
       // private readonly Dictionary<string, QuestItem> questItemsCache = new Dictionary<string, QuestItem>();
        private readonly Dictionary<string, List<string>> displayNameCache = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, string> questFolderPaths = new Dictionary<string, string>();
        
        private void DisplayQuestsInCanvas()
        {
            QuestPanel.Controls.Clear();
            ToolTip toolTip = new ToolTip();

            var questItems = GetQuestsForSelectedIndex(out int questItemCount);

            int maxGridItems = questItemCount + 10;
            int columnCount = 30;
            int totalItems = 0;
            var buffer = new List<Control>();

            foreach (var questItem in questItems)
            {
                if (!questItem.IsBroken)       
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

        //private List<QuestItem> FilterBrokenItems()
        //{
        //    return questItemsCache.Values.Where(item => !item.IsBroken).ToList();
        //}


        public List<QuestItem> GetQuestsForSelectedIndex(out int questItemCount)
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

                        foreach (var fileName in snbtFiles)
                        {
                            string questFile = fileName;
                            string displayName = fileName;     
                            bool isBroken = string.IsNullOrEmpty(displayName);

                            Image questImage = null;
                            try
                            {
                                questImage = LoadQuestImage(questFile);
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
            else
            {
                Console.WriteLine("No valid selection in the ComboBox.");
            }

            questItemCount = selectedNodeQuests.Count;
            return selectedNodeQuests;
        }


        public Image LoadQuestImage(string questFileName)
        {
            List<string> displayNames = GetQuestDisplayNames(questFileName);
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


        public List<string> GetQuestDisplayNames(string questFileName)
        {
            if (displayNameCache.ContainsKey(questFileName))
            {
                return displayNameCache[questFileName];
            }

            List<string> displayNames = new List<string>();

            displayNames.Add(questFileName);           

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
