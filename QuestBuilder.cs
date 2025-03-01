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
        ConfigManager configManager;
        private List<ProjectProperties> projectProperties;
        private List<(string name, string registry, int maxDamage, string oreDict)> oreDictionaryItems;
        public QuestBuilder(BuildQuests buildQuests)
        {
            this.buildQuests = buildQuests;
            configManager = ConfigManager.Instance;
            oreDictionaryItems = new List<(string name, string registry, int maxDamage, string oreDict)>();
            projectProperties = new List<ProjectProperties>();
        }

        public void PopulateQuestBox(string filename)
        {
            buildQuests.QuestBox.Clear();
            string questFolderPath = configManager.Config.QuestFolder.ToString();

            string[] files = Directory.GetFiles(questFolderPath, filename, SearchOption.AllDirectories);

            if (files.Length == 0)
            {
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
            }
        }

        public async Task PopulateQuestBoxRemoveDependenciesAsync(string filename)
        {
            buildQuests.QuestBox.Clear();
            string questFolderPath = configManager.Config.QuestFolder.ToString();

            string[] files = Directory.GetFiles(questFolderPath, filename, SearchOption.AllDirectories);

            if (files.Length == 0)
            {
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
            }
        }

        public void OutputQuestRecipeAndDependencies(string questName, bool includeDependencies)
        {
            var recipes = projectProperties.Where(r => r.Quests != null && r.Quests.Contains(questName)).ToList();

            foreach (var recipe in recipes)
            {
                string questFileName = recipe.Quests;
                List<string> ingredients = new List<string>
        {
            recipe.A, recipe.B, recipe.C, recipe.D, recipe.E, recipe.F, recipe.G, recipe.H, recipe.I
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
                    ExtractLinesToDictionary(uidList);
                }
            }
        }



        private void ProcessIngredientForDependencies(string ingredient, List<Tuple<string, string>> uidList)
        {
            var recipes = projectProperties.Where(r => r.DisplayName == ingredient).ToList();

            foreach (var recipe in recipes)
            {
                string taskUid = recipe.TaskUID;
                string questFile = recipe.Quests;

                if (questFile.EndsWith(".snbt"))
                {
                    questFile = questFile.Substring(0, questFile.Length - 5);
                }

                uidList.Add(new Tuple<string, string>(questFile, taskUid));
            }
        }

        public string FindFileInSubfolders(string baseDirectory, string fileName)
        {
            string[] files = Directory.GetFiles(baseDirectory, fileName, SearchOption.AllDirectories);
            return files.Length > 0 ? files[0] : null;
        }

        public void SaveQuestBoxContent()
        {
            string filePath = FindFileInSubfolders(configManager.Config.QuestFolder, buildQuests.treeView1.SelectedNode.Text.ToString());

            try
            {
                File.WriteAllText(filePath, buildQuests.QuestBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtractLinesToDictionary(List<Tuple<string, string>> uidList)
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
                                            QueryUID(new Tuple<string, string>(cleanedUid, ""));
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
                InsertDependencies(uidList);
            }

            for (int i = counter; i < lines.Length; i++)
            {
                buildQuests.QuestBox.AppendText(lines[i] + Environment.NewLine);
            }
        }

        private void InsertDependencies(List<Tuple<string, string>> uidList)
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
                QueryUID(new Tuple<string, string>(uid, ""));
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

        private void QueryUID(Tuple<string, string> uidInfo)
        {
            string questFile = uidInfo.Item1.Trim(new char[] { '"', ',', '\t', ' ' });

            var matchingRecipes = projectProperties.Where(r => r.Quests != null && r.Quests.Contains(questFile)).ToList();

            if (matchingRecipes.Any())
            {
                foreach (var recipe in matchingRecipes)
                {
                    string displayName = recipe.DisplayName;

                    buildQuests.DependencyBox.AppendText(displayName + Environment.NewLine);
                    Application.DoEvents();
                }
            }
            else
            {
            }
        }

    }
}