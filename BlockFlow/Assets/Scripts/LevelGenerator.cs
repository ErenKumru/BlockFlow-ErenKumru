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
    [SerializeField] private Transform wallParent;

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

        LevelManager.OnLevelRead += GenerateLevel;
    }

    public void GenerateLevel(LevelData levelData)
    {
        Grid grid = GenerateGrid(levelData);
        SpawnBlocks(levelData, grid);
        SpawnGrinders(levelData, grid);
        SpawnWalls(levelData, grid);

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

    private void SpawnBlocks(LevelData levelData, Grid grid)
    {
        Vector3 gridMin = Vector3.zero - Vector3.one;
        Vector3 gridMax = new Vector3(grid.GetWidth(), 2, grid.GetHeight());

        foreach(BlockSpawnData blockSpawnData in levelData.blockSpawnDatas)
        {
            if(blocksDictianory.TryGetValue(blockSpawnData.key, out Block blockPrefab))
            {
                Block block = Instantiate(blockPrefab, new Vector3(blockSpawnData.x, 0, blockSpawnData.y), blockPrefab.transform.rotation, blockParent);
                block.Initialize(blockSpawnData, gridMin, gridMax);
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

    private void SpawnWalls(LevelData levelData, Grid grid)
    {
        int widthPos = grid.GetWidth() - 1;
        int heightPos = grid.GetHeight() - 1;

        //Horizontal walls
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            Cell bottomCell = grid.GetCell(x, 0);
            Cell topCell = grid.GetCell(x, heightPos);

            if(!bottomCell.HasHorizontalGrinder())
            {
                GameObject wall = Instantiate(wallPrefab, bottomCell.transform.position, wallPrefab.transform.rotation, wallParent);
                wall.transform.Rotate(new Vector3(0, 90, 0));
                wall.transform.localPosition += Vector3.back * 0.5f;
            }

            if(!topCell.HasHorizontalGrinder())
            {
                GameObject wall = Instantiate(wallPrefab, topCell.transform.position, wallPrefab.transform.rotation, wallParent);
                wall.transform.Rotate(new Vector3(0, 270, 0));
                wall.transform.localPosition += Vector3.forward * 0.5f;
            }
        }

        //Vertical walls
        for(int y = 0; y < grid.GetWidth(); y++)
        {
            Cell leftCell = grid.GetCell(0, y);
            Cell rightCell = grid.GetCell(widthPos, y);

            if(!leftCell.HasVerticalGrinder())
            {
                GameObject wall = Instantiate(wallPrefab, leftCell.transform.position, wallPrefab.transform.rotation, wallParent);
                wall.transform.Rotate(new Vector3(0, 180, 0));
                wall.transform.localPosition += Vector3.left * 0.5f;
            }

            if(!rightCell.HasVerticalGrinder())
            {
                GameObject wall = Instantiate(wallPrefab, rightCell.transform.position, wallPrefab.transform.rotation, wallParent);
                wall.transform.localPosition += Vector3.right * 0.5f;
            }
        }

        //Corner walls
        Vector3 bottomLeft = new Vector3(-0.5f, 0, -0.5f);
        Vector3 bottomRight = Vector3.right * widthPos + new Vector3(0.5f, 0, -0.5f);
        Vector3 topLeft = Vector3.forward * heightPos + new Vector3(-0.5f, 0, 0.5f);
        Vector3 topRight = new Vector3(widthPos, 0, heightPos) + new Vector3(0.5f, 0 , 0.5f);

        Vector3 rotationBL = Vector3.zero;
        Vector3 rotationBR = new Vector3(0, -90, 0);
        Vector3 rotationTL = new Vector3(0, 90, 0);
        Vector3 rotationTR = new Vector3(0, 180, 0);

        SpawnCorner(bottomLeft, rotationBL);
        SpawnCorner(bottomRight, rotationBR);
        SpawnCorner(topLeft, rotationTL);
        SpawnCorner(topRight, rotationTR);
    }

    private void SpawnCorner(Vector3 position, Vector3 rotation)
    {
        GameObject cornerWall = Instantiate(cornerWallPrefab, position, cornerWallPrefab.transform.rotation, wallParent);
        cornerWall.transform.Rotate(rotation);
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelRead -= GenerateLevel;
    }
}
