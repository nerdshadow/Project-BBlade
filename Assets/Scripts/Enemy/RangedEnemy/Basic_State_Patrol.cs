using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Basic_State_Patrol : AI_Base_State
{
    public Basic_State_Patrol(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement, AI_Canvas _npcCanvas) 
        : base(_npc, _agent, _animator, _target, _npcStats, _npcStateBeh, _npcMovement, _npcCanvas)
    {
        stateName = STATE.PATROL;
    }
    int currentIndex = -1;
    float lastDist = Mathf.Infinity;
    List<GameObject> wayPoints;
    GameObject wayPointTarget = null;
    public override void Enter()
    {
        //Debug.Log("EnterPatrol");
        //agent.velocity = Vector3.zero;
        GameManager.Alerting.AddListener(PlayerFound);
        int currentIndex = -1;
        lastDist = Mathf.Infinity;
        if (npcStateBeh.wayPoints.Count < 1 
            || npcStateBeh.wayPoints[0] == null
            || npcStateBeh.canPatrol == false)
        {
            Debug.Log("No waypoints for " + npc.name + ", or cannot patrol. Returning to Idle");
            nextState = new Basic_State_Idle(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
            stage = EVENT.EXIT;
            return;
        }
        else
        {
            wayPoints = npcStateBeh.wayPoints;
        }
        for (int i = 0; i < wayPoints.Count; i++)
        {
            GameObject thisWP = wayPoints[i];
            float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
            if (distance < lastDist)
            {
                currentIndex = i;
                lastDist = distance;
            }
        }
        wayPointTarget = wayPoints[currentIndex];
        if (CheckPathTo(wayPointTarget) == true)
        {
            npcMovement.canMove = true;
            agent.destination = wayPointTarget.transform.position;
        }
        agent.isStopped = false;
        npcMovement.canRotate = true;
        npcMovement.canMove = true;
        npcStateBeh.Change_Anim_MoveX_Weight(1f, 0.5f);
        npcStateBeh.Change_Anim_CombatValue(0f, 1f);
        npcStats.currentSpeed = npcStats.currentCalmSpeed;
        ChangeMovementMultiplier();
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
        CheckDeadBody();
        DetectingPlayer();
        if (playerDetected == true)
        {
            nextState = new Basic_State_Pursue_Attack(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
            stage = EVENT.EXIT;
            return;
        }
        if (npcStateBeh.canPatrol == false)
        {
            nextState = new Basic_State_Idle(npc, agent, anim, playerGO, npcStats, npcStateBeh, npcMovement, npcCanvas);
            stage = EVENT.EXIT;
            return;
        }
        if (DistanceTo(wayPointTarget) <= 1)
        {
            if (currentIndex >= wayPoints.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            wayPointTarget = wayPoints[currentIndex];
            if (CheckPathTo(wayPointTarget) == true && agent.enabled)
            {
                //Debug.Log("Path is good");
                //agent.destination = wayPointTarget.transform.position;
            }
        }
        ChangeMovementMultiplier();
        //base.Update();
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
        GameManager.Alerting.RemoveListener(PlayerFound);
        base.Exit();
    }
}
