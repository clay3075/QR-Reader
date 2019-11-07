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
                var inputLength = (int) appSettings.GetValue("WAVInputLength", typeof(int));
                var sampleSize = (int) appSettings.GetValue("SampleSize", typeof(int));
                var percentCutoff = (double) appSettings.GetValue("AudioPercentage", typeof(double));
                var retries = (int) appSettings.GetValue("Retries", typeof(int));

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
                    WAVComparer.StoreBaseWAVFile(inputPath, inputLength);
                    WAVRecorder.PlayAudioTrack(inputPath);
                    WAVRecorder.Record(sampleSize, outputPath);
                    var percentMatch = WAVComparer.PercentMatch(outputPath, sampleSize / 1000, retries);
                    message = percentMatch >= percentCutoff ? "Good mic quality." : $"Bad mic quality. {percentMatch}";
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
