using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTB_Quests
{
    public class QuestBuilder
    {
        private readonly BuildQuests buildQuests;
        public string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";
        private readonly string questFolderPath = ConfigManager.Config.QuestFolder.ToString();

        public QuestBuilder(BuildQuests buildQuests)
        {
            this.buildQuests = buildQuests;
        }

        public void PopulateQuestBox(string filename)
        {
            buildQuests.QuestBox.Clear();
            string questFolderPath = ConfigManager.Config.QuestFolder.ToString();

            string[] files = Directory.GetFiles(questFolderPath, filename, SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                Console.WriteLine($"File not found: {filename}");
                return;
            }

            string selectedFilePath = files[0];

            if (File.Exists(selectedFilePath))
            {
                string[] lines = File.ReadAllLines(selectedFilePath);

                foreach (string line in lines)
                {
                    buildQuests.QuestBox.AppendText(line + Environment.NewLine);
                }

            }
            else
            {
                Console.WriteLine($"Selected file does not exist: {selectedFilePath}");
            }
        }

        public async Task PopulateQuestBoxRemoveDependenciesAsync(string filename)
        {
            buildQuests.QuestBox.Clear();
            string questFolderPath = ConfigManager.Config.QuestFolder.ToString();

            string[] files = Directory.GetFiles(questFolderPath, filename, SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                Console.WriteLine($"File not found: {filename}");
                return;
            }

            string selectedFilePath = files[0];

            if (File.Exists(selectedFilePath))
            {
                string[] lines;
                using (StreamReader reader = new StreamReader(selectedFilePath))
                {
                    string fileContent = await reader.ReadToEndAsync();
                    lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                }

                bool insideDependencies = false;
                StringBuilder questBoxContent = new StringBuilder();

                foreach (string line in lines)
                {
                    if (line.Contains("dependencies:"))
                    {
                        insideDependencies = true;
                        continue;
                    }

                    if (insideDependencies)
                    {
                        if (line.Contains("],"))
                        {
                            insideDependencies = false;
                        }
                        continue;
                    }

                    questBoxContent.AppendLine(line);
                }

                buildQuests.QuestBox.Text = questBoxContent.ToString();
            }
            else
            {
                Console.WriteLine($"Selected file does not exist: {selectedFilePath}");
            }
        }




        public void OutputQuestRecipeAndDependencies(string questName, bool includeDependencies)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Recipes WHERE Quests LIKE @QuestName";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QuestName", "%" + questName + "%");

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string questFileName = reader["Quests"].ToString();
                                List<string> ingredients = new List<string>
                        {
                            reader["A"].ToString(),
                            reader["B"].ToString(),
                            reader["C"].ToString(),
                            reader["D"].ToString(),
                            reader["E"].ToString(),
                            reader["F"].ToString(),
                            reader["G"].ToString(),
                            reader["H"].ToString(),
                            reader["I"].ToString()
                        };
                                if (includeDependencies)
                                {
                                    List<Tuple<string, string>> uidList = new List<Tuple<string, string>>();
                                    foreach (var ingredient in ingredients)
                                    {
                                        if (!string.IsNullOrEmpty(ingredient) && ingredient != "N/A")
                                        {
                                            ProcessIngredientForDependencies(ingredient, uidList);

                                        }
                                    }
                                    ExtractLinesToDictionary(connection, uidList);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("No matching recipes found.");
                        }
                    }
                }
            }
        }

        private void ProcessIngredientForDependencies(string ingredient, List<Tuple<string, string>> uidList)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT TaskUID, Quests 
            FROM Recipes 
            WHERE DisplayName = @DisplayName";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DisplayName", ingredient);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string taskUid = reader["TaskUID"].ToString();
                                string questFile = reader["Quests"].ToString();

              
                                if (questFile.EndsWith(".snbt"))
                                {
                                    questFile = questFile.Substring(0, questFile.Length - 5);
                                }

                                uidList.Add(new Tuple<string, string>(questFile, taskUid));
                          
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No matching quests found for ingredient: {ingredient}");
                            Application.DoEvents();
                        }
                    }
                }
            }
        }


        public string FindFileInSubfolders(string baseDirectory, string fileName)
        {
            string[] files = Directory.GetFiles(baseDirectory, fileName, SearchOption.AllDirectories);
            return files.Length > 0 ? files[0] : null;
        }

        public void SaveQuestBoxContent()
        {
            string filePath = FindFileInSubfolders(questFolderPath, buildQuests.treeView1.SelectedNode.Text.ToString());

            try
            {
                File.WriteAllText(filePath, buildQuests.QuestBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ExtractLinesToDictionary(SQLiteConnection connection, List<Tuple<string, string>> uidList)
        {
            string[] lines = buildQuests.QuestBox.Text.Split('\n');
            Dictionary<int, string> linesDict = new Dictionary<int, string>();
            bool insideDependencies = false;
            bool uidlistHasContents = false;
            int counter = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                linesDict.Add(i, lines[i].Trim());
            }

            buildQuests.QuestBox.Clear();
            buildQuests.DependencyBox.Clear();

            HashSet<string> uidSet = new HashSet<string>(uidList.Select(tuple => tuple.Item1).Where(uid => !lines.Contains(uid)).Select(uid => uid.Trim(new char[] { '"', ',', '\t', ' ' })));

            if (uidSet.Any())
            {
                uidlistHasContents = true;
            }

            bool dependenciesFound = false;
            foreach (string line in lines)
            {
                if (line.Contains("dependencies:"))
                {
                    dependenciesFound = true;
                    break;
                }
            }

            do
            {
                buildQuests.QuestBox.AppendText(lines[counter] + Environment.NewLine);
                counter++;
            }
            while (counter < 4);

            foreach (string line in lines)
            {
                if (lines[counter].Contains("dependencies:"))
                {
                    while (lines[counter].Contains("dependencies:"))
                    {
                        if (lines[counter].Contains("dependencies:") && !lines[counter].Contains("min_required_dependencies"))
                        {
                            insideDependencies = true;
                            Console.WriteLine("Entering dependencies group.");

                            while (insideDependencies)
                            {
                                if (lines[counter].Contains("],"))
                                {
                                    insideDependencies = false;
                                    buildQuests.QuestBox.AppendText(lines[counter] + Environment.NewLine);
                                    Application.DoEvents();
                                    counter++;
                                    continue;
                                }
                                else
                                {
                                    buildQuests.QuestBox.AppendText(lines[counter] + Environment.NewLine);
                                    counter++;
                                    if (lines[counter] != null)
                                    {
                                        string cleanedUid = lines[counter].Trim(new char[] { '"', ',', '\t', ' ' });
                                        if (IsValidUid(cleanedUid))
                                        {
                                            QueryUID(new Tuple<string, string>(cleanedUid, ""), connection);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            counter++;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            if (!dependenciesFound && uidlistHasContents)
            {
                InsertDependencies(uidList, connection);
            }

            for (int i = counter; i < lines.Length; i++)
            {
                buildQuests.QuestBox.AppendText(lines[i] + Environment.NewLine);
            }
        }

        private void InsertDependencies(List<Tuple<string, string>> uidList, SQLiteConnection connection)
        {
            HashSet<string> uidSet = new HashSet<string>(uidList.Select(tuple => tuple.Item1.TrimEnd(',')));

            HashSet<string> existingDependencies = new HashSet<string>();
            string[] existingLines = buildQuests.QuestBox.Text.Split('\n');
            bool insideDependencies = false;

            foreach (string line in existingLines)
            {
                if (line.Contains("dependencies:"))
                {
                    insideDependencies = true;
                    continue;
                }

                if (insideDependencies)
                {
                    if (line.Contains("],"))
                    {
                        insideDependencies = false;
                        continue;
                    }

                    string existingDependency = line.Trim(new char[] { '\t', '\"', ',' });
                    existingDependencies.Add(existingDependency);
                }
            }

            uidSet.ExceptWith(existingDependencies);

            if (uidSet.Count > 0)
            {
                int lastIndex = uidList.Count - 1;
                var lastTuple = uidList[lastIndex];
                string cleanedUid = lastTuple.Item1.TrimEnd(',');
                uidList[lastIndex] = new Tuple<string, string>(cleanedUid, lastTuple.Item2);
            }

            string openingText = "\tdependencies: [";
            string closingText = "\t],";

            buildQuests.QuestBox.AppendText(openingText + Environment.NewLine);

            foreach (var uid in uidSet)
            {
                QueryUID(new Tuple<string, string>(uid, ""), connection);
                buildQuests.QuestBox.AppendText($"\t\t\"{uid}\"," + Environment.NewLine);
                Application.DoEvents();
            }

            buildQuests.QuestBox.AppendText(closingText + Environment.NewLine);
            Application.DoEvents();
        }

        private bool IsValidUid(string uid)
        {
            return !string.IsNullOrEmpty(uid) && uid.Length == 8 && uid.All(char.IsLetterOrDigit);
        }

        private void QueryUID(Tuple<string, string> uidInfo, SQLiteConnection connection)
        {
            string questFile = uidInfo.Item1.Trim(new char[] { '"', ',', '\t', ' ' });

            string query = @"
                    SELECT DisplayName
                    FROM Recipes
                    WHERE Quests LIKE @QuestFile";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestFile", "%" + questFile + "%");

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string displayName = reader["DisplayName"].ToString();

                            buildQuests.DependencyBox.AppendText(displayName + Environment.NewLine);
                            Application.DoEvents();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Matching Quest Found.");
                    }
                }
            }
        }
    }
}