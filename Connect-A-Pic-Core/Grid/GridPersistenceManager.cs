using CAP_Contracts;
using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CAP_Core.Grid
{
    public class GridPersistenceManager
    {
        public GridPersistenceManager(GridManager myGrid , IDataAccessor dataAccessor , IComponentFactory componentFactory)
        {
            MyGrid = myGrid;
            DataAccessor = dataAccessor;
            ComponentFactory = componentFactory;
        }

        public GridManager MyGrid { get; }
        public IDataAccessor DataAccessor { get; }
        public IComponentFactory ComponentFactory { get; }

        public async Task SaveAsync(string path)
        {
            List<GridComponentData> gridData = new ();
            for (int x = 0 ; x < MyGrid.Tiles.GetLength(0); x++)
            {
                for(int y = 0; y < MyGrid.Tiles.GetLength(1); y++)
                {
                    if (MyGrid.Tiles[x, y]?.Component == null ) continue;
                    var component = MyGrid.Tiles[x, y].Component;
                    if (x != component.GridXMainTile || y != component.GridYMainTile) continue;
                    gridData.Add(new GridComponentData()
                    {
                        Identifier = component.Identifier,
                        Rotation = (int)component.Rotation90CounterClock,
                        X = x,
                        Y = y,
                    }) ;
                }
            }
            var json = JsonSerializer.Serialize(gridData);
            await DataAccessor.Write(path,json);
        }
        public async Task LoadAsync(string path)
        {
            await Task.Run(() =>
            {
                var json = DataAccessor.ReadAsText(path);
                var gridData = JsonSerializer.Deserialize<List<GridComponentData>>(json);
                MyGrid.DeleteAllComponents();

                foreach (var data in gridData)
                {
                    var component = ComponentFactory.CreateComponentByIdentifier(data.Identifier);
                    component.Rotation90CounterClock = (DiscreteRotation)data.Rotation;
                    MyGrid.PlaceComponent(data.X, data.Y, component);
                }
            });
        }

        public class GridComponentData
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Rotation { get; set; }
            public string Identifier { get; set; }
        }
    }

}
