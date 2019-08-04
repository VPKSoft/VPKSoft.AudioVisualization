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
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using VPKSoft.AudioVisualization.CommonClasses;
using VPKSoft.AudioVisualization.CommonClasses.BaseClasses;

namespace VPKSoft.AudioVisualization
{
    /// <summary>
    /// A control to visualize audio as a bars.
    /// Implements the <see cref="VPKSoft.AudioVisualization.CommonClasses.BaseClasses.AudioVisualizationBase" />
    /// </summary>
    /// <seealso cref="VPKSoft.AudioVisualization.CommonClasses.BaseClasses.AudioVisualizationBase" />
    public partial class AudioVisualizationBars : AudioVisualizationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioVisualizationBars"/> class.
        /// </summary>
        public AudioVisualizationBars()
        {
            InitializeComponent();
            tmVisualize.Tick += TmVisualize_Tick;
            Disposed += AudioVisualizationBars_Disposed;
            ShouldResizeChildren += AudioVisualizationBars_ShouldResizeChildren;

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

        private void AudioVisualizationBars_ShouldResizeChildren(object sender, EventArgs e)
        {
            if (DisplayHertzLabels)
            {
                tlpMain.RowStyles[2] = new RowStyle(SizeType.Absolute, HertzLabelsHeight);
            }
        }

        private void AudioVisualizationBars_Disposed(object sender, EventArgs e)
        {
            tmVisualize.Tick -= TmVisualize_Tick;
            Disposed -= AudioVisualizationBars_Disposed;
            // ReSharper disable once DelegateSubtraction
            ShouldResizeChildren -= AudioVisualizationBars_ShouldResizeChildren;
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
            RaiseDataCalculatedEvent(this, new DataCalculatedEventArgs {PeakFrequency = GetPeakFrequency()});
            tmVisualize.Enabled = true;
        }


        /// <summary>
        /// Gets or sets a value indicating whether to display the hertz labels.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets a value indicating whether to display the hertz labels.")]
        [Category("Appearance")]
        public override bool DisplayHertzLabels
        {
            get => pnKHzLabels.Visible;
            set
            {
                if (pnKHzLabels.Visible != value)
                {
                    if (value)
                    {
                        tlpMain.RowStyles[2] = new RowStyle(SizeType.Absolute, 20);
                        pnKHzLabels.Visible = true;
                        tlpMain.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 20);
                        tlpMain.ColumnStyles[2] = new ColumnStyle(SizeType.Absolute, 20);
                    }
                    else
                    {
                        pnKHzLabels.Visible = false;
                        tlpMain.RowStyles[2] = new RowStyle(SizeType.Absolute, 0);
                        tlpMain.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 0);
                        tlpMain.ColumnStyles[2] = new ColumnStyle(SizeType.Absolute, 0);
                    }
                }
            }
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

        /// <summary>
        /// Gets or sets a value indicating whether to draw the bars using gradient color.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets a value indicating whether to draw the bars using gradient color.")]
        [Category("Appearance")]
        public bool DrawWithGradient { get; set; }

        /// <summary>
        /// Gets or sets the left channel gradient starting color.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the left channel gradient starting color.")]
        [Category("Appearance")]
        public Color ColorGradientLeftStart { get; set; } = Color.LightSteelBlue;

        /// <summary>
        /// Gets or sets the left channel gradient ending color.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the left channel gradient ending color.")]
        [Category("Appearance")]
        public Color ColorGradientLeftEnd { get; set; } = Color.MidnightBlue;

        /// <summary>
        /// Gets or sets the right channel gradient starting color.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the right channel gradient starting color.")]
        [Category("Appearance")]
        public Color ColorGradientRightStart { get; set; } = Color.LimeGreen;

        /// <summary>
        /// Gets or sets the right channel gradient ending color.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the right channel gradient ending color.")]
        [Category("Appearance")]
        public Color ColorGradientRightEnd { get; set; } = Color.DarkGreen;

        /// <summary>
        /// Creates a brush based on the given parameters.
        /// </summary>
        /// <param name="left">if set to <c>true</c> the left channel's brush is requested.</param>
        /// <param name="height">The height of the draw area.</param>
        /// <returns>A <see cref="Brush"/> instance based on the given parameters.</returns>
        private Brush CreateBrush(bool left, int height)
        {
            if (DrawWithGradient)
            {
                return new LinearGradientBrush(
                    new Point(0, 0),
                    new Point(0, height),
                    left ? ColorGradientLeftEnd : ColorGradientRightEnd,
                    left ? ColorGradientLeftStart : ColorGradientRightStart);
            }

            return CombineChannels
                ? new SolidBrush(ColorAudioChannelLeft)
                : new SolidBrush(left ? ColorAudioChannelLeft : ColorAudioChannelRight);
        }

        /// <summary>
        /// Draws the bars audio visualization bars.
        /// </summary>
        /// <param name="left">if set to <c>true</c> the left channel bars are drawn.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        private void DrawBars(bool left, PaintEventArgs e)
        {
            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }

            if (HertzSpan == 0)
            {
                return;
            }

            try
            {
                double barStep = (double) e.ClipRectangle.Width / HertzSpan;

                if (ValidData)
                {
                    Brush brush = CreateBrush(left, e.ClipRectangle.Height);

                    using (brush)
                    {
                        var barValues = CreateWeightedFftArray(HertzSpan, e.ClipRectangle.Height);

                        var values = left || CombineChannels ? barValues.left : barValues.right;


                        for (int i = 0; i < values.Count; i++)
                        {
                            int barValue = e.ClipRectangle.Bottom - (int) values[i];

                            e.Graphics.FillRectangle(brush,
                                new Rectangle((int) (i * barStep), barValue, (int) barStep - 1,
                                    e.ClipRectangle.Height - barValue));
                        }
                    }

                    if (CombineChannels)
                    {
                        brush = CreateBrush(left, e.ClipRectangle.Height);

                        using (brush)
                        {
                            var barValues = CreateWeightedFftArray(HertzSpan, e.ClipRectangle.Height);

                            var values = barValues.right;


                            for (int i = 0; i < values.Count; i++)
                            {
                                int barValue = e.ClipRectangle.Bottom - (int) values[i];

                                e.Graphics.FillRectangle(brush,
                                    new Rectangle((int) (i * barStep), barValue, (int) barStep - 1,
                                        e.ClipRectangle.Height - barValue));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
            }
        }

        private void PnKHzLabels_Paint(object sender, PaintEventArgs e)
        {
            if (DisplayHertzLabels)
            {
                PaintHertzLabels((Panel) sender, e, pnLeft.Left, pnLeft.Right);
            }
        }

        private void PnLeft_Paint(object sender, PaintEventArgs e)
        {
            DrawBars(true, e);
        }

        private void PnRight_Paint(object sender, PaintEventArgs e)
        {
            if (CombineChannels)
            {
                return;
            }

            DrawBars(false, e);
        }
    }
}
