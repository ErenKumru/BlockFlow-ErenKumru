using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public event Action<Block> OnReadyToGrind;

    [Header("KEY")]
    public string key;

    [Header("Parts")]
    [SerializeField] private BlockPart[] parts;

    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private List<BoxCollider> shapePartBoxColliders;
    [SerializeField] private SpriteRenderer horizontalArrow;
    [SerializeField] private SpriteRenderer verticalArrow;
    [SerializeField] private Ice ice;

    [Header("Runtime Values")]
    [SerializeField] private Vector3 initialShift;
    [SerializeField] private float minDistance = 0.01f;
    [SerializeField] private float boxCastStep = 0.05f;
    [SerializeField] private float castMargin = 0.01f;
    [SerializeField] private LayerMask collisionMask;

    //Data
    private Color color;
    private Restriction restriction;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private void Awake()
    {
        BoardController.OnSuccessfulGrind += PrepareToGrind;
    }

    public void Initialize(BlockSpawnData blockSpawnData, Vector3 gridMin, Vector3 gridMax)
    {
        //Shift to actual board position
        rigidBody.position += initialShift;

        //Assign data
        color = blockSpawnData.color;
        restriction = (Restriction)blockSpawnData.restriction;

        meshRenderer.material.color = color;
        meshRenderer.material.SetVector("_ClipMin", new Vector4(gridMin.x, gridMin.y, gridMin.z, 0));
        meshRenderer.material.SetVector("_ClipMax", new Vector4(gridMax.x, gridMax.y, gridMax.z, 0));

        //Display movement arrows
        if(restriction == Restriction.Horizontal)
            horizontalArrow.gameObject.SetActive(true);
        else if(restriction == Restriction.Vertical)
            verticalArrow.gameObject.SetActive(true);

        foreach(BlockPart blockPart in parts)
        {
            blockPart.gameObject.layer = LayerMask.NameToLayer("Unselected");
            blockPart.SetActive(true);
        }

        //Activate ice
        if(blockSpawnData.iceCount <= 0)
            ice.gameObject.SetActive(false);
        else if(blockSpawnData.iceCount > 0)
        {
            ice.Initialize(blockSpawnData.iceCount);
            ice.gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if(!hasTarget)
            return;

        TryMoveToTarget();
    }

    private void TryMoveToTarget()
    {
        Vector3 currentPosition = rigidBody.position;
        Vector3 direction = targetPosition - currentPosition;
        float distance = direction.magnitude;

        //If movement is smaller than minDistance no need to move
        if(distance < minDistance)
        {
            hasTarget = false;
            return;
        }

        direction.Normalize();
        float movedDistance = 0f;
        Vector3 stepPosition;

        //Calculate how far the movement can happen in small steps, until it is blocked
        while(movedDistance < distance)
        {
            float step = Mathf.Min(boxCastStep, distance - movedDistance);
            stepPosition = currentPosition + direction * movedDistance;

            if(IsPathBlocked(stepPosition, direction, step))
            {
                break;
            }

            movedDistance += step;
        }

        //Actually move the body to the calculated safe target position
        if(movedDistance > 0f)
        {
            stepPosition = currentPosition + direction * movedDistance;
            rigidBody.MovePosition(stepPosition);
        }

        hasTarget = false;
    }

    public bool IsPathBlocked(Vector3 position, Vector3 direction, float distance)
    {
        foreach(BoxCollider boxCollider in shapePartBoxColliders)
        {
            Vector3 boxColliderWorldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            Vector3 center = position + (boxColliderWorldCenter - rigidBody.position);
            Vector3 halfExtents = Vector3.Scale(boxCollider.size * 0.5f, boxCollider.transform.lossyScale);

            //If movement blocked for this boxCollider no need to check for the rest, cannot move!
            if(Physics.BoxCast(center, halfExtents, direction, 
                out RaycastHit hit, boxCollider.transform.rotation, distance + castMargin, collisionMask))
            {
                if(!shapePartBoxColliders.Contains((BoxCollider)hit.collider))
                    return true;
            }
        }

        //Free to move
        return false;
    }

    public void SetTargetPosition(Vector3 position)
    {
        if(restriction == Restriction.Horizontal)
            position.z = rigidBody.position.z;
        else if(restriction == Restriction.Vertical)
            position.x = rigidBody.position.x;

        targetPosition = position;
        hasTarget = true;
    }

    public void Snap(bool directly = false)
    {
        Vector3 snapVector = GetSnapVector(parts[0]);

        if(directly)
            transform.localPosition = snapVector;
        else
            SetTargetPosition(snapVector);
    }

    public Vector3 GetSnapVector(BlockPart part)
    {
        Vector3 snapVector = transform.localPosition - part.SnapDifference();
        return snapVector;
    }

    public void SetKinematic(bool value)
    {
        foreach(BlockPart part in parts)
        {
            if(value)
                part.gameObject.layer = LayerMask.NameToLayer("Unselected");
            else
                part.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        rigidBody.isKinematic = value;
    }

    public Color GetColor()
    {
        return color;
    }

    private void PrepareToGrind(Cell cell, Grinder grinder, Block block)
    {
        //Not grind
        if(block != this)
        {
            //Check for ice, if no ice do nothing
            return;
        }

        //Grind block
        foreach(BlockPart blockPart in parts)
        {
            Cell partCell = blockPart.GetCurrentCell();
            partCell.ClearCell();
            blockPart.SetActive(false);
        }

        foreach(BoxCollider boxCollider in shapePartBoxColliders)
        {
            boxCollider.enabled = false;
        }

        hasTarget = false;
        SetKinematic(false);
        Snap(true);

        OnReadyToGrind?.Invoke(this);
    }

    private void OnDestroy()
    {
        BoardController.OnSuccessfulGrind -= PrepareToGrind;
    }

    private enum Restriction
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
    }
}
