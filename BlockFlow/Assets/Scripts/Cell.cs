using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public static event Action<Cell, Grinder, Block> OnTryGrind;

    private Vector2Int coordinates;

    private BlockPart currentBlockPart = null;
    private List<Grinder> grinders = new List<Grinder>();

    public void Initialize(int x, int y)
    {
        coordinates = new Vector2Int(x, y);
        name = "Cell_" + y + "_" + x;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Cell & BlockPart interaction
        if(other.TryGetComponent(out BlockPart blockPart))
        {
            //Set pair
            blockPart.SetCurrentCell(this);
            FillCell(blockPart);

            //Grinder interaction
            if(grinders != null && grinders.Count > 0)
            {
                Block block = blockPart.GetBlock();

                foreach(Grinder grinder in grinders)
                {
                    //Matching color
                    if(grinder.GetColor() == block.GetColor())
                    {
                        OnTryGrind?.Invoke(this, grinder, block);
                        break;
                    }
                }
            }
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

    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }

    public List<Grinder> GetGrinders()
    {
        return grinders;
    }
}
