using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Basic_State_Pursue_Attack : AI_Base_State
{
    public Basic_State_Pursue_Attack(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement, AI_Canvas _npcCanvas)
        : base(_npc, _agent, _animator, _target, _npcStats, _npcStateBeh, _npcMovement, _npcCanvas)
    {
        stateName = STATE.PURSUE;
    }
    public override void Enter()
    {
        //Debug.Log("EnterPursue");
        npcStateBeh.Change_Anim_MoveX_Weight(1f, 0.5f);
        npcStateBeh.Change_Anim_CombatValue(1f, 0.5f);
        npcStats.currentSpeed = npcStats.currentCombatSpeed;
        agent.isStopped = false;
        npcMovement.canRotate = true;
        CheckPathTo(playerGO);
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
        if (PlayerExistAndAlive() == false)
        {
            nextState = new Basic_State_Idle(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
            stage = EVENT.EXIT;
            return;
        }
        CheckPathTo(playerGO);
        if (DistanceTo(playerGO) <= npcStats.currentMeleeAtkRange * 2)
        {
            npcMovement.rotateTarget = playerGO;
        }
        else
        {
            npcMovement.rotateTarget = null;
        }

        TryAttack();
        if (agent.enabled == true)
        {
            agent.destination = playerGO.transform.position;
        }
    }
    void TryAttack()
    {
        float distanceToPlayer = DistanceTo(playerGO);
        if (distanceToPlayer <= npcStats.currentMeleeAtkRange)
        {
            npcMovement.canMove = false;
            if (AngleTo(playerGO) <= 30)
            {
                TryMeleeAttack();
                return;
            }
        }
        else if (distanceToPlayer <= npcStats.currentRangeAtkRange)
        {
            Vector3 hitDir = playerGO.GetComponent<Collider>().bounds.center - npcStats.eyeLocation.position;
            RaycastHit hit;
            if (Physics.Raycast(npcStats.eyeLocation.position, hitDir, out hit, npcStats.currentRangeAtkRange))
            {
                //Debug.Log("Hit = " + (hit.collider.tag == "Player"));
                if (hit.collider.tag == "Player")
                {
                    npcMovement.canMove = false;
                    TryRangeAttack();
                    return;
                }
            }
        }
        else
        {
            npcMovement.canMove = true;
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
