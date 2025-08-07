using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    //Swap this to make it a HolderSO
    public List<TextAsset> levels;

    //class starts here
    public static event Action<LevelData> OnGenerateLevel;

    private LevelData levelData;
    private int currentLevelIndex = 0;

    public Grid Grid; //TODO: Might be better out of this class, look later

    //TODO: Only for test purposes will be changed later with the level progress system
    private void Start()
    {
        //Read Level Data
        levelData = JsonUtility.FromJson<LevelData>(levels[currentLevelIndex].text);
        OnGenerateLevel?.Invoke(levelData);
    }
}
