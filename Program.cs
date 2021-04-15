using System;
using System.IO;
using System.Linq;
using System.Threading;

using TreadManager;

using TreadReader;
using TreadReader.HandHeld;
using TreadReader.Render;

namespace HandheldSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connect to your Handheld Wifi and press any key to continue");
            Console.ReadKey();

            //required as platform image driver
            Helpers.ImageProcessing = new ProcessImageSkiaSharpImplementation();

            //Initialize the Hanhheld driver and pass a renderer to produce PNGs
            DeviceDriver handheld = new(VeldridRender.InitFromVulkan());

            //Try to link to device in direct mode (must bu connected to device WIFI)
            handheld.Connect();

            Console.WriteLine("Status = " + (handheld.Connected ? "Connected" : "Not Connected"));

            //Keep rescanning until disconnected
            while (handheld.Connected)
            {
                Console.WriteLine("Input the VRN:");
                string vrn = Console.ReadLine();
                //Making a Scan is Optional, you can use a syngle Tyre obj like handheld.StartScan(new Tyre()); should you want
                Scan tmpScan = new(vrn);

                tmpScan.AddTyre(0, 1, 0, "Front Left");
                tmpScan.AddTyre(0, 2, 0, "Front Right");
                //OR
                tmpScan.Tyres.Add(new Tyre() { Axle = 1, Index = 0, Hub = 1, Placement = "Rear Left" });
                tmpScan.Tyres.Add(new Tyre() { Axle = 1, Index = 0, Hub = 2, Placement = "Rear Right" });

                Console.WriteLine("Scanning " + vrn);
                //Scan through vehicle
                foreach (Tyre tmpTyre in tmpScan.Tyres)
                {
                    if (tmpTyre.Axle == 1) { tmpTyre.Meta.Season = TyreSeason.winter; }//Set winter tyres for rear
                    while (tmpTyre.ScanningIssue != ScanIssue.None)
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("Use the handheld to scan the " + tmpTyre.Placement);
                        handheld.StartScan(tmpTyre);
                        if (tmpTyre.ScanningIssue != ScanIssue.None)
                        {
                            Console.WriteLine(tmpTyre.ScanningIssue.ToString());
                        }
                        else
                        {
                            //GetRender process all necessary steps to get depths and corrections and return a png
                            File.WriteAllBytes(Path.GetFullPath("./" + tmpScan.Vehicle.VRN + "_" + tmpTyre.Axle + "_" + tmpTyre.Hub + ".png"), tmpTyre.GetRender()); // Save the png to file

                            //Some output
                            Console.WriteLine("Result: " + Helpers.TreadDepthResult(tmpTyre.MinDepth, tmpTyre.Meta.Season).ToString());
                            Console.WriteLine("Depths: " + string.Join(", ", tmpTyre.Depths));
                            Console.WriteLine("Has defects: " + (tmpTyre.Inflation != 0 || tmpTyre.Misalignment != 0).ToString());

                        }
                    }
                }

            }
            Console.WriteLine("Device Disconnected");
        }
    }
}
