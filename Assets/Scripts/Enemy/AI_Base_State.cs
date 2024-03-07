using System.Collections;
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
    protected GameObject playerGO;
    protected NavMeshAgent agent;
    protected CharacterStats npcStats = null;
    protected AI_StateBehaviour npcStateBeh = null;
    protected AI_Movement npcMovement = null;
    protected AI_Canvas npcCanvas = null;
    protected AI_Base_State nextState;
    public float detectRadius = 10f;
    public bool playerDetected = false;
    public AI_Base_State(GameObject _npc, NavMeshAgent _agent, Animator _animator, 
                        GameObject _playerGO, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, 
                        AI_Movement _npcMovement, AI_Canvas _npcCanvas)
    {
        npc = _npc;
        agent = _agent;
        anim = _animator;
        this.playerGO = _playerGO;
        npcStateBeh = _npcStateBeh;
        npcStats = _npcStats;
        npcMovement = _npcMovement;
        npcCanvas = _npcCanvas;
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
    public virtual bool PlayerExistAndAlive()
    {
        //Debug.Log("Checking player existance");
        if (playerGO == null)
        {
            playerGO = npcStateBeh.gameManager.playerRef;
            //Debug.Log("Checking player existance" + playerGO);
        }
        if (playerGO == null)
        {
            //Debug.Log("Checking player existance" + playerGO);
            return false;
        }
        if (playerGO.GetComponent<PlayerStats>().isDead == true)
        {
            //Debug.Log("Checking player existance" + playerGO);
            return false;
        }
        //Debug.Log("Checking player existance" + playerGO);
        return true;
    }
    public virtual float DistanceTo(GameObject _target)
    {
        if (_target == null)
        {
            Debug.LogError("Target is null");
            return 0;
        }
        if (_target.GetComponent<Collider>())
        {
            Debug.DrawLine(npc.GetComponent<Collider>().ClosestPointOnBounds(_target.GetComponent<Collider>().bounds.center),
                _target.GetComponent<Collider>().ClosestPointOnBounds(npc.GetComponent<Collider>().bounds.center), Color.cyan, 0.01f);
            return Vector3.Distance(npc.GetComponent<Collider>().ClosestPointOnBounds(_target.GetComponent<Collider>().bounds.center),
                _target.GetComponent<Collider>().ClosestPointOnBounds(npc.GetComponent<Collider>().bounds.center));
        }
        else
        {
            Debug.DrawLine(npc.GetComponent<Collider>().ClosestPointOnBounds(_target.transform.position),
               _target.transform.position, Color.cyan, 0.01f);
            return Vector3.Distance(npc.GetComponent<Collider>().ClosestPointOnBounds(_target.transform.position),
                _target.transform.position);
        }
    }
    public virtual float AngleTo(GameObject _target)
    {
        if (_target == null)
        {
            Debug.LogError("Target is null");
            return 0;
        }
        Vector3 targetDir = _target.transform.position - npc.transform.position;
        return Vector3.Angle(targetDir, npc.transform.forward);
    }
    public virtual void RotateTowards(GameObject _target)
    {
        if (_target == null)
        {
            Debug.LogError("Target is null");
            return;
        }
        Vector3 lookPos = _target.transform.position - npc.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, rotation, 1f * Time.deltaTime);
    }
    public virtual bool CheckPlayer()
    {
        if(PlayerExistAndAlive() == false)
            return false;

        Vector3 dirToPlayer = playerGO.transform.position - npc.transform.position;
        //Check distance, angles, time to detect
        //Debug.Log("Magni = " + (dirToPlayer.magnitude <= npcStats.currentDetectDistance));
        if (dirToPlayer.magnitude >= npcStats.currentDetectDistance)
            return false;

        //if (Vector3.Dot(dirToPlayer.normalized, npc.transform.forward)
        //    <= Mathf.Cos(npcStats.currentDetectAngle * Mathf.Deg2Rad))
        //    return false;
        //Debug.Log("Angle = " + (Vector3.Angle(dirToPlayer, npc.transform.forward)
            //<= npcStats.currentDetectAngle));
        if (Vector3.Angle(dirToPlayer, npc.transform.forward)
            >= npcStats.currentDetectAngle)
            return false;
        Vector3 hitDir = playerGO.GetComponent<Collider>().bounds.center - npcStats.eyeLocation.position;
        RaycastHit hit;
        if (Physics.Raycast(npcStats.eyeLocation.position, hitDir, out hit, npcStats.currentDetectDistance, npcStats.layersToDetect))
        {
            //Debug.Log("Hit = " + (hit.collider.tag == "Player"));
            if (hit.collider.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }
    protected float currentDetectTime = 0f;
    public virtual void DetectingPlayer()
    {
        //Debug.Log("Current detect " + currentDetectTime);
        if (CheckPlayer() != true)
        {
            if (currentDetectTime > 0f)
            {
                currentDetectTime -= Time.deltaTime;
                npcCanvas.ChangeVisionValue(currentDetectTime / npcStats.maxDetectTime);
            }
            else
            {
                npcCanvas.ChangeVisionValue(0);
                npcCanvas.ActivateVisionSlider(false);
                currentDetectTime = 0f;
            }
            return;
        }
        npcCanvas.ActivateVisionSlider(true);
        if (currentDetectTime >= npcStats.maxDetectTime)
        {
            npcCanvas.ChangeVisionValue(npcStats.maxDetectTime);
            npcCanvas.ActivateVisionSlider(false);
            playerDetected = true;
            npcCanvas.StartCoroutine(FireAnAlert());
            npcStateBeh.gameManager.AlertAll();
            currentDetectTime = 0f;
            return;
        }
        currentDetectTime += Time.deltaTime;
        npcCanvas.ChangeVisionValue(currentDetectTime / npcStats.maxDetectTime);
    }
    protected IEnumerator FireAnAlert()
    {
        npcCanvas.ActivateAlert(true);
        yield return new WaitForSeconds(1f);
        npcCanvas.ActivateAlert(false);
    }
    protected virtual void PlayerFound()
    {
        //Debug.Log("Alert player found!");
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
