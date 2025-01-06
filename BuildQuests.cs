using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class BuildQuests : Form
    {
        public string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";
        QuestBuilder questBuilder;
        QuestTreeViewManager questTreeViewManager;

        public BuildQuests()
        {
            InitializeComponent();
            InitializeTreeViewManager();
            InitializeQuestBuilder();
        }

        private void InitializeQuestBuilder()
        {
            questBuilder = new QuestBuilder(this);
        }

        private void InitializeTreeViewManager()
        {
            questTreeViewManager = new QuestTreeViewManager(treeView1);
            questTreeViewManager.InitializeTreeView(SelectedIndexClick);
            questTreeViewManager.LoadFileTree(ConfigManager.Config.QuestFolder);
        }

        private void SaveAllQuests_Click(object sender, EventArgs e)
        {
            questTreeViewManager.FilterNodes(treeView1.Nodes, node =>
            {
                questBuilder.PopulateQuestBox(node.Text);
                bool includeDependencies = true;
                questBuilder.OutputQuestRecipeAndDependencies(node.Text.ToString(), includeDependencies);
                Application.DoEvents();
                string filePath = questBuilder.FindFileInSubfolders(ConfigManager.Config.QuestFolder, node.Text.ToString());

                try
                {
                    File.WriteAllText(filePath, QuestBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private async void SelectedIndexClick(object sender, EventArgs e)
        {
            bool includeDependencies = checkBox1.Checked;
            if (includeDependencies)
            {
                questBuilder.PopulateQuestBox(treeView1.SelectedNode.Text);
                questBuilder.OutputQuestRecipeAndDependencies(treeView1.SelectedNode.Text.ToString(), includeDependencies);
            }
            else
            {
                await questBuilder.PopulateQuestBoxRemoveDependenciesAsync(treeView1.SelectedNode.Text);
            }
        }

        private void SaveQuest_Click(object sender, EventArgs e)
        {
            questBuilder.SaveQuestBoxContent();
        }

        private async void SaveAndRemoveAllDependencies_Click(object sender, EventArgs e)
        {
            await questTreeViewManager.FilterNodesAsync(treeView1.Nodes, async node =>
            {
                await questBuilder.PopulateQuestBoxRemoveDependenciesAsync(node.Text);
                string filePath = questBuilder.FindFileInSubfolders(ConfigManager.Config.QuestFolder, node.Text.ToString());

                try
                {
                    await RetryAsync(async () =>
                    {
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            await writer.WriteAsync(QuestBox.Text);
                        }
                    }, maxRetries: 3);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        public async Task RetryAsync(Func<Task> action, int maxRetries = 3, int delayMilliseconds = 1000)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    await action();
                    return;
                }
                catch
                {
                    attempts++;
                    if (attempts >= maxRetries)
                        throw;

                    await Task.Delay(delayMilliseconds);
                }
            }
        }
    }
}
