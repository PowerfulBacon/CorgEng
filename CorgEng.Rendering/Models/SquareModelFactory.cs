using CorgEng.Core.Modules;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Models
{
    [Dependency(defaultDependency = true)]
    public class SquareModelFactory : ISquareModelFactory
    {

        private static Model model;

        private static float[] vertices = {
            0.5f, 0.5f, 0,        //(1, 1)
            0.5f, -0.5f, 0,       //Bottom Right
            -0.5f, -0.5f, 0,      //(-1, -1)
            -0.5f, -0.5f, 0,      //Bottom Left
            -0.5f, 0.5f, 0,       //(-1, 1)
            0.5f, 0.5f, 0,         //Top Right
        };

        private static float[] uvs = {
            1.0f, 1.0f,
            1.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f
        };

        [ModuleLoad(mainThread = true)]
        private static void CreateSquareModel()
        {
            model = new Model();
            model.GenerateBuffers(vertices, uvs);
        }

        public IModel CreateModel()
        {
            return model;;
        }

    }
}
