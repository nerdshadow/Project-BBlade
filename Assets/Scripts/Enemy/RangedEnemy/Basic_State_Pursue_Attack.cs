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
        ChangeMovementMultiplier();
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
        TryRotate();
        TryAttack();

        if (agent.enabled == true)
        {
            agent.destination = playerGO.transform.position;
        }
        ChangeMovementMultiplier();
    }
    void TryRotate()
    {
        float distanceToPlayer = DistanceTo(playerGO);
        if (distanceToPlayer > npcStats.currentRangeAtkRange * 1.1f)
        {
            npcMovement.rotateTarget = null;
            npcStateBeh.Change_Anim_MoveX_Weight(1f, 0.5f);
            //Debug.Log("Changing move to 1");
            npcMovement.canMove = true;
            return;
        }
        else if (distanceToPlayer <= npcStats.currentMeleeAtkRange * 1.2f)
        {
            npcMovement.rotateTarget = playerGO;
            return;
        }
        else if (distanceToPlayer <= npcStats.currentRangeAtkRange * 1.1f)
        {
            Vector3 hitDir = playerGO.GetComponent<Collider>().bounds.center - npcStateBeh.characterColl.bounds.center;
            RaycastHit hit;
            if (Physics.Raycast(npcStateBeh.characterColl.bounds.center, hitDir, out hit, npcStats.currentRangeAtkRange * 1.1f))
            {
                //Debug.Log("Hit = " + (hit.collider.tag == "Player"));
                if (hit.collider.tag == "Player")
                {
                    npcMovement.rotateTarget = playerGO;
                    return;
                }
            }
        }
        npcMovement.rotateTarget = null;
        npcMovement.canMove = true;
        npcStateBeh.Change_Anim_MoveX_Weight(1f, 0.5f);
        //Debug.Log("Changing move to 1");
    }
    [SerializeField]
    static float buffAtk = 1f;
    float time = buffAtk;
    void TryAttack()
    {
        float distanceToPlayer = DistanceTo(playerGO);
        if (distanceToPlayer <= npcStats.currentMeleeAtkRange)
        {
            if (AngleTo(playerGO) <= 20)
            {
                npcMovement.canMove = false;
                npcStateBeh.Change_Anim_MoveX_Weight(0f, 0.5f);
                TryMeleeAttack();
                return;
            }
        }
        else if (distanceToPlayer <= npcStats.currentRangeAtkRange)
        {
            Vector3 hitDir = playerGO.GetComponent<Collider>().bounds.center - npcStateBeh.characterColl.bounds.center;
            RaycastHit hit;
            if (Physics.Raycast(npcStateBeh.characterColl.bounds.center, hitDir, out hit, npcStats.currentRangeAtkRange))
            {
                //Debug.Log("Hit = " + (hit.collider.tag == "Player"));
                if (hit.collider.tag == "Player")
                {
                    npcStateBeh.Change_Anim_MoveX_Weight(0f, 0.5f);
                    npcMovement.canMove = false;
                    if (AngleTo(playerGO) <= 5f)
                    {
                        if (time <= 0)
                        {
                            TryRangeAttack();
                            time = buffAtk;
                            return;
                        }
                        else
                        {
                            time -= Time.deltaTime;
                            return;
                        }
                    }
                }
            }
            time = buffAtk;
        }
        npcStateBeh.Change_Anim_MoveX_Weight(1f, 0.5f);
        //Debug.Log("Changing move to 1");
    }
    public override void Exit()
    {
        base.Exit();
    }
}
