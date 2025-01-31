using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class QuestGridForm : Form
    {
        private QuestTreeViewManager questTreeViewManager;
        private string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";
        private Timer holdTimer;

        public QuestGridForm()
        {
            InitializeComponent();
            SetupQuestGrid();
            InitializeTreeViewManager();
            holdTimer = new Timer();
            holdTimer.Interval = 1500;
            holdTimer.Tick += HoldTimer_Tick;

            if (questGridControl == null)
            {
                questGridControl = new TableLayoutPanel
                {
                    // Set appropriate properties for questGridControl
                };
            }
        }


        public void HoldTimer_Tick(object sender, EventArgs e)
        {
            holdTimer.Stop();

            if (currentDragSource != null && currentDragTarget != null)
            {
                CreateDynamicPlaceholder();
            }
        }

        private void InitializeTreeViewManager()
        {
            questTreeViewManager = new QuestTreeViewManager(questTreeView);
            questTreeViewManager.InitializeTreeView(QuestTreeView_DoubleClick);
            questTreeViewManager.LoadFolderTree(ConfigManager.Config.QuestFolder);
        }

        private void SetupQuestGrid()
        {
            for (int i = 0; i < questGridControl.ColumnCount; i++)
            {
                questGridControl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            }
        }

        private void QuestTreeView_DoubleClick(object sender, EventArgs e)
        {
            if (questTreeView.SelectedNode != null)
            {
                MessageBox.Show($"Quest Selected: {questTreeView.SelectedNode.Text}");
            }
        }

        private void QuestTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            questTreeViewManager.questItemsCache.Clear();
            Console.WriteLine("QuestTreeView_AfterSelect triggered.");

            DirectoryInfo directoryInfo = null;

            if (e.Node != null)
            {
                Console.WriteLine($"Node selected: {e.Node.Text}");

                if (e.Node.Tag is DirectoryInfo tempDirectoryInfo)
                {
                    directoryInfo = tempDirectoryInfo;
                    Console.WriteLine($"Node tag is a DirectoryInfo: {directoryInfo.FullName}");
                }
                else
                {
                    Console.WriteLine("Node tag is not a DirectoryInfo.");
                }
            }
            else
            {
                Console.WriteLine("Selected node is null.");
            }

            if (directoryInfo != null && DirectoryContainsSNBTFiles(directoryInfo))
            {
                Console.WriteLine("Directory contains .snbt files.");

                questTreeViewManager.PreloadData(connectionString);
                Console.WriteLine("Preloaded data.");

                DisplayQuestsInGrid(directoryInfo);
            }
            else
            {
                Console.WriteLine("Directory does not contain .snbt files or directoryInfo is null.");
            }
        }

        private bool DirectoryContainsSNBTFiles(DirectoryInfo directoryInfo)
        {
            var excludedFiles = new HashSet<string> { "file.snbt", "index.snbt" };

            var snbtFiles = directoryInfo.GetFiles("*.snbt");

            if (snbtFiles.Length == 0)
            {
                return false;
            }

            foreach (var file in snbtFiles)
            {
                if (!excludedFiles.Contains(file.Name))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetRelativePath(string basePath, string targetPath)
        {
            Console.WriteLine($"Calculating relative path from basePath: {basePath} to targetPath: {targetPath}");

            Uri baseUri = new Uri(AppendDirectorySeparator(basePath), UriKind.Absolute);
            Uri targetUri = new Uri(AppendDirectorySeparator(targetPath), UriKind.Absolute);

            Uri relativeUri = baseUri.MakeRelativeUri(targetUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            Console.WriteLine($"Relative path: {relativePath}");

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }

        private static string AppendDirectorySeparator(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        public bool IsPathAtDesiredDepth(DirectoryInfo directoryInfo)
        {
            string basePath = ConfigManager.Config.QuestFolder;
            string targetPath = directoryInfo.FullName;

            Console.WriteLine($"Checking path depth for basePath: {basePath} and targetPath: {targetPath}");

            string relativePath = GetRelativePath(basePath, targetPath);
            int depth = relativePath.Split(Path.DirectorySeparatorChar).Length - 1;

            Console.WriteLine($"Calculated depth: {depth}");

            return depth == 2;
        }

        private void AddControlsToGrid(List<Control> controls, int column, int row)
        {
            questGridControl.SuspendLayout();

            foreach (var control in controls)
            {
                questGridControl.Controls.Add(control, column, row);
            }

            questGridControl.ResumeLayout();
        }

        private List<QuestItem> FilterBrokenItems()
        {
            return questTreeViewManager.questItemsCache.Values.Where(item => !item.IsBroken).ToList();
        }
    }
}
