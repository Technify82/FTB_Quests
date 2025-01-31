using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class QuestGridForm
    {
        private QuestGridForm questGridForm;
        private PictureBox dynamicPlaceholder;
        private PictureBox currentDragSource;
        private PictureBox currentDragTarget;

        private void DisplayQuestsInGrid(DirectoryInfo directoryInfo)
        {
            Console.WriteLine("DisplayQuestsInGrid called.");
            questTreeViewManager.PreloadData(connectionString);
            Console.WriteLine("Data preloaded.");
            var nonBrokenItems = FilterBrokenItems();
            Console.WriteLine($"Filtered non-broken items: {nonBrokenItems.Count}");
            questGridControl.Controls.Clear();
            questGridControl.RowStyles.Clear();
            questGridControl.RowCount = 0;
            questGridControl.AllowDrop = true;
            int columnCount = questGridControl.ColumnCount;
            int totalItems = 0;
            var buffer = new List<Control>();

            foreach (var questItem in nonBrokenItems)
            {
                var pictureBox = new PictureBox
                {
                    Image = questItem.QuestImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 32,
                    Height = 32,
                    Tag = questItem.FileName,
                    AllowDrop = true
                };
                EnableDragAndDrop(pictureBox);
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
                buffer.Add(pictureBox);
                totalItems++;
                if (buffer.Count >= 100)
                {
                    AddControlsToGrid(buffer, column, row);
                    buffer.Clear();
                }
                Console.WriteLine($"Added non-broken item: {questItem.FileName} at row {row}, column {column}");
            }

            if (buffer.Count > 0)
            {
                AddControlsToGrid(buffer, totalItems % columnCount, totalItems / columnCount);
                Console.WriteLine("Added remaining buffered controls.");
            }

            var woopsImage = questTreeViewManager.GetEmbeddedImage("Woops.png");
            Console.WriteLine("Loaded 'Woops' image.");

            while (totalItems < columnCount * questGridControl.RowCount / 2)
            {
                int row = totalItems / (columnCount * 2);
                int column = (totalItems * 2) % columnCount;
                var pictureBox = new PictureBox
                {
                    Image = woopsImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 32,
                    Height = 32,
                    AllowDrop = true
                };
                EnableDragAndDrop(pictureBox);
                buffer.Add(pictureBox);
                totalItems++;
                if (buffer.Count >= 100)
                {
                    AddControlsToGrid(buffer, column, row);
                    buffer.Clear();
                }
                Console.WriteLine($"Added 'Woops' image at row {row}, column {column}");
            }

            if (buffer.Count > 0)
            {
                AddControlsToGrid(buffer, totalItems % columnCount, totalItems / columnCount);
                Console.WriteLine("Added remaining 'Woops' buffered controls.");
            }

            Console.WriteLine("DisplayQuestsInGrid completed.");
        }


        public void EnableDragAndDrop(PictureBox pictureBox)
        {
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.DragEnter += PictureBox_DragEnter;
            pictureBox.DragOver += PictureBox_DragOver; // Attach DragOver event
            pictureBox.DragDrop += PictureBox_DragDrop;
            pictureBox.DragLeave += PictureBox_DragLeave; // Attach DragLeave event
        }

        public void PictureBox_MouseDown(object sender, MouseEventArgs e)
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

        public void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PictureBox)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        public void PictureBox_DragOver(object sender, DragEventArgs e)
        {
            var targetPictureBox = sender as PictureBox;
            if (targetPictureBox != null)
            {
                ShowDynamicPlaceholder(targetPictureBox);
            }
        }

        public void PictureBox_DragLeave(object sender, EventArgs e)
        {
            RemoveDynamicPlaceholder();
        }

        public void PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            var pictureBox = e.Data.GetData(typeof(PictureBox)) as PictureBox;
            var targetPictureBox = sender as PictureBox;

            if (pictureBox != null && targetPictureBox != null && dynamicPlaceholder != null)
            {
                var sourceIndex = questGridControl.Controls.GetChildIndex(pictureBox);
                var targetIndex = questGridControl.Controls.GetChildIndex(dynamicPlaceholder);
                questGridControl.Controls.Remove(dynamicPlaceholder);
                dynamicPlaceholder = null;
                if (sourceIndex != targetIndex)
                {
                    questGridControl.Controls.Remove(pictureBox);
                    questGridControl.Controls.Add(pictureBox);
                    questGridControl.Controls.SetChildIndex(pictureBox, targetIndex);

                    questGridControl.Invalidate();
                    questGridControl.Update();
                }
            }
            else
            {
                RemoveDynamicPlaceholder();
            }
        }

        public void ShowDynamicPlaceholder(PictureBox targetPictureBox)
        {
            if (questGridControl == null)
            {
                Console.WriteLine("Error: questGridControl is not initialized.");
                return;
            }

            if (dynamicPlaceholder == null)
            {
                dynamicPlaceholder = CreatePlaceholder();
                questGridControl.Controls.Add(dynamicPlaceholder);
            }

            if (targetPictureBox != null)
            {
                var targetIndex = questGridControl.Controls.GetChildIndex(targetPictureBox);
                questGridControl.Controls.SetChildIndex(dynamicPlaceholder, targetIndex);
                questGridControl.Invalidate();
                questGridControl.Update();
            }
            else
            {
                Console.WriteLine("Warning: targetPictureBox is null.");
            }
        }

        public void RemoveDynamicPlaceholder()
        {
            if (questGridControl == null)
            {
                Console.WriteLine("Error: questGridControl is not initialized.");
                return;
            }

            if (dynamicPlaceholder != null)
            {
                questGridControl.Controls.Remove(dynamicPlaceholder);
                dynamicPlaceholder = null;
                questGridControl.Invalidate();
                questGridControl.Update();
            }
            else
            {
                Console.WriteLine("Warning: dynamicPlaceholder is null.");
            }
        }

        public PictureBox CreatePlaceholder()
        {
            var pictureBox = new PictureBox
            {
                Width = 32,
                Height = 32,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };

            return pictureBox;
        }
        public void CreateDynamicPlaceholder()
        {
            if (dynamicPlaceholder == null)
            {
                dynamicPlaceholder = new PictureBox
                {
                    Width = 32,
                    Height = 32,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.LightGray
                };

                var targetIndex = questGridForm.questGridControl.Controls.GetChildIndex(currentDragTarget);
                questGridForm.questGridControl.Controls.Add(dynamicPlaceholder);
                questGridForm.questGridControl.Controls.SetChildIndex(dynamicPlaceholder, targetIndex);

                questGridForm.questGridControl.Invalidate();
                questGridForm.questGridControl.Update();
            }
        }
    }
}
