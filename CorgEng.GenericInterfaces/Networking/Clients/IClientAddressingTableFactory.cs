﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Clients
{
    public interface IClientAddressingTableFactory
    {

        IClientAddressingTable CreateAddressingTable();

    }
}
