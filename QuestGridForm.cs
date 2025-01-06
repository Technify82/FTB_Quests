using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
//using FTB_Quests_Caching;


namespace FTB_Quests
{
    public class QuestGridForm : Form
    {
        private QuestTreeViewManager questTreeViewManager;
        private string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";

        private System.ComponentModel.IContainer components;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem stuffToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem hideBrokenItemsToolStripMenuItem;
        private TableLayoutPanel questGridControl;
        private TreeView questTreeView;
        private ToolTip toolTip;   

        public QuestGridForm()
        {
            InitializeComponent();
            SetupQuestGrid();
            InitializeTreeViewManager();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.stuffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideBrokenItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.questGridControl = new System.Windows.Forms.TableLayoutPanel();
            this.questTreeView = new System.Windows.Forms.TreeView();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stuffToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1811, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.stuffToolStripMenuItem.Name = "stuffToolStripMenuItem";
            this.stuffToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.stuffToolStripMenuItem.Text = "Stuff";
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideBrokenItemsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            this.hideBrokenItemsToolStripMenuItem.Name = "hideBrokenItemsToolStripMenuItem";
            this.hideBrokenItemsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.hideBrokenItemsToolStripMenuItem.Text = "Hide Broken Items";
            this.hideBrokenItemsToolStripMenuItem.Click += new System.EventHandler(this.HideBrokenItemsToolStripMenuItem_Click);
            this.questGridControl.Location = new System.Drawing.Point(0, 0);
            this.questGridControl.Name = "questGridControl";
            this.questGridControl.Size = new System.Drawing.Size(200, 100);
            this.questGridControl.TabIndex = 0;
            this.questTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.questTreeView.Location = new System.Drawing.Point(0, 24);
            this.questTreeView.Name = "questTreeView";
            this.questTreeView.Size = new System.Drawing.Size(250, 1066);
            this.questTreeView.TabIndex = 1;
            this.questTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.QuestTreeView_AfterSelect);
            this.questGridControl.ColumnCount = 20;
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.questGridControl.Location = new System.Drawing.Point(256, 24);
            this.questGridControl.Name = "questGridControl";
            this.questGridControl.RowCount = 2;
            this.questGridControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.questGridControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.questGridControl.Size = new System.Drawing.Size(1555, 1066);
            this.questGridControl.TabIndex = 2;
            this.ClientSize = new System.Drawing.Size(1811, 1090);
            this.Controls.Add(this.questGridControl);
            this.Controls.Add(this.questTreeView);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "QuestGridForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
            if (e.Node != null && e.Node.Tag is DirectoryInfo directoryInfo)
            {
                questTreeViewManager.PreloadData(connectionString);

                DisplayQuestsInGrid(directoryInfo);
            }
        }

        private void DisplayQuestsInGrid(DirectoryInfo directoryInfo)
        {
            // Preload data for all quests
            questTreeViewManager.PreloadData(connectionString);

            // Filter broken items
            var nonBrokenItems = FilterBrokenItems();

            // Clear the existing controls in the grid
            questGridControl.Controls.Clear();
            questGridControl.RowStyles.Clear();
            questGridControl.RowCount = 0;

            int columnCount = questGridControl.ColumnCount;
            int totalItems = 0;

            // Add non-broken quest items to the grid, skipping every other cell
            foreach (var questItem in nonBrokenItems)
            {
                var pictureBox = new PictureBox
                {
                    Image = questItem.QuestImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 32,
                    Height = 32,
                    Tag = questItem.FileName // Store the file name for reference
                };

                // Enable drag-and-drop
                EnableDragAndDrop(pictureBox);

                // Set ToolTip text to the first display name
                if (questTreeViewManager.DisplayNameCache.ContainsKey(questItem.FileName))
                {
                    var displayNames = questTreeViewManager.DisplayNameCache[questItem.FileName];
                    if (displayNames.Count > 0)
                    {
                        toolTip.SetToolTip(pictureBox, displayNames[0]);
                    }
                }

                int row = totalItems / (columnCount * 2);
                int column = (totalItems * 2) % columnCount;

                if (row >= questGridControl.RowCount)
                {
                    questGridControl.RowCount++;
                    questGridControl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }

                questGridControl.Controls.Add(pictureBox, column, row);
                totalItems++;

                // Debugging information
                Console.WriteLine($"Added non-broken item: {questItem.FileName} at row {row}, column {column}");
            }

            // Fill remaining spaces with "Woops" image
            var woopsImage = questTreeViewManager.GetEmbeddedImage("Woops.png");
            while (totalItems < columnCount * questGridControl.RowCount / 2)
            {
                int row = totalItems / (columnCount * 2);
                int column = (totalItems * 2) % columnCount;

                var pictureBox = new PictureBox
                {
                    Image = woopsImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 32,
                    Height = 32
                };

                // Enable drag-and-drop
                EnableDragAndDrop(pictureBox);

                questGridControl.Controls.Add(pictureBox, column, row);
                totalItems++;

                // Debugging information
                Console.WriteLine($"Added 'Woops' image at row {row}, column {column}");
            }
        }

