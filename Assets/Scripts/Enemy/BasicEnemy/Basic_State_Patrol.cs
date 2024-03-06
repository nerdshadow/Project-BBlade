using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Basic_State_Patrol : AI_Base_State
{
    public Basic_State_Patrol(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement) 
        : base(_npc, _agent, _animator, _target, _npcStats, _npcStateBeh, _npcMovement)
    {
        stateName = STATE.PATROL;
    }
    int currentIndex = -1;
    float lastDist = Mathf.Infinity;
    List<GameObject> wayPoints;
    GameObject wayPointTarget = null;
    public override void Enter()
    {
        Debug.Log("EnterPatrol");
        //agent.velocity = Vector3.zero;
        int currentIndex = -1;
        lastDist = Mathf.Infinity;
        if (npcStateBeh.wayPoints.Count < 1 || npcStateBeh.canPatrol == false)
        {
            Debug.Log("No waypoints for " + npc.name + ", or cannot patrol. Returning to Idle");
            nextState = new Basic_State_Idle(npc, agent, anim, killTarget, npcStats, npcStateBeh, npcMovement);
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
        if (FindPlayer() == true
                && CheckPathToKillTarget() == true)
        {
            nextState = new Basic_State_PursueAndAttack(npc, agent, anim, killTarget, npcStats, npcStateBeh, npcMovement);
            stage = EVENT.EXIT;
            return;
        }
        if (npcStateBeh.canPatrol == false)
        {
            nextState = new Basic_State_Idle(npc, agent, anim, killTarget, npcStats, npcStateBeh, npcMovement);
            stage = EVENT.EXIT;
            return;
        }
        //Debug.Log("Distance " + DistanceTo(wayPointTarget));
        if (DistanceTo(wayPointTarget) <= 1)
        {
            //Debug.Log("In changing index");
            if (currentIndex >= wayPoints.Count - 1)
            {
                //Debug.Log("In changing index to 0");
                currentIndex = 0;
            }
            else
            {
                //Debug.Log("In changing index to ++");
                currentIndex++;
            }
            wayPointTarget = wayPoints[currentIndex];
            if (CheckPathTo(wayPointTarget) == true && agent.enabled)
            {
                //Debug.Log("Path is good");
                //agent.destination = wayPointTarget.transform.position;
            }
        }
        //base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
