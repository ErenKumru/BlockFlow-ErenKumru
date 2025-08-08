using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public static event Action<LevelData> OnGenerateLevel;
   
    //TODO: Swap this to make it a HolderSO
    public List<TextAsset> levels;

    private LevelData levelData;
    private int currentLevelIndex = 0;

    //TODO: Only for test purposes will be changed later with the level progress system
    private void Start()
    {
        //Read Level Data
        levelData = JsonUtility.FromJson<LevelData>(levels[currentLevelIndex].text);
        OnGenerateLevel?.Invoke(levelData);
    }
}
