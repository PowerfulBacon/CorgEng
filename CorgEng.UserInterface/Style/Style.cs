using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Style
{
    internal abstract class Style
    {

        public abstract void ParseSettings(IDictionary<string, string> settings);

    }
}
