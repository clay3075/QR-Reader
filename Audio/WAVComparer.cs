﻿using System;
using System.IO;
using SoundFingerprinting;
using SoundFingerprinting.Audio;
using SoundFingerprinting.Builder;
using SoundFingerprinting.DAO.Data;
using SoundFingerprinting.InMemory;

namespace AudioAnalyzer
{
    public static class WAVComparer
    {
        private static readonly IModelService modelService = new InMemoryModelService(); // store fingerprints in RAM
        private static readonly IAudioService audioService = new SoundFingerprintingAudioService(); // default audio library

        public static void StoreBaseWAVFile(string pathToWAV, int secondsToAnalyze)
        {
            var fileName2 = Path.GetFileNameWithoutExtension(pathToWAV);
            File.Copy(pathToWAV, fileName2 + "(2).wav", true);

            var track = new TrackData("123456789", "TechFriends", "TechFriends", "TechFriends", 2018, secondsToAnalyze);

            // store track metadata in the datasource
            var trackReference = modelService.InsertTrack(track);

            // create hashed fingerprints
            var hashedFingerprints = FingerprintCommandBuilder.Instance
                .BuildFingerprintCommand()
                .From(pathToWAV)
                .UsingServices(audioService)
                .Hash()
                .Result;

            // store hashes in the database for later retrieval
            modelService.InsertHashDataForTrack(hashedFingerprints, trackReference);
        }

        public static double PercentMatch(string pathToWAV, int secondsToAnalyze, int tries = 3)
        {
            if (tries == 0)
                return 0;
            try
            {
                const int startAtSecond = 0; // start at the beginning

                // query the underlying database for similar audio sub-fingerprints
                var queryResult = QueryCommandBuilder.Instance.BuildQueryCommand()
                    .From(pathToWAV, secondsToAnalyze, startAtSecond)
                    .UsingServices(modelService, audioService)
                    .Query()
                    .Result;

                if (!queryResult.ContainsMatches)
                    throw new Exception("Try again.");

                return queryResult.BestMatch.Confidence; // confidence in which this track is a match
            }
            catch
            {
                return PercentMatch(pathToWAV, secondsToAnalyze, tries - 1);
            }
        }
    }
}