#region License
/*
MIT License

Copyright(c) 2021 Petteri Kautonen

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
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using VPKSoft.AudioVisualization.CommonClasses;
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
            Disposed += AudioVisualizationPlot_Disposed;
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

            CalculateHertzLabelHeight(pnKHzLabels.Handle, pnKHzLabels.Font);
        }

        private void AudioVisualizationBars_ShouldResizeChildren(object sender, EventArgs e)
        {
            if (DisplayHertzLabels)
            {
                tlpMain.RowStyles[2] = new RowStyle(SizeType.Absolute, HertzLabelsHeight);
            }
        }

        private void AudioVisualizationPlot_Disposed(object sender, EventArgs e)
        {
            Disposed -= AudioVisualizationPlot_Disposed;
            tmVisualize.Tick -= TmVisualize_Tick;
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

        /// <summary>
        /// Gets or sets a value whether to use anti-aliasing with the curve drawing.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets a value whether to use anti-aliasing with the curve drawing.")]
        [Category("Appearance")]
        public bool UseAntiAliasing { get; set; } = true;

        /// <summary>
        /// Gets or sets the line draw mode of a line type graph.
        /// </summary>
        [Description("Gets or sets the line draw mode of a line type graph.")]
        [Browsable(true)]
        [Category("Appearance")]
        public LineDrawMode LineDrawMode { get; set; }

        /// <summary>
        /// Gets or sets the value that specifies the tension of the curve.
        /// </summary>
        [Browsable(true)]
        [Category("Gets or sets the value that specifies the tension of the curve.")]
        public float LineCurveTension { get; set; } = 1;

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
            try
            {
                if (UseAntiAliasing)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                }

                double width = ((Panel) sender).Width;
                double height = ((Panel) sender).Height;

                using (var brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillRectangle(brush, e.ClipRectangle);
                }

                if (ValidData)
                {
                    List<Point> linePoints = GetPoints(true, width, height);

                    using (var pen = new Pen(ColorAudioChannelLeft))
                    {
                        if (LineDrawMode == LineDrawMode.Curve)
                        {
                            e.Graphics.DrawCurve(pen, linePoints.ToArray(), LineCurveTension);
                        }
                        
                        if (LineDrawMode == LineDrawMode.Line)
                        {
                            e.Graphics.DrawLines(pen, linePoints.ToArray());
                        }
                    }

                    if (CombineChannels)
                    {
                        linePoints =  GetPoints(true, width, height);

                        using (var pen = new Pen(ColorAudioChannelRight))
                        {
                            if (LineDrawMode == LineDrawMode.Curve)
                            {
                                e.Graphics.DrawCurve(pen, linePoints.ToArray(), LineCurveTension);
                            }
                        
                            if (LineDrawMode == LineDrawMode.Line)
                            {
                                e.Graphics.DrawLines(pen, linePoints.ToArray());
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

        private void PnRight_Paint(object sender, PaintEventArgs e)
        {
            if (UseAntiAliasing)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            try
            {
                if (CombineChannels)
                {
                    return;
                }

                double width = ((Panel) sender).Width;
                double height = ((Panel) sender).Height;
                using (var brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillRectangle(brush, e.ClipRectangle);
                }

                if (ValidData)
                {
                    List<Point> linePoints = GetPoints(false, width, height);

                    using (var pen = new Pen(ColorAudioChannelRight))
                    {
                        if (LineDrawMode == LineDrawMode.Curve)
                        {
                            e.Graphics.DrawCurve(pen, linePoints.ToArray(), LineCurveTension);
                        }
                        
                        if (LineDrawMode == LineDrawMode.Line)
                        {
                            e.Graphics.DrawLines(pen, linePoints.ToArray());
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

        private void AudioVisualizationPlot_SizeChanged(object sender, EventArgs e)
        {
            ResetRelativeView();
        }
    }
}
