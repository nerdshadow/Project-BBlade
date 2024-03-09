using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using System.Runtime.ConstrainedExecution;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    GameObject loadingScreen;
    public bool gameIsPaused = false;
    public GameObject playerRef = null;
    public Camera playerCameraRef = null;
    bool playerFound = false;
    #region Score
    [SerializeField]
    int finalScore = 0;
    [SerializeField]
    ScoreBehaviour scoreBeh;
    
    #endregion
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
        //Debug test
        Application.targetFrameRate = 61;
        playerCameraRef = Camera.main;
        scoreBeh = FindObjectOfType<ScoreBehaviour>();
    }
    public void ExitGame()
    {
        //Debug.Log("Trying exit game");
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
        if (loadOperation.isDone)
        {
            DoOnLoadingScene();
        }
    }
    void DoOnLoadingScene()
    {
        Time.timeScale = 1f;
        InputManager.ChangeControlsMappingToGameplay();
        ObjectPoolManager.instance.ClearPools();
    }
    public void PauseGame()
    {
        //pause game
        //Debug.Log("Pausinggame");
        gameIsPaused = true;
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        //resume game
        //Debug.Log("UnPausinggame");
        gameIsPaused = false;
        Time.timeScale = 1f;
    }
    public void ChangePlayer(GameObject _GO)
    {
        if (_GO == null)
        {
            //Debug.LogError("Player GO is null");
            return;
        }
        playerRef = _GO;
    }
    public static UnityEvent Alerting = new UnityEvent();
    public void AlertAll()
    {
        Alerting.Invoke();
        playerFound = true;
    }
    [SerializeField]
    List<GameObject> aliveEnemies = new List<GameObject>();
    public void AddEnemyToList(GameObject _enemy)
    {
        if (aliveEnemies.Contains(_enemy))
            return;
        aliveEnemies.Add( _enemy );
    }
    public static UnityEvent OpenExit = new UnityEvent();
    public void RemoveEnemyFromList(GameObject _enemy)
    {
        if (!aliveEnemies.Contains(_enemy))
            return;
        aliveEnemies.Remove(_enemy);
        if (aliveEnemies.Count == 0)
        {
           Invoke("OnAllEnemiesDead", 1f);
        }
    }
    void OnAllEnemiesDead()
    {
        Debug.Log("All enemies dead");
        OpenExit.Invoke();
        FinalizeScore();
    }
    public void AddScore(int score)
    {
        scoreBeh.AddScore(score);
        finalScore += score;
    }
    void FinalizeScore()
    {
        if (!playerFound)
        {
            scoreBeh.ChangeScore((int)Mathf.Round( finalScore * 1.5f));
        }
    }
}
