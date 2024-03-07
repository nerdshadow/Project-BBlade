using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_State_Handler : AI_StateBehaviour
{
    protected override void Init()
    {
        base.Init();
        currentState = new Basic_State_Idle(this.gameObject, agent, anim, playerRef, characterStats, this, npcMovement, npcCanvas);
    }
}
