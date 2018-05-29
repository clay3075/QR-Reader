using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var appSettings = new AppSettingsReader();

                var outputPath = (string) appSettings.GetValue("WAVOutputPath", typeof(string));
                var inputPath = (string) appSettings.GetValue("WAVInputPath", typeof(string));
                var sampleSize = (int) appSettings.GetValue("SampleSize", typeof(int));
                var percentCutoff = (double) appSettings.GetValue("AudioPercentage", typeof(double));

                var message = "";

                if (!WAVRecorder.HasRecordingDevices())
                {
                    message = "No recording device. ";
                }
                if (!WAVRecorder.HasOutputDevices())
                {
                    message += "No output device (stereo). ";
                }
                if (string.IsNullOrEmpty(message))
                {
                    WAVComparer.StoreBaseWAVFile(inputPath, 17);
                    WAVRecorder.PlayAudioTrack(inputPath);
                    WAVRecorder.Record(sampleSize, outputPath);
                    if (WAVComparer.PercentMatch(outputPath, sampleSize / 1000) >= percentCutoff)
                    {
                        message = $"Good mic quality. {WAVComparer.PercentMatch(outputPath, sampleSize / 1000)}";
                    }
                    else
                    {
                        message = $"Bad mic quality. {WAVComparer.PercentMatch(outputPath, sampleSize / 1000)}";
                    }
                }
                Console.WriteLine(message);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred when running audio test. " + ex.Message);
            }
        }
    }
}
