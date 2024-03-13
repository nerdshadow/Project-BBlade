using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    public bool IsDead { get; set; }
    public void Die();
}
