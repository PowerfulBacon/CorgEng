using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Audio;
using CorgEng.GenericInterfaces.Logging;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;

namespace GJ2022.Audio
{
    [Dependency]
    internal unsafe class AudioMaster : IAudioMaster
    {

        [UsingDependency]
        private static ILogger Logger = null!;

        private static Device* audioDevice = null;
        private static Context* audioContext = null;

        private static List<uint> buffers = new List<uint>();

        private static AL audioLib;

        public static bool UsingAudio = false;

        public void UpdateListener(float x, float y, float z)
        {
            if (!UsingAudio)
                return;
            audioLib.SetListenerProperty(ListenerVector3.Position, x, y, z);
        }

        /// <summary>
        /// Initialize the audio master.
        /// Will cleanup the old devices and contexts if called multiple times.
        /// </summary>
        [ModuleLoad]
        public static bool Initialize()
        {
            try
            {

                if (audioLib == null)
                    audioLib = AL.GetApi();

                //Get the AL context
                ALContext audioLibraryContext = ALContext.GetApi();

                //Delete existing audio device
                if (audioDevice != null)
                {
                    audioLibraryContext.CloseDevice(audioDevice);
                    audioDevice = null;
                }

                //Delete existing audio context
                if (audioContext != null)
                {
                    audioLibraryContext.DestroyContext(audioContext);
                    audioContext = null;
                }

                UsingAudio = false;

                //Setup the audio device
                audioDevice = audioLibraryContext.OpenDevice("");

                //Check to see if audio device creation was successful
                if (audioDevice == null)
                {
                    Logger.WriteLine("Error: Unable to create OpenAL audio device!", LogType.ERROR);
                    return false;
                }

                //Create audio context
                audioContext = audioLibraryContext.CreateContext(audioDevice, null);
                audioLibraryContext.MakeContextCurrent(audioContext);

                //Get errors
                AudioError error = audioLib.GetError();
                if (error != AudioError.NoError)
                    throw new Exception($"Audio error: {error}");

                UsingAudio = true;

                Logger.WriteLine("Audio setup successfully.", LogType.DEBUG);
                //Setup was successful
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteLine(e, LogType.ERROR);
                return false;
            }
        }

        /// <summary>
        /// Cleanup the audio files
        /// </summary>
        [ModuleTerminate]
        public void Cleanup()
        {
            if (!UsingAudio)
                return;
            ALContext audioLibraryContext = ALContext.GetApi();
            AL audioLibrary = AL.GetApi();
            audioLibraryContext.CloseDevice(audioDevice);
            audioLibraryContext.DestroyContext(audioContext);
            audioLibraryContext.Dispose();
            audioLibrary.Dispose();
        }

    }
}
