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
    string mainMenuSceneName = "Scene_MainMenu";
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
        gameManager.LoadLevel(mainMenuSceneName);
    }
    public void ExitGame()
    {
        gameManager.ExitGame();
    }
    public void PlayerDeath()
    {
        gameplayUI.SetActive(false);
        InputManager.ChangeControlsMappingToMenu();
        deathMenu.SetActive(true);
    }
    public void RestartCurrentLevel()
    {
        gameManager.LoadLevel(SceneManager.GetActiveScene().name);
    }
}
