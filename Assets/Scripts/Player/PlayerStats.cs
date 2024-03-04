using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour, IKillable
{
    public float currentMovSpeed = 2f;
    public bool isDead = false;
    public static UnityEvent playerDied = new UnityEvent();
    [ContextMenu("Die")]
    public void Die()
    {
        Debug.Log("Player Died");
        playerDied.Invoke();
        isDead = true;
    }
}
