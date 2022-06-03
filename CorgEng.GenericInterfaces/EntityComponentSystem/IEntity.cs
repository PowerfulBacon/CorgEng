using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IEntity
    {

        List<IComponent> Components { get; }

        int ContentsIndex { get; set; }

        int Identifier { get; }

        void AddComponent(IComponent component);

        void RemoveComponent(IComponent component, bool networked);

        void HandleSignal(IEvent signal);

    }
}
