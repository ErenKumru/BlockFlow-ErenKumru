using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private BlockPart currentBlockPart = null;
    private List<Grinder> grinders = new List<Grinder>();

    private void OnTriggerEnter(Collider other)
    {
        //Pair Cell & BlockPart
        if(other.TryGetComponent(out BlockPart blockPart))
        {
            blockPart.SetCurrentCell(this);
            FillCell(blockPart);
        }

        //Grinder interaction
        if(grinders != null || grinders.Count > 0)
        {
            //TODO: Grinder checks & grinds if possible
        }
    }

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

    public void SetGrinder(Grinder grinder)
    {
        grinders.Add(grinder);
    }

    public List<Grinder> GetGrinder()
    {
        return grinders;
    }
}
