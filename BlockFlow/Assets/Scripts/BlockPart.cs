using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPart : MonoBehaviour
{
    private Cell currentCell = null;
    private Block block;

    private void Awake()
    {
        block = GetComponentInParent<Block>();
    }

    public Vector3 SnapDifference()
    {
        Vector3 snapVector = Vector3.zero;
        snapVector.x = transform.position.x - currentCell.transform.position.x;
        snapVector.z = transform.position.z - currentCell.transform.position.z;
        return snapVector;
    }

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

    public Block GetBlock()
    {
        return block;
    }

    public void SetActive(bool status)
    {
        gameObject.SetActive(status);
    }
}
