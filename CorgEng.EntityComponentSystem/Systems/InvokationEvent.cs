using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Systems
{
    public class InvokationAction
    {

        /// <summary>
        /// The action to invoke
        /// </summary>
        public Action Action { get; }

        public string CallingFile { get; }

        public string CallingMemberName { get; }

        public int CallingLineNumber { get; }

        public InvokationAction(Action action, string callingFile, string callingMemberName, int callingLineNumber)
        {
            Action = action;
            CallingFile = callingFile;
            CallingMemberName = callingMemberName;
            CallingLineNumber = callingLineNumber;
        }
    }
}
