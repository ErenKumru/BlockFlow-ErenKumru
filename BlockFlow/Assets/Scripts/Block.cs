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
    [SerializeField] private List<BoxCollider> shapePartBoxColliders;
    [SerializeField] private SpriteRenderer horizontalArrow;
    [SerializeField] private SpriteRenderer verticalArrow;

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


    public void Initialize(BlockSpawnData blockSpawnData)
    {
        //Shift to actual board position
        rigidBody.position += initialShift;

        //Assign data
        color = blockSpawnData.color;
        restriction = (Restriction)blockSpawnData.restriction;

        meshRenderer.material.color = color;

        //Display movement arrows
        if(restriction == Restriction.Horizontal)
            horizontalArrow.gameObject.SetActive(true);
        else if(restriction == Restriction.Vertical)
            verticalArrow.gameObject.SetActive(true);

        foreach(BlockPart blockPart in parts)
        {
            blockPart.SetActive(true);
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

    private bool IsPathBlocked(Vector3 position, Vector3 direction, float distance)
    {
        foreach(BoxCollider boxCollider in shapePartBoxColliders)
        {
            Vector3 boxColliderWorldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            Vector3 stepStart = position + (boxColliderWorldCenter - rigidBody.position);
            Vector3 halfExtents = Vector3.Scale(boxCollider.size * 0.5f, boxCollider.transform.lossyScale);

            //If movement blocked for this boxCollider no need to check for the rest, cannot move!
            if(Physics.BoxCast(stepStart, halfExtents, direction, 
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

    public void SetKinematic(bool value)
    {
        rigidBody.isKinematic = value;
    }

    private enum Restriction
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
    }
}
