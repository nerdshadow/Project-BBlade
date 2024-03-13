using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    GameObject loadingScreen;
    public bool gameIsPaused = false;
    public GameObject playerRef = null;
    public Camera playerCameraRef = null;
    public bool playerFound = false;
    #region Score
    [SerializeField]
    int finalScore = 0;
    [SerializeField]
    ScoreBehaviour scoreBeh;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += InitManager;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= InitManager;
    }
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
        Alerting.RemoveAllListeners();
    }
    void InitManager(Scene _scene, LoadSceneMode _mode)
    {
        //Debug test
        Application.targetFrameRate = 61;
        playerCameraRef = Camera.main;
        scoreBeh = FindObjectOfType<ScoreBehaviour>();
        Alerting.RemoveAllListeners();
    }
    public void ExitGame()
    {
        //Debug.Log("Trying exit game");
        Application.Quit();
    }
    public void LoadLevel(SceneField scene)
    {

        StopAllCoroutines();
        if(AudioManager.instance != null)
            AudioManager.instance.StopAllCoroutines();
        if(InputManager.instance != null)
            InputManager.instance.StopAllCoroutines();
        if(ObjectPoolManager.instance != null)
            ObjectPoolManager.instance.StopAllCoroutines();
        aliveEnemies.Clear();
        Alerting.RemoveAllListeners();
        GameObject currentLoadingScreen;
        if(playerRef != null)
            currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<GameplayMenuBehavior>().transform);
        else
            currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<Canvas>().transform);
        Slider currentSlider = currentLoadingScreen.GetComponentInChildren<Slider>();
        StartCoroutine(LoadLevelAsync(scene, currentSlider));
    }

    IEnumerator LoadLevelAsync(SceneField _scene, Slider _loadingSlider)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_scene);
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
    public void LoadLevel(string sceneName)
    {

        StopAllCoroutines();
        if (AudioManager.instance != null)
            AudioManager.instance.StopAllCoroutines();
        if (InputManager.instance != null)
            InputManager.instance.StopAllCoroutines();
        if (ObjectPoolManager.instance != null)
            ObjectPoolManager.instance.StopAllCoroutines();
        aliveEnemies.Clear();
        Alerting.RemoveAllListeners();
        GameObject currentLoadingScreen;
        if (playerRef != null)
            currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<GameplayMenuBehavior>().transform);
        else
            currentLoadingScreen = Instantiate(loadingScreen, FindObjectOfType<Canvas>().transform);
        Slider currentSlider = currentLoadingScreen.GetComponentInChildren<Slider>();
        StartCoroutine(LoadLevelAsync(sceneName, currentSlider));
    }
    IEnumerator LoadLevelAsync(string _sceneName, Slider _loadingSlider)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_sceneName);
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
    [SerializeField]
    AudioClip menuMusic;
    void DoOnLoadingScene()
    {
        Time.timeScale = 1f;
        InputManager.ChangeControlsMappingToGameplay();
        if(ObjectPoolManager.instance != null)
            ObjectPoolManager.instance.ClearPools();
        AudioManager.instance.PlayMusicForced(menuMusic, true);
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
    public void ChangeGameSpeed(float _gameSpeed)
    {
        Time.timeScale = _gameSpeed;
    }
    public void ChangeGameSpeed(float _endGameSpeed, float _timeToChange)
    {
        StartCoroutine(TimeChanging(_endGameSpeed, _timeToChange));
    }
    IEnumerator TimeChanging(float _endGameSpeed, float _timeToChange)
    {
        float timeElapsed = 0f;
        while (timeElapsed < _timeToChange)
        {
            Time.timeScale = Mathf.Lerp(1, _endGameSpeed, timeElapsed / _timeToChange);
            //Debug.Log(Time.timeScale);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null; 
        }
        Time.timeScale = _endGameSpeed;
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
    [SerializeField]
    AudioClip battleMusicClip;
    [SerializeField]
    AudioClip endBattleMusicClip;
    public void AlertAll()
    {
        Alerting.Invoke();
        AudioManager.instance.PlayMusicForced(battleMusicClip, true);
        playerFound = true;
        Alerting.RemoveAllListeners();
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
        OpenExit.RemoveAllListeners();
        aliveEnemies.Clear();
        AudioManager.instance.PlayMusicForced(endBattleMusicClip, false);
        FinalizeScore();
    }
    public void AddScore(int score)
    {
        if(scoreBeh != null)
            scoreBeh.AddScore(score);
        finalScore += score;
    }
    void FinalizeScore()
    {
        if (!playerFound)
        {
            if(scoreBeh != null)
                scoreBeh.ChangeScore((int)Mathf.Round( finalScore * 1.5f));
        }
    }
}
