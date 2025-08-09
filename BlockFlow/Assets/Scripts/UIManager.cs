using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private GameObject successPanel;

    private void Awake()
    {
        LevelGenerator.OnLevelGenerated += UpdateLevelText;
        BoardController.OnTimeChanged += UpdateTimeText;
        BoardController.OnBoardCleared += DisplaySuccessPanel;
        nextLevelButton.onClick.AddListener(SceneManager.Instance.LoadNextLevel);
    }

    private void UpdateLevelText(Grid grid, LevelData levelData)
    {
        levelText.text = (levelData.levelIndex + 1).ToString();
    }

    private void UpdateTimeText(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;

        timeText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    private void DisplaySuccessPanel()
    {
        successPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        LevelGenerator.OnLevelGenerated -= UpdateLevelText;
        BoardController.OnTimeChanged -= UpdateTimeText;
        BoardController.OnBoardCleared -= DisplaySuccessPanel;
        nextLevelButton.onClick.RemoveAllListeners();
    }
}
