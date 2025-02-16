namespace FTB_Quests
{
    partial class DataDisplay
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
            this.RecipeDataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.PotionDataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.RecipeDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PotionDataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RecipeDataGridView1
            // 
            this.RecipeDataGridView1.AllowUserToOrderColumns = true;
            this.RecipeDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecipeDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.RecipeDataGridView1.Location = new System.Drawing.Point(6, 3);
            this.RecipeDataGridView1.Name = "RecipeDataGridView1";
            this.RecipeDataGridView1.Size = new System.Drawing.Size(1921, 836);
            this.RecipeDataGridView1.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 47);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(391, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // PotionDataGridView1
            // 
            this.PotionDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PotionDataGridView1.Location = new System.Drawing.Point(6, 6);
            this.PotionDataGridView1.Name = "PotionDataGridView1";
            this.PotionDataGridView1.Size = new System.Drawing.Size(1921, 830);
            this.PotionDataGridView1.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 73);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1941, 868);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.RecipeDataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1933, 842);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Shaped Recipes";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.PotionDataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1933, 842);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Potions";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // DataDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1965, 953);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBox1);
            this.Name = "DataDisplay";
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.RecipeDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PotionDataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.TableLayoutPanel SelectedRecipeDataTable;
        private System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.DataGridView RecipeDataGridView1;
        private System.Windows.Forms.DataGridView PotionDataGridView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        //private System.Windows.Forms.DataGridViewTextBoxColumn recipeIdDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn groupDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn widthDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn heightDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn registryDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn maxDamageDataGridViewTextBoxColumn;
    }
}