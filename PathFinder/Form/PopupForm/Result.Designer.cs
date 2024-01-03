namespace RouteOptimizer
{

    partial class Form_detailResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_detailResult));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ts_export = new System.Windows.Forms.ToolStripLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dgv_result = new System.Windows.Forms.DataGridView();
            this.col1_round = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col2_method = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col3_length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col4_curve = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col5_segments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1_Wiring = new System.Windows.Forms.TabPage();
            this.tabPage2_CableDuct = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.col1_tagNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col2_system = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col3_cableSpec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col4_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col5_to = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col6_signal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col7_cableLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col8_PIDNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col9_dwgNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col10_remark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.col1_segment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col2_length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col3_totalCableArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col4_cableDuct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_result)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1_Wiring.SuspendLayout();
            this.tabPage2_CableDuct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ts_export});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(802, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ts_export
            // 
            this.ts_export.Name = "ts_export";
            this.ts_export.Size = new System.Drawing.Size(41, 22);
            this.ts_export.Text = "Export";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.930502F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 98.0695F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.24119F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 69.75881F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(793, 530);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.990025F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 96.00997F));
            this.tableLayoutPanel2.Controls.Add(this.dgv_result, 1, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(15, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.875F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.125F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(777, 144);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // dgv_result
            // 
            this.dgv_result.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgv_result.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_result.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_result.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col1_round,
            this.col2_method,
            this.col3_length,
            this.col4_curve,
            this.col5_segments});
            this.dgv_result.GridColor = System.Drawing.SystemColors.Menu;
            this.dgv_result.Location = new System.Drawing.Point(31, 24);
            this.dgv_result.Margin = new System.Windows.Forms.Padding(0);
            this.dgv_result.Name = "dgv_result";
            this.dgv_result.Size = new System.Drawing.Size(714, 120);
            this.dgv_result.TabIndex = 5;
            // 
            // col1_round
            // 
            this.col1_round.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col1_round.HeaderText = "Round";
            this.col1_round.Name = "col1_round";
            this.col1_round.ReadOnly = true;
            // 
            // col2_method
            // 
            this.col2_method.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col2_method.HeaderText = "Analysis Methodology";
            this.col2_method.Name = "col2_method";
            this.col2_method.ReadOnly = true;
            // 
            // col3_length
            // 
            this.col3_length.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col3_length.HeaderText = "Total Length (m)";
            this.col3_length.Name = "col3_length";
            this.col3_length.ReadOnly = true;
            // 
            // col4_curve
            // 
            this.col4_curve.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col4_curve.HeaderText = "# of Curve";
            this.col4_curve.Name = "col4_curve";
            this.col4_curve.ReadOnly = true;
            // 
            // col5_segments
            // 
            this.col5_segments.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col5_segments.HeaderText = "# of Segments";
            this.col5_segments.Name = "col5_segments";
            this.col5_segments.ReadOnly = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.949062F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 97.05094F));
            this.tableLayoutPanel3.Controls.Add(this.tabControl1, 1, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(15, 160);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.489676F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.51032F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(746, 339);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1_Wiring);
            this.tabControl1.Controls.Add(this.tabPage2_CableDuct);
            this.tabControl1.Location = new System.Drawing.Point(24, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(688, 292);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1_Wiring
            // 
            this.tabPage1_Wiring.Controls.Add(this.dataGridView1);
            this.tabPage1_Wiring.Location = new System.Drawing.Point(4, 22);
            this.tabPage1_Wiring.Name = "tabPage1_Wiring";
            this.tabPage1_Wiring.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1_Wiring.Size = new System.Drawing.Size(680, 266);
            this.tabPage1_Wiring.TabIndex = 0;
            this.tabPage1_Wiring.Text = "Wiring Schedule";
            this.tabPage1_Wiring.UseVisualStyleBackColor = true;
            // 
            // tabPage2_CableDuct
            // 
            this.tabPage2_CableDuct.Controls.Add(this.dataGridView2);
            this.tabPage2_CableDuct.Location = new System.Drawing.Point(4, 22);
            this.tabPage2_CableDuct.Name = "tabPage2_CableDuct";
            this.tabPage2_CableDuct.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2_CableDuct.Size = new System.Drawing.Size(680, 266);
            this.tabPage2_CableDuct.TabIndex = 1;
            this.tabPage2_CableDuct.Text = "Cable Duct Schedule";
            this.tabPage2_CableDuct.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col1_tagNo,
            this.col2_system,
            this.col3_cableSpec,
            this.col4_from,
            this.col5_to,
            this.col6_signal,
            this.col7_cableLength,
            this.col8_PIDNo,
            this.col9_dwgNo,
            this.col10_remark});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.Menu;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(674, 260);
            this.dataGridView1.TabIndex = 6;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // col1_tagNo
            // 
            this.col1_tagNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col1_tagNo.HeaderText = "Tag No.";
            this.col1_tagNo.Name = "col1_tagNo";
            this.col1_tagNo.ReadOnly = true;
            // 
            // col2_system
            // 
            this.col2_system.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col2_system.HeaderText = "System";
            this.col2_system.Name = "col2_system";
            this.col2_system.ReadOnly = true;
            // 
            // col3_cableSpec
            // 
            this.col3_cableSpec.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col3_cableSpec.HeaderText = "Cable Spec.";
            this.col3_cableSpec.Name = "col3_cableSpec";
            this.col3_cableSpec.ReadOnly = true;
            // 
            // col4_from
            // 
            this.col4_from.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col4_from.HeaderText = "From";
            this.col4_from.Name = "col4_from";
            this.col4_from.ReadOnly = true;
            // 
            // col5_to
            // 
            this.col5_to.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col5_to.HeaderText = "To";
            this.col5_to.Name = "col5_to";
            this.col5_to.ReadOnly = true;
            // 
            // col6_signal
            // 
            this.col6_signal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col6_signal.HeaderText = "Signal";
            this.col6_signal.Name = "col6_signal";
            this.col6_signal.ReadOnly = true;
            // 
            // col7_cableLength
            // 
            this.col7_cableLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col7_cableLength.HeaderText = "Cable Length";
            this.col7_cableLength.Name = "col7_cableLength";
            this.col7_cableLength.ReadOnly = true;
            // 
            // col8_PIDNo
            // 
            this.col8_PIDNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col8_PIDNo.HeaderText = "P&ID No.";
            this.col8_PIDNo.Name = "col8_PIDNo";
            // 
            // col9_dwgNo
            // 
            this.col9_dwgNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col9_dwgNo.HeaderText = "DWG No.";
            this.col9_dwgNo.Name = "col9_dwgNo";
            this.col9_dwgNo.ReadOnly = true;
            // 
            // col10_remark
            // 
            this.col10_remark.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col10_remark.HeaderText = "Remark";
            this.col10_remark.Name = "col10_remark";
            // 
            // dataGridView2
            // 
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col1_segment,
            this.col2_length,
            this.col3_totalCableArea,
            this.col4_cableDuct});
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.GridColor = System.Drawing.SystemColors.Menu;
            this.dataGridView2.Location = new System.Drawing.Point(3, 3);
            this.dataGridView2.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(674, 260);
            this.dataGridView2.TabIndex = 7;
            // 
            // col1_segment
            // 
            this.col1_segment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col1_segment.HeaderText = "Segment";
            this.col1_segment.Name = "col1_segment";
            this.col1_segment.ReadOnly = true;
            // 
            // col2_length
            // 
            this.col2_length.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col2_length.HeaderText = "Length (m)";
            this.col2_length.Name = "col2_length";
            this.col2_length.ReadOnly = true;
            // 
            // col3_totalCableArea
            // 
            this.col3_totalCableArea.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col3_totalCableArea.HeaderText = "Total Cable Area";
            this.col3_totalCableArea.Name = "col3_totalCableArea";
            this.col3_totalCableArea.ReadOnly = true;
            // 
            // col4_cableDuct
            // 
            this.col4_cableDuct.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col4_cableDuct.HeaderText = "Suggested Cable Duct";
            this.col4_cableDuct.Name = "col4_cableDuct";
            this.col4_cableDuct.ReadOnly = true;
            // 
            // Form_detailResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(802, 564);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_detailResult";
            this.Text = "Detail Result";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_result)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1_Wiring.ResumeLayout(false);
            this.tabPage2_CableDuct.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel ts_export;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dgv_result;
        private System.Windows.Forms.DataGridViewTextBoxColumn col1_round;
        private System.Windows.Forms.DataGridViewTextBoxColumn col2_method;
        private System.Windows.Forms.DataGridViewTextBoxColumn col3_length;
        private System.Windows.Forms.DataGridViewTextBoxColumn col4_curve;
        private System.Windows.Forms.DataGridViewTextBoxColumn col5_segments;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1_Wiring;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabPage2_CableDuct;
        private System.Windows.Forms.DataGridViewTextBoxColumn col1_tagNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn col2_system;
        private System.Windows.Forms.DataGridViewTextBoxColumn col3_cableSpec;
        private System.Windows.Forms.DataGridViewTextBoxColumn col4_from;
        private System.Windows.Forms.DataGridViewTextBoxColumn col5_to;
        private System.Windows.Forms.DataGridViewTextBoxColumn col6_signal;
        private System.Windows.Forms.DataGridViewTextBoxColumn col7_cableLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn col8_PIDNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn col9_dwgNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn col10_remark;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn col1_segment;
        private System.Windows.Forms.DataGridViewTextBoxColumn col2_length;
        private System.Windows.Forms.DataGridViewTextBoxColumn col3_totalCableArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn col4_cableDuct;
    }
}