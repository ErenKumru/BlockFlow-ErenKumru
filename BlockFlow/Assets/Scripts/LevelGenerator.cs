using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private List<Block> blockPrefabs;
    [SerializeField] private Transform cellParent;
    [SerializeField] private Transform blockParent;
    //TODO: Add walls and grinders etc.

    private Dictionary<string, Block> blocksDictianory;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        blocksDictianory = new Dictionary<string, Block>();

        foreach (Block block in blockPrefabs)
        {
            blocksDictianory.Add(block.key, block);
        }

        LevelManager.OnGenerateLevel += GenerateLevel;
    }

    public void GenerateLevel(LevelData levelData)
    {
        GenerateGrid(levelData);
        SpawnBlocks(levelData);
    }

    private void GenerateGrid(LevelData levelData)
    {
        Grid grid = new Grid(levelData.width, levelData.height);

        for(int y = 0;  y < levelData.height; y++)
        {
            for(int x = 0; x < levelData.width; x++)
            {
                Cell cell = Instantiate(cellPrefab, new Vector3(x, 0, y), cellPrefab.transform.rotation, cellParent);
                grid.SetCell(x, y, cell);
            }
        }

        LevelManager.Instance.Grid = grid;
    }

    private void SpawnBlocks(LevelData levelData)
    {
        foreach(BlockSpawnData blockSpawnData in levelData.blockSpawnDatas)
        {
            if(blocksDictianory.TryGetValue(blockSpawnData.key, out Block blockPrefab))
            {
                Block block = Instantiate(blockPrefab, new Vector3(blockSpawnData.x, 0, blockSpawnData.y), blockPrefab.transform.rotation, blockParent);
                block.Initialize(blockSpawnData);
            }
        }
    }

    private void OnDestroy()
    {
        LevelManager.OnGenerateLevel -= GenerateLevel;
    }
}
