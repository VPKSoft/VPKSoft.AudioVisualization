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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using VPKSoft.AudioVisualization.CommonClasses.BaseClasses;

namespace VPKSoft.AudioVisualization
{
    /// <summary>
    /// A control to visualize audio as a curve.
    /// Implements the <see cref="VPKSoft.AudioVisualization.CommonClasses.BaseClasses.AudioVisualizationBase" />
    /// </summary>
    /// <seealso cref="VPKSoft.AudioVisualization.CommonClasses.BaseClasses.AudioVisualizationBase" />
    public partial class AudioVisualizationPlot : AudioVisualizationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioVisualizationPlot"/> class.
        /// </summary>
        public AudioVisualizationPlot()
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

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        [Description("Gets or sets the background color for the control.")]
        [Browsable(true)]
        [Category("Appearance")]
        public override Color BackColor { get; set; } = Color.Black;

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

        private void PnLeft_Paint(object sender, PaintEventArgs e)
        {
            double width = ((Panel) sender).Width;
            double height = ((Panel) sender).Height;
            e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);
            if (ValidData)
            {
                List<Point> linePoints = GetPoints(true, width, height);
                e.Graphics.DrawLines(Pens.Aqua, linePoints.ToArray());

                if (CombineChannels)
                {
                    linePoints = GetPoints(false, width, height);
                    e.Graphics.DrawLines(Pens.LimeGreen, linePoints.ToArray());
                }
            }
        }

        private void PnRight_Paint(object sender, PaintEventArgs e)
        {
            if (CombineChannels)
            {
                return;
            }

            double width = ((Panel) sender).Width;
            double height = ((Panel) sender).Height;
            e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);
            if (ValidData)
            {
                List<Point> linePoints = GetPoints(true, width, height);
                e.Graphics.DrawLines(Pens.LimeGreen, linePoints.ToArray());
            }
        }

        private void PnKHzLabels_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);

            int hertz = (int) MaxHertz == 1 ? 24000 : (int)MaxHertz;

                int hertzLabelCount = hertz / 1000 / 2 + 1;

                double startPoint = pnLeft.Left - (e.Graphics.MeasureString("0", Font).Width / 2);

                double endPoint = (pnLeft.Right - pnLeft.Left) +
                                  (e.Graphics.MeasureString((hertz / 1000).ToString(), Font).Width / 2);

                double stepping = (endPoint - startPoint) / (hertzLabelCount - 1);

                double currentPoint = startPoint;

                for (int i = 0; i < hertzLabelCount; i++)
                {
                    if (i == 0)
                    {
                        e.Graphics.DrawString("0", Font, Brushes.DarkMagenta, (float)startPoint, 0);
                    }
                    else
                    {
                        e.Graphics.DrawString((i * 2).ToString(), Font, Brushes.DarkMagenta,
                            (float) currentPoint + (e.Graphics.MeasureString((i * 2).ToString(), Font).Width / 2), 0);
                    }

                    currentPoint += stepping;
                }
        }

        private void AudioVisualizationPlot_SizeChanged(object sender, EventArgs e)
        {
            pnKHzLabels.Refresh();
        }
    }
}
