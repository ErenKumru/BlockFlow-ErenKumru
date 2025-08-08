using System;
using System.Collections;
using System.Collections.Generic;
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
        Vector3 direction = grinder.transform.position - block.transform.position;
        direction.y = block.transform.position.y;
        Vector3 targetPosition = direction.normalized * Mathf.Max(grid.GetWidth(), grid.GetHeight());
        bool isBlocked = block.IsPathBlocked(targetPosition, direction, targetPosition.magnitude);

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
