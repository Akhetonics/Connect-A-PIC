using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using Tiles;

public class Grid
{
    public delegate void OnGridCreatedHandler(Tile[,] Tiles);
    public delegate void OnComponentChangedEventHandler(ComponentBase component, int x, int y);
    public event OnGridCreatedHandler OnGridCreated;
    public event OnComponentChangedEventHandler OnComponentPlacedOnTile;
    public event OnComponentChangedEventHandler OnComponentRemoved;
    public event OnComponentChangedEventHandler OnComponentRotated;
    public event OnComponentChangedEventHandler OnComponentMoved;

    public Tile[,] Tiles { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
    }
    public void CreateGrid()
    {
        Tiles = new Tile[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x, y] = new Tile(x, y);
            }
        }
        OnGridCreated?.Invoke(Tiles);
    }

    public bool IsColliding(int x, int y, int sizeX, int sizeY)
    {
        if (IsInGrid(x, y, sizeX, sizeY) == false)
        {
            return true;
        }

        for (int i = x; i < x + sizeX; i++)
        {
            for (int j = y; j < y + sizeY; j++)
            {
                if (Tiles[i, j]?.Component != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool RotateComponentBy90(int tileX, int tileY)
    {
        ComponentBase component = GetComponentAt(tileX, tileY);
        if (component == null) return false;
        var tile = Tiles[tileX, tileY];
        if (tile == null || tile.Component == null) return false;

        var rotatedComponent = tile.Component;
        int x = tile.Component.GridXMainTile;
        int y = tile.Component.GridYMainTile;
        UnregisterComponentAt(tile.GridX, tile.GridY);
        rotatedComponent.RotateBy90();
        try
        {
            PlaceComponent(x, y, rotatedComponent);
        }
        catch (ComponentCannotBePlacedException)
        {
            rotatedComponent.Rotation90 = rotatedComponent.Rotation90 - 1;
            PlaceComponent(x, y, rotatedComponent);
        }
        OnComponentRotated?.Invoke(rotatedComponent, tileX, tileY);
        return true;
    }
    public bool IsInGrid(int x, int y, int width, int height)
    {
        return x >= 0 && y >= 0 && x + width <= this.Width && y + height <= this.Height;
    }
    public ComponentBase GetComponentAt(int x, int y)
    {
        if (IsInGrid(x, y, 1, 1) == false)
        {
            return null;
        }
        return Tiles[x, y].Component;
    }

    public void PlaceComponent(int x, int y, ComponentBase component)
    {
        if (IsColliding(x, y, component.WidthInTiles, component.HeightInTiles))
        {
            throw new ComponentCannotBePlacedException(component);
        }
        component.RegisterPositionInGrid(x, y);
        for (int i = 0; i < component.WidthInTiles; i++)
        {
            for (int j = 0; j < component.HeightInTiles; j++)
            {
                int gridX = x + i;
                int gridY = y + j;
                Tiles[gridX, gridY].Component = component;
            }
        }
        OnComponentPlacedOnTile?.Invoke(component, x, y);
    }
    public void UnregisterComponentAt(int x, int y)
    {
        ComponentBase item = GetComponentAt(x, y);
        if (item == null) return;
        x = item.GridXMainTile;
        y = item.GridYMainTile;
        for (int i = 0; i < item.WidthInTiles; i++)
        {
            for (int j = 0; j < item.HeightInTiles; j++)
            {
                Tiles[x + i, y + j].Component = null;
            }
        }
        OnComponentRemoved?.Invoke(item, x, y);
        item.ClearGridData();
    }
    //public ComponentBase PlaceComponentByType(int x, int y, Type componentType)
    //{
    //    ComponentBase item = ComponentFactory.Instance.CreateComponent(componentType).;
    //    if (IsColliding(x, y, item.WidthInTiles, item.HeightInTiles))
    //    {
    //        return null;
    //    }
    //    PlaceComponent(x, y, item);
    //    return item;
    //}
    public bool MoveComponent(int x , int y, ComponentBase component)
    {
        int oldMainGridx = component.GridXMainTile;
        int oldMainGridy = component.GridYMainTile;
        UnregisterComponentAt(component.GridXMainTile, component.GridYMainTile); // to avoid blocking itself from moving only one tile into its own subtiles
        try
        {
            PlaceComponent(x, y, component);
            OnComponentMoved?.Invoke(component, x, y);
            return true;
        }
        catch (ComponentCannotBePlacedException)
        {
            PlaceComponent(oldMainGridx, oldMainGridy, component);
        }
        return false;
    }
}
