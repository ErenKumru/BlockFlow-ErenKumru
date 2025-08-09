using System;
using System.Collections;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static event Action<Cell, Grinder, Block> OnSuccessfulGrind;

    private Grid grid;
    private int timeInSeconds;
    private int currentLevel;

    private void Awake()
    {
        LevelGenerator.OnLevelGenerated += InitializeBoard;
        Cell.OnTryGrind += TryGrind;
    }

    public void InitializeBoard(Grid grid, LevelData levelData)
    {
        this.grid = grid;
        currentLevel = levelData.levelIndex + 1;
        timeInSeconds = levelData.seconds;
    }

    private void TryGrind(Cell cell, Grinder grinder, Block block)
    {
        //Check if blocked
        grinder.SetColliderActive(false);
        Vector3 snappedPosition = block.GetSnapVector(cell.GetBlockPart());
        Vector3 direction = grinder.transform.position - snappedPosition;
        direction.y = block.transform.position.y;

        if(grinder.IsVertical())
            direction.z = 0;
        else
            direction.x = 0;

        float movedDistance = 0f;
        Vector3 stepPosition;
        float distance = direction.magnitude * 2;
        bool isBlocked = false;

        while(movedDistance < distance)
        {
            float step = Mathf.Min(0.05f, distance - movedDistance);
            stepPosition = snappedPosition + direction * movedDistance;

            if(block.IsPathBlocked(stepPosition, direction, step))
            {
                isBlocked = true;
                break;
            }

            movedDistance += step;
        }

        //Actually grind
        if(!isBlocked)
        {
            block.OnReadyToGrind += grinder.Grind;
            OnSuccessfulGrind?.Invoke(cell, grinder, block);
        }
    }

    private void OnDestroy()
    {
        LevelGenerator.OnLevelGenerated -= InitializeBoard;
        Cell.OnTryGrind -= TryGrind;
    }
}
