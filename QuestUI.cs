using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class QuestUI : Form
    {
        //private QuestTreeViewManager questTreeViewManager;
        private readonly string connectionString = $"Data Source={ConfigManager.Config.DatabaseFile};Version=3;";

        public QuestUI()
        {
            InitializeComponent();
            LoadQuestlines();
            InitializeDragDropHandlers();
                    }

        private void InitializeDragDropHandlers()
        {
            QuestPanel.AllowDrop = true;
            QuestPanel.DragEnter += QuestPanel_DragEnter;
            QuestPanel.DragDrop += QuestPanel_DragDrop;
        }

        private void QuestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(QuestList.SelectedIndex.ToString());
            var selectedFolderName = QuestList.SelectedItem.ToString();

            if (questFolderPaths.TryGetValue(selectedFolderName, out var selectedFolderPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(selectedFolderPath);
                if (DirectoryContainsSNBTFiles(directoryInfo))
                {
                    DisplayQuestsInCanvas();
                }
                else
                {
                    Console.WriteLine("Selected directory does not contain valid .snbt files.");
                }
                Application.DoEvents();
                LoadDependenciesForSelectedIndex(); // Load dependencies based on the selected index
                DrawDependencies();
            }
        }

        private void AddControlsToCanvas(List<Control> controls)
        {
            foreach (var control in controls)
            {
                QuestPanel.Controls.Add(control);
            }
        }
    }
}
