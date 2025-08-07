using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPart : MonoBehaviour
{
    private Cell currentCell = null;

    public void SetCurrentCell(Cell cell)
    {
        if(currentCell != null && currentCell.GetBlockPart() == this)
            currentCell.ClearCell();

        currentCell = cell;
    }

    public Cell GetCurrentCell()
    {
        return currentCell;
    }

    public void SetActive(bool status)
    {
        gameObject.SetActive(status);
    }
}
