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
    SceneField menuLevelScene;
    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    AudioClip menuMusic;
    [SerializeField]
    AudioClip battleMusic;
    private void Start()
    {
        gameManager = GameManager.instance;
        AudioManager.instance.PlayMusicForced(menuMusic, true);
        CheckAudio();
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
    public void ExitToMainMenuGame()
    {
        gameManager.LoadLevel(menuLevelScene);
    }
    public void LoadLevel()
    {
        gameManager.LoadLevel(levelScene);
    }
    public void PlayMenuMusic()
    {
        AudioManager.instance.PlayMusicForced(menuMusic, true);
    }
    public void PlayBattleMusic()
    {
        AudioManager.instance.PlayMusicForced(battleMusic, true);
    }
    public void ChangeMasterVolume(float _volume)
    {
        AudioManager.instance.SetMasterVolume(_volume);
    }
    public void ChangeMusicVolume(float _volume)
    {
        AudioManager.instance.SetMusicVolume(_volume);
    }
    public void ChangeSFXVolume(float _volume)
    {
        AudioManager.instance.SetSFXVolume(_volume);
    }
    [SerializeField]
    GameObject settingsWindow;
    public void OpenSettings()
    {
        settingsWindow.SetActive(!settingsWindow.activeSelf);
    }
    [SerializeField]
    AudioSettingsSO audioSettingsSO;
    [SerializeField]
    Slider masterSlider;
    [SerializeField]
    Slider musicSlider;
    [SerializeField]
    Slider sfxSlider;
    void CheckAudio()
    {
        masterSlider.value = audioSettingsSO.masterVolume;
        musicSlider.value = audioSettingsSO.musicVolume;
        sfxSlider.value = audioSettingsSO.sfxVolume;
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
