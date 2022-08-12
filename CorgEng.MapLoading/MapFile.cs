using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.MapLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.MapLoading
{
    public class MapFile : IMapFile
    {

        public int MapWidth => throw new NotImplementedException();

        public int MapHeight => throw new NotImplementedException();

        public List<IEntityDefinition>[,] MapContents => throw new NotImplementedException();

    }
}
