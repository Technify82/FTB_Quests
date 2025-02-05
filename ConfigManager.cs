using System.IO;
using System.Text.Json;

namespace FTB_Quests
{

    public class ConfigManager
    {
        private static ConfigManager _instance;
        private static readonly object _lock = new object();

        public ConfigProperties Config { get; private set; }

        private ConfigManager()
        {
            LoadConfiguration();
        }

        public static ConfigManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ConfigManager();
                    }
                    return _instance;
                }
            }
        }

        public void LoadConfiguration()
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

        public void SaveConfiguration()
        {
            string json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("Configuration.json", json);
        }
    }
}