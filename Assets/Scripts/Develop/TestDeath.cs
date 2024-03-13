using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDeath : MonoBehaviour, IKillable
{
    bool isDead = false;
    [SerializeField]
    Collider testCollider;

    public bool IsDead { get => isDead; set => isDead = value; }

    [ContextMenu("TestKill")]
    void Kill()
    {
        testCollider.GetComponent<IKillable>().Die();
    }
    public void Die()
    {
        if (isDead == true)
        {
            return;
        }
        isDead = true;
        Debug.Log(this.name + " died"); 
    }
}
