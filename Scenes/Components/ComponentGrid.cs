using System;
using System.Collections.Generic;

public class ComponentGrid
{
	private class ComponentGridEntry
	{
		public int X { get; set; }
		public int Y { get; set; }
		public ComponentItem Comp { get; set; }
		public int CompOrientation { get; set; }
		public int CompOffsetX { get; set; }
		public int CompOffsetY { get; set; }
	}

	public int SizeX { get; private set; }
	public int SizeY { get; private set; }

	private List<ComponentItem> _components = new List<ComponentItem>();

	private ComponentGridEntry[,] _grid;

	public ComponentGrid(int sizeX, int sizeY)
	{
		this.SizeX = sizeX;
		this.SizeY = sizeY;

		this._grid = new ComponentGridEntry[this.SizeX, this.SizeY];

		// Pre-fill grid coordinates
		for (int i=0; i<sizeX; i++)
		{
			for (int j=0; j<sizeY; j++)
			{
				this._grid[i, j].X = i;
				this._grid[i, j].Y = j;
			}
		}
	}

	public bool checkCollision(int x, int y, int sizeX, int sizeY)
	{
		if (x < 0 || y < 0 || (x + sizeX - 1) >= this.SizeX || (y + sizeY - 1) > this.SizeY)
		{
			// Out of Bounds
			return true;
		}

		for (int i = x; i < (x + sizeX - 1); i++)
		{
			for (int j = y; j < (y + sizeY - 1); j++)
			{
				if (this._grid[i, j].Comp != null)
				{
					return true;
				}
			}
		}
		
		return false;
	}

	public ComponentItem getComponentAt(int x, int y)
	{
		if (x < 0 || y < 0 || x >= this.SizeX || y > this.SizeY)
		{
			return null;
		}

		return this._grid[x, y].Comp;
	}

	public void placeComponent(int x, int y, int orientation, ComponentItemPrototype prototype)
	{
		if (this.checkCollision(x, y, prototype.SizeX, prototype.SizeY))
		{
			// Out of Bounds
			return;
		}

		ComponentItem item = new ComponentItem(prototype);

		for (int i = 0; i < prototype.SizeX; i++)
		{
			for (int j = 0; j < prototype.SizeY; j++)
			{
				this._grid[x + i, y + j].Comp = item;
				this._grid[x + i, y + j].CompOrientation = orientation;
				this._grid[x + i, y + j].CompOffsetX = i;
				this._grid[x + i, y + j].CompOffsetY = j;
			}
		}
	}

	public void removeComponentAt(int x, int y)
	{
		ComponentItem item = this.getComponentAt(x, y);

		if (item == null)
		{
			return;
		}

		for (int i = 0; i < item.SizeX; i++)
		{
			for (int j = 0; j < item.SizeY; j++)
			{
				this._grid[x + i, y + j].Comp = null;
				this._grid[x + i, y + j].CompOrientation = 0;
				this._grid[x + i, y + j].CompOffsetX = 0;
				this._grid[x + i, y + j].CompOffsetY = 0;
			}
		}
	}
}
