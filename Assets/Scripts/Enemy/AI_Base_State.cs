using UnityEngine;
using UnityEngine.AI;

public class AI_Base_State
{
    public enum STATE
    {
        IDLE,
        PATROL,
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
    protected GameObject killTarget;
    protected NavMeshAgent agent;
    protected CharacterStats npcStats = null;
    protected AI_StateBehaviour npcStateBeh = null;
    protected AI_Movement npcMovement = null;
    protected AI_Base_State nextState;
    public float detectRadius = 10f;

    public AI_Base_State(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement)
    {
        npc = _npc;
        agent = _agent;
        anim = _animator;
        this.killTarget = _target;
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
    public virtual bool PlayerTargetExist()
    {
        if (killTarget == null)
            return false;
        if (killTarget.GetComponent<PlayerStats>().isDead == true)
            return false;

        return true;
    }
    public virtual float DistanceToKillTarget()
    {
        if (!PlayerTargetExist())
        {
            return 0;
        }
        if (killTarget.GetComponent<Collider>())
        {
            Debug.DrawLine(npc.GetComponent<Collider>().ClosestPointOnBounds(killTarget.GetComponent<Collider>().bounds.center),
                killTarget.GetComponent<Collider>().ClosestPointOnBounds(npc.GetComponent<Collider>().bounds.center), Color.cyan, 0.01f);
            return Vector3.Distance(npc.GetComponent<Collider>().ClosestPointOnBounds(killTarget.GetComponent<Collider>().bounds.center),
                killTarget.GetComponent<Collider>().ClosestPointOnBounds(npc.GetComponent<Collider>().bounds.center));
        }
        else
        {
            Debug.DrawLine(npc.GetComponent<Collider>().ClosestPointOnBounds(killTarget.transform.position),
               killTarget.transform.position, Color.cyan, 0.01f);
            return Vector3.Distance(npc.GetComponent<Collider>().ClosestPointOnBounds(killTarget.transform.position),
                killTarget.transform.position);
        }
    }
    public virtual float DistanceTo(GameObject _target)
    {
        if (_target == null)
        {
            return 0;
        }
        Debug.DrawLine(npc.GetComponent<Collider>().ClosestPointOnBounds(_target.transform.position),
            _target.transform.position, Color.cyan, 0.01f);
        return Vector3.Distance(npc.GetComponent<Collider>().ClosestPointOnBounds(_target.transform.position),
            _target.transform.position);
    }
    public virtual float AngleToTarget()
    {
        if (!PlayerTargetExist())
        {
            return 0;
        }
        Vector3 targetDir = killTarget.transform.position - npc.transform.position;
        return Vector3.Angle(targetDir, npc.transform.forward);
    }

    public virtual void RotateTowardsTarget()
    {
        Vector3 lookPos = killTarget.transform.position - npc.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, rotation, 1f * Time.deltaTime);
    }

    public virtual bool FindPlayer()
    {
        if (PlayerTargetExist() == false)
        {
            Collider[] potTargets =
            Physics.OverlapSphere(npc.transform.position, detectRadius, LayerMask.GetMask("CharacterLayer"));
            for (int i = 0; i < potTargets.Length; i++)
            {
                PlayerStats potStats = potTargets[i].GetComponent<PlayerStats>();
                if (potStats != null && potStats.isDead == false)
                {
                    killTarget = potTargets[i].gameObject;
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

    public bool CheckPathToKillTarget()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(agent.transform.position, killTarget.transform.position, NavMesh.AllAreas, path);
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
    public bool CheckPathTo(GameObject _target)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(agent.transform.position, _target.transform.position, NavMesh.AllAreas, path);
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
