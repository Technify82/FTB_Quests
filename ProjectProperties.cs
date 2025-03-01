using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FTB_Quests
{
    public class ProjectProperties
    {
        public int RecipeID { get; set; }
        public string InputPattern { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string E { get; set; }
        public string F { get; set; }
        public string G { get; set; }
        public string H { get; set; }
        public string I { get; set; }
        public string OutputItem { get; set; }
        public string ItemName { get; set; }
        public string ItemId { get; set; }
        public string ItemMeta { get; set; }
        public string DisplayName { get; set; }
        public string OreDict { get; set; }
        public int Quantity { get; set; }
        public string[] Ingredients { get; set; }
        public string Quests { get; set; }
        public string TaskUID { get; set; }
    }

    public class TempItems
    {
        public List<string> ItemNames { get; set; } = new List<string>();
        public List<string> DisplayNames { get; set; } = new List<string>();
        public string ItemMeta { get; set; }
        public string ItemId { get; set; }
    }

    public class Potions
    {
        public int RecipeID { get; set; }
        public string InputPattern { get; set; }
        public string PotionName { get; set; }
        public string PotionDisplayName { get; set; }
        public string OreDict { get; set; }
        public string Quests { get; set; }
        public string TaskUID { get; set; }
    }

    public class MappingProperties
    {
        public Dictionary<string, string> ItemNameDictionary { get; set; } = new Dictionary<string, string>
    {
        {"dye", "minecraft:dye"},
        {"glass", "minecraft:stained_glass"},
        {"wool", "minecraft:wool"},
        {"ore:gemQuartzBlack", "actuallyadditions:item_misc:5"}
    };

        public Dictionary<string, int> ColorMapping { get; set; } = new Dictionary<string, int>
    {
        { "Black", 15 }, { "Red", 14 }, { "Green", 13 }, { "Brown", 12 },
        { "Blue", 11 }, { "Purple", 10 }, { "Cyan", 9 }, { "LightGray", 8 },
        { "Gray", 7 }, { "Pink", 6 }, { "Lime", 5 }, { "Yellow", 4 },
        { "LightBlue", 3 }, { "Magenta", 2 }, { "Orange", 1 }, { "White", 0 }
    };
    }


    public class QuestItem
    {
        public string FileName { get; set; }
        public bool IsBroken { get; set; }
        public Image QuestImage { get; set; }
    }

    public class ConfigProperties
    {
        public string ProjectFolder { get; set; }
        public string RecipeFile { get; set; }
        public string ItemPanelFile { get; set; }
        public string ImageFolder { get; set; }
        public string QuestFolder { get; set; }
        public string OreDictionary { get; set; }
        public string DatabaseFile { get; set; }
        public string SourceLocation { get; set; }
        public bool UseCache { get; set; }
    }
}
