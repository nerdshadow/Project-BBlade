using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    //int sceneNumber = 1;
    [SerializeField]
    string firstLevelName = "level_0";
    [SerializeField]
    string levelName = "level_0";
    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public void StartNewGame()
    {
        gameManager.LoadLevel(firstLevelName);
    }
    public void ExitGame()
    {
        gameManager.ExitGame();
    }
    public void LoadLevel()
    {
        gameManager.LoadLevel(levelName);
    }
}
