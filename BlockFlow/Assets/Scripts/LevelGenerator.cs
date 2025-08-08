using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static event Action<Grid, LevelData> OnLevelGenerated;

    [Header("Prefabs")]
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private List<Block> blockPrefabs;
    [SerializeField] private List<Grinder> grinderPrefabs;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject cornerWallPrefab;

    [Header("Parents")]
    [SerializeField] private Transform cellParent;
    [SerializeField] private Transform blockParent;
    [SerializeField] private Transform grinderParent;

    //Data
    private Dictionary<string, Block> blocksDictianory;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        blocksDictianory = new Dictionary<string, Block>();

        foreach(Block block in blockPrefabs)
        {
            blocksDictianory.Add(block.key, block);
        }

        LevelManager.OnGenerateLevel += GenerateLevel;
    }

    public void GenerateLevel(LevelData levelData)
    {
        Grid grid = GenerateGrid(levelData);
        SpawnBlocks(levelData);
        SpawnGrinders(levelData, grid);
        SpawnWalls(levelData);

        OnLevelGenerated?.Invoke(grid, levelData);
    }

    private Grid GenerateGrid(LevelData levelData)
    {
        Grid grid = new Grid(levelData.width, levelData.height);

        for(int y = 0;  y < levelData.height; y++)
        {
            for(int x = 0; x < levelData.width; x++)
            {
                Cell cell = Instantiate(cellPrefab, new Vector3(x, 0, y), cellPrefab.transform.rotation, cellParent);
                cell.Initialize(x, y);
                grid.SetCell(x, y, cell);
            }
        }

        return grid;
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

    private void SpawnGrinders(LevelData levelData, Grid grid)
    {
        foreach(GrinderSpawnData grinderSpawnData in levelData.grinderSpawnDatas)
        {
            //Spawn grinder
            int grinderIndex = grinderSpawnData.size - 1;
            Grinder grinderPrefab = grinderPrefabs[grinderIndex];
            Grinder grinder = Instantiate(grinderPrefab, new Vector3(grinderSpawnData.x, 0, grinderSpawnData.y), grinderPrefab.transform.rotation, grinderParent);
            grinder.Initialize(grinderSpawnData);

            //Set grinder to cells
            Cell cell;

            for(int i = 0; i < grinderSpawnData.size; i++)
            {
                if(grinderSpawnData.isVertical)
                    cell = grid.GetCell(grinderSpawnData.x, grinderSpawnData.y + i);
                else
                    cell = grid.GetCell(grinderSpawnData.x + i, grinderSpawnData.y);

                cell.SetGrinder(grinder);
            }
        }
    }

    private void SpawnWalls(LevelData levelData)
    {

    }

    private void OnDestroy()
    {
        LevelManager.OnGenerateLevel -= GenerateLevel;
    }
}
