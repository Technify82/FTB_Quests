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
            ((System.ComponentModel.ISupportInitialize)(this.RecipeDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // RecipeDataGridView1
            // 
            this.RecipeDataGridView1.AllowUserToOrderColumns = true;
            this.RecipeDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecipeDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.RecipeDataGridView1.Location = new System.Drawing.Point(-38, 73);
            this.RecipeDataGridView1.Name = "RecipeDataGridView1";
            this.RecipeDataGridView1.Size = new System.Drawing.Size(1510, 433);
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
            // DataDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1534, 561);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.RecipeDataGridView1);
            this.Name = "DataDisplay";
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.RecipeDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.TableLayoutPanel SelectedRecipeDataTable;
        private System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.DataGridView RecipeDataGridView1;
        //private System.Windows.Forms.DataGridViewTextBoxColumn recipeIdDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn groupDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn widthDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn heightDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn registryDataGridViewTextBoxColumn;
       //private System.Windows.Forms.DataGridViewTextBoxColumn maxDamageDataGridViewTextBoxColumn;
    }
}