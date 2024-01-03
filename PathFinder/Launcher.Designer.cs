namespace RouteOptimizer
{

    partial class Launcher
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
            this.mT_analysis = new MetroFramework.Controls.MetroTile();
            this.mT_PSetting = new MetroFramework.Controls.MetroTile();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.mT_Setting = new MetroFramework.Controls.MetroTile();
            this.SuspendLayout();
            // 
            // mT_analysis
            // 
            this.mT_analysis.ActiveControl = null;
            this.mT_analysis.Location = new System.Drawing.Point(111, 110);
            this.mT_analysis.Name = "mT_analysis";
            this.mT_analysis.Size = new System.Drawing.Size(233, 212);
            this.mT_analysis.TabIndex = 0;
            this.mT_analysis.Text = "Analysis";
            this.mT_analysis.UseSelectable = true;
            // 
            // mT_PSetting
            // 
            this.mT_PSetting.ActiveControl = null;
            this.mT_PSetting.Location = new System.Drawing.Point(350, 110);
            this.mT_PSetting.Name = "mT_PSetting";
            this.mT_PSetting.Size = new System.Drawing.Size(189, 124);
            this.mT_PSetting.TabIndex = 1;
            this.mT_PSetting.Text = "Project Setting";
            this.mT_PSetting.UseSelectable = true;
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.Location = new System.Drawing.Point(350, 240);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(95, 82);
            this.metroTile1.TabIndex = 2;
            this.metroTile1.Text = "Maunal\r\nDownload";
            this.metroTile1.UseSelectable = true;
            // 
            // mT_Setting
            // 
            this.mT_Setting.ActiveControl = null;
            this.mT_Setting.Location = new System.Drawing.Point(451, 240);
            this.mT_Setting.Name = "mT_Setting";
            this.mT_Setting.Size = new System.Drawing.Size(88, 82);
            this.mT_Setting.TabIndex = 3;
            this.mT_Setting.Text = "Setting";
            this.mT_Setting.UseSelectable = true;
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 402);
            this.Controls.Add(this.mT_Setting);
            this.Controls.Add(this.metroTile1);
            this.Controls.Add(this.mT_PSetting);
            this.Controls.Add(this.mT_analysis);
            this.Name = "Launcher";
            this.Text = "Launcher";
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTile mT_analysis;
        private MetroFramework.Controls.MetroTile mT_PSetting;
        private MetroFramework.Controls.MetroTile metroTile1;
        private MetroFramework.Controls.MetroTile mT_Setting;
    }
}