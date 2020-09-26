**2020/07/12, v.1.0.0.4**
* Properties added:
  - *MinorityPercentageStepping*
  - *MinorityCropOnBarLevel* 
  - *FftWindowType*, which defaults to *WindowType.Hanning*
* Added *CustomWindowFunc* to apply custom [FFT](https://en.wikipedia.org/wiki/Fast_Fourier_transform) window.
* Added *PmcAction* action to enable data modification with PMC ([Pulse Code Modulation](https://en.wikipedia.org/wiki/Pulse-code_modulation)) data.
* Added *FftAfterAction* action to allow data modification after the [FFT](https://en.wikipedia.org/wiki/Fast_Fourier_transform) windowing and handling.
* Fixed the weight calculation for the bar graph.
* Added a possibility to crop the frequencies by percentage after the bars have been calculated.
* New [FFT](https://en.wikipedia.org/wiki/Fast_Fourier_transform) windowing methods provided by the [FftSharp](https://github.com/swharden/FftSharp) library.

﻿**2020/09/26, v.1.0.5**
 * Properties added:
   - *LineDrawMode to AudioVisualizationPlot*
   - *LineCurveTension to AudioVisualizationPlot*
   - *RelativeView to AudioVisualizationBars*
   - *NoiseTolerance to base class*
 * Bug fixes.
