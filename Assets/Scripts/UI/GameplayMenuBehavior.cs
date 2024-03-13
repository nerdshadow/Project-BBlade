using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayMenuBehavior : MonoBehaviour
{
    [SerializeField]
    MainControls playerControls;
    [SerializeField]
    MainControls.GameplayActions mainControlsMap;
    [SerializeField]
    MainControls.UIActions menuControlsMap;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject gameplayMenu;
    [SerializeField]
    GameObject gameplayUI;
    [SerializeField]
    GameObject deathMenu;
    [SerializeField]
    SceneField mainMenuScene;
    private void Start()
    {
        if (playerControls == null)
            playerControls = InputManager.inputActions;
        mainControlsMap = playerControls.Gameplay;
        menuControlsMap = playerControls.UI;
        if (playerControls != null)
            EnableControls();
        InputManager.ChangeControlsMappingToGameplay();
        gameManager = GameManager.instance;
    }
    private void OnEnable()
    {
        if (playerControls != null)
            EnableControls();
        PlayerStats.playerDied.AddListener(PlayerDeath);
    }
    private void OnDisable()
    {
        if (playerControls != null)
            DisableControls();
    }
    void EnableControls()
    {
        mainControlsMap.OpenMenu.performed += OpenMenu;
        mainControlsMap.OpenMenu.performed += ChangeControlsMapping;
        mainControlsMap.OpenMenu.Enable();

        menuControlsMap.CloseMenu.performed += CloseMenu;
        menuControlsMap.CloseMenu.performed += ChangeControlsMapping;
        menuControlsMap.CloseMenu.Enable();
    }
    void DisableControls()
    {
        mainControlsMap.OpenMenu.performed -= OpenMenu;
        mainControlsMap.OpenMenu.performed -= ChangeControlsMapping;
        mainControlsMap.OpenMenu.Disable();

        menuControlsMap.CloseMenu.performed -= CloseMenu;
        menuControlsMap.CloseMenu.performed -= ChangeControlsMapping;
        menuControlsMap.CloseMenu.Disable();
    }
    public void ChangeControlsMapping(InputAction.CallbackContext context)
    {
        //Debug.Log("Changing controls map");
        if (context.action == mainControlsMap.OpenMenu)
        {
            InputManager.ChangeControlsMappingToMenu();
        }
        else if (context.action == menuControlsMap.CloseMenu)
        {
            InputManager.ChangeControlsMappingToGameplay();
        }
    }
    public void OpenMenu(InputAction.CallbackContext context)
    {
        //Debug.Log("opening menu");
        gameManager.PauseGame();
        gameplayMenu.SetActive(true);
        CheckAudio();
    }
    public void CloseMenu(InputAction.CallbackContext context)
    {
        //Debug.Log("closing menu");
        gameManager.ResumeGame();
        gameplayMenu.SetActive(false);
    }
    public void CloseMenu()
    {
        //Debug.Log("closing menu");
        gameManager.ResumeGame();
        gameplayMenu.SetActive(false);
        InputManager.ChangeControlsMappingToGameplay();
    }
    public void ReturnToMainMenu()
    {
        gameplayMenu.SetActive(false);
        gameManager.LoadLevel(mainMenuScene);
    }
    public void ExitGame()
    {
        gameManager.ExitGame();
    }
    [SerializeField]
    AudioClip deathAudioClip;
    public void PlayerDeath()
    {
        gameplayUI.SetActive(false);
        AudioManager.instance.PlayMusicForced(deathAudioClip, true);
        InputManager.ChangeControlsMappingToMenu();
        DisableControls();
        gameManager.playerCameraRef.GetComponent<PlayerCamera>().ChangeAberration();
        gameManager.playerCameraRef.GetComponent<PlayerCamera>().ChangeVignette();
        gameManager.ChangeGameSpeed(0.1f, 2f);
        CanvasGroup deathGroup = deathMenu.GetComponent<CanvasGroup>();
        deathGroup.alpha = 0f;    
        deathMenu.SetActive(true);
        StartCoroutine(ShowCanvasGroup(deathGroup, 2f));
    }
    IEnumerator ShowCanvasGroup(CanvasGroup canvasgroup, float timeToShow)
    {
        float timePassed = 0f;
        while (timePassed < timeToShow)
        {
            canvasgroup.alpha = Mathf.Lerp(0, 1, timePassed / timeToShow);
            timePassed += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasgroup.alpha = 1f;
    }
    public void RestartCurrentLevel()
    {
        gameManager.LoadLevel(SceneManager.GetActiveScene().name);
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
}
