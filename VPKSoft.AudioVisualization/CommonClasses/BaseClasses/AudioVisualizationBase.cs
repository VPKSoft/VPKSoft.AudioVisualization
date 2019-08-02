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
using System.Linq;
using System.Windows.Forms;
using NAudio.Wave;

namespace VPKSoft.AudioVisualization.CommonClasses.BaseClasses
{
    // (C)::https://github.com/swharden/ScottPlot
    // (C)::https://github.com/swharden/Csharp-Data-Visualization
    // (C)::https://github.com/naudio/NAudio
    /// <summary>
    /// A base class for the audio visualization controls.
    /// Implements the <see cref="System.Windows.Forms.UserControl" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.UserControl" />
    public partial class AudioVisualizationBase : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioVisualizationBase"/> class.
        /// </summary>
        public AudioVisualizationBase()
        {
            InitializeComponent();
            Disposed += AudioVisualizationBase_Disposed;
        }

        /// <summary>
        /// Handles the Disposed event of the AudioVisualizationBase control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AudioVisualizationBase_Disposed(object sender, EventArgs e)
        {
            Disposed -= AudioVisualizationBase_Disposed;
            Stop();
        }

        /// <summary>
        /// Gets or sets the action to be used to log an exception.
        /// </summary>
        public static Action<Exception> ExceptionLogAction { get; set; }

        // ReSharper disable once IdentifierTypo
        private WasapiLoopbackCapture WasapiLoopbackCapture { get; set; }

        /// <summary>
        /// Gets or sets the PCM (Pulse Code Modulation) data recorded from the <see cref="DataAvailable"/> event (Left channel stereo).
        /// </summary>
        internal double[] DataPcmLeft { get; set; }

        /// <summary>
        /// Gets or sets the FFT data (the audio encoded with Fast Fourier Transformation, Left channel stereo).
        /// </summary>
        internal double[] DataFftLeft { get; set; }

        /// <summary>
        /// Gets or sets the PCM (Pulse Code Modulation) data recorded from the <see cref="DataAvailable"/> event (Right channel stereo).
        /// </summary>
        internal double[] DataPcmRight { get; set; }

        /// <summary>
        /// Gets or sets the FFT data (the audio encoded with Fast Fourier Transformation, Right channel stereo).
        /// </summary>
        internal double[] DataFftRight { get; set; }

        // ReSharper disable once CommentTypo
        /// <summary>
        /// Gets the value of how many bytes per sample the WASAPI capture has.
        /// </summary>
        internal int BytesPerSample => WasapiLoopbackCapture?.WaveFormat.BitsPerSample / 8 ?? 4;

