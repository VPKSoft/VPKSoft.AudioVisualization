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
using System.Linq;
using System.Windows.Forms;
using VPKSoft.AudioVisualization;

namespace TestApplication
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            cmbFftWindowingStyle.Items.AddRange(Enum.GetValues(typeof(WindowType)).Cast<object>().ToArray());
            cmbFftWindowingStyle.SelectedIndex = 0;
        }

        private void CbCombineChannels_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            audioVisualizationPlot1.CombineChannels = checkBox.Checked;
            audioVisualizationBars1.CombineChannels = checkBox.Checked;
        }

        private void BtStartStop_Click(object sender, EventArgs e)
        {
            if (audioVisualizationPlot1.IsStarted)
            {
                audioVisualizationPlot1.Stop();
                btStartStopLine.Text = @"Start";
            }
            else
            {
                audioVisualizationPlot1.Start();
                btStartStopLine.Text = @"Stop";
            }
        }

        private void BtStartStopBars_Click(object sender, EventArgs e)
        {
            if (audioVisualizationBars1.IsStarted)
            {
                audioVisualizationBars1.Stop();
                btStartStopBars.Text = @"Start";
            }
            else
            {
                audioVisualizationBars1.Start();
                btStartStopBars.Text = @"Stop";
            }
        }

        private void RbGraph_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBoth.Checked)
            {
                tlpGraphs.RowStyles[0] = new RowStyle(SizeType.Percent, 50);
                tlpGraphs.RowStyles[1] = new RowStyle(SizeType.Percent, 50);
                audioVisualizationPlot1.Start();
                audioVisualizationBars1.Start();
            }
            else if (rbCurve.Checked)
            {
                tlpGraphs.RowStyles[0] = new RowStyle(SizeType.Percent, 100);
                tlpGraphs.RowStyles[1] = new RowStyle(SizeType.Percent, 0);
                audioVisualizationBars1.Stop();
                audioVisualizationPlot1.Start();
                btStartStopBars.Text = @"Stop";
            }
            else if (rbBars.Checked)
            {
                tlpGraphs.RowStyles[0] = new RowStyle(SizeType.Percent, 0);
                tlpGraphs.RowStyles[1] = new RowStyle(SizeType.Percent, 100);
                audioVisualizationPlot1.Stop();
                audioVisualizationBars1.Start();
                btStartStopLine.Text = @"Stop";
            }
        }

        private void CbUseGradientWithBars_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            audioVisualizationBars1.DrawWithGradient = checkBox.Checked;
        }

        private void cmbFftWindowingStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = (ComboBox) sender;
            if (combo.SelectedItem != null)
            {
                audioVisualizationBars1.FftWindowType = (WindowType) combo.SelectedItem;
                audioVisualizationPlot1.FftWindowType = (WindowType) combo.SelectedItem;
            }
        }

        private void cbBarLevelCropping_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            audioVisualizationBars1.MinorityCropOnBarLevel = checkBox.Checked;
            audioVisualizationPlot1.MinorityCropOnBarLevel = checkBox.Checked;
        }

        private void nudMinorityCrop_ValueChanged(object sender, EventArgs e)
        {
            var value = (int) ((NumericUpDown) sender).Value;
            audioVisualizationBars1.MinorityCropPercentage = value;
            audioVisualizationPlot1.MinorityCropPercentage = value;
        }
    }
}
