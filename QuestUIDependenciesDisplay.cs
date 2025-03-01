using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class QuestUI
    {
        private Dictionary<string, Dictionary<string, List<string>>> dependencies = new Dictionary<string, Dictionary<string, List<string>>>();
        //private Dictionary<string, Dictionary<string, List<string>>> dependencies = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, List<string>> questRecipes = new Dictionary<string, List<string>>(); // Holds quest and their corresponding recipes


        public void InitializeDependencies()
        {
            LoadDependenciesForSelectedIndex();
        }


        private void LoadDependenciesForSelectedIndex()
        {
            int selectedIndex = QuestList.SelectedIndex;
            if (selectedIndex >= 0)
            {
                string selectedFolderName = QuestList.SelectedItem.ToString();
                if (questFolderPaths.TryGetValue(selectedFolderName, out var selectedFolderPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(selectedFolderPath);
                    var snbtFiles = new List<string>();

                    foreach (var file in directoryInfo.GetFiles("*.snbt"))
                    {
                        snbtFiles.Add(file.Name);
                    }

                    foreach (var snbtFile in snbtFiles)
                    {
                        LoadAndStoreDependencies(snbtFile);
                    }
                }
            }
            else
            {
                Console.WriteLine("No valid selection in the ComboBox.");
            }
        }


        private void LoadAndStoreDependencies(string snbtFile)
        {
            // Simulate the data that would come from the database
            // Here, we use the questRecipes dictionary to hold recipe data for each quest
            if (questRecipes.TryGetValue(snbtFile, out var recipes))
            {
                foreach (var recipe in recipes)
                {
                    string displayName = recipe; // Simulate display name retrieval
                    if (!dependencies.ContainsKey(snbtFile))
                    {
                        dependencies[snbtFile] = new Dictionary<string, List<string>>();
                    }

                    foreach (string ingredient in recipe.Split(','))
                    {
                        if (!string.IsNullOrEmpty(ingredient))
                        {
                            if (!dependencies[snbtFile].ContainsKey(ingredient))
                            {
                                dependencies[snbtFile][ingredient] = new List<string>();
                            }
                            dependencies[snbtFile][ingredient].Add(displayName);
                        }
                    }
                }
            }

            SaveDependenciesGraphToFile();
        }


        private void DrawDependencies()
        {
            using (Graphics g = QuestPanel.CreateGraphics())
            {
                foreach (var snbtFile in dependencies)
                {
                    string item = snbtFile.Key;
                    foreach (var dependency in snbtFile.Value)
                    {
                        string ingredient = dependency.Key;
                        List<string> dependentQuests = dependency.Value;

                        foreach (string dependentQuest in dependentQuests)
                        {
                            string dependentQuestFile = null;
                            foreach (var file in dependencies)
                            {
                                if (file.Value.ContainsKey(dependentQuest))
                                {
                                    dependentQuestFile = file.Key;
                                    break;
                                }
                            }

                            if (dependentQuestFile != null)
                            {
                                PictureBox ingredientBox = FindPictureBoxByTag(item);
                                PictureBox dependentQuestBox = FindPictureBoxByTag(dependentQuestFile);

                                if (ingredientBox != null && dependentQuestBox != null)
                                {
                                    Point start = ingredientBox.Location + new Size(ingredientBox.Width / 2, ingredientBox.Height / 2);
                                    Point end = dependentQuestBox.Location + new Size(dependentQuestBox.Width / 2, dependentQuestBox.Height / 2);

                                    g.DrawLine(Pens.Black, start, end);
                                }
                            }
                        }
                    }
                }
            }
        }


        private PictureBox FindPictureBoxByTag(string tag)
        {
            foreach (Control control in QuestPanel.Controls)
            {
                if (control is PictureBox pictureBox && pictureBox.Tag.ToString() == tag)
                {
                    return pictureBox;
                }
            }
            return null;
        }


        private void SaveDependenciesGraphToFile()
        {
            string cacheFolderPath = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(configManager.Config.ProjectFolder));
            Directory.CreateDirectory(cacheFolderPath);
            string filePath = Path.Combine(cacheFolderPath, "dependencies_graph.txt");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Dependency Graph:");

                foreach (var snbtFile in dependencies.Keys)
                {
                    writer.WriteLine($"SNBT File: {snbtFile}");
                    foreach (var ingredient in dependencies[snbtFile].Keys)
                    {
                        writer.WriteLine($"Ingredient: {ingredient}");
                        WriteDependenciesToFile(writer, snbtFile, ingredient, 1, new HashSet<string>());
                    }
                }
            }

            Console.WriteLine($"Dependencies graph saved to {filePath}");
        }


        private void WriteDependenciesToFile(StreamWriter writer, string snbtFile, string ingredient, int level, HashSet<string> visited)
        {
            if (!dependencies.ContainsKey(snbtFile) || !dependencies[snbtFile].ContainsKey(ingredient) || visited.Contains(ingredient))
            {
                return;
            }

            visited.Add(ingredient);

            foreach (var dependent in dependencies[snbtFile][ingredient])
            {
                writer.Write(new string(' ', level * 2));
                writer.WriteLine($"|-- {dependent}");
                WriteDependenciesToFile(writer, snbtFile, dependent, level + 1, visited);
            }
        }
    }
}
