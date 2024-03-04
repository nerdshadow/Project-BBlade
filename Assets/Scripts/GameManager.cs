using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    GameObject loadingScreen;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitManager();
    }
    void InitManager()
    {
        
    }
    public void ExitGame()
    {
        Debug.Log("Trying exit game");
        Application.Quit();
    }
    public void LoadLevel(string levelName)
    {
        GameObject currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<Canvas>().transform);
        Slider currentSlider = currentLoadingScreen.GetComponentInChildren<Slider>();
        StartCoroutine(LoadLevelAsync(levelName, currentSlider));
    }
    IEnumerator LoadLevelAsync(string _levelName, Slider _loadingSlider)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_levelName);
        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            _loadingSlider.value = progress;
            yield return null;
        }
    }
    public void PauseGame()
    {
        //pause game
        Debug.Log("Pausinggame");
    }
    public void ResumeGame()
    {
        //resume game
        Debug.Log("UnPausinggame");
    }
}
