using System;
using System.Collections;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public static event Action<LevelData> OnLevelRead;

    [SerializeField] private LevelHolderSO levelHolder;

    private LevelData currentLevelData;
    private int currentLevelIndex = 0;

    public override void Awake()
    {
        base.Awake();
        BoardController.OnBoardCleared += UpdateCurrentLevelIndex;
    }

    private void Start()
    {
        ReadLevel();
    }

    public void ReadLevel()
    {
        //Read Level Data
        currentLevelData = JsonUtility.FromJson<LevelData>(levelHolder.GetLevel(currentLevelIndex).text);
        OnLevelRead?.Invoke(currentLevelData);
    }

    private void UpdateCurrentLevelIndex()
    {
        //Circling same levels
        currentLevelIndex = (currentLevelIndex + 1) % levelHolder.GetLevelCount();
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    private void OnDestroy()
    {
        BoardController.OnBoardCleared -= UpdateCurrentLevelIndex;
    }
}
