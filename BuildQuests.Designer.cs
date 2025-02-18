﻿namespace FTB_Quests
{
    partial class BuildQuests
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.QuestBox = new System.Windows.Forms.RichTextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SaveQuest = new System.Windows.Forms.Button();
            this.SaveAllQuests = new System.Windows.Forms.Button();
            this.DependencyBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SaveAndRemoveAllDependencies = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 52);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(192, 520);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SelectedIndexClick);
            // 
            // QuestBox
            // 
            this.QuestBox.Location = new System.Drawing.Point(210, 52);
            this.QuestBox.Name = "QuestBox";
            this.QuestBox.Size = new System.Drawing.Size(453, 298);
            this.QuestBox.TabIndex = 1;
            this.QuestBox.Text = "";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(345, 394);
            this.checkBox1.MaximumSize = new System.Drawing.Size(133, 17);
            this.checkBox1.MinimumSize = new System.Drawing.Size(133, 17);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(133, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Include Dependencies";
            this.checkBox1.UseVisualStyleBackColor = true;
//            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // SaveQuest
            // 
            this.SaveQuest.Location = new System.Drawing.Point(484, 394);
            this.SaveQuest.MaximumSize = new System.Drawing.Size(200, 50);
            this.SaveQuest.MinimumSize = new System.Drawing.Size(200, 50);
            this.SaveQuest.Name = "SaveQuest";
            this.SaveQuest.Size = new System.Drawing.Size(200, 50);
            this.SaveQuest.TabIndex = 4;
            this.SaveQuest.Text = "Save Quest";
            this.SaveQuest.UseVisualStyleBackColor = true;
            this.SaveQuest.Click += new System.EventHandler(this.SaveQuest_Click);
            // 
            // SaveAllQuests
            // 
            this.SaveAllQuests.Location = new System.Drawing.Point(484, 450);
            this.SaveAllQuests.MaximumSize = new System.Drawing.Size(200, 50);
            this.SaveAllQuests.MinimumSize = new System.Drawing.Size(200, 50);
            this.SaveAllQuests.Name = "SaveAllQuests";
            this.SaveAllQuests.Size = new System.Drawing.Size(200, 50);
            this.SaveAllQuests.TabIndex = 5;
            this.SaveAllQuests.Text = "Add Dependencies to All Quests";
            this.SaveAllQuests.UseVisualStyleBackColor = true;
            this.SaveAllQuests.Click += new System.EventHandler(this.SaveAllQuests_Click);
            // 
            // DependencyBox
            // 
            this.DependencyBox.Location = new System.Drawing.Point(210, 392);
            this.DependencyBox.Name = "DependencyBox";
            this.DependencyBox.Size = new System.Drawing.Size(129, 177);
            this.DependencyBox.TabIndex = 6;
            this.DependencyBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(207, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "QUEST";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "QUEST LIST";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(207, 376);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "DEPENDENCIES";
            // 
            // SaveAndRemoveAllDependencies
            // 
            this.SaveAndRemoveAllDependencies.Location = new System.Drawing.Point(484, 506);
            this.SaveAndRemoveAllDependencies.MaximumSize = new System.Drawing.Size(200, 50);
            this.SaveAndRemoveAllDependencies.MinimumSize = new System.Drawing.Size(200, 50);
            this.SaveAndRemoveAllDependencies.Name = "SaveAndRemoveAllDependencies";
            this.SaveAndRemoveAllDependencies.Size = new System.Drawing.Size(200, 50);
            this.SaveAndRemoveAllDependencies.TabIndex = 10;
            this.SaveAndRemoveAllDependencies.Text = "Remove Dependencies from All Quests";
            this.SaveAndRemoveAllDependencies.UseVisualStyleBackColor = true;
            this.SaveAndRemoveAllDependencies.Click += new System.EventHandler(this.SaveAndRemoveAllDependencies_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(731, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // BuildQuests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 602);
            this.Controls.Add(this.SaveAndRemoveAllDependencies);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DependencyBox);
            this.Controls.Add(this.SaveAllQuests);
            this.Controls.Add(this.SaveQuest);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.QuestBox);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BuildQuests";
            this.Text = "BuildQuests";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.RichTextBox QuestBox;
        public System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button SaveQuest;
        private System.Windows.Forms.Button SaveAllQuests;
        public System.Windows.Forms.RichTextBox DependencyBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button SaveAndRemoveAllDependencies;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}