namespace RouteOptimizer
{
    partial class DuctSizeOptimization
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
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.SG = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OptimalDuctSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalCable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
            this.label1.Location = new System.Drawing.Point(29, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Duct Size Optimization";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SG,
            this.OptimalDuctSize,
            this.Length,
            this.TotalCable});
            this.dataGridView1.Location = new System.Drawing.Point(22, 88);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(867, 377);
            this.dataGridView1.TabIndex = 4;
            // 
            // SG
            // 
            this.SG.DataPropertyName = "SG";
            this.SG.HeaderText = "Segment";
            this.SG.Name = "SG";
            this.SG.ReadOnly = true;
            this.SG.Width = 400;
            // 
            // OptimalDuctSize
            // 
            this.OptimalDuctSize.DataPropertyName = "OptimalDuctSize";
            this.OptimalDuctSize.HeaderText = "OptimalDuctSize(mm2)";
            this.OptimalDuctSize.Name = "OptimalDuctSize";
            this.OptimalDuctSize.ReadOnly = true;
            this.OptimalDuctSize.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.OptimalDuctSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.OptimalDuctSize.Width = 200;
            // 
            // Length
            // 
            this.Length.DataPropertyName = "Length";
            this.Length.HeaderText = "Length(m)";
            this.Length.Name = "Length";
            this.Length.ReadOnly = true;
            // 
            // TotalCable
            // 
            this.TotalCable.DataPropertyName = "TotalCable";
            this.TotalCable.HeaderText = "TotalCable";
            this.TotalCable.Name = "TotalCable";
            this.TotalCable.ReadOnly = true;
            this.TotalCable.Width = 120;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(741, 494);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 36);
            this.button1.TabIndex = 5;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DuctSizeOptimization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 553);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Name = "DuctSizeOptimization";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form6";
            this.Load += new System.EventHandler(this.DuctSizeOptimization_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SG;
        private System.Windows.Forms.DataGridViewTextBoxColumn OptimalDuctSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn Length;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalCable;
    }
}