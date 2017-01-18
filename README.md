# WixGAExtension
Bring Google Analytics into your Windows installer.

With just one line in your WiX installer you can track installations and uninstallations. Get insights about your application installed base.

![Visual Studio](https://raw.githubusercontent.com/frederiksen/WixGAExtension/master/documentation/screenshot.PNG)

And get the result in Google Analytics:

![Google Analytics](https://raw.githubusercontent.com/frederiksen/WixGAExtension/master/documentation/GA-screenshot.PNG)

# How to use
1. Create a free Google Analytics project - https://www.google.com/analytics - and get a tracking code
2. Download the WiX extension: [WixGAExtension.dll](https://github.com/frederiksen/WixGAExtension/releases)
3. From your WiX project reference `WixGAExtension.dll`
4. Add this namespace: xmlns:ga="https://github.com/frederiksen/WixGAExtension"
5. Add this line under the `Product` node: <ga:GoogleAnalytics TrackingId="UA-90448268-1" /> with your tracking code
6. Done :-)

Here's a test project:
https://github.com/frederiksen/WixGAExtension/tree/master/src/TestSetupProject

# Future
Also track information about PC hardware/software. I.e.: CPU, memory, Windows version, ...
