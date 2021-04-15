# HandheldSample
Handheld sample project (.NET 5)

As you will find in **Program.cs**, all you need to do a simple scan is:
 - add the Imager dependency to **TreadReader.Helpers.ImageProcessing** ( "= new ProcessImageSkiaSharpImplementation();" )
 - Initate the driver as **TreadReader.HandHeld.DeviceDriver handheld = new(VeldridRender.InitFromVulkan());** where VeldridRender.InitFromVulkan() is by preference or custom
 - Connect (must be in same network, fixed IP as direct mode or pass the new IP when on network mode)
 - Scan using a existing tyre object with prefilled information
 
 *Updated nuget on request or as a private user on our nuget feed.
