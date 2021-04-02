using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    #region Singleton
    public static LoadingManager instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    AsyncOperation loadingScene;  
    
    public void StartLoadingScene(GameSceneType gameSceneType)
    {
        string _name = string.Empty;
        if (gameSceneType == GameSceneType.Menu)
            _name = "Menu";
        else if (gameSceneType == GameSceneType.Normal)
            _name = "Normal Mode";
        else if (gameSceneType == GameSceneType.Hardcore)
            _name = "Hardcore Mode";
        else
            _name = "Reverse";

        StartCoroutine("LoadingScreen", _name);
    }
    public void AllowSceneActivation()
    {
        if(loadingScene != null && loadingScene.progress == 0.9f)
        {
            loadingScene.allowSceneActivation = true;
        }
    }
    IEnumerator LoadingScreen(string name)
    {
        // ассинхронно загружаем сцену
        loadingScene = SceneManager.LoadSceneAsync(name);
        // запрещаем ее всключение
        loadingScene.allowSceneActivation = false;

        yield return null;
    }

}
