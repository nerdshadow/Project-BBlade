using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Basic_State_Idle : AI_Base_State
{
    public Basic_State_Idle(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement, AI_Canvas _npcCanvas)
        : base(_npc, _agent, _animator, _target, _npcStats, _npcStateBeh, _npcMovement, _npcCanvas)
    {
        stateName = STATE.IDLE;
    }

    public override void Enter()
    {
        //Debug.Log("EnterIdle");
        GameManager.Alerting.AddListener(PlayerFound);
        npcStateBeh.Change_Anim_MoveX_Weight(0f, 0.5f);
        npcStateBeh.Change_Anim_CombatValue(0f, 1f);
        npcStats.currentSpeed = npcStats.currentCalmSpeed;
        ChangeMovementMultiplier();
        agent.velocity = Vector3.zero;
        if(agent.isActiveAndEnabled)
            agent.isStopped = true;
        npcMovement.canRotate = false;
        npcMovement.canMove = false;
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
        DetectingPlayer();
        if (playerDetected == true)
        {
            nextState = new Basic_State_Pursue_Attack(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
            stage = EVENT.EXIT;
            return;
        }
        if (npcStateBeh.canPatrol == true && npcStateBeh.wayPoints.Count > 0)
        {
            nextState = new Basic_State_Patrol(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
            stage = EVENT.EXIT;
            return;
        }
        base.Update();
    }
    protected override void PlayerFound()
    {
        playerDetected = true;
        npcCanvas.StartCoroutine(FireAnAlert());
        nextState = new Basic_State_Pursue_Attack(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
        stage = EVENT.EXIT;
        base.PlayerFound();
        return;
    }
    public override void Exit()
    {
        base.Exit();
    }
}
