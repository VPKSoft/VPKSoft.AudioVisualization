namespace TestApplication
{
    partial class FormMain
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
            this.button1 = new System.Windows.Forms.Button();
            this.cbCombineChannels = new System.Windows.Forms.CheckBox();
            this.audioVisualizationPlot1 = new VPKSoft.AudioVisualization.AudioVisualizationPlot();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(12, 415);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // cbCombineChannels
            // 
            this.cbCombineChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbCombineChannels.AutoSize = true;
            this.cbCombineChannels.Location = new System.Drawing.Point(137, 421);
            this.cbCombineChannels.Name = "cbCombineChannels";
            this.cbCombineChannels.Size = new System.Drawing.Size(113, 17);
            this.cbCombineChannels.TabIndex = 2;
            this.cbCombineChannels.Text = "Combine channels";
            this.cbCombineChannels.UseVisualStyleBackColor = true;
            this.cbCombineChannels.CheckedChanged += new System.EventHandler(this.CbCombineChannels_CheckedChanged);
            // 
            // audioVisualizationPlot1
            // 
            this.audioVisualizationPlot1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.audioVisualizationPlot1.BackColor = System.Drawing.Color.Black;
            this.audioVisualizationPlot1.CombineChannels = false;
            this.audioVisualizationPlot1.Location = new System.Drawing.Point(-1, 12);
            this.audioVisualizationPlot1.Name = "audioVisualizationPlot1";
            this.audioVisualizationPlot1.Size = new System.Drawing.Size(789, 397);
            this.audioVisualizationPlot1.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cbCombineChannels);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.audioVisualizationPlot1);
            this.DoubleBuffered = true;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VPKSoft.AudioVisualization.AudioVisualizationPlot audioVisualizationPlot1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbCombineChannels;
    }
}

