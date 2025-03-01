using System.Collections.Generic;
using System.Linq;

namespace FTB_Quests


{
    internal class OreDictLogic
    {
        ConfigManager configManager;
        private readonly MainForm form;
        private List<ProjectProperties> projectProperties;
        private List<(string name, string registry, int maxDamage, string oreDict)> oreDictionaryItems;
        public MappingProperties MappingProps { get; set; }

        public OreDictLogic(MainForm form)
        {
            configManager = ConfigManager.Instance;
            this.form = form;
            oreDictionaryItems = new List<(string name, string registry, int maxDamage, string oreDict)>();
            projectProperties = new List<ProjectProperties>();
            MappingProps = new MappingProperties();
        }

        public void CompileOreDictInformation()
        {
            var oreIngredients = new HashSet<string>();
            var oreDictNames = new Dictionary<string, List<string>>();
            var displayNames = new Dictionary<string, string>();

            foreach (var oreItem in oreDictionaryItems)
            {
                oreIngredients.Add(oreItem.name);
                if (!oreDictNames.ContainsKey(oreItem.oreDict))
                {
                    oreDictNames[oreItem.oreDict] = new List<string>();
                }
                oreDictNames[oreItem.oreDict].Add(oreItem.name);
                displayNames[oreItem.oreDict] = oreItem.registry;
            }

            DisplayNameFinding(oreIngredients.ToDictionary(ingredient => ingredient, ingredient => ingredient), oreDictNames);
            SwapOreItemsForDisplayNames(oreIngredients.ToDictionary(ingredient => ingredient, ingredient => ingredient), oreDictNames, displayNames);
        }

        public void DisplayNameFinding(Dictionary<string, string> oreIngredients, Dictionary<string, List<string>> oreDictNames)
        {
            form.toolStripProgressBar1.Maximum = oreIngredients.Count;
            form.toolStripProgressBar1.Value = 0;

            foreach (var ingredient in oreIngredients)
            {
                string ingredientValue = ingredient.Value;
                var containsColorMapping = GetColorCodeIfExists(ingredientValue);
                if (ingredientValue.Contains("dye"))
                {
                    containsColorMapping = containsColorMapping == 15 ? 0 : containsColorMapping == 0 ? 15 : containsColorMapping;
                }

                string itemName = GetItemName(ingredientValue, containsColorMapping, MappingProps.ItemNameDictionary);
                bool displayNamesFound = false;

                if (!string.IsNullOrEmpty(itemName))
                {
                    displayNamesFound = ProcessIngredientFromProperties(ingredient.Key, itemName, "ItemName");
                    displayNamesFound |= ProcessIngredientFromProperties(ingredient.Key, itemName, "OreDict");
                }

                if (!displayNamesFound && oreDictNames.TryGetValue(ingredientValue.Replace("ore:", "").Trim(), out var items))
                {
                    foreach (var item in items)
                    {
                        displayNamesFound = ProcessIngredientFromProperties(ingredient.Key, item, "ItemName");
                        displayNamesFound |= ProcessIngredientFromProperties(ingredient.Key, item, "OreDict");
                        if (displayNamesFound) break;
                    }
                }

                form.toolStripProgressBar1.Value++;
            }
        }


        private string GetItemName(string ingredientValue, int? containsColorMapping, Dictionary<string, string> itemNameDictionary)
        {
            foreach (var item in itemNameDictionary)
            {
                if (ingredientValue.Contains(item.Key))
                {
                    return containsColorMapping.HasValue ? $"{item.Value}:{containsColorMapping}" : item.Value;
                }
            }
            return null;
        }

        public void SwapOreItemsForDisplayNames(Dictionary<string, string> oreIngredients, Dictionary<string, List<string>> oreDictNames, Dictionary<string, string> displayNames)
        {
            var preDictionaryMatches = new Dictionary<string, string>();

            foreach (var oreIngredient in oreIngredients.Keys)
            {
                foreach (var oreDict in oreDictNames)
                {
                    if (displayNames.ContainsKey(oreDict.Key) && oreIngredient.Contains(oreDict.Key))
                    {
                        string colorPart = oreIngredient.Replace(oreDict.Key, "").Replace("ore:", "");

                        if (MappingProps.ColorMapping.TryGetValue(colorPart, out int colorCode))
                        {
                            string itemName = oreDict.Value.FirstOrDefault(name => name.Contains(colorCode.ToString()));
                            if (!string.IsNullOrEmpty(itemName))
                            {
                                preDictionaryMatches[oreIngredient] = displayNames[oreDict.Key];
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var match in preDictionaryMatches)
            {
                string oreIngredient = match.Key;
                string displayName = match.Value;

                for (char col = 'A'; col <= 'I'; col++)
                {
                    var propertiesToUpdate = projectProperties
                        .Where(p => typeof(ProjectProperties).GetProperty(col.ToString())?.GetValue(p)?.ToString() == oreIngredient)
                        .ToList();

                    foreach (var prop in propertiesToUpdate)
                    {
                        typeof(ProjectProperties).GetProperty(col.ToString())?.SetValue(prop, displayName);
                    }
                }
            }
        }

        private int? GetColorCodeIfExists(string ingredientValue)
        {
            foreach (var color in MappingProps.ColorMapping.Keys)
            {
                if (ingredientValue.Contains(color))
                {
                    return MappingProps.ColorMapping[color];
                }
            }
            return null;
        }

        private bool ProcessIngredientFromProperties(string ingredientKey, string itemName, string type)
        {
            var displayNames = new List<string>();

            if (type == "ItemName")
            {
                displayNames = projectProperties
                    .Where(p => p.ItemName == itemName)
                    .Select(p => p.DisplayName)
                    .Distinct()
                    .ToList();
            }
            else if (type == "OreDict")
            {
                displayNames = projectProperties
                    .Where(p => p.OreDict == itemName)
                    .Select(p => p.DisplayName)
                    .Distinct()
                    .ToList();
            }

            if (displayNames.Any())
            {
                BatchUpdateDisplayNamesFromProperties(ingredientKey, displayNames);
                return true;
            }

            return false;
        }

        private void BatchUpdateDisplayNamesFromProperties(string ingredient, List<string> displayNames)
        {
            foreach (var displayName in displayNames)
            {
                for (char col = 'A'; col <= 'I'; col++)
                {
                    var propertiesToUpdate = projectProperties
                        .Where(p => typeof(ProjectProperties).GetProperty(col.ToString())?.GetValue(p)?.ToString() == ingredient)
                        .ToList();

                    foreach (var prop in propertiesToUpdate)
                    {
                        typeof(ProjectProperties).GetProperty(col.ToString())?.SetValue(prop, displayName);
                    }
                }
            }
        }
    }
}
