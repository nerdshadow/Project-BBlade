using UnityEngine;
using UnityEngine.AI;

public class Basic_State_PursueAndAttack : AI_Base_State
{
    public Basic_State_PursueAndAttack(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement)
        : base(_npc, _agent, _animator, _target, _npcStats, _npcStateBeh, _npcMovement)
    {
        stateName = STATE.PURSUE;
    }
    public override void Enter()
    {
        //Debug.Log("EnterPursue");
        npcStateBeh.Change_Anim_MoveX_Weight(1f, 0.5f);
        agent.isStopped = false;
        npcMovement.canRotate = true;
        base.Enter();
    }

    public override void Update()
    {
        if (npcStats.isDead == true)
        {
            nextState = new Basic_State_Death(npc, agent, anim, killTarget, npcStats, npcStateBeh, npcMovement);
            stage = EVENT.EXIT;
            return;
        }
        if (PlayerTargetExist() == false
                || CheckPathToKillTarget() == false)
        {
            nextState = new Basic_State_Idle(npc, agent, anim, killTarget, npcStats, npcStateBeh, npcMovement);
            stage = EVENT.EXIT;
            return;
        }

        if (DistanceToKillTarget() <= npcStats.currentAtkRange * 2)
        {
            npcMovement.target = killTarget;
        }
        else
        {
            npcMovement.target = null;
        }

        if (DistanceToKillTarget() <= npcStats.currentAtkRange)
        {
            npcMovement.canMove = false;
            if (AngleToTarget() <= 30)
            {
                TryMeleeAttack();
                return;
            }
            
        }
        else
        {
            npcMovement.canMove = true;
        }
        if (agent.enabled == true)
        {
            agent.destination = killTarget.transform.position;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}