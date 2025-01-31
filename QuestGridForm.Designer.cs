using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTB_Quests
{
    public partial class QuestGridForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        //public System.ComponentModel.IContainer components;
        public MenuStrip menuStrip1;
        public ToolStripMenuItem stuffToolStripMenuItem;
        public ToolStripMenuItem optionsToolStripMenuItem;
        public ToolStripMenuItem hideBrokenItemsToolStripMenuItem;
        public TableLayoutPanel questGridControl;
        public TreeView questTreeView;
        public ToolTip toolTip;

                private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolTip = new System.Windows.Forms.ToolTip(components);
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            stuffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            hideBrokenItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            questGridControl = new System.Windows.Forms.TableLayoutPanel();
            questTreeView = new System.Windows.Forms.TreeView();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            stuffToolStripMenuItem,
            optionsToolStripMenuItem});
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(1811, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // stuffToolStripMenuItem
            // 
            stuffToolStripMenuItem.Name = "stuffToolStripMenuItem";
            stuffToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            stuffToolStripMenuItem.Text = "Stuff";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            hideBrokenItemsToolStripMenuItem});
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // hideBrokenItemsToolStripMenuItem
            // 
 //           hideBrokenItemsToolStripMenuItem.Name = "hideBrokenItemsToolStripMenuItem";
//            hideBrokenItemsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
 //           hideBrokenItemsToolStripMenuItem.Text = "Hide Broken Items";
//            hideBrokenItemsToolStripMenuItem.Click += new System.EventHandler(HideBrokenItemsToolStripMenuItem_Click);
            // 
            // questGridControl
            // 
            questGridControl.AutoScroll = true;
            questGridControl.ColumnCount = 20;
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            questGridControl.Location = new System.Drawing.Point(256, 24);
            questGridControl.Name = "questGridControl";
            questGridControl.RowCount = 2;
            questGridControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            questGridControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            questGridControl.Size = new System.Drawing.Size(1555, 1066);
            questGridControl.TabIndex = 2;
            // 
            // questTreeView
            // 
            questTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            questTreeView.Location = new System.Drawing.Point(0, 24);
            questTreeView.Name = "questTreeView";
            questTreeView.Size = new System.Drawing.Size(250, 1066);
            questTreeView.TabIndex = 1;
            questTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(QuestTreeView_AfterSelect);
            // 
            // QuestGridForm
            // 
            ClientSize = new System.Drawing.Size(1811, 1090);
            Controls.Add(questGridControl);
            Controls.Add(questTreeView);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "QuestGridForm";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }
    }
}
