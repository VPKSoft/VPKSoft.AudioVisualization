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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btStartStopLine = new System.Windows.Forms.Button();
            this.cbCombineChannels = new System.Windows.Forms.CheckBox();
            this.btStartStopBars = new System.Windows.Forms.Button();
            this.tlpGraphs = new System.Windows.Forms.TableLayoutPanel();
            this.audioVisualizationPlot1 = new VPKSoft.AudioVisualization.AudioVisualizationPlot();
            this.audioVisualizationBars1 = new VPKSoft.AudioVisualization.AudioVisualizationBars();
            this.gbGrapsh = new System.Windows.Forms.GroupBox();
            this.rbBars = new System.Windows.Forms.RadioButton();
            this.rbCurve = new System.Windows.Forms.RadioButton();
            this.rbBoth = new System.Windows.Forms.RadioButton();
            this.cbUseGradientWithBars = new System.Windows.Forms.CheckBox();
            this.lbFftWindowingStyle = new System.Windows.Forms.Label();
            this.cmbFftWindowingStyle = new System.Windows.Forms.ComboBox();
            this.cbBarLevelCropping = new System.Windows.Forms.CheckBox();
            this.lbMinorityCrop = new System.Windows.Forms.Label();
            this.nudMinorityCrop = new System.Windows.Forms.NumericUpDown();
            this.tlpGraphs.SuspendLayout();
            this.gbGrapsh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinorityCrop)).BeginInit();
            this.SuspendLayout();
            // 
            // btStartStopLine
            // 
            this.btStartStopLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btStartStopLine.Location = new System.Drawing.Point(12, 427);
            this.btStartStopLine.Name = "btStartStopLine";
            this.btStartStopLine.Size = new System.Drawing.Size(75, 23);
            this.btStartStopLine.TabIndex = 1;
            this.btStartStopLine.Text = "Start";
            this.btStartStopLine.UseVisualStyleBackColor = true;
            this.btStartStopLine.Click += new System.EventHandler(this.BtStartStop_Click);
            // 
            // cbCombineChannels
            // 
            this.cbCombineChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbCombineChannels.AutoSize = true;
            this.cbCombineChannels.Location = new System.Drawing.Point(180, 427);
            this.cbCombineChannels.Name = "cbCombineChannels";
            this.cbCombineChannels.Size = new System.Drawing.Size(113, 17);
            this.cbCombineChannels.TabIndex = 2;
            this.cbCombineChannels.Text = "Combine channels";
            this.cbCombineChannels.UseVisualStyleBackColor = true;
            this.cbCombineChannels.CheckedChanged += new System.EventHandler(this.CbCombineChannels_CheckedChanged);
            // 
            // btStartStopBars
            // 
            this.btStartStopBars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btStartStopBars.Location = new System.Drawing.Point(93, 427);
            this.btStartStopBars.Name = "btStartStopBars";
            this.btStartStopBars.Size = new System.Drawing.Size(75, 23);
            this.btStartStopBars.TabIndex = 4;
            this.btStartStopBars.Text = "Start";
            this.btStartStopBars.UseVisualStyleBackColor = true;
            this.btStartStopBars.Click += new System.EventHandler(this.BtStartStopBars_Click);
            // 
            // tlpGraphs
            // 
            this.tlpGraphs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpGraphs.ColumnCount = 1;
            this.tlpGraphs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGraphs.Controls.Add(this.audioVisualizationPlot1, 0, 0);
            this.tlpGraphs.Controls.Add(this.audioVisualizationBars1, 0, 1);
            this.tlpGraphs.Location = new System.Drawing.Point(12, 12);
            this.tlpGraphs.Name = "tlpGraphs";
            this.tlpGraphs.RowCount = 2;
            this.tlpGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpGraphs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpGraphs.Size = new System.Drawing.Size(776, 409);
            this.tlpGraphs.TabIndex = 6;
            // 
            // audioVisualizationPlot1
            // 
            this.audioVisualizationPlot1.ColorAudioChannelLeft = System.Drawing.Color.Aqua;
            this.audioVisualizationPlot1.ColorAudioChannelRight = System.Drawing.Color.LimeGreen;
            this.audioVisualizationPlot1.ColorHertzLabels = System.Drawing.Color.Magenta;
            this.audioVisualizationPlot1.CombineChannels = false;
            this.audioVisualizationPlot1.CustomWindowFunc = ((System.Func<int, double[]>)(resources.GetObject("audioVisualizationPlot1.CustomWindowFunc")));
            this.audioVisualizationPlot1.DisplayHertzLabels = true;
            this.audioVisualizationPlot1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.audioVisualizationPlot1.FftWindowType = VPKSoft.AudioVisualization.WindowType.Hanning;
            this.audioVisualizationPlot1.Location = new System.Drawing.Point(3, 3);
            this.audioVisualizationPlot1.MinorityCropOnBarLevel = false;
            this.audioVisualizationPlot1.MinorityCropPercentage = 3;
            this.audioVisualizationPlot1.MinorityPercentageStepping = 1000;
            this.audioVisualizationPlot1.Name = "audioVisualizationPlot1";
            this.audioVisualizationPlot1.NoiseTolerance = 1D;
            this.audioVisualizationPlot1.RefreshRate = 30;
            this.audioVisualizationPlot1.Size = new System.Drawing.Size(770, 198);
            this.audioVisualizationPlot1.TabIndex = 0;
            this.audioVisualizationPlot1.UseAntiAliasing = true;
            // 
            // audioVisualizationBars1
            // 
            this.audioVisualizationBars1.ColorAudioChannelLeft = System.Drawing.Color.Aqua;
            this.audioVisualizationBars1.ColorAudioChannelRight = System.Drawing.Color.LimeGreen;
            this.audioVisualizationBars1.ColorGradientLeftEnd = System.Drawing.Color.DarkGreen;
            this.audioVisualizationBars1.ColorGradientLeftStart = System.Drawing.Color.SpringGreen;
            this.audioVisualizationBars1.ColorGradientRightEnd = System.Drawing.Color.MidnightBlue;
            this.audioVisualizationBars1.ColorGradientRightStart = System.Drawing.Color.LightSteelBlue;
            this.audioVisualizationBars1.ColorHertzLabels = System.Drawing.Color.Magenta;
            this.audioVisualizationBars1.CombineChannels = false;
            this.audioVisualizationBars1.CustomWindowFunc = ((System.Func<int, double[]>)(resources.GetObject("audioVisualizationBars1.CustomWindowFunc")));
            this.audioVisualizationBars1.DisplayHertzLabels = true;
            this.audioVisualizationBars1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.audioVisualizationBars1.DrawWithGradient = false;
            this.audioVisualizationBars1.FftWindowType = VPKSoft.AudioVisualization.WindowType.Hanning;
            this.audioVisualizationBars1.HertzSpan = 128;
            this.audioVisualizationBars1.Location = new System.Drawing.Point(3, 207);
            this.audioVisualizationBars1.MinorityCropOnBarLevel = false;
            this.audioVisualizationBars1.MinorityCropPercentage = 3;
            this.audioVisualizationBars1.MinorityPercentageStepping = 1000;
            this.audioVisualizationBars1.Name = "audioVisualizationBars1";
            this.audioVisualizationBars1.NoiseTolerance = 1D;
            this.audioVisualizationBars1.RefreshRate = 30;
            this.audioVisualizationBars1.RelativeView = true;
            this.audioVisualizationBars1.Size = new System.Drawing.Size(770, 199);
            this.audioVisualizationBars1.TabIndex = 5;
            // 
            // gbGrapsh
            // 
            this.gbGrapsh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbGrapsh.Controls.Add(this.rbBars);
            this.gbGrapsh.Controls.Add(this.rbCurve);
            this.gbGrapsh.Controls.Add(this.rbBoth);
            this.gbGrapsh.Location = new System.Drawing.Point(406, 427);
            this.gbGrapsh.Name = "gbGrapsh";
            this.gbGrapsh.Size = new System.Drawing.Size(382, 39);
            this.gbGrapsh.TabIndex = 7;
            this.gbGrapsh.TabStop = false;
            this.gbGrapsh.Text = "Grapsh";
            // 
            // rbBars
            // 
            this.rbBars.AutoSize = true;
            this.rbBars.Location = new System.Drawing.Point(278, 16);
            this.rbBars.Name = "rbBars";
            this.rbBars.Size = new System.Drawing.Size(46, 17);
            this.rbBars.TabIndex = 2;
            this.rbBars.Text = "Bars";
            this.rbBars.UseVisualStyleBackColor = true;
            this.rbBars.CheckedChanged += new System.EventHandler(this.RbGraph_CheckedChanged);
            // 
            // rbCurve
            // 
            this.rbCurve.AutoSize = true;
            this.rbCurve.Location = new System.Drawing.Point(144, 16);
            this.rbCurve.Name = "rbCurve";
            this.rbCurve.Size = new System.Drawing.Size(53, 17);
            this.rbCurve.TabIndex = 1;
            this.rbCurve.Text = "Curve";
            this.rbCurve.UseVisualStyleBackColor = true;
            this.rbCurve.CheckedChanged += new System.EventHandler(this.RbGraph_CheckedChanged);
            // 
            // rbBoth
            // 
            this.rbBoth.AutoSize = true;
            this.rbBoth.Checked = true;
            this.rbBoth.Location = new System.Drawing.Point(3, 16);
            this.rbBoth.Name = "rbBoth";
            this.rbBoth.Size = new System.Drawing.Size(47, 17);
            this.rbBoth.TabIndex = 0;
            this.rbBoth.TabStop = true;
            this.rbBoth.Text = "Both";
            this.rbBoth.UseVisualStyleBackColor = true;
            this.rbBoth.CheckedChanged += new System.EventHandler(this.RbGraph_CheckedChanged);
            // 
            // cbUseGradientWithBars
            // 
            this.cbUseGradientWithBars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbUseGradientWithBars.AutoSize = true;
            this.cbUseGradientWithBars.Location = new System.Drawing.Point(180, 450);
            this.cbUseGradientWithBars.Name = "cbUseGradientWithBars";
            this.cbUseGradientWithBars.Size = new System.Drawing.Size(162, 17);
            this.cbUseGradientWithBars.TabIndex = 8;
            this.cbUseGradientWithBars.Text = "Use gradient colors with bars";
            this.cbUseGradientWithBars.UseVisualStyleBackColor = true;
            this.cbUseGradientWithBars.CheckedChanged += new System.EventHandler(this.CbUseGradientWithBars_CheckedChanged);
            // 
            // lbFftWindowingStyle
            // 
            this.lbFftWindowingStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbFftWindowingStyle.AutoSize = true;
            this.lbFftWindowingStyle.Location = new System.Drawing.Point(406, 475);
            this.lbFftWindowingStyle.Name = "lbFftWindowingStyle";
            this.lbFftWindowingStyle.Size = new System.Drawing.Size(82, 13);
            this.lbFftWindowingStyle.TabIndex = 9;
            this.lbFftWindowingStyle.Text = "FFT windowing:";
            // 
            // cmbFftWindowingStyle
            // 
            this.cmbFftWindowingStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbFftWindowingStyle.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbFftWindowingStyle.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbFftWindowingStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFftWindowingStyle.FormattingEnabled = true;
            this.cmbFftWindowingStyle.Location = new System.Drawing.Point(574, 472);
            this.cmbFftWindowingStyle.Name = "cmbFftWindowingStyle";
            this.cmbFftWindowingStyle.Size = new System.Drawing.Size(214, 21);
            this.cmbFftWindowingStyle.TabIndex = 10;
            this.cmbFftWindowingStyle.SelectedIndexChanged += new System.EventHandler(this.cmbFftWindowingStyle_SelectedIndexChanged);
            // 
            // cbBarLevelCropping
            // 
            this.cbBarLevelCropping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbBarLevelCropping.AutoSize = true;
            this.cbBarLevelCropping.Location = new System.Drawing.Point(180, 473);
            this.cbBarLevelCropping.Name = "cbBarLevelCropping";
            this.cbBarLevelCropping.Size = new System.Drawing.Size(226, 17);
            this.cbBarLevelCropping.TabIndex = 11;
            this.cbBarLevelCropping.Text = "Use bar level cropping (Bar graph needed)";
            this.cbBarLevelCropping.UseVisualStyleBackColor = true;
            this.cbBarLevelCropping.CheckedChanged += new System.EventHandler(this.cbBarLevelCropping_CheckedChanged);
            // 
            // lbMinorityCrop
            // 
            this.lbMinorityCrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbMinorityCrop.AutoSize = true;
            this.lbMinorityCrop.Location = new System.Drawing.Point(12, 454);
            this.lbMinorityCrop.Name = "lbMinorityCrop";
            this.lbMinorityCrop.Size = new System.Drawing.Size(127, 13);
            this.lbMinorityCrop.TabIndex = 12;
            this.lbMinorityCrop.Text = "Minority crop percentage:";
            // 
            // nudMinorityCrop
            // 
            this.nudMinorityCrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudMinorityCrop.Location = new System.Drawing.Point(15, 470);
            this.nudMinorityCrop.Name = "nudMinorityCrop";
            this.nudMinorityCrop.Size = new System.Drawing.Size(120, 20);
            this.nudMinorityCrop.TabIndex = 13;
            this.nudMinorityCrop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudMinorityCrop.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudMinorityCrop.ValueChanged += new System.EventHandler(this.nudMinorityCrop_ValueChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 503);
            this.Controls.Add(this.nudMinorityCrop);
            this.Controls.Add(this.lbMinorityCrop);
            this.Controls.Add(this.cbBarLevelCropping);
            this.Controls.Add(this.cmbFftWindowingStyle);
            this.Controls.Add(this.lbFftWindowingStyle);
            this.Controls.Add(this.cbUseGradientWithBars);
            this.Controls.Add(this.gbGrapsh);
            this.Controls.Add(this.tlpGraphs);
            this.Controls.Add(this.btStartStopBars);
            this.Controls.Add(this.cbCombineChannels);
            this.Controls.Add(this.btStartStopLine);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "Test application (VPKSoft.AudioVisualization)";
            this.tlpGraphs.ResumeLayout(false);
            this.gbGrapsh.ResumeLayout(false);
            this.gbGrapsh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinorityCrop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VPKSoft.AudioVisualization.AudioVisualizationPlot audioVisualizationPlot1;
        private System.Windows.Forms.Button btStartStopLine;
        private System.Windows.Forms.CheckBox cbCombineChannels;
        private System.Windows.Forms.Button btStartStopBars;
        private VPKSoft.AudioVisualization.AudioVisualizationBars audioVisualizationBars1;
        private System.Windows.Forms.TableLayoutPanel tlpGraphs;
        private System.Windows.Forms.GroupBox gbGrapsh;
        private System.Windows.Forms.RadioButton rbBars;
        private System.Windows.Forms.RadioButton rbCurve;
        private System.Windows.Forms.RadioButton rbBoth;
        private System.Windows.Forms.CheckBox cbUseGradientWithBars;
        private System.Windows.Forms.Label lbFftWindowingStyle;
        private System.Windows.Forms.ComboBox cmbFftWindowingStyle;
        private System.Windows.Forms.CheckBox cbBarLevelCropping;
        private System.Windows.Forms.Label lbMinorityCrop;
        private System.Windows.Forms.NumericUpDown nudMinorityCrop;
    }
}

