﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Shaders
{
    public interface IShaderFactory
    {

        IShaderSet CreateShaderSet(string shaderName);

    }
}
