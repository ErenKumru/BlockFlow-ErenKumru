using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private Cell[,] cells;

    public Grid(int width, int height)
    {
        cells = new Cell[width, height];
    }

    public void SetCell(int x, int y, Cell cell)
    {
        cells[x, y] = cell;
    }

    public Cell GetCell(int x, int y) 
    { 
        return cells[x, y]; 
    }

    public int GetWidth()
    {
        return cells.GetLength(0);
    }

    public int GetHeight() 
    { 
        return cells.GetLength(1); 
    }
}
