using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class AI_Base_State
{
    public enum STATE
    {
        IDLE,
        PURSUE,
        BATTLE,
        DEAD
    }
    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    }

    public STATE stateName;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected GameObject target;
    protected NavMeshAgent agent;
    protected CharacterStats npcStats = null;
    protected AI_StateBehaviour npcStateBeh = null;
    protected AI_Movement npcMovement = null;
    protected AI_Base_State nextState;

    public AI_Base_State(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement)
    {
        npc = _npc;
        agent = _agent;
        anim = _animator;
        this.target = _target;
        npcStateBeh = _npcStateBeh;
        npcStats = _npcStats;
        npcMovement = _npcMovement;
        stage = EVENT.ENTER;
    }
    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }
    public AI_Base_State Process()
    {
        if (stage == EVENT.ENTER) { Enter(); }
        if (stage == EVENT.UPDATE) { Update(); }
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;

    }
    public virtual bool TargetExist()
    {
        if (target == null)
            return false;
        if (target.GetComponent<PlayerStats>().isDead == true)
            return false;

        return true;
    }
    public virtual float DistanceToTarget()
    {
        if (!TargetExist())
        {
            return 0;
        }
        if (target.GetComponent<Collider>())
        {
            UnityEngine.Debug.DrawLine(npc.GetComponent<Collider>().ClosestPointOnBounds(target.GetComponent<Collider>().bounds.center),
                target.GetComponent<Collider>().ClosestPointOnBounds(npc.GetComponent<Collider>().bounds.center), Color.cyan, 0.01f);
            return Vector3.Distance(npc.GetComponent<Collider>().ClosestPointOnBounds(target.GetComponent<Collider>().bounds.center),
                target.GetComponent<Collider>().ClosestPointOnBounds(npc.GetComponent<Collider>().bounds.center));
        }
        else
        {
            UnityEngine.Debug.DrawLine(npc.GetComponent<Collider>().ClosestPointOnBounds(target.transform.position),
               target.transform.position, Color.cyan, 0.01f);
            return Vector3.Distance(npc.GetComponent<Collider>().ClosestPointOnBounds(target.transform.position),
                target.transform.position);
        }
    }
    public virtual float AngleToTarget()
    {
        if (!TargetExist())
        {
            return 0;
        }
        Vector3 targetDir = target.transform.position - npc.transform.position;
        return Vector3.Angle(targetDir, npc.transform.forward);
    }

    public virtual void RotateTowardsTarget()
    {
        Vector3 lookPos = target.transform.position - npc.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, rotation, 1f * Time.deltaTime);
    }

    public virtual bool FindPlayer()
    {
        if (TargetExist() == false)
        {
            Collider[] potTargets =
            Physics.OverlapSphere(npc.transform.position, 100f, LayerMask.GetMask("CharacterLayer"));
            for (int i = 0; i < potTargets.Length; i++)
            {
                PlayerStats potStats = potTargets[i].GetComponent<PlayerStats>();
                if (potStats != null && potStats.isDead == false)
                {
                    target = potTargets[i].gameObject;
                    return true;
                }
            }
        }
        else
        {
            return true;
        }
        return false;
    }

    public bool CheckPath()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(agent.transform.position, target.transform.position, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
            return true;        
        }
        else 
        { 
            return false; 
        }
    }

    public virtual void TryMeleeAttack()
    {
        if (npcStateBeh.canMeleeAttack == true)
        {
            npcStateBeh.ReloadAtk();
            anim.CrossFade("Attack", 0.1f);
        }
    }
}
