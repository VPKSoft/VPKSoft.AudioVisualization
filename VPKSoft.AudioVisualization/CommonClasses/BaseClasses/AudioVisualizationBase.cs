#region License
/*
MIT License

Copyright(c) 2020 Petteri Kautonen

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
using FftSharp;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using VPKSoft.DropOutStack;

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
    public partial class AudioVisualizationBase : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioVisualizationBase"/> class.
        /// </summary>
        public AudioVisualizationBase()
        {
            InitializeComponent();
            Disposed += AudioVisualizationBase_Disposed;
            listenMmDevice = WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice();
            PropertyChanged += AudioVisualizationBase_PropertyChanged;
            IsBarVisualization = GetType() == typeof(AudioVisualizationBars);
        }

        /// <summary>
        /// Calculates the height of the hertz label panel.
        /// </summary>
        /// <param name="handle">The handle to a control to measure the text size from.</param>
        /// <param name="font">The font to be used to measure the text size.</param>
        /// <returns>True if the <see cref="HertzLabelsHeight"/> property value was changed.</returns>
        internal bool CalculateHertzLabelHeight(IntPtr handle, Font font)
        {
            using (var graphics = Graphics.FromHwnd(Handle))
            {
                var newHeight = (int) (graphics.MeasureString(MeasureHertzLabelString, Font).Height + 8);
                bool result = newHeight != HertzLabelsHeight;
                HertzLabelsHeight = newHeight;
                return result;
            }
        }

        /// <summary>
        /// A string to measure the height of the hertz label panel.
        /// </summary>
        private const string MeasureHertzLabelString = "0123456789";

        /// <summary>
        /// Gets or sets the height of the hertz label panel.
        /// </summary>
        internal int HertzLabelsHeight { get; set; } = 21;

        /// <summary>
        /// Gets or sets the adjustment on the volume multipliers in case the <see cref="AudioVisualizationBars.RelativeView"/> is set.
        /// Set this value to 1 to disable the feature.
        /// </summary>
        // ReSharper disable once InconsistentNaming, the inherited class uses this as a base for a property..
        internal double relativeViewTimeAdjust = 1.001;

        /// <summary>
        /// Handles the PropertyChanged event of the AudioVisualizationBase control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void AudioVisualizationBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Contains("Color"))
            {
                Refresh();
            }
            else if (e.PropertyName.Equals("HertzSpan"))
            {
                if (ValidData)
                {
                    Refresh();
                }
            }
            else if (e.PropertyName == nameof(Font))
            {
                if (CalculateHertzLabelHeight(Handle, Font))
                {
                    ShouldResizeChildren?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Occurs when the font has changed and the <see cref="HertzLabelsHeight"/> property value changed.
        /// </summary>
        internal EventHandler ShouldResizeChildren;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
#pragma warning disable CS0067 // the PropertyChanged.Fody does dependency injection to the properties, so do disable the warning..
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

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
        /// Gets the list of MM devices to be used with the <see cref="ListenMmDevice"/> property.
        /// </summary>
        [Browsable(false)]
        public List<MMDevice> MmDevices
        {
            get
            {
                var result = new List<MMDevice>();
                using (var enumerator = new MMDeviceEnumerator())
                {
                    result.AddRange(enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All).ToArray());
                }

                return result;
            }
        }

        // the MM device can be changed on the fly..
        private MMDevice listenMmDevice;

        /// <summary>
        /// Gets or sets the MM device to use for audio visualization.
        /// </summary>
        [Browsable(false)]
        public MMDevice ListenMmDevice
        {
            get => listenMmDevice;

            set
            {
                if (value != listenMmDevice && value != null)
                {
                    listenMmDevice = value;
                    if (IsStarted)
                    {
                        Stop();
                        Start();
                    }
                }
            }
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
                    return (double) SampleRate / DataFftLeft.Length / 2;
                }

                return 1;
            }
        }

        /// <summary>
        /// Gets the sample rate of the <see cref="WasapiLoopbackCapture"/> device.
        /// </summary>
        /// <value>The sample rate.</value>
        internal int SampleRate
        {
            get
            {
                var rate = WasapiLoopbackCapture?.WaveFormat.SampleRate;
                return rate ?? 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is valid FFT data (the audio encoded with Fast Fourier Transformation) available for rendering.
        /// </summary>
        /// <value><c>true</c> if [there is valid FFT data (the audio encoded with Fast Fourier Transformation) available for rendering; otherwise, <c>false</c>.</value>
        internal bool ValidData => (DataFftLeft != null && DataFftLeft.Length > 0) && (DataFftRight != null && DataFftRight.Length > 0);

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
                    WasapiLoopbackCapture = new WasapiLoopbackCapture(ListenMmDevice);
                    WasapiLoopbackCapture.DataAvailable += DataAvailable;
                    WasapiLoopbackCapture.StartRecording();
                    Channels = WasapiLoopbackCapture.WaveFormat.Channels;
                    BitsPerSample = WasapiLoopbackCapture.WaveFormat.BitsPerSample;
                    tmVisualize.Enabled = true;
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

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        [Description("Gets or sets the background color for the control.")]
        [Browsable(true)]
        [Category("Appearance")]
        public override Color BackColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the color of the hertz labels.
        /// </summary>
        [Description("Gets or sets the color of the hertz labels.")]
        [Browsable(true)]
        [Category("Appearance")]
        public Color ColorHertzLabels { get; set; } = Color.Magenta;

        /// <summary>
        /// Gets or sets the color of the left audio channel visualization.
        /// </summary>
        [Description("Gets or sets the color of the left audio channel visualization.")]
        [Browsable(true)]
        [Category("Appearance")]
        public Color ColorAudioChannelLeft { get; set; } = Color.Aqua;

        /// <summary>
        /// Gets or sets the color of the left audio channel visualization.
        /// </summary>
        [Description("Gets or sets the color of the right audio channel visualization.")]
        [Browsable(true)]
        [Category("Appearance")]
        public Color ColorAudioChannelRight { get; set; } = Color.LimeGreen;

        /// <summary>
        /// Gets or sets the refresh rate for the audio visualization.
        /// </summary>
        [Description("Gets or sets the refresh rate for the audio visualization.")]
        [Browsable(true)]
        [Category("Behaviour")]
        public int RefreshRate
        {
            get => tmVisualize.Interval;

            set
            {
                if (tmVisualize.Interval != value && value >= 5)
                {
                    tmVisualize.Interval = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minority crop percentage.
        /// </summary>
        [Description("Gets or sets the minority crop percentage.")]
        [Browsable(true)]
        [Category("Behaviour")]
        public int MinorityCropPercentage { get; set; } = 2;

        /// <summary>
        /// Gets or sets the minority percentage stepping. I.e. How large the stepping is when cropping the data. <seealso cref="MinorityCropPercentage"/>
        /// </summary>
        [Description("Gets or sets the minority percentage stepping.")]
        [Browsable(true)]
        [Category("Behaviour")]
        public int MinorityPercentageStepping { get; set; } = 1000;

        /// <summary>
        /// Gets or sets a value indicating whether the minority cropping should applied on a bar graph after the bar values are calculated.
        /// </summary>
        [Description(" Gets or sets a value indicating whether the minority cropping should applied on a bar graph after the bar values are calculated.")]
        [Browsable(true)]
        [Category("Behaviour")]
        public bool MinorityCropOnBarLevel { get; set; }

        /// <summary>
        /// "Gets or sets the FFT window type applied to the signal.
        /// </summary>
        [Description("Gets or sets the FFT window type applied to the signal.")]
        [Browsable(true)]
        [Category("Behaviour")]
        public WindowType FftWindowType { get; set; } = WindowType.Hanning;

        /// <summary>
        /// Gets the audio intensity which is considered as noise and is not visualized.
        /// </summary>
        [Description("Gets the audio intensity which is considered as noise and is not visualized.")]
        [Browsable(true)]
        [Category("Behaviour")]
        public double NoiseTolerance { get; set; } = 1.0;

        /// <summary>
        /// Gets a value indicating whether the MM audio device listening is started.
        /// </summary>
        [Browsable(false)]
        public bool IsStarted => WasapiLoopbackCapture != null;

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
                    tmVisualize.Enabled = false;
                    using (WasapiLoopbackCapture)
                    {
                        WasapiLoopbackCapture.DataAvailable -= DataAvailable;
                        WasapiLoopbackCapture.StopRecording();
                        WasapiLoopbackCapture = null;
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
        /// Gets or sets the custom FFT window function. <seealso cref="WindowType.Custom"/>.
        /// </summary>
        /// <value>The custom window function.</value>
        [Browsable(false)]
        public Func<int, double[]> CustomWindowFunc { get; set; } = delegate(int points)
        {
            var result = new double[points];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 1;
            }

            return result;
        };

        /// <summary>
        /// Gets or sets the PMC (Pulse Code Modulation) action for the left and for the right audio channel.
        /// </summary>
        [Browsable(false)]
        private Action<double[], double[]> PmcAction { get; set; } = delegate {  };

        /// <summary>
        /// Gets or sets the action to performed for the audio data after the FFT windowing.
        /// </summary>
        [Browsable(false)]
        private Action<double[], double[]> FftAfterAction { get; set; } = delegate {  };


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

                PmcAction(DataFftLeft, DataPcmRight); // call use the user defined action..

                double[] window;
                switch (FftWindowType)
                {
                    case WindowType.Custom:
                        window = CustomWindowFunc(fftPoints);
                        break;

                    case WindowType.Hamming:
                        window = Window.Hamming(fftPoints);
                        break;

                    case WindowType.Blackman:
                        window = Window.Blackman(fftPoints);
                        break;

                    case WindowType.BlackmanExact:
                        window = Window.BlackmanExact(fftPoints);
                        break;

                    case WindowType.BlackmanHarris:
                        window = Window.BlackmanHarris(fftPoints);
                        break;

                    case WindowType.FlatTop:
                        window = Window.FlatTop(fftPoints);
                        break;

                    case WindowType.Bartlett:
                        window = Window.Bartlett(fftPoints);
                        break;

                    case WindowType.Cosine:
                        window = Window.Cosine(fftPoints);
                        break;

                    default:
                        window = new double[fftPoints];
                        for (int i = 0; i < window.Length; i++)
                        {
                            window[i] = 1;
                        }

                        break;
                }

                NAudio.Dsp.Complex[] fftFullLeft = new NAudio.Dsp.Complex[fftPoints];
                NAudio.Dsp.Complex[] fftFullRight = new NAudio.Dsp.Complex[fftPoints];

                for (int i = 0; i < fftPoints; i++)
                {

                    fftFullLeft[i].X = (float) (DataPcmLeft[i] * window[i]);
                    fftFullRight[i].X = (float) (DataPcmRight[i] * window[i]);
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

                FftAfterAction(DataFftLeft, DataFftRight); // call use the user defined action..

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
        /// Gets a value indicating whether this instance is of type of <see cref="AudioVisualizationBars"/>.
        /// </summary>
        private bool IsBarVisualization { get; }

        /// <summary>
        /// Crops the FFT (Fast Fourier Transformation) data from the highest peak values with a given minority percentage value.
        /// </summary>
        internal void CropFftWithMinorityPercentage()
        {
            if (IsBarVisualization)
            {
                if (MinorityCropOnBarLevel)
                {
                    return;
                }
            }

            var steps = MinorityPercentageStepping;

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
                    while ((DataFftLeft.Count(f => f >= max) / count * 100) < MinorityCropPercentage)
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

        private DropOutStack<(double peak, DateTime time)> FftPeakPerSecond { get; } = new DropOutStack<(double peak, DateTime time)>(500);

        /// <summary>
        /// Gets the FFT data maximum intensity.
        /// </summary>
        /// <value>The data FFT data maximum intensity.</value>
        internal double DataFftMax
        {
            get
            {
                var current = ValidData ? Math.Max(DataFftLeft.Max(), DataFftRight.Max()) : 0;
                FftPeakPerSecond.Push((current, DateTime.Now));

                if (FftPeakPerSecond.Count == 0)
                {
                    return 0;
                }

                var time = FftPeakPerSecond.Min(f => f.time);

                var divisor = (DateTime.Now - time).TotalSeconds;

                return divisor != 0 ? FftPeakPerSecond.Average(f => f.peak) / divisor : 0;
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
                    CropFftWithMinorityPercentage();
                    double xScaleStepping = width / DataFftRight.Length;
                    double yScaleStepping = height / Math.Max(DataFftRight.Max(), DataFftLeft.Max());
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
        /// Gets the FFT (Fast Fourier Transformation) data as a list of points weighted with the point amount.
        /// </summary>
        /// <param name="left">A value indicating whether to use the left or the right channel.</param>
        /// <param name="width">The width to adjust the point to.</param>
        /// <param name="height">The height to adjust the point to.</param>
        /// <returns>A list of points representing the FFT (Fast Fourier Transformation) data.</returns>
        internal List<Point> GetPointsWeighted(bool left, double width, double height)
        {
            List<Point> linePoints = new List<Point>();
            try
            {
                if (ValidData)
                {
                    CropFftWithMinorityPercentage();

                    var values = CreateWeightedFftArray((int)width, (int)height);

                    double xScaleStepping = width / values.left.Count;
                    var data = left ? values.left : values.right;
                    for (int i = 0; i < data.Count; i++)
                    {
                        int x = (int) (i * xScaleStepping);
                        int y = (int) (data[i]);
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
        /// A delegate for the <see cref="DataCalculated"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The data calculated event arguments.</param>
        public delegate void OnDataCalculated(object sender, DataCalculatedEventArgs e);

        /// <summary>
        /// Occurs when the audio data is calculated and visualized.
        /// </summary>
        public event OnDataCalculated DataCalculated;

        /// <summary>
        /// Raises the data calculated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataCalculatedEventArgs"/> instance containing the event data.</param>
        internal void RaiseDataCalculatedEvent(object sender, DataCalculatedEventArgs e)
        {
            DataCalculated?.Invoke(sender, e);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display the hertz labels.
        /// </summary>
        public virtual bool DisplayHertzLabels { get; set; }

        /// <summary>
        /// Gets the peak frequency of the current FTT (Fast Fourier Transform Data).
        /// </summary>
        /// <param name="ignoreBelowHz">A value of which below the frequency should be ignored in hertz.</param>
        /// <returns>The peak frequency in hertz.</returns>
        public double GetPeakFrequency(double ignoreBelowHz = 200)
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

        /// <summary>
        /// Paints the hertz labels to a given <see cref="Panel"/> instance.
        /// </summary>
        /// <param name="panel">The panel to paint the hertz labels into.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the paint event data.</param>
        /// <param name="left">The left-most position where to center the first label.</param>
        /// <param name="right">The right-most position where to center the last label.</param>
        internal void PaintHertzLabels(Panel panel, PaintEventArgs e, int left, int right)
        {
            try
            {
                e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);

                int hertz = (int) MaxHertz == 1 ? 24000 : (int) MaxHertz;

                int hertzLabelCount = hertz / 1000 / 2 + 1;

                double startPoint = left - (e.Graphics.MeasureString("0", Font).Width / 2);

                double endPoint = (right - left) +
                                  (e.Graphics.MeasureString((hertz / 1000).ToString(), Font).Width / 2);

                double stepping = (endPoint - startPoint) / (hertzLabelCount - 1);

                double currentPoint = startPoint;

                using (var brush = new SolidBrush(ColorHertzLabels))
                {
                    for (int i = 0; i < hertzLabelCount; i++)
                    {
                        if (i == 0)
                        {
                            e.Graphics.DrawString("0", Font, brush, (float) startPoint, 4);
                        }
                        else
                        {
                            e.Graphics.DrawString((i * 2).ToString(), Font, brush,
                                (float) currentPoint + (e.Graphics.MeasureString((i * 2).ToString(), Font).Width / 2),
                                4);
                        }

                        currentPoint += stepping;
                    }
                }
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
            }
        }

        private void AudioVisualizationBase_SizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// A class to store volume channel multiplier data on the <see cref="AudioVisualizationBars.RelativeView"/> mode.
        /// </summary>
        internal class SpanMultiplierData
        {
            // a field for the SpanMultiplier property..

            /// <summary>
            /// Gets or sets the span multiplier.
            /// </summary>
            /// <value>The span multiplier.</value>
            internal double SpanMultiplier { get; set; } = double.MaxValue;

            internal static List<SpanMultiplierData> InstanceList(int capacity)
            {
                var result = new List<SpanMultiplierData>();
                for (int i = 0; i < capacity; i++)
                {
                    result.Add(new SpanMultiplierData());
                }

                return result;
            }

            internal void Adjust(double upMultiplier, double newValue)
            {
                var newMultiplier = SpanMultiplier * upMultiplier;
                newMultiplier = newMultiplier > newValue ? newValue : newMultiplier;

                SpanMultiplier = newMultiplier;
            }
        }

        internal List<SpanMultiplierData> SpanMultipliersRight { get; private set; } = new List<SpanMultiplierData>();

        internal List<SpanMultiplierData> SpanMultipliersLeft { get; private set; } = new List<SpanMultiplierData>();

        internal double SpanMin { get; private set; }

        internal double SpanMax { get; private set; }

        /// <summary>
        /// Resets the relative view in case the <see cref="AudioVisualizationBars.RelativeView"/> property is set. This is good to call when the playing song has been changed.
        /// </summary>
        public void ResetRelativeView()
        {
            SpanMultipliersLeft = new List<SpanMultiplierData>();
            SpanMultipliersRight = new List<SpanMultiplierData>();
            SpanMax = 0;
            SpanMin = 0;
        }

        /// <summary>
        /// Applies the span multiplier in case the <see cref="AudioVisualizationBars.RelativeView"/> is set.
        /// </summary>
        /// <param name="values">The values to modify with the relative multiplier.</param>
        /// <param name="left">A value indicating whether the left channel is in question.</param>
        internal void ApplySpanMultiplier(List<double> values, bool left)
        {
            if (this is AudioVisualizationBars)
            {
                if (!((AudioVisualizationBars)(this)).RelativeView)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            var arraySize = Math.Min(values.Count, left ? SpanMultipliersLeft.Count : SpanMultipliersRight.Count);
            for (int i = 0; i < arraySize; i++)
            {
                values[i] *= left ? SpanMultipliersLeft[i].SpanMultiplier : SpanMultipliersRight[i].SpanMultiplier;
            }
        }

        /// <summary>
        /// Updates the <see cref="SpanMultipliersLeft"/> and <see cref="SpanMultipliersRight"/> property values in case the <see cref="AudioVisualizationBars.RelativeView"/> is set.
        /// </summary>
        /// <param name="left">The left channel FFT audio data.</param>
        /// <param name="right">The right channel FFT audio data.</param>
        private void UpdateSpanMinMax(IReadOnlyList<double> left, IReadOnlyList<double> right)
        {
            try
            {
                if (this is AudioVisualizationBars)
                {
                    if (!((AudioVisualizationBars)(this)).RelativeView)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                var max = Math.Max(left.Max(), right.Max());
                var min = Math.Min(left.Min(), right.Min());
                var spanMinLeft = new List<double>(new double [left.Count]);
                var spanMaxLeft = new List<double>(new double [left.Count]);
                var spanMinRight = new List<double>(new double [left.Count]);
                var spanMaxRight = new List<double>(new double [left.Count]);

                if (max > SpanMax)
                {
                    SpanMax = max;
                }

                if (min < SpanMin)
                {
                    SpanMin = min;
                }

                if (SpanMultipliersLeft.Count != left.Count)
                {
                    SpanMultipliersLeft = SpanMultiplierData.InstanceList(left.Count);
                    SpanMultipliersRight = SpanMultiplierData.InstanceList(left.Count);
                }

                for (int i = 0; i < spanMaxLeft.Count; i++)
                {
                    if (left[i] > spanMaxLeft[i])
                    {
                        spanMaxLeft[i] = left[i];
                    }

                    if (right[i] > spanMaxRight[i])
                    {
                        spanMaxRight[i] = right[i];
                    }

                    if (left[i] < spanMinLeft[i])
                    {
                        spanMinLeft[i] = left[i];
                    }

                    if (right[i] < spanMinRight[i])
                    {
                        spanMinRight[i] = right[i];
                    }
                }

                for (int i = 0; i < spanMaxLeft.Count; i++)
                {
                    var multiplier = (SpanMax - SpanMin) / (spanMaxLeft[i] - spanMinLeft[i]);

                    SpanMultipliersLeft[i].Adjust(relativeViewTimeAdjust, multiplier);

                    multiplier = (SpanMax - SpanMin) / (spanMaxRight[i] - spanMinRight[i]);
                    SpanMultipliersRight[i].Adjust(relativeViewTimeAdjust, multiplier);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogAction?.Invoke(ex);
            }
        }

        internal (List<double> left, List<double> right) CreateWeightedFftArray(int hertzSpan, int height)
        {

            List<double> resultLeft = new List<double>();
            List<double> resultRight = new List<double>();

            if (DataFftMax < NoiseTolerance)
            {
                return (resultLeft, resultRight);
            }

            try
            {
                double span = (double)DataFftLeft.Length / hertzSpan;
                if (span <= 0)
                {
                    return (resultLeft, resultRight);
                }

                if (ValidData)
                {
                    CropFftWithMinorityPercentage();

                    for (double i = 0; i < DataFftLeft.Length; i += span)
                    {
                        List<double> sampleSpanLeft = new List<double>();
                        List<double> sampleSpanRight = new List<double>();
                        for (double j = i; j < i + span && j < DataFftLeft.Length; j++)
                        {
                            sampleSpanLeft.Add(DataFftLeft[(int)j]);
                            sampleSpanRight.Add(DataFftRight[(int)j]);
                        }

                        double weightLeft = sampleSpanLeft.Where(f => f > 0).Sum();
                        double weightRight = sampleSpanRight.Where(f => f > 0).Sum();

                        resultLeft.Add(weightLeft / span);
                        resultRight.Add(weightRight / span);
                    }

                    // extra cropping for bars..
                    if (!Cropped && MinorityCropOnBarLevel)
                    {
                        var max = Math.Max(resultLeft.Max(), resultRight.Max());
                        var count = resultLeft.Count;
                        var step = max / MinorityPercentageStepping;

                        while ((resultLeft.Count(f => f >= max) * 100 / count ) < MinorityCropPercentage)
                        {
                            max -= step;
                        }

                        for (int i = 0; i < resultLeft.Count; i++)
                        {
                            if (resultLeft[i] > max)
                            {
                                resultLeft[i] = max;
                            }

                            if (resultRight[i] > max)
                            {
                                resultRight[i] = max;
                            }
                        }

                        Cropped = true;
                    }

                    double yScaleStepping = Math.Max(resultLeft.Max(), resultRight.Max()) / height;
                    for (int i = 0; i < resultLeft.Count; i++)
                    {
                        resultLeft[i] = resultLeft[i] / yScaleStepping;
                        resultRight[i] = resultRight[i] / yScaleStepping;
                    }
                }

                UpdateSpanMinMax(resultLeft, resultRight);

                return (resultLeft, resultRight);
            }
            catch (Exception ex)
            {
                // report the exception..
                ExceptionLogAction?.Invoke(ex);
                return (resultLeft, resultRight);
            }
        }
    }
}
