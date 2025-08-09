using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelHolderSO", menuName = "Levels/LevelHolder")]
public class LevelHolderSO : ScriptableObject
{
    [SerializeField] private List<TextAsset> levels;

    public TextAsset GetLevel(int index)
    {
        return levels[index];
    }

    public int GetLevelCount()
    {
        return levels.Count;
    }
}
