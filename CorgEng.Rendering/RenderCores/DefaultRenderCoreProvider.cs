using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.RenderCores
{
	[Dependency(priority = 50)]
	internal class DefaultRenderCoreProvider : IRenderCoreProvider
	{
		public IRenderCore CreateDefaultRenderCore()
		{
			return new DefaultRenderCore();
		}
	}
}
