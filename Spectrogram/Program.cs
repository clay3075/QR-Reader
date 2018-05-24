using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectrogram
{
    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = new AppSettingsReader();

            var outputPath = (string)appSettings.GetValue("WAVOutputPath", typeof(string));
            var sampleSize = (int) appSettings.GetValue("SampleSize", typeof(int));

            var fileInfo = WAVRecorder.Record(sampleSize, outputPath);
            Console.WriteLine(fileInfo.Length);

            Console.Read();
        }
    }
}