        /// <summary>
        /// Gets the value of how many points is between one hertz of the FTT (Fast Fourier Transform Data).
        /// </summary>
        internal double PointSpacingHz
        {
            get
            {
                if (WasapiLoopbackCapture != null && DataFftLeft != null && DataFftLeft.Length > 0)
                {
                    return (double) WasapiLoopbackCapture?.WaveFormat.SampleRate / DataFftLeft.Length / 2;
                }

                return 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is valid FFT data (the audio encoded with Fast Fourier Transformation) available for rendering.
        /// </summary>
        /// <value><c>true</c> if [there is valid FFT data (the audio encoded with Fast Fourier Transformation) available for rendering; otherwise, <c>false</c>.</value>
        internal bool ValidData => !(DataFftLeft == null || DataFftLeft.Length == 0);

        // ReSharper disable once CommentTypo
        /// <summary>
        /// Starts the WASAPI loop capture and the timer.
        /// </summary>
        public void Start()
        {
            try
            {
                if (WasapiLoopbackCapture == null)
                {
                    WasapiLoopbackCapture = new WasapiLoopbackCapture();
                    WasapiLoopbackCapture.DataAvailable += DataAvailable;
                    WasapiLoopbackCapture.StartRecording();
                    tmVisualize.Enabled = true;
                    Channels = WasapiLoopbackCapture.WaveFormat.Channels;
                    BitsPerSample = WasapiLoopbackCapture.WaveFormat.BitsPerSample;
                }
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
                Channels = 1;
                BitsPerSample = 0;
            }
        }

        // ReSharper disable once CommentTypo
        /// <summary>
        /// Stops the WASAPI loop capture and the timer.
        /// </summary>
        public void Stop()
        {
            try
            {
                if (WasapiLoopbackCapture != null)
                {
                    using (WasapiLoopbackCapture)
                    {
                        WasapiLoopbackCapture = new WasapiLoopbackCapture();
                        WasapiLoopbackCapture.DataAvailable -= DataAvailable;
                        WasapiLoopbackCapture.StopRecording();
                        tmVisualize.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
            }
            Channels = 1;
            BitsPerSample = 0;
        }

        /// <summary>
        /// Gets the amount of channels in a <see cref="WasapiLoopbackCapture"/>.
        /// </summary>
        [Browsable(false)]
        public int Channels { get; private set; } = 1;

        /// <summary>
        /// Gets the number of bits per sample (usually 16 or 32, sometimes 24 or 8).
        /// </summary>
        [Browsable(false)]
        public int BitsPerSample { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the audio is in mono format.
        /// </summary>
        [Browsable(false)]
        public bool IsMonoAudio => Channels == 1;

        /// <summary>
        /// Occurs when there is data available in the <see cref="WasapiLoopbackCapture"/> class instance when its recording.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="WaveInEventArgs"/> instance containing the event data.</param>
        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            int samplesRecorded = e.BytesRecorded / Channels / BytesPerSample;
            DataPcmLeft = new double[samplesRecorded];
            DataPcmRight = new double[samplesRecorded];
            for (int i = 0; i < samplesRecorded; i++)
            {
                try
                {
                    if (Channels == 2)
                    {
                        DataPcmLeft[i] = GetSample(e.Buffer, i * BytesPerSample * 2);

                        DataPcmRight[i] = GetSample(e.Buffer, (i + 1) * BytesPerSample * 2);

                    }
                    else
                    {
                        DataPcmLeft[i] = GetSample(e.Buffer, i * BytesPerSample);
                        DataPcmRight[i] = DataPcmLeft[i];
                    }
                }
                catch (Exception ex)
                {
                    // report the exception..
                    ExceptionLogAction?.Invoke(ex);
                }
            }
            UpdateFft();
        }

        /// <summary>
        /// Gets a sample as PCM (Pulse Code Modulation) form a byte array based on the value of the <see cref="BitsPerSample"/>.
        /// </summary>
        /// <param name="buffer">The buffer to use to get the sample from.</param>
        /// <param name="offset">The offset within the byte array.</param>
        /// <returns>A <c>double</c> value converted from the buffer if successful; otherwise 0.</returns>
        private double GetSample(byte[] buffer, int offset)
        {
            try
            {
                switch (BitsPerSample)
                {
                    case 8: return buffer[offset];
                    case 16: return BitConverter.ToInt16(buffer, offset);
                    case 24: return BitConverter.ToInt32(buffer, offset) & 0xFFFFFF;
                    case 32: return BitConverter.ToInt32(buffer, offset);
                }
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
            }

            return 0;
        }

        /// <summary>
        /// Updates the Fast Fourier Transformation arrays from the <see cref="DataPcmLeft"/> and the <see cref="DataFftRight"/> properties.
        /// </summary>
        private void UpdateFft()
        {
            try
            {
                if (DataPcmLeft == null || DataPcmLeft.Length == 0)
                {
                    return;
                }

                int fftPoints = 2;
                while (fftPoints * 2 <= DataPcmLeft.Length)
                {
                    fftPoints *= 2;
                }

                NAudio.Dsp.Complex[] fftFullLeft = new NAudio.Dsp.Complex[fftPoints];
                NAudio.Dsp.Complex[] fftFullRight = new NAudio.Dsp.Complex[fftPoints];

                for (int i = 0; i < fftPoints; i++)
                {
                    fftFullLeft[i].X = (float) (DataPcmLeft[i] * NAudio.Dsp.FastFourierTransform.HammingWindow(i, fftPoints));
                    fftFullRight[i].X = (float) (DataPcmRight[i] * NAudio.Dsp.FastFourierTransform.HammingWindow(i, fftPoints));
                }

                NAudio.Dsp.FastFourierTransform.FFT(true, (int) Math.Log(fftPoints, 2.0), fftFullLeft);
                NAudio.Dsp.FastFourierTransform.FFT(true, (int) Math.Log(fftPoints, 2.0), fftFullRight);

                DataFftLeft = new double[fftPoints / 2];
                DataFftRight = new double[fftPoints / 2];

                for (int i = 0; i < fftPoints / 2; i++)
                {
                    double fftLeftLeft = Math.Abs(fftFullLeft[i].X + fftFullLeft[i].Y);
                    double fftRightLeft = Math.Abs(fftFullLeft[fftPoints - i - 1].X + fftFullLeft[fftPoints - i - 1].Y);

                    double fftLeftRight = Math.Abs(fftFullRight[i].X + fftFullRight[i].Y);
                    double fftRightRight = Math.Abs(fftFullRight[fftPoints - i - 1].X + fftFullRight[fftPoints - i - 1].Y);

                    DataFftLeft[i] = fftLeftLeft + fftRightLeft;
                    DataFftRight[i] = fftLeftRight + fftRightRight;
                }

                Cropped = false;
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
            }
        }

        /// <summary>
        /// Gets the maximum hertz value of the FFT (Fast Fourier Transformation) data.
        /// </summary>
        internal double MaxHertz
        {
            get
            {
                try
                {
                    if (ValidData)
                    {
                        return DataFftLeft.Length * PointSpacingHz;
                    }
                }
                catch (Exception ex)
                {
                    // report the exception..
                    ExceptionLogAction?.Invoke(ex);
                }

                return 1;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CropFftWithMinorityPercentage"/> has already called on the FFT (Fast Fourier Transformation) data.
        /// </summary>
        private bool Cropped { get; set; }

        /// <summary>
        /// Crops the FFT (Fast Fourier Transformation) data from the highest peak values with a given minority percentage value.
        /// </summary>
        /// <param name="percentage">The minority percentage value.</param>
        /// <param name="steps">A size of a step while calculating downwards with the peak values.</param>
        internal void CropFftWithMinorityPercentage(double percentage = 1, int steps = 1000)
        {
            if (ValidData)
            {
                try
                {
                    if (Cropped)
                    {
                        return;
                    }

                    double max = Math.Max(DataFftLeft.Max(), DataFftRight.Max());
                    double step = max / steps;
                    double count = DataFftLeft.Length;
                    while ((DataFftLeft.Count(f => f >= max) / count * 100) < percentage)
                    {
                        max -= step;
                    }

                    for (int i = 0; i < DataFftLeft.Length; i++)
                    {
                        if (DataFftLeft[i] > max)
                        {
                            DataFftLeft[i] = max;
                        }

                        if (DataFftRight[i] > max)
                        {
                            DataFftRight[i] = max;
                        }
                    }

                    Cropped = true;
                }
                catch (Exception ex)
                {
                    // report the exception..
                    ExceptionLogAction?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// Gets the FFT (Fast Fourier Transformation) data as a list of points.
        /// </summary>
        /// <param name="left">A value indicating whether to use the left or the right channel.</param>
        /// <param name="width">The width to adjust the point to.</param>
        /// <param name="height">The height to adjust the point to.</param>
        /// <returns>A list of points representing the FFT (Fast Fourier Transformation) data.</returns>
        internal List<Point> GetPoints(bool left, double width, double height)
        {
            List<Point> linePoints = new List<Point>();
            try
            {
                if (ValidData)
                {
                    CropFftWithMinorityPercentage(2);
                    double xScaleStepping = width / DataFftRight.Length;
                    double yScaleStepping = height / DataFftRight.Max();
                    var data = left ? DataFftLeft : DataFftRight;
                    for (int i = 0; i < data.Length; i++)
                    {
                        int x = (int) (i * xScaleStepping);
                        int y = (int) (data[i] * yScaleStepping);
                        y = (int) height - y;
                        if (y < 0)
                        {
                            y = 0;
                        }

                        linePoints.Add(new Point(x, y));
                    }
                }
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
            }

            return linePoints;
        }

        /// <summary>
        /// Gets the peak frequency of the current FTT (Fast Fourier Transform Data).
        /// </summary>
        /// <param name="ignoreBelowHz">A value of which below the frequency should be ignored in hertz.</param>
        /// <returns>The peak frequency in hertz.</returns>
        internal double GetPeakFrequency(double ignoreBelowHz = 200)
        {
            try
            {
                if (DataFftLeft == null)
                {
                    return ignoreBelowHz;
                }

                double peakAmplitude = 0;
                double peakIndex = 0;
                int lowestIndex = (int) (ignoreBelowHz / PointSpacingHz);

                for (int i = lowestIndex; i < DataFftLeft.Length; i++)
                {
                    if (Math.Max(DataFftLeft[i], DataFftRight[i]) > peakAmplitude)
                    {
                        peakAmplitude = Math.Max(DataFftLeft[i], DataFftRight[i]);
                        peakIndex = i;
                    }
                }

                double peakFrequency = (peakIndex) * PointSpacingHz;
                return peakFrequency;
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
            }

            return ignoreBelowHz;
        }
    }
}
