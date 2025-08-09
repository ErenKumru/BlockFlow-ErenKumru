using System;
using System.Collections;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static event Action<Cell, Grinder, Block> OnSuccessfulGrind;
    public static event Action OnTimeEnd;
    public static event Action OnBoardCleared;
    public static event Action<int> OnTimeChanged;

    private Grid grid;
    private int currentLevel;
    private int timeInSeconds;
    private int blockCount = 0;
    private Coroutine timeCountRoutine;

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
        blockCount = levelData.blockSpawnDatas.Count;
        timeCountRoutine = StartCoroutine(CountTime());
    }

    private void CheckBoardClear()
    {
        blockCount--;

        if(blockCount == 0)
        {
            OnBoardCleared?.Invoke();
        }
    }

    private IEnumerator CountTime()
    {
        while(timeInSeconds > 0)
        {
            OnTimeChanged?.Invoke(timeInSeconds);
            yield return new WaitForSeconds(1);
            timeInSeconds--;
        }

        //Time ended
        OnTimeEnd?.Invoke();
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
            float step = Mathf.Min(0.01f, distance - movedDistance);
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
            CheckBoardClear();
        }
    }

    private void OnDestroy()
    {
        LevelGenerator.OnLevelGenerated -= InitializeBoard;
        Cell.OnTryGrind -= TryGrind;
    }
}
