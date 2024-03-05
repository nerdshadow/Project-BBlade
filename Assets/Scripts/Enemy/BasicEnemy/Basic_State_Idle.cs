using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Basic_State_Idle : AI_Base_State
{
    public Basic_State_Idle(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement)
        : base(_npc, _agent, _animator, _target, _npcStats, _npcStateBeh, _npcMovement)
    {
        stateName = STATE.IDLE;
    }

    public override void Enter()
    {
        //Debug.Log("EnterIdle");
        npcStateBeh.Change_Anim_MoveX_Weight(0f, 0.5f);
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        base.Enter();
    }

    public override void Update()
    {
        if (npcStats.isDead == true)
        {
            nextState = new Basic_State_Death(npc, agent, anim, target, npcStats, npcStateBeh, npcMovement);
            stage = EVENT.EXIT;
            return;
        }
        if (FindPlayer() == true
                && CheckPath() == true)
        {
            nextState = new Basic_State_PursueAndAttack(npc, agent, anim, target, npcStats, npcStateBeh, npcMovement);
            stage = EVENT.EXIT;
            return;
        }
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
