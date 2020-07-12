// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace VPKSoft.AudioVisualization
{
    /// <summary>
    /// The FFT window type.
    /// </summary>
    public enum WindowType
    {
        /// <summary>
        /// No windowing in the FFT.
        /// </summary>
        None,

        /// <summary>
        /// The custom window <see cref="System.Func{T,TResult}"/>
        /// </summary>
        Custom,

        /// <summary>
        /// The Hanning window.
        /// </summary>
        Hanning,

        /// <summary>
        /// The Hamming window.
        /// </summary>
        Hamming,

        /// <summary>
        /// The Blackman window.
        /// </summary>
        Blackman,

        /// <summary>
        /// The Blackman exact window.
        /// </summary>
        BlackmanExact,

        /// <summary>
        /// The Blackman-Harris window.
        /// </summary>
        BlackmanHarris,

        /// <summary>
        /// The Flattop window.
        /// </summary>
        FlatTop,

        /// <summary>
        /// The Bartlett window.
        /// </summary>
        Bartlett,

        /// <summary>
        /// The Cosine window.
        /// </summary>
        Cosine,
    }
}
