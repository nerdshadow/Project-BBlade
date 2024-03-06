using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour, IKillable
{
    public float currentMovSpeed = 2f;
    public bool isDead = false;
    public static UnityEvent playerDied = new UnityEvent();
    [SerializeField]
    GameManager gameManager;
    private void Start()
    {
        if (gameManager == null)
            gameManager = GameManager.instance;
        gameManager.ChangePlayer(gameObject);
    }
    [ContextMenu("Die")]
    public void Die()
    {
        if(isDead == true)
            return;
        Debug.Log("Player Died");
        playerDied.Invoke();
        isDead = true;
    }
}
