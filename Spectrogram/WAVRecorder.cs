using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using NAudio.Wave;

namespace Spectrogram
{
    public static class WAVRecorder
    {
        public static FileInfo Record(int sampleSizeInMilliseconds, string outputPath)
        {
            // Redefine the capturer instance with a new instance of the LoopbackCapture class
            //var CaptureInstance = new WasapiLoopbackCapture();
            var CaptureInstance = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(0).Channels)
            };
            // Redefine the audio writer instance with the given configuration
            var RecordedAudioWriter = new WaveFileWriter(outputPath, CaptureInstance.WaveFormat);

            // When the capturer receives audio, start writing the buffer into the mentioned file
            CaptureInstance.DataAvailable += (s, a) =>
            {
                // Write buffer into the file of the writer instance
                RecordedAudioWriter.Write(a.Buffer, 0, a.BytesRecorded);
            };

            // When the Capturer Stops, dispose instances of the capturer and writer
            CaptureInstance.RecordingStopped += (s, a) =>
            {
                RecordedAudioWriter.Dispose();
                RecordedAudioWriter = null;
                CaptureInstance.Dispose();
            };

            // Start audio recording !
            CaptureInstance.StartRecording();
            var interval = Stopwatch.StartNew();
            while (interval.ElapsedMilliseconds <= sampleSizeInMilliseconds)
            {
                //do nothing we are recording
            }
            CaptureInstance.StopRecording();

            return new FileInfo(outputPath);
        }

        public static bool HasRecordingDevices()
        {
            return WaveIn.DeviceCount > 0;
        }

        public static bool HasOutputDevices()
        {
            return WaveOut.DeviceCount > 0;
        }
    }

    
}