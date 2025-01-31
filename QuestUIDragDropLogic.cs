using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class QuestUI
    {

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            var pictureBox = sender as PictureBox;
            if (pictureBox != null)
            {
                string Quest = pictureBox.Tag.ToString();
               
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var pictureBox = sender as PictureBox;
            if (pictureBox != null && e.Button == MouseButtons.Left)
            {
                var newLocation = pictureBox.Location + new Size(e.X, e.Y);
                pictureBox.Location = newLocation;

                string Quest = pictureBox.Tag.ToString();
                
            }
        }

        private void QuestPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PictureBox)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void QuestPanel_DragDrop(object sender, DragEventArgs e)
        {
            var pictureBox = (PictureBox)e.Data.GetData(typeof(PictureBox));
            if (pictureBox != null)
            {
                var point = QuestPanel.PointToClient(new Point(e.X, e.Y));
                pictureBox.Location = point;
                QuestPanel.Controls.Add(pictureBox);
            }
        }

    }
}
