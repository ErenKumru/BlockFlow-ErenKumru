using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("KEY")]
    public string key;

    [Header("Parts")]
    [SerializeField] private BlockPart[] parts;

    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rigidBody;
    //[SerializeField] private SpriteRenderer spriteRenderer; //restriction dislaying arrow as child object, might need its own class

    [Header("Transform Modifications")]
    [SerializeField] private Vector3 initialShift;

    //Data
    private Color color;
    private Restriction restriction;

    public void Initialize(BlockSpawnData blockSpawnData)
    {
        //Shift to actual board position
        rigidBody.position += initialShift;

        //Assign data
        color = blockSpawnData.color;
        restriction = (Restriction)blockSpawnData.restriction;

        meshRenderer.material.color = color;
        //TODO: show restriction related visuals etc. 

        foreach(BlockPart blockPart in parts)
        {
            blockPart.SetActive(true);
        }
    }

    private enum Restriction
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
    }
}
