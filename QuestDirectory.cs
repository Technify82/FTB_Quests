
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FTB_Quests
{
    public class QuestLinker
    {
        private readonly MainForm form;
        private readonly Dictionary<string, string> fileMap = new Dictionary<string, string>();
        public List<ProjectProperties> projectProperties;
        public List<Potions> potions;
        ConfigManager configManager;

        public QuestLinker(MainForm form, List<ProjectProperties> projectProperties, List<Potions> potionProperties)
        {
            this.form = form;
            this.projectProperties = projectProperties;
            this.potions = potions;
            configManager = ConfigManager.Instance;

        }

        public void QuestDirectoryScan()
        {
            form.toolStripStatusLabel1.Text = "QuestDirectoryScan Start";
            Application.DoEvents();

            string folderPath = configManager.Config.QuestFolder;

            if (Directory.Exists(folderPath))
            {
                var maps = ReadDirectories(folderPath);
                var itemIdToQuestMap = maps.Item1;
                var uidToQuestMap = maps.Item2;

                foreach (var entry in itemIdToQuestMap)
                {
                    string itemId = entry.Key;
                    var questNames = entry.Value;

                    foreach (var questName in questNames)
                    {
                        foreach (var project in projectProperties)
                        {
                            if (project.ItemName == itemId)
                            {
                                project.Quests = questName;
                            }
                        }
                    }
                }

                foreach (var entry in uidToQuestMap)
                {
                    string uid = entry.Key;
                    var questNames = entry.Value;

                    foreach (var questName in questNames)
                    {
                        foreach (var project in projectProperties)
                        {
                            if (project.Quests == questName)
                            {
                                project.TaskUID = uid;
                            }
                        }
                    }
                }
                form.toolStripProgressBar2.Value = 0;
                form.toolStripProgressBar2.Maximum = itemIdToQuestMap.Count + uidToQuestMap.Count;

            }

            form.toolStripStatusLabel1.Text = "QuestDirectoryScan Done";
            Application.DoEvents();
        }

        private Tuple<Dictionary<string, List<string>>, Dictionary<string, List<string>>> ReadDirectories(string folderPath)
        {
            string[] folderArray = Directory.GetDirectories(folderPath);
            int folderCount = folderArray.Length;

            form.toolStripProgressBar1.Value = 0;
            form.toolStripProgressBar1.Maximum = folderCount;

            var itemIdToQuestMap = new Dictionary<string, List<string>>();
            var uidToQuestMap = new Dictionary<string, List<string>>();

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
                            string fileContent = File.ReadAllText(file);
                            if (!string.IsNullOrEmpty(fileContent))
                            {
                                string filename = Path.GetFileName(file);
                                fileMap[filename] = file;

                                ProcessFileContent(fileContent, filename, itemIdToQuestMap, uidToQuestMap);
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

            return Tuple.Create(itemIdToQuestMap, uidToQuestMap);
        }

        private void ProcessFileContent(string fileContent, string filename, Dictionary<string, List<string>> itemIdToQuestMap, Dictionary<string, List<string>> uidToQuestMap)
        {
            string[] lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            bool insideTasks = false;

            foreach (string line in lines)
            {
                if (line.Contains("id:"))
                {
                    string itemId = ExtractItemNameWithDamage(line);

                    if (itemId != null)
                    {
                        if (!itemIdToQuestMap.ContainsKey(itemId))
                        {
                            itemIdToQuestMap[itemId] = new List<string>();
                        }
                        itemIdToQuestMap[itemId].Add(filename);
                    }
                }

                if (line.Contains("tasks:"))
                {
                    insideTasks = true;
                    continue;
                }

                if (insideTasks)
                {
                    var uidRegex = new Regex(@"uid:\s*""([^""]+)""");
                    Match uidMatch = uidRegex.Match(line);

                    if (uidMatch.Success)
                    {
                        string uid = uidMatch.Groups[1].Value;
                        if (!uidToQuestMap.ContainsKey(uid))
                        {
                            uidToQuestMap[uid] = new List<string>();
                        }
                        uidToQuestMap[uid].Add(filename);

                        insideTasks = false;
                    }
                }

                if (line.Contains("}]"))
                {
                    insideTasks = false;
                }
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