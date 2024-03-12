using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    //int sceneNumber = 1;
    [SerializeField]
    SceneField firstLevelScene;
    [SerializeField]
    SceneField levelScene;
    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    AudioClip menuMusic;
    private void Start()
    {
        gameManager = GameManager.instance;
        AudioManager.instance.PlayMusicForced(menuMusic, true);
        CheckGraphics();
    }
    public void StartNewGame()
    {
        gameManager.LoadLevel(firstLevelScene);
    }
    public void ExitGame()
    {
        gameManager.ExitGame();
    }
    public void LoadLevel()
    {
        gameManager.LoadLevel(levelScene);
    }
    public void ChangeMasterVolume(float _volume)
    {
        AudioManager.instance.SetMasterVolume(_volume);
    }
    public void ChangeSFXVolume(float _volume)
    {
        AudioManager.instance.SetSFXVolume(_volume);
    }
    public void ChangeMusicVolume(float _volume)
    {
        AudioManager.instance.SetMusicVolume(_volume);
    }
    [SerializeField]
    GameObject settingsWindow;
    public void OpenSettings()
    {
        settingsWindow.SetActive(!settingsWindow.activeSelf);
    }
    #region "Graphics"
    [Header("Graphics")]
    Resolution prevRes;
    bool prevScreen;
    void CheckGraphics()
    {
        Debug.Log(Screen.currentResolution);
        prevRes = Screen.currentResolution;
        prevScreen = Screen.fullScreen;
        Screen.SetResolution(prevRes.width, prevRes.height, prevScreen);
    }
    #endregion Graphics
}
