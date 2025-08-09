using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneMan = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : Singleton<SceneManager>
{
    public void LoadNextLevel()
    {
        StartCoroutine(SafeLoadProcess());
    }

    private IEnumerator SafeLoadProcess()
    {
        AsyncOperation asyncLoad = SceneMan.LoadSceneAsync(SceneMan.GetActiveScene().buildIndex);

        //Wait for the scene to fully load
        while(!asyncLoad.isDone)
            yield return null;

        //Wait one extra frame for scene objects to initialize
        yield return null;

        //Read next level
        LevelManager.Instance.ReadLevel();
    }
}
