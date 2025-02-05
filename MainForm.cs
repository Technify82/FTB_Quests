using System;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class MainForm : Form
    {
        ConfigManager configManager;
                private readonly Configuration configuration;
        private QuestLinker questLinker;
        private NewParser newParser;
        private DataDisplay dataDisplay;
        private PopulateRecipeGrid populateRecipeGrid;
        private BuildQuests buildQuests;
        OreDictLogic oreDictLogic;
        private QuestUI questUI;

        public MainForm()
        {
            InitializeComponent();
            Show();
            configuration = new Configuration(this);
            configManager = ConfigManager.Instance;

            configuration.LoadConfiguration();
            configuration.PopulateTextBoxes();
            InitializeComponents();
        }

        
        private bool componentsInitialized = false;

        private void InitializeComponents()
        {
            try
            {
                if (configManager.Config.ProjectFolder != null)
                {
                    questLinker = new QuestLinker(this);
                    newParser = new NewParser(this);
                    newParser.CheckDatabaseAndPopulateRecipeText();
                    oreDictLogic = new OreDictLogic(this);
                    dataDisplay = new DataDisplay();
                    populateRecipeGrid = new PopulateRecipeGrid(this);
                    dataDisplay.DataDisplay_Load();
                    buildQuests = new BuildQuests();
                    questUI = new QuestUI();
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

        public void UpdateConfiguration()

        {
            
            configuration.LoadConfiguration();    
 
            questLinker = new QuestLinker(this);     
            newParser = new NewParser(this);
            newParser.CheckDatabaseAndPopulateRecipeText();
            oreDictLogic = new OreDictLogic(this);
            populateRecipeGrid = new PopulateRecipeGrid(this);
            buildQuests = new BuildQuests();
            questUI = new QuestUI();
            dataDisplay.DataDisplay_Load();     
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
            if (configManager.Config.QuestFolder != null)
            {
                questLinker.QuestDirectoryScan();
            }
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar2.Value = 0;
        }

        private void ConfigurationToolStrip_Click(object sender, EventArgs e)
        {
            Configuration configForm = new Configuration(this);
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

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (questUI == null || questUI.IsDisposed)
            {
                questUI = new QuestUI();
            }

            questUI.Show();
        }
    }
}
