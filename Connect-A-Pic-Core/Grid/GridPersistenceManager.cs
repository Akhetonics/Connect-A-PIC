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
        public GridPersistenceManager(GridManager myGrid, IDataAccessor dataAccessor)
        {
            MyGrid = myGrid;
            DataAccessor = dataAccessor;
        }

        public GridManager MyGrid { get; }
        public IDataAccessor DataAccessor { get; }

        public async Task<bool> SaveAsync(string path)
        {
            List<GridComponentData> gridData = new();
            for (int x = 0; x < MyGrid.Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < MyGrid.Tiles.GetLength(1); y++)
                {
                    if (MyGrid.Tiles[x, y]?.Component == null) continue;
                    var component = MyGrid.Tiles[x, y].Component;
                    if (x != component.GridXMainTile || y != component.GridYMainTile) continue;

                    gridData.Add(new GridComponentData()
                    {
                        Identifier = component.Identifier,
                        Rotation = (int)component.Rotation90CounterClock,
                        Sliders = component.GetAllSliders(),
                        X = x,
                        Y = y,
                    }) ;
                }
            }
            var json = JsonSerializer.Serialize(gridData);
            return await DataAccessor.Write(path, json);
        }
        public async Task LoadAsync(string path, IComponentFactory componentFactory)
        {
            var json = DataAccessor.ReadAsText(path);
            var gridData = JsonSerializer.Deserialize<List<GridComponentData>>(json);
            MyGrid.DeleteAllComponents();

            foreach (var data in gridData)
            {
                var component = componentFactory.CreateComponentByIdentifier(data.Identifier);
                component.Rotation90CounterClock = (DiscreteRotation)data.Rotation;
                LoadSliders(data, component);
                MyGrid.PlaceComponent(data.X, data.Y, component);
            }
        }

        /// <summary>
        /// inserts the slider with Value, Number, Min/Max values from GridComponentData into the actual Component 
        /// Or updates its Value if the slider is already defined by the ComponentDraft
        /// </summary>
        /// <param name="data"></param>
        /// <param name="component"></param>
        private static void LoadSliders(GridComponentData? data, Component component)
        {
            if (data?.Sliders != null)
            {
                foreach (var sliderToLoad in data.Sliders)
                {
                    var predefinedSlider = component.GetSlider(sliderToLoad.Number);
                    if(predefinedSlider == null)
                    {
                        component.AddSlider(sliderToLoad.Number, sliderToLoad);
                    }
                    else
                    {
                        predefinedSlider.Value = sliderToLoad.Value;
                    }
                }
            }
        }

        public class GridComponentData
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Rotation { get; set; }
            public string Identifier { get; set; }
            public List<Slider>? Sliders { get; set; }
        }
        public class GridSliderData
        {
            public GridSliderData(int nr , double value)
            {
                Nr = nr;
                Value = value;
            }
            public int Nr { get; set; }
            public double Value { get; set; }
        }
    }

}
