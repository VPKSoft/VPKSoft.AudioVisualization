namespace VPKSoft.AudioVisualization
{
    partial class AudioVisualizationPlot
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnRight = new System.Windows.Forms.Panel();
            this.pnLeft = new System.Windows.Forms.Panel();
            this.pnKHzLabels = new System.Windows.Forms.Panel();
            this.tlpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 3;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Controls.Add(this.pnRight, 1, 1);
            this.tlpMain.Controls.Add(this.pnLeft, 1, 0);
            this.tlpMain.Controls.Add(this.pnKHzLabels, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(734, 199);
            this.tlpMain.TabIndex = 0;
            // 
            // pnRight
            // 
            this.pnRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnRight.Location = new System.Drawing.Point(20, 89);
            this.pnRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnRight.Name = "pnRight";
            this.pnRight.Size = new System.Drawing.Size(694, 89);
            this.pnRight.TabIndex = 1;
            this.pnRight.Paint += new System.Windows.Forms.PaintEventHandler(this.PnRight_Paint);
            // 
            // pnLeft
            // 
            this.pnLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnLeft.Location = new System.Drawing.Point(20, 0);
            this.pnLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnLeft.Name = "pnLeft";
            this.pnLeft.Size = new System.Drawing.Size(694, 89);
            this.pnLeft.TabIndex = 0;
            this.pnLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.PnLeft_Paint);
            // 
            // pnKHzLabels
            // 
            this.tlpMain.SetColumnSpan(this.pnKHzLabels, 3);
            this.pnKHzLabels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnKHzLabels.Location = new System.Drawing.Point(0, 178);
            this.pnKHzLabels.Margin = new System.Windows.Forms.Padding(0);
            this.pnKHzLabels.Name = "pnKHzLabels";
            this.pnKHzLabels.Size = new System.Drawing.Size(734, 21);
            this.pnKHzLabels.TabIndex = 2;
            this.pnKHzLabels.Paint += new System.Windows.Forms.PaintEventHandler(this.PnKHzLabels_Paint);
            // 
            // AudioVisualizationPlot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "AudioVisualizationPlot";
            this.Size = new System.Drawing.Size(734, 199);
            this.SizeChanged += new System.EventHandler(this.AudioVisualizationPlot_SizeChanged);
            this.tlpMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnRight;
        private System.Windows.Forms.Panel pnLeft;
        private System.Windows.Forms.Panel pnKHzLabels;
    }
}
