using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAP_Core.Helpers;

namespace CAP_Core.Components.Creation
{
    public interface IComponentFactory
    {
        Component CreateComponent(int componentTypeNumber);
        Component CreateComponentByIdentifier(string identifier);
        IntVector GetDimensions(int componentTypeNumber);
        void InitializeComponentDrafts(List<Component> componentDrafts);
    }
}
