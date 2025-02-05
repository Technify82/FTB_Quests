using System;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    public class ConfigProperties
    {
        public string ProjectFolder { get; set; }
        public string RecipeFile { get; set; }
        public string ItemPanelFile { get; set; }
        public string ImageFolder { get; set; }
        public string QuestFolder { get; set; }
        public string OreDictionary { get; set; }
        public string DatabaseFile { get; set; }
        public bool UseCache { get; set; }
    }
}
