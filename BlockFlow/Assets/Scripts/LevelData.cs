using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int levelIndex;
    public int seconds;
    public int width;
    public int height;
    public List<GrinderSpawnData> grinderSpawnDatas;
    public List<BlockSpawnData> blockSpawnDatas;
}

[Serializable]
public struct GrinderSpawnData
{
    public int x;
    public int y;
    public int size;
    public bool isVertical;
    public Color color;
}

[Serializable]
public struct BlockSpawnData
{
    public string key;
    public int x;
    public int y;
    public Color color;
    public int restriction; //0: None, 1: Horizontal, 2: Vertical
    public int iceCount; //0: None, >0: Add ice with count
}
