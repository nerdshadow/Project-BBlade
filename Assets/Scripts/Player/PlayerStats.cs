using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour, IKillable
{
    public float currentMovSpeed = 2f;
    public bool isDead = false;
    public bool isMortal = true;
    public static UnityEvent playerDied = new UnityEvent();
    public static UnityEvent<Transform> turnPlayerToKiller = new UnityEvent<Transform>();
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
        if (isDead == true || isMortal == false)
            return;
        Debug.Log("Player Died");
        currentMovSpeed = 0f;
        playerDied.Invoke();
        isDead = true;
    }
    public void TurnPlayerModelTo(Transform _killerTransform)
    {
        turnPlayerToKiller.Invoke(_killerTransform);
    }
}
