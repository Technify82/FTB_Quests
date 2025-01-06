
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FTB_Quests
{
    public class QuestLinker
    {
        private readonly MainForm form;
        private readonly Dictionary<string, string> fileMap = new Dictionary<string, string>();
        public string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";

        public QuestLinker(MainForm form)
        {
            this.form = form;
        }

        public void QuestDirectoryScan()
        {
            var overallStartTime = DateTime.Now;

            form.toolStripStatusLabel1.Text = $"{overallStartTime}: QuestDirectoryScan Start";
            Application.DoEvents();

            bool columnExists = CheckIfColumnExists("Quests");

            string folderPath = ConfigManager.Config.QuestFolder;

            if (Directory.Exists(folderPath))
            {
                var maps = ReadDirectories(folderPath, columnExists);
                var itemIdToQuestMap = maps.Item1;
                var uidToQuestMap = maps.Item2;

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        form.toolStripProgressBar2.Value = 0;
                        form.toolStripProgressBar2.Maximum = itemIdToQuestMap.Count + uidToQuestMap.Count; // Adjust progress bar maximum

                        var dbUpdateStartTime = DateTime.Now;
                        BatchUpdateDatabase(itemIdToQuestMap, uidToQuestMap, connection, transaction);
                        var dbUpdateEndTime = DateTime.Now;
                        Console.WriteLine($"Database update execution time: {(dbUpdateEndTime - dbUpdateStartTime).TotalMilliseconds} ms");

                        transaction.Commit();
                    }
                }
            }

            var overallEndTime = DateTime.Now;
            Console.WriteLine($"Overall QuestDirectoryScan execution time: {(overallEndTime - overallStartTime).TotalMilliseconds} ms");

            form.toolStripStatusLabel1.Text = $"{overallEndTime}: QuestDirectoryScan Done";
            Application.DoEvents();
        }


        private bool CheckIfColumnExists(string columnName)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = $"PRAGMA table_info(Recipes)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["name"].ToString().Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private Tuple<Dictionary<string, List<string>>, Dictionary<string, List<string>>> ReadDirectories(string folderPath, bool columnExists)
        {
            string[] folderArray = Directory.GetDirectories(folderPath);
            int folderCount = folderArray.Length;

            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = folderCount;

            var itemIdToQuestMap = new Dictionary<string, List<string>>();
            var uidToQuestMap = new Dictionary<string, List<string>>();

            var directoryScanStartTime = DateTime.Now;
            Console.WriteLine("Starting directory scanning.");

            foreach (var folder in folderArray)
            {
                try
                {
                    Console.WriteLine($"Processing folder: {folder}");
                    if (Directory.Exists(folder))
                    {
                        string[] files = Directory.GetFiles(folder);
                        form.toolStripProgressBar2.Maximum = files.Length;

                        foreach (var file in files)
                        {
                            var fileReadStartTime = DateTime.Now;
                            string fileContent = File.ReadAllText(file);
                            var fileReadEndTime = DateTime.Now;
                            Console.WriteLine($"File read execution time for {file}: {(fileReadEndTime - fileReadStartTime).TotalMilliseconds} ms");

                            if (!string.IsNullOrEmpty(fileContent))
                            {
                                string filename = Path.GetFileName(file);
                                fileMap[filename] = file;

                                if (columnExists)
                                {
                                    ProcessFileContent(fileContent, filename, itemIdToQuestMap, uidToQuestMap);
                                }
                            }

                            if (form.toolStripProgressBar2.Value < form.toolStripProgressBar2.Maximum &&
                                form.toolStripProgressBar2.Value % 10 == 0)
                            {
                                form.toolStripProgressBar2.Value++;
                            }
                        }
                    }

                    if (form.toolStripProgressBar1.Value < form.toolStripProgressBar1.Maximum)
                    {
                        form.toolStripProgressBar1.Value++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing folder {folder}: {ex.Message}");
                }
            }

            var directoryScanEndTime = DateTime.Now;
            Console.WriteLine($"Directory scan execution time: {(directoryScanEndTime - directoryScanStartTime).TotalMilliseconds} ms");

            // Return both dictionaries as a tuple
            return Tuple.Create(itemIdToQuestMap, uidToQuestMap);
        }


        private void ProcessFileContent(string fileContent, string filename, Dictionary<string, List<string>> itemIdToQuestMap, Dictionary<string, List<string>> uidToQuestMap)
        {
            //Console.WriteLine($"Processing file content for file: {filename}");
            string[] lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            bool insideTasks = false;

            foreach (string line in lines)
            {
                if (line.Contains("id:"))
                {
                    string itemId = ExtractItemNameWithDamage(line);

                    if (itemId != null)
                    {
                        //Console.WriteLine($"Found itemId: {itemId} in file: {filename}");
                        if (!itemIdToQuestMap.ContainsKey(itemId))
                        {
                            itemIdToQuestMap[itemId] = new List<string>();
                        }
                        itemIdToQuestMap[itemId].Add(filename);
                        // Console.WriteLine($"Added questName: {filename} for itemId: {itemId}");
                    }
                }

                // Check for the start of the tasks block
                if (line.Contains("tasks:"))
                {
                    insideTasks = true;
                    continue; // Move to the next line to get the uid
                }

                // If inside the tasks block, look for the uid
                if (insideTasks)
                {
                    var uidRegex = new Regex(@"uid:\s*""([^""]+)""");
                    Match uidMatch = uidRegex.Match(line);

                    if (uidMatch.Success)
                    {
                        string uid = uidMatch.Groups[1].Value;
                        // Console.WriteLine($"Found task uid: {uid} in file: {filename}");
                        if (!uidToQuestMap.ContainsKey(uid))
                        {
                            uidToQuestMap[uid] = new List<string>();
                        }
                        uidToQuestMap[uid].Add(filename);
                        // Console.WriteLine($"Mapped task uid: {uid} to quest file: {filename}");
                        insideTasks = false; // Exit the tasks block after finding the uid
                    }
                }

                // Reset insideTasks flag if we encounter the end of tasks block
                if (line.Contains("}]"))
                {
                    insideTasks = false;
                }
            }
        }


        private void BatchUpdateDatabase(Dictionary<string, List<string>> itemIdToQuestMap, Dictionary<string, List<string>> uidToQuestMap, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            var batchUpdateStartTime = DateTime.Now;
            const int batchSize = 50; // Adjust the batch size as needed
            var commandText = new StringBuilder();
            var parameters = new List<SQLiteParameter>();

            int currentBatchSize = 0;

            // Process itemIdToQuestMap
            foreach (var entry in itemIdToQuestMap)
            {
                string itemId = entry.Key;
                var questNames = entry.Value;

                foreach (var questName in questNames)
                {
                    commandText.AppendLine($"UPDATE Recipes SET Quests = @Quests{currentBatchSize} WHERE ItemName = @ItemName{currentBatchSize};");
                    parameters.Add(new SQLiteParameter($"@Quests{currentBatchSize}", questName));
                    parameters.Add(new SQLiteParameter($"@ItemName{currentBatchSize}", itemId));
                    // Console.WriteLine($"Preparing to update database for itemId: {itemId} with questName: {questName}");

                    currentBatchSize++;

                    if (currentBatchSize >= batchSize)
                    {
                        ExecuteBatchUpdate(connection, transaction, commandText, parameters);
                        commandText.Clear();
                        parameters.Clear();
                        currentBatchSize = 0;
                    }
                }
            }

            // Process uidToQuestMap
            foreach (var entry in uidToQuestMap)
            {
                string uid = entry.Key;
                var questNames = entry.Value;

                foreach (var questName in questNames)
                {
                    commandText.AppendLine($"UPDATE Recipes SET TaskUID = @TaskUID{currentBatchSize} WHERE Quests = @Quests{currentBatchSize};");
                    parameters.Add(new SQLiteParameter($"@TaskUID{currentBatchSize}", uid));
                    parameters.Add(new SQLiteParameter($"@Quests{currentBatchSize}", questName));
                    //   Console.WriteLine($"Preparing to update database for task uid: {uid} with questName: {questName}");

                    currentBatchSize++;

                    if (currentBatchSize >= batchSize)
                    {
                        ExecuteBatchUpdate(connection, transaction, commandText, parameters);
                        commandText.Clear();
                        parameters.Clear();
                        currentBatchSize = 0;
                    }
                }
            }

            // Execute any remaining updates
            if (currentBatchSize > 0)
            {
                ExecuteBatchUpdate(connection, transaction, commandText, parameters);
            }

            var batchUpdateEndTime = DateTime.Now;
            Console.WriteLine($"Batch database update execution time: {(batchUpdateEndTime - batchUpdateStartTime).TotalMilliseconds} ms");
        }


        private void ExecuteBatchUpdate(SQLiteConnection connection, SQLiteTransaction transaction, StringBuilder commandText, List<SQLiteParameter> parameters)
        {
            using (var command = new SQLiteCommand(commandText.ToString(), connection, transaction))
            {
                command.Parameters.AddRange(parameters.ToArray());
                command.ExecuteNonQuery();
            }
        }

        public void CheckItemInDatabase()
        {
            string[] itemNameLines = form.RecipeTextDetails.Lines;

            bool itemExists = false;
            string itemData = "";
            foreach (string line in itemNameLines)
            {
                if (line.Contains("id:"))
                {
                    string itemId = ExtractItemId(line);

                    if (itemId != null)
                    {
                        using (var connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();
                            string query = "SELECT ItemName FROM Recipes WHERE ItemName = @ItemName";

                            using (var command = new SQLiteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ItemName", itemId);

                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        if (!reader.IsDBNull(0))
                                        {
                                            itemExists = true;
                                            itemData = reader.GetString(0);
                                        }
                                    }
                                }
                            }
                        }
                        if (itemExists)
                        {
                            break;
                        }
                    }
                }
            }

            if (itemExists)
            {
                Console.WriteLine("yes");
                Console.WriteLine(itemData);
            }
            else
            {
                Console.WriteLine("no");
            }
        }

        public static string ExtractItemId(string description)
        {
            string cleanedDescription = description.Replace("\\\"", "\"");

            int idIndex = cleanedDescription.IndexOf("id:");

            if (idIndex != -1)
            {
                int startIndex = idIndex + "id:".Length;

                int endIndex = cleanedDescription.IndexOf(',', startIndex);

                if (endIndex != -1)
                {
                    string itemId = cleanedDescription.Substring(startIndex, endIndex - startIndex).Trim();

                    if (itemId.StartsWith("\"") && itemId.EndsWith("\""))
                    {
                        itemId = itemId.Substring(1, itemId.Length - 2);
                    }

                    return itemId;
                }
            }

            return null;
        }

        public string ExtractItemNameWithDamage(string description)
        {
            string cleanedDescription = description.Replace("\\\"", "\"");

            string itemId = ExtractItemId(cleanedDescription);

            string damage = ExtractDamage(cleanedDescription);

            if (itemId != null && damage != null && damage != "0")
            {
                return $"{itemId}:{damage}";
            }

            return itemId;
        }

        public string ExtractDamage(string description)
        {
            string pattern = "Damage:(\\d+)s";
            Match match = Regex.Match(description, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
    }
}