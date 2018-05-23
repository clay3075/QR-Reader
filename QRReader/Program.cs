using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;
using ZXing;

namespace QRReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = new AppSettingsReader();
         
            var barcodeReader = new BarcodeReader();
            var outputPath = (string)appSettings.GetValue("PictureOutputPath", typeof(string));

            var startTime = DateTime.Now;
            var timeOut = (int)appSettings.GetValue("ScanTimeOut", typeof(int));
            var reading = true;

            var capture = new VideoCapture(0);

            Console.WriteLine("Starting Video Capture");

            while (reading)
            {
                try
                {
                    var image = capture.QueryFrame();
                    var qrInfo = barcodeReader.Decode(image.Bitmap);
                    if (!string.IsNullOrEmpty(qrInfo?.Text))
                    {
                        reading = false;
                        Console.WriteLine(qrInfo?.Text);
                        image.Save(outputPath);
                    }
                    if (DateTime.Now >= startTime.AddMilliseconds(timeOut))
                    {
                        reading = false;
                        Console.WriteLine("Timeout reached with no QR code detected");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.Read();
        }
    }
}
