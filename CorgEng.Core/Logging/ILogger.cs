using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Core.Logging
{
    public interface ILogger
    {

        void WriteLine(object message, LogType logType = LogType.MESSAGE);

    }
}
