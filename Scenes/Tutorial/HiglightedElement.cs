using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HighlightedElement<T>
{
    public T HighlightedNode;

    public float marginTop = 0;
    public float marginLeft = 0;
    public float marginRight = 0;
    public float marginBottom = 0;

    public float OffsetX = 0;
    public float OffsetY = 0;

    public float customSizeX = 0;
    public float customSizeY = 0;

    public HighlightedElement(T highlightedNode)
    {
        HighlightedNode = highlightedNode;
    }

    public HighlightedElement<T> SetMargins(float marginLeft, float marginTop, float marginRight, float marginBottom)
    {
        this.marginLeft = marginLeft;
        this.marginTop = marginTop;
        this.marginRight = marginRight;
        this.marginBottom = marginBottom;
        return this;
    }
    public HighlightedElement<T> SetMargins(float allMargins)
    {
        this.marginLeft = allMargins;
        this.marginTop = allMargins;
        this.marginRight = allMargins;
        this.marginBottom = allMargins;
        return this;
    }
    public HighlightedElement<T> SetOffsets(float offsetX, float offsetY)
    {
        this.OffsetX = offsetX;
        this.OffsetY = offsetY;
        return this;
    }
    public HighlightedElement<T> SetSize(float sizeX, float sizeY)
    {
        this.customSizeX = sizeX;
        this.customSizeY = sizeY;
        return this;
    }
}