        private void HideBrokenItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Define the directoryInfo object
            DirectoryInfo directoryInfo = new DirectoryInfo(ConfigManager.Config.QuestFolder);

            // Clear the existing controls in the grid
            questGridControl.Controls.Clear();
            questGridControl.RowStyles.Clear();
            questGridControl.RowCount = 0;

            int columnCount = questGridControl.ColumnCount;
            int totalItems = 0;

            // Add quest items to the grid, skipping every other cell
            foreach (var file in directoryInfo.GetFiles("*.snbt"))
            {
                var displayNames = questTreeViewManager.GetQuestDisplayNames(file.Name, connectionString);
                var questImage = questTreeViewManager.LoadQuestImage(file.Name, connectionString);

                var pictureBox = new PictureBox
                {
                    Image = questImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 32,
                    Height = 32,
                    Tag = file.Name // Store the file name for reference
                };

                // Enable drag-and-drop
                EnableDragAndDrop(pictureBox);

                // Set ToolTip text to the first display name
                if (displayNames.Count > 0)
                {
                    toolTip.SetToolTip(pictureBox, displayNames[0]);
                }

                int row = totalItems / (columnCount * 2);
                int column = (totalItems * 2) % columnCount;

                if (row >= questGridControl.RowCount)
                {
                    questGridControl.RowCount++;
                    questGridControl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }

                questGridControl.Controls.Add(pictureBox, column, row);
                totalItems++;

                // Debugging information
                Console.WriteLine($"Added item: {file.Name} at row {row}, column {column}");
            }

            // Fill remaining spaces with "Woops" image
            var woopsImage = questTreeViewManager.GetEmbeddedImage("Woops.png");
            while (totalItems < columnCount * questGridControl.RowCount / 2)
            {
                int row = totalItems / (columnCount * 2);
                int column = (totalItems * 2) % columnCount;

                var pictureBox = new PictureBox
                {
                    Image = woopsImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 32,
                    Height = 32
                };

                // Enable drag-and-drop
                EnableDragAndDrop(pictureBox);

                questGridControl.Controls.Add(pictureBox, column, row);
                totalItems++;

                // Debugging information
                Console.WriteLine($"Added 'Woops' image at row {row}, column {column}");
            }
        }



        private List<QuestItem> FilterBrokenItems()
        {
            return questTreeViewManager.questItemsCache.Values.Where(item => !item.IsBroken).ToList();
        }

        private void EnableDragAndDrop(PictureBox pictureBox)
        {
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.DragEnter += PictureBox_DragEnter;
            pictureBox.DragDrop += PictureBox_DragDrop;
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var pictureBox = sender as PictureBox;
                if (pictureBox != null)
                {
                    pictureBox.DoDragDrop(pictureBox, DragDropEffects.Move);
                }
            }
        }

        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PictureBox)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            var pictureBox = e.Data.GetData(typeof(PictureBox)) as PictureBox;
            var targetPictureBox = sender as PictureBox;

            if (pictureBox != null && targetPictureBox != null)
            {
                var sourceIndex = questGridControl.Controls.GetChildIndex(pictureBox);
                var targetIndex = questGridControl.Controls.GetChildIndex(targetPictureBox);

                questGridControl.Controls.SetChildIndex(pictureBox, targetIndex);
                questGridControl.Controls.SetChildIndex(targetPictureBox, sourceIndex);
            }
        }

        //private void CacheQuestData(string questId, string questData)
        //{
        //    FileCacheManager.AddToCache(questId, questData);
        //}

        //private string RetrieveCachedQuestData(string questId)
        //{
        //    return FileCacheManager.GetFromCache(questId);
        //}


    }
}
