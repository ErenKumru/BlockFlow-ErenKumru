using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Awake()
    {
        LevelGenerator.OnLevelGenerated += SetupCamera;
    }

    private void SetupCamera(Grid grid, LevelData levelData)
    {
        Vector2 firstCellPos = grid.GetCell(0, 0).transform.position;
        Vector2 lastCellPos = grid.GetCell(levelData.width - 1, levelData.height - 1).transform.position;

        ResizeCamera(firstCellPos, lastCellPos);
        CenterCameraToGrid(lastCellPos, levelData);
    }

    private void ResizeCamera(Vector2 firstCellPos, Vector2 lastCellPos)
    {
        float sceneWidth = Mathf.Abs(lastCellPos.x - firstCellPos.x) + 2.5f;
        float sceneHeight = Mathf.Abs(lastCellPos.y - firstCellPos.y) + 2.5f;

        float unitsPerPixel = Mathf.Max(sceneWidth, sceneHeight) / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        Camera.main.orthographicSize = Mathf.Max(desiredHalfHeight, 3);
    }

    private void CenterCameraToGrid(Vector2 lastCellPos, LevelData levelData)
    {
        Camera cam = Camera.main;
        Vector3 centerPos = lastCellPos / 2f;
        centerPos.y = levelData.height * 2;
        cam.transform.position = centerPos;
    }

    private void OffsetGridUp()
    {
        Vector2 pos = transform.position;
        pos.y += 0.5f;
        transform.position = pos;
    }

    private void OnDestroy()
    {
        LevelGenerator.OnLevelGenerated -= SetupCamera;
    }
}
