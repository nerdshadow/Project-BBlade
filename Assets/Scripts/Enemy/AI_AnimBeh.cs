using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_AnimBeh : MonoBehaviour
{
    [SerializeField]
    AI_StateBehaviour stateBeh;
    private void Start()
    {
        if(stateBeh == null)
            stateBeh = GetComponentInParent<AI_StateBehaviour>();
    }
    public void DoMeleeAttack()
    {
        stateBeh.DoMeleeAttack();
    }
    public void DoRangedAttack()
    {
        stateBeh.DoRangedAttack();
    }
}
