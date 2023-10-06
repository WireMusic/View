using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stage.Core
{
    public interface ILayer
    {
        string Name { get; }

        void OnAttach();
        void OnDetach();
        void OnUpdate();
        void OnUI();
    }
}
