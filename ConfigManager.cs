using System.IO;
using System.Text.Json;

namespace FTB_Quests
{
    public static class ConfigManager
    {
        public static ConfigProperties Config { get; set; }

        static ConfigManager()
        {
            LoadConfiguration();
        }

        public static void LoadConfiguration()
        {
            if (File.Exists("Configuration.json"))
            {
                string json = File.ReadAllText("Configuration.json");
                Config = JsonSerializer.Deserialize<ConfigProperties>(json);
            }
            else
            {
                Config = new ConfigProperties();
            }
        }
    }
}