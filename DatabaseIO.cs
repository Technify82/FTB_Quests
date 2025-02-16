using System;
using System.Data.SQLite;

namespace FTB_Quests
{
    public class DatabaseIO
    {
        readonly ConfigManager configManager;

        public DatabaseIO()
        {
            configManager = ConfigManager.Instance;
        }
        public void PurgeDatabase()
        {
            string connectionString = $"Data Source={configManager.Config.DatabaseFile};Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deletePotions = "DROP TABLE IF EXISTS Potions";
                        string deleteRecipesQuery = "DROP TABLE IF EXISTS Recipes";

                        using (SQLiteCommand command = new SQLiteCommand(deletePotions, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        using (SQLiteCommand command = new SQLiteCommand(deleteRecipesQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        Console.WriteLine("Database purged successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"An error occurred while purging the database: {ex.Message}");
                    }
                }
                connection.Close();
                CreateTables();
            }
        }
        public void CreateTables()
        {
            string connectionString = $"Data Source={configManager.Config.DatabaseFile};Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createRecipesTableQuery = @"CREATE TABLE IF NOT EXISTS Recipes (
                RecipeID INTEGER PRIMARY KEY AUTOINCREMENT,
                InputPattern TEXT,
                A TEXT,
                B TEXT,
                C TEXT,
                D TEXT,
                E TEXT,
                F TEXT,
                G TEXT,
                H TEXT,
                I TEXT,
                OutputItem TEXT,
                ItemName TEXT,
                ItemId TEXT,
                ItemMeta TEXT,
                DisplayName TEXT,
                OreDict TEXT,
                Quantity INTEGER,
                Quests TEXT,
                TaskUID TEXT
            );";
                using (SQLiteCommand command = new SQLiteCommand(createRecipesTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                string createPotionsTableQuery = @"CREATE TABLE IF NOT EXISTS Potions (
                RecipeID INTEGER PRIMARY KEY AUTOINCREMENT,
                InputPattern TEXT,
                PotionName TEXT,
                PotionDisplayName TEXT,
                OreDict TEXT,
                Quests TEXT,
                TaskUID TEXT
            );";
                using (SQLiteCommand command = new SQLiteCommand(createPotionsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
                Console.WriteLine("Tables created successfully.");
            }
        }
    }
}
