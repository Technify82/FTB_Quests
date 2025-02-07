using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class Configuration
    {
        public void FolderSelect(string method)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                switch (method)
                {
                    case "ProjectFolder":

                        folderBrowserDialog.Description = "Select a Folder";
                        folderBrowserDialog.ShowNewFolderButton = true;
                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            projectFolderTextBox.Text = folderBrowserDialog.SelectedPath;
                            config.ProjectFolder = folderBrowserDialog.SelectedPath;
                            UpdateTextBoxes(config.ProjectFolder);
                            EnableControls();
                        }
                        break;

                    case "ImageFolder":

                        folderBrowserDialog.Description = "Select a folder containing recipe images.";
                        folderBrowserDialog.ShowNewFolderButton = true;
                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            imageFolderTextBox.Text = folderBrowserDialog.SelectedPath;
                        }

                        break;

                    case "QuestFolder":
                        folderBrowserDialog.Description = "Select a folder containing quests";
                        folderBrowserDialog.ShowNewFolderButton = true;
                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            QuestFolderLocation.Text = folderBrowserDialog.SelectedPath;
                        }

                        break;
                }
            }
        }

        public void FileSelect(string method)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                switch (method)
                {
                    case "RecipeFile":

                        openFileDialog.Title = "Select a recipe file";
                        openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                        openFileDialog.DefaultExt = "csv";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            recipeFileTextBox.Text = openFileDialog.FileName;
                        }
                        break;

                    case "ItemPanel":

                        openFileDialog.Title = "Select an item panel file";
                        openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                        openFileDialog.DefaultExt = "csv";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            itemPanelFileTextBox.Text = openFileDialog.FileName;
                        }
                        break;

                    case "OreDictFile":

                        openFileDialog.Title = "Select an Ore Dictionary file";
                        openFileDialog.Filter = "TXT files (*.txt)|*.txt";
                        openFileDialog.DefaultExt = "txt";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            OreDictFileLocation.Text = openFileDialog.FileName;
                        }
                        break;
                    case "DatabaseFile":
                        openFileDialog.Title = "Select an SQL Database";
                        openFileDialog.Filter = "DB files (*.db)|*.db";
                        openFileDialog.DefaultExt = "db";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            config.DatabaseFile = openFileDialog.FileName;
                            DatabaseFile.Text = openFileDialog.FileName;
                        }
                        break;
                }
            }
        }
    }
}
