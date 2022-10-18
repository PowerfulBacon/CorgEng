using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Audio;
using CorgEng.GenericInterfaces.Logging;
using Silk.NET.OpenAL;
using System.Threading.Tasks;

namespace GJ2022.Audio
{
    [Dependency]
    internal class AudioSource : IAudioSource
    {

        [UsingDependency]
        private static ILogger Logger = null!;

        private uint source;

        public void PlaySound(string file, float x, float y, float z = 0, float gain = 1.0f, bool repeating = false)
        {
            try
            {
                if (!AudioMaster.UsingAudio)
                    return;
                //Get the AL API
                AL al = AL.GetApi();
                //Get the audio file
                AudioFile audioFile = AudioCache.GetAudioFile(file);
                //Create the audio source
                source = al.GenSource();
                //Play the source
                if (gain != 1.0f)
                    al.SetSourceProperty(source, SourceFloat.Gain, gain);
                if (repeating)
                    al.SetSourceProperty(source, SourceBoolean.Looping, repeating);
                al.SetSourceProperty(source, SourceInteger.Buffer, audioFile.Buffer);
                al.SetSourceProperty(source, SourceVector3.Position, x, y, z);
                al.SourcePlay(source);
                //Delete the source after the playtime
                if (!repeating)
                {
                    Task.Run(async delegate
                    {
                        await Task.Delay((int)(audioFile.PlayTime * 1000) + 1000);
                        al.DeleteSource(source);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(e, LogType.ERROR);
            }
        }

    }
}
