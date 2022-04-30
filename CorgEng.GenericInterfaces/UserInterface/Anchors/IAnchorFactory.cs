﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Anchors
{
    public interface IAnchorFactory
    {

        IAnchor CreateAnchor(IAnchorDetails leftAnchorDetails, IAnchorDetails rightAnchorDetails, IAnchorDetails topAnchorDetails, IAnchorDetails bottomAnchorDetails);

    }
}
