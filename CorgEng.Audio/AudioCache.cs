using System.Collections.Generic;

namespace GJ2022.Audio
{
    internal static class AudioCache
    {

        private const string AUDIO_PATH = "";

        private static Dictionary<string, AudioFile> audioFileCache = new Dictionary<string, AudioFile>();

        public static AudioFile GetAudioFile(string file)
        {
            string filePath = $"{AUDIO_PATH}{file}";
            //Already cached
            if (audioFileCache.ContainsKey(file))
                return audioFileCache[file];
            //Load into the cache
            AudioFile createdFile = new AudioFile(filePath);
            audioFileCache.Add(file, createdFile);
            return createdFile;
        }

    }
}
