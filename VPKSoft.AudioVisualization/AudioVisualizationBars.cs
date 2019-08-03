#region License
/*
MIT License

Copyright(c) 2019 Petteri Kautonen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
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

        private void PnBar_Paint(object sender, PaintEventArgs e)
        {
            if (CombineChannels && sender.Equals(pnRight))
            {
                return;
            }

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
                Brush brush;
                if (CombineChannels)
                {
                    brush = new SolidBrush(ColorAudioChannelLeft);
                }
                else
                {
                    brush = new SolidBrush(sender.Equals(pnLeft) ? ColorAudioChannelLeft : ColorAudioChannelRight);
                }

                using (brush)
                {
                    var barValues = CreateWeightedFftArray(HertzSpan, e.ClipRectangle.Height);

                    var values = sender.Equals(pnLeft) || CombineChannels ? barValues.left : barValues.right;


                    for (int i = 0; i < values.Count; i++)
                    {
                        int barValue = e.ClipRectangle.Bottom - (int) values[i];

                        e.Graphics.FillRectangle(brush,
                            new Rectangle((int)(i * barStep), barValue, (int)barStep - 1, e.ClipRectangle.Height - barValue));
                    }
                }

                if (CombineChannels)
                {
                    brush = new SolidBrush(ColorAudioChannelRight);

                    using (brush)
                    {
                        var barValues = CreateWeightedFftArray(HertzSpan, e.ClipRectangle.Height);

                        var values = barValues.right;


                        for (int i = 0; i < values.Count; i++)
                        {
                            int barValue = e.ClipRectangle.Bottom - (int) values[i];

                            e.Graphics.FillRectangle(brush,
                                new Rectangle((int)(i * barStep), barValue, (int)barStep - 1, e.ClipRectangle.Height - barValue));
                        }
                    }

                }
            }
        }

        private void PnKHzLabels_Paint(object sender, PaintEventArgs e)
        {
            PaintHertzLabels((Panel)sender, e, pnLeft.Left, pnLeft.Right);
        }
    }
}
