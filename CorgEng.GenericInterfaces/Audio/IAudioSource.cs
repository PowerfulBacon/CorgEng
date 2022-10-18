using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Audio
{
    public interface IAudioSource
    {

        void PlaySound(string file, float x, float y, float z = 0, float gain = 1.0f, bool repeating = false);

    }
}
