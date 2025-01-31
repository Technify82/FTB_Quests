using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class QuestUI
    {
        private Dictionary<string, Dictionary<string, List<string>>> dependencies = new Dictionary<string, Dictionary<string, List<string>>>();


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
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT A, B, C, D, E, F, G, H, I, DisplayName
            FROM Recipes
            WHERE Quests LIKE @Quest";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Quest", "%" + snbtFile + "%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string displayName = reader["DisplayName"].ToString();
                            if (!dependencies.ContainsKey(snbtFile))
                            {
                                dependencies[snbtFile] = new Dictionary<string, List<string>>();
                            }

                            foreach (string column in new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" })
                            {
                                string ingredient = reader[column].ToString();
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
                }
                connection.Close();
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
            string cacheFolderPath = Path.Combine(Environment.CurrentDirectory, "Cache", Path.GetFileName(ConfigManager.Config.ProjectFolder));
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
