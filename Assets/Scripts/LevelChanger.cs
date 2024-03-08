using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    GameManager gameManager;
    public bool canLoadLevel = false;
    public string levelName = "level_0";
    private void Start()
    {
        if (gameManager == null)
            gameManager = GameManager.instance;
        GameManager.OpenExit.AddListener(OpenChanger);
    }
    void OpenChanger()
    {
        canLoadLevel = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats != null && stats.isDead == false && canLoadLevel == true)
        {
            gameManager.LoadLevel(levelName);
        }
    }

}
