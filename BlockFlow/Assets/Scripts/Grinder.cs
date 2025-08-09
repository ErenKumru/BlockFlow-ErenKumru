using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider col;
    [SerializeField] private ParticleSystem dustParticles;

    [Header("Runtime Values")]
    [SerializeField] private Vector3 initialShift;

    //Data
    private bool isVertical;
    private Color color;

    public void Initialize(GrinderSpawnData grinderSpawnData)
    {
        //Assign data
        isVertical = grinderSpawnData.isVertical;
        color = grinderSpawnData.color;
        
        foreach(Material material in meshRenderer.materials)
        {
            material.color = color;
        }

        ShiftAndRotate(grinderSpawnData);

        ParticleSystem.MainModule mainModule = dustParticles.main;
        mainModule.startColor = color;
    }

    public void Grind(Block block)
    {
        dustParticles.Play();

        Vector3 difference = transform.position - block.transform.position;
        difference.y = block.transform.position.y;
        Vector3 target = block.transform.position - transform.right * difference.magnitude;
        target.y = block.transform.position.y;
        block.transform.DOMove(target, 0.8f).OnComplete(() =>
        {
            block.OnReadyToGrind -= Grind;
            Destroy(block.gameObject);
            SetColliderActive(true);
        });
    }

    private void ShiftAndRotate(GrinderSpawnData grinderSpawnData)
    {
        //Shift positon & rotate to get the right position
        Vector3 shift = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        if(isVertical)
        {
            shift.z = initialShift.z * (grinderSpawnData.size - 1);

            if(grinderSpawnData.x == 0)
                shift.x = -initialShift.x;
            else
            {
                shift.x = initialShift.x;
                rotation.y = 180;
            }
        }
        else
        {
            shift.x = initialShift.x * (grinderSpawnData.size - 1);

            if(grinderSpawnData.y == 0)
            {
                shift.z = -initialShift.z;
                rotation.y = -90;
            }
            else
            {
                shift.z = initialShift.z;
                rotation.y = 90;
            }
        }

        transform.localPosition += shift;
        transform.Rotate(rotation, Space.Self);
    }

    public bool IsVertical()
    {
        return isVertical;
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetColliderActive(bool status)
    {
        col.enabled = status;
    }
}
