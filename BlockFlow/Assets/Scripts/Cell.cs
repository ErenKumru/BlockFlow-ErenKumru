using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private BlockPart currentBlockPart = null;
    //private Grinder grinder = null;

    public void FillCell(BlockPart blockPart)
    {
        currentBlockPart = blockPart;
    }

    public void ClearCell()
    {
        currentBlockPart = null;
    }

    public BlockPart GetBlockPart()
    {
        return currentBlockPart;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out BlockPart blockPart))
        {
            blockPart.SetCurrentCell(this);
            FillCell(blockPart);
        }
    }
}
