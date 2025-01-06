using System;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class MainForm : Form
    {
        public string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";
        private readonly Configuration configuration;
        private QuestLinker questLinker;
        private NewParser newParser;
        private DataDisplay dataDisplay;
        private PopulateRecipeGrid populateRecipeGrid;
        private BuildQuests buildQuests;
        private QuestGridForm questGridForm;
        OreDictLogic oreDictLogic;
        public MainForm()
        {
            InitializeComponent();
            Show();
            configuration = new Configuration();
            configuration.LoadConfiguration();
            InitializeComponents();
        }

        private bool componentsInitialized = false;

        private void InitializeComponents()
        {
            try
            {
                if (ConfigManager.Config.ProjectFolder != null)
                {
                    questLinker = new QuestLinker(this);
                    newParser = new NewParser(this);
                    newParser.CheckDatabaseAndPopulateRecipeText();
                    oreDictLogic = new OreDictLogic(connectionString, this);
                    dataDisplay = new DataDisplay();
                    populateRecipeGrid = new PopulateRecipeGrid(this);
                    dataDisplay.DataDisplay_Load();
                    buildQuests = new BuildQuests();
                    questGridForm = new QuestGridForm();
                    componentsInitialized = true;

                }
                else
                {
                    MessageBox.Show("Project folder is not set.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during initialization: {ex.Message}");
            }
        }

        private void RecipeText_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!componentsInitialized)
            {
                MessageBox.Show("Please initialize components first.");
                return;
            }
            populateRecipeGrid.GridParser(RecipeText, this);
            questLinker.CheckItemInDatabase();
        }

        private void ViewRecipeDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataDisplay == null || dataDisplay.IsDisposed)
            {
                dataDisplay = new DataDisplay();
            }
            dataDisplay.DataDisplay_Load();
            dataDisplay.Show();
        }

        private void ParseRecipeFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RecipeText.Items.Clear();
            if (newParser == null)
            {
                newParser = new NewParser(this);
            }
            newParser.ParseRecipeFile();
            oreDictLogic.CompileOreDictInformation();
            toolStripProgressBar1.Value = 0;
        }

        private void ParseQuestsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ConfigManager.Config.QuestFolder != null)
            {
                questLinker.QuestDirectoryScan();
            }
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar2.Value = 0;
        }

        private void ConfigurationToolStrip_Click(object sender, EventArgs e)
        {
            Configuration configForm = new Configuration();
            configForm.Show();
        }

        private void ExitToolStrip_Click(object sender, EventArgs e) => Close();

        private void ParseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Depending on your modpack, this could take a while.");
            newParser.ParseRecipeFile();
            oreDictLogic.CompileOreDictInformation();
            questLinker.QuestDirectoryScan();
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar2.Value = 0;
        }

        private void BuildQuestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (buildQuests == null || buildQuests.IsDisposed)
            {
                buildQuests = new BuildQuests();
            }

            buildQuests.Show();
        }

        private void OrganizeQuestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (questGridForm == null || questGridForm.IsDisposed)
            {
                questGridForm = new QuestGridForm();
            }

            questGridForm.Show();
        }
    }
}
