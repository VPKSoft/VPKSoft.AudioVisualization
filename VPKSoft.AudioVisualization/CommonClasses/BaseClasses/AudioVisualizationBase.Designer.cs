namespace VPKSoft.AudioVisualization.CommonClasses.BaseClasses
{
    partial class AudioVisualizationBase
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
            this.components = new System.ComponentModel.Container();
            this.tmVisualize = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmVisualize
            // 
            this.tmVisualize.Interval = 30;
            // 
            // AudioVisualizationBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AudioVisualizationBase";
            this.SizeChanged += new System.EventHandler(this.AudioVisualizationBase_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Timer tmVisualize;
    }
}
