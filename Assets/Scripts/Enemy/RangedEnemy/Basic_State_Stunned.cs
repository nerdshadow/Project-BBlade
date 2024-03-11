using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Basic_State_Stunned : AI_Base_State
{
    STATE lastStateName;
    public Basic_State_Stunned(STATE lastState, GameObject _npc, NavMeshAgent _agent, Animator _animator, 
        GameObject _playerGO, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, 
        AI_Movement _npcMovement, AI_Canvas _npcCanvas) 
        : base(_npc, _agent, _animator, _playerGO, _npcStats, _npcStateBeh, _npcMovement, _npcCanvas)
    {
        stateName = STATE.STUN;
        lastStateName = lastState;
    }
    public override void Enter()
    {
        anim.speed = 0.1f;
        agent.velocity = Vector3.zero;
        if (agent.isActiveAndEnabled)
            agent.isStopped = true;
        npcMovement.canRotate = false;
        npcMovement.canMove = false;
        BeStunned(0.5f);
        base.Enter();
    }
    public override void Update()
    {
        if (npcStats.isDead == true)
        {
            nextState = new Basic_State_Death(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
            stage = EVENT.EXIT;
            return;
        }
        base.Update();
    }
    public override void Exit()
    {
        npcStateBeh.StopCoroutine(waitingstun);
        base.Exit();
    }
    Coroutine waitingstun;
    void BeStunned(float stunTime)
    {
        if (waitingstun != null)
            npcStateBeh.StopCoroutine(waitingstun);
        waitingstun = npcStateBeh.StartCoroutine(WaitStun(stunTime));
    }
    IEnumerator WaitStun(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        anim.speed = 1f;
        LeaveStun();
    }
    void LeaveStun()
    {
        switch (lastStateName)
        {
            case STATE.IDLE:
                nextState = new Basic_State_Idle(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
                break;
            case STATE.PATROL:
                nextState = new Basic_State_Patrol(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
                break;
            case STATE.PURSUE:
                nextState = new Basic_State_Pursue_Attack(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
                break;
            default:
                nextState = new Basic_State_Idle(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
                break;
        }
        stage = EVENT.EXIT;
        return;
    }
}
