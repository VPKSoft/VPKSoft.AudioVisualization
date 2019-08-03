using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VPKSoft.AudioVisualization.CommonClasses.BaseClasses;

namespace VPKSoft.AudioVisualization
{
    public partial class AudioVisualizationBars : AudioVisualizationBase
    {
        public AudioVisualizationBars()
        {
            InitializeComponent();
            tmVisualize.Tick += TmVisualize_Tick;

            try
            {
                var propertyInfo = typeof(Panel)
                    .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(pnLeft, true);
                    propertyInfo.SetValue(pnRight, true);
                    propertyInfo.SetValue(pnKHzLabels, true);
                }
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
            }
        }

        private void TmVisualize_Tick(object sender, EventArgs e)
        {
            if (!ValidData)
            {
                return;
            }

            tmVisualize.Enabled = false;
            pnLeft.Refresh();
            pnRight.Refresh();
            tmVisualize.Enabled = true;
        }

        private bool combineChannels;

        /// <summary>
        /// Gets or set a value whether to display the left and right channels as combined.
        /// </summary>
        public bool CombineChannels
        {
            get => combineChannels;
            set
            {
                if (value != combineChannels)
                {
                    combineChannels = value;
                    if (value)
                    {
                        pnRight.Visible = false;
                        tlpMain.SetRowSpan(pnLeft, 2);
                    }
                    else
                    {
                        tlpMain.SetRowSpan(pnLeft, 1);
                        pnRight.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the graph bars in hertz.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the width of the graph bars in hertz.")]
        [Category("Appearance")]
        public int HertzSpan { get; set; } = 64;

        private void PnLeft_Paint(object sender, PaintEventArgs e)
        {
            double width = ((Panel) sender).Width;
            double height = ((Panel) sender).Height;

            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }

            if (HertzSpan == 0)
            {
                return;
            }

            double barStep = (double)e.ClipRectangle.Width / HertzSpan;

            if (ValidData)
            {
                using (var brush = new SolidBrush(ColorAudioChannelLeft))
                {
                    List<double> barValues = CreateWeightedFftArray(true, HertzSpan, e.ClipRectangle.Height);

                    for (int i = 0; i < barValues.Count; i++)
                    {
                        int barValue = e.ClipRectangle.Bottom - (int) barValues[i];

                        e.Graphics.FillRectangle(brush,
                            new Rectangle((int)((double)i * barStep), barValue, (int)barStep - 1, e.ClipRectangle.Height - barValue));
                    }
                }
            }
        }

        private void PnRight_Paint(object sender, PaintEventArgs e)
        {
        }

        private void PnKHzLabels_Paint(object sender, PaintEventArgs e)
        {
            PaintHertzLabels((Panel)sender, e, pnLeft.Left, pnLeft.Right);
        }
    }
}
