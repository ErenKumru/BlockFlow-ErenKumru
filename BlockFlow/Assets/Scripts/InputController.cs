using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private float dragHeight = 0f; // Fixed Y plane for dragging
    [SerializeField] private LayerMask blockLayer;  // Layer of draggable blocks

    private Camera mainCamera;
    private Plane dragPlane;
    private Ray ray;
    private Vector3 dragOffset;
    private Block selectedBlock;

    private bool canInput = true;

    private void Awake()
    {
        BoardController.OnSuccessfulGrind += DisableInput;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        dragPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                TrySelectBlock(touch.position);
            }
            else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if(selectedBlock != null)
                    UpdateDrag(touch.position);
            }
            else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                DeselectBlock();
            }
        }
    }

    private void TrySelectBlock(Vector2 screenPosition)
    {
        canInput = true;
        ray = mainCamera.ScreenPointToRay(screenPosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 100f, blockLayer))
        {
            if(hit.transform.TryGetComponent(out selectedBlock))
            {
                dragOffset = hit.point - selectedBlock.transform.localPosition;
                selectedBlock.SetKinematic(false);
            }
        }
    }

    private void UpdateDrag(Vector2 screenPosition)
    {
        if(!canInput)
            return;

        ray = mainCamera.ScreenPointToRay(screenPosition);

        if(dragPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 targetPosition = hitPoint - dragOffset;
            selectedBlock.SetTargetPosition(targetPosition);
        }
    }

    private void DeselectBlock()
    {
        if(selectedBlock != null)
        {
            selectedBlock.Snap();
            selectedBlock.SetKinematic(true);
        }

        selectedBlock = null;
    }

    private void DisableInput(Cell cell, Grinder grinder, Block block)
    {
        canInput = false;
    }

    private void OnDestroy()
    {
        BoardController.OnSuccessfulGrind -= DisableInput;
    }
}
