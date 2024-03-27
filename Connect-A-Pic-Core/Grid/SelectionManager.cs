using CAP_Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Grid
{
    public class SelectionManager
    {
        public UniqueObservableCollection<IntVector> Selections { get; set; } = new();
        public SelectionManager(GridManager grid)
        {
            grid.ComponentMover.OnComponentRemoved += (Components.Component component, int x, int y) =>
            {
                IntVector? elementToRemove = null;
                foreach( IntVector s in Selections)
                {
                    if(s.X == x && s.Y == y)
                    {
                        elementToRemove = s;
                        break;
                    }
                }
                if(elementToRemove != null)
                {
                    Selections.Remove(elementToRemove);
                }  
            };
        }

    }
}
