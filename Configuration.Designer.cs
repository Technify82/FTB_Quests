namespace FTB_Quests
{
    partial class Configuration
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.findRecipeFile = new System.Windows.Forms.Button();
            this.findImagePanelFile = new System.Windows.Forms.Button();
            this.findImageFolder = new System.Windows.Forms.Button();
            this.recipeFileTextBox = new System.Windows.Forms.TextBox();
            this.itemPanelFileTextBox = new System.Windows.Forms.TextBox();
            this.imageFolderTextBox = new System.Windows.Forms.TextBox();
            this.projectFolder = new System.Windows.Forms.Button();
            this.projectFolderTextBox = new System.Windows.Forms.TextBox();
            this.QuestFolderLocation = new System.Windows.Forms.TextBox();
            this.FindQuestFolder = new System.Windows.Forms.Button();
            this.OreDictFileLocation = new System.Windows.Forms.TextBox();
            this.OreDictButton = new System.Windows.Forms.Button();
            this.DatabaseFile = new System.Windows.Forms.TextBox();
            this.DatabaseFileButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.CacheInfoBox = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // findRecipeFile
            // 
            this.findRecipeFile.Location = new System.Drawing.Point(6, 58);
            this.findRecipeFile.Name = "findRecipeFile";
            this.findRecipeFile.Size = new System.Drawing.Size(118, 20);
            this.findRecipeFile.TabIndex = 0;
            this.findRecipeFile.Text = "Shaped Recipe File";
            this.findRecipeFile.UseVisualStyleBackColor = true;
            this.findRecipeFile.Click += new System.EventHandler(this.RecipeFile_Click);
            // 
            // findImagePanelFile
            // 
            this.findImagePanelFile.Location = new System.Drawing.Point(6, 84);
            this.findImagePanelFile.Name = "findImagePanelFile";
            this.findImagePanelFile.Size = new System.Drawing.Size(118, 20);
            this.findImagePanelFile.TabIndex = 1;
            this.findImagePanelFile.Text = "Item Panel File";
            this.findImagePanelFile.UseVisualStyleBackColor = true;
            this.findImagePanelFile.Click += new System.EventHandler(this.ItemPanelFile_Click);
            // 
            // findImageFolder
            // 
            this.findImageFolder.Location = new System.Drawing.Point(6, 110);
            this.findImageFolder.Name = "findImageFolder";
            this.findImageFolder.Size = new System.Drawing.Size(118, 20);
            this.findImageFolder.TabIndex = 2;
            this.findImageFolder.Text = "Image Folder";
            this.findImageFolder.UseVisualStyleBackColor = true;
            this.findImageFolder.Click += new System.EventHandler(this.ItemImagesFolder_Click);
            // 
            // recipeFileTextBox
            // 
            this.recipeFileTextBox.Location = new System.Drawing.Point(126, 58);
            this.recipeFileTextBox.Name = "recipeFileTextBox";
            this.recipeFileTextBox.Size = new System.Drawing.Size(500, 20);
            this.recipeFileTextBox.TabIndex = 3;
            // 
            // itemPanelFileTextBox
            // 
            this.itemPanelFileTextBox.Location = new System.Drawing.Point(126, 84);
            this.itemPanelFileTextBox.Name = "itemPanelFileTextBox";
            this.itemPanelFileTextBox.Size = new System.Drawing.Size(500, 20);
            this.itemPanelFileTextBox.TabIndex = 4;
            // 
            // imageFolderTextBox
            // 
            this.imageFolderTextBox.Location = new System.Drawing.Point(126, 110);
            this.imageFolderTextBox.Name = "imageFolderTextBox";
            this.imageFolderTextBox.Size = new System.Drawing.Size(500, 20);
            this.imageFolderTextBox.TabIndex = 5;
            // 
            // projectFolder
            // 
            this.projectFolder.Location = new System.Drawing.Point(6, 6);
            this.projectFolder.Name = "projectFolder";
            this.projectFolder.Size = new System.Drawing.Size(118, 20);
            this.projectFolder.TabIndex = 10;
            this.projectFolder.Text = "Project Folder";
            this.projectFolder.UseVisualStyleBackColor = true;
            this.projectFolder.Click += new System.EventHandler(this.ProjectFolder_Click);
            // 
            // projectFolderTextBox
            // 
            this.projectFolderTextBox.Location = new System.Drawing.Point(126, 6);
            this.projectFolderTextBox.Name = "projectFolderTextBox";
            this.projectFolderTextBox.Size = new System.Drawing.Size(500, 20);
            this.projectFolderTextBox.TabIndex = 1;
            // 
            // QuestFolderLocation
            // 
            this.QuestFolderLocation.Location = new System.Drawing.Point(126, 32);
            this.QuestFolderLocation.Name = "QuestFolderLocation";
            this.QuestFolderLocation.Size = new System.Drawing.Size(500, 20);
            this.QuestFolderLocation.TabIndex = 2;
            // 
            // FindQuestFolder
            // 
            this.FindQuestFolder.Location = new System.Drawing.Point(6, 32);
            this.FindQuestFolder.Name = "FindQuestFolder";
            this.FindQuestFolder.Size = new System.Drawing.Size(118, 20);
            this.FindQuestFolder.TabIndex = 14;
            this.FindQuestFolder.Text = "Find Quest Folder";
            this.FindQuestFolder.UseVisualStyleBackColor = true;
            this.FindQuestFolder.Click += new System.EventHandler(this.FindQuestFolder_Click);
            // 
            // OreDictFileLocation
            // 
            this.OreDictFileLocation.Location = new System.Drawing.Point(126, 136);
            this.OreDictFileLocation.Name = "OreDictFileLocation";
            this.OreDictFileLocation.Size = new System.Drawing.Size(500, 20);
            this.OreDictFileLocation.TabIndex = 6;
            // 
            // OreDictButton
            // 
            this.OreDictButton.Location = new System.Drawing.Point(6, 136);
            this.OreDictButton.Name = "OreDictButton";
            this.OreDictButton.Size = new System.Drawing.Size(118, 20);
            this.OreDictButton.TabIndex = 16;
            this.OreDictButton.Text = "Ore Dictionary File";
            this.OreDictButton.UseVisualStyleBackColor = true;
            this.OreDictButton.Click += new System.EventHandler(this.OreDictButton_Click);
            // 
            // DatabaseFile
            // 
            this.DatabaseFile.Location = new System.Drawing.Point(126, 162);
            this.DatabaseFile.Name = "DatabaseFile";
            this.DatabaseFile.Size = new System.Drawing.Size(500, 20);
            this.DatabaseFile.TabIndex = 19;
            // 
            // DatabaseFileButton
            // 
            this.DatabaseFileButton.Location = new System.Drawing.Point(6, 162);
            this.DatabaseFileButton.Name = "DatabaseFileButton";
            this.DatabaseFileButton.Size = new System.Drawing.Size(118, 20);
            this.DatabaseFileButton.TabIndex = 20;
            this.DatabaseFileButton.Text = "Database File";
            this.DatabaseFileButton.UseVisualStyleBackColor = true;
            this.DatabaseFileButton.Click += new System.EventHandler(this.DatabaseFile_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(660, 537);
            this.tabControl1.TabIndex = 21;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.projectFolder);
            this.tabPage1.Controls.Add(this.DatabaseFileButton);
            this.tabPage1.Controls.Add(this.findRecipeFile);
            this.tabPage1.Controls.Add(this.DatabaseFile);
            this.tabPage1.Controls.Add(this.findImagePanelFile);
            this.tabPage1.Controls.Add(this.OreDictButton);
            this.tabPage1.Controls.Add(this.findImageFolder);
            this.tabPage1.Controls.Add(this.OreDictFileLocation);
            this.tabPage1.Controls.Add(this.recipeFileTextBox);
            this.tabPage1.Controls.Add(this.FindQuestFolder);
            this.tabPage1.Controls.Add(this.itemPanelFileTextBox);
            this.tabPage1.Controls.Add(this.QuestFolderLocation);
            this.tabPage1.Controls.Add(this.imageFolderTextBox);
            this.tabPage1.Controls.Add(this.projectFolderTextBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(652, 511);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.CacheInfoBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(652, 511);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // CacheInfoBox
            // 
            this.CacheInfoBox.Location = new System.Drawing.Point(6, 6);
            this.CacheInfoBox.Name = "CacheInfoBox";
            this.CacheInfoBox.Size = new System.Drawing.Size(640, 502);
            this.CacheInfoBox.TabIndex = 0;
            this.CacheInfoBox.Text = "";
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 561);
            this.Controls.Add(this.tabControl1);
            this.Name = "Configuration";
            this.Text = "Configuration";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button findRecipeFile;
        private System.Windows.Forms.Button findImagePanelFile;
        private System.Windows.Forms.Button findImageFolder;
        private System.Windows.Forms.TextBox recipeFileTextBox;
        private System.Windows.Forms.TextBox itemPanelFileTextBox;
        private System.Windows.Forms.TextBox imageFolderTextBox;
        private System.Windows.Forms.Button projectFolder;
        private System.Windows.Forms.TextBox projectFolderTextBox;
        private System.Windows.Forms.TextBox QuestFolderLocation;
        private System.Windows.Forms.Button FindQuestFolder;
        private System.Windows.Forms.TextBox OreDictFileLocation;
        private System.Windows.Forms.Button OreDictButton;
        private System.Windows.Forms.TextBox DatabaseFile;
        private System.Windows.Forms.Button DatabaseFileButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox CacheInfoBox;
    }
}