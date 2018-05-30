using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using ZXing;

namespace QRReader
{
    class Program
    {
        static void Main()
        {
            try
            {
                var appSettings = new AppSettingsReader();

                var barcodeReader = new BarcodeReader();
                var outputPath = (string) appSettings.GetValue("PictureOutputPath", typeof(string));

                var startTime = DateTime.Now;
                var timeOut = (int) appSettings.GetValue("ScanTimeOut", typeof(int));
                var cameraID = 0;

                var capDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (capDevices.Count == 0)
                {
                    Console.WriteLine("No camera is available.");
                    return;
                }

                var capture = new VideoCaptureDevice(capDevices[0].MonikerString);

                var max = capture.VideoCapabilities[0];
                foreach (var res in capture.VideoCapabilities)
                {
                    if (res.FrameSize.Width > max.FrameSize.Width)
                    {
                        max = res;
                    }
                }
                capture.VideoResolution = max;
                capture.NewFrame += (sender, args) =>
                {
                    var image = (Bitmap) args.Frame.Clone();
                    var qrInfo = barcodeReader.Decode(image);
                    if (!string.IsNullOrEmpty(qrInfo?.Text))
                    {
                        Console.WriteLine(qrInfo?.Text);
                        image.Save(outputPath);
                        capture.SignalToStop();
                    }
                    if (DateTime.Now >= startTime.AddMilliseconds(timeOut))
                    {
                        Console.WriteLine("Timeout reached with no QR code detected");
                        image.Save(outputPath);
                        capture.SignalToStop();
                    }
                };
                //Console.WriteLine("Starting video capture");
                capture.Start();
                capture.WaitForStop();

                //Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured when running camera test.");
            }
            

        }

    }
}
