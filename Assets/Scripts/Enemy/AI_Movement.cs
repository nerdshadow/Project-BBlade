using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;
    Vector3 worldDeltaPosition = Vector3.zero;
    [SerializeField]
    Rigidbody npcRigidbody;
    [SerializeField]
    CharacterStats npcStats;
    public bool canMove = true;
    Vector3 moveDir;
    [SerializeField]
    float groundDrag = 1f;
    [SerializeField]
    float FinalForceMod = 8f;
    public bool canRotate = true;
    
    public GameObject rotateTarget = null;
    private void Start()
    {
        if(agent == null)
            agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        if (npcRigidbody == null)
            npcRigidbody = GetComponent<Rigidbody>();
        npcRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
        if(npcStats == null)
            npcStats = GetComponent<CharacterStats>();
        ChangeDrag();
    }
    private void FixedUpdate()
    {
        SnapAgentToNPC();
        Rotate();
        Move();
        //if (agent.isStopped)
        //    Debug.Log("agent stopped");
    }
    protected virtual void StopAgent()
    {
        worldDeltaPosition = agent.nextPosition - transform.position;
        if (worldDeltaPosition.magnitude > 2f)
            agent.isStopped = true;
        else
            agent.isStopped = false;
    }
    protected virtual void SnapAgentToNPC()
    {
        worldDeltaPosition = agent.nextPosition - transform.position;
        if (worldDeltaPosition.magnitude > 3f)
            agent.nextPosition = transform.position + 0.5f * worldDeltaPosition;
    }
    void ChangeDrag()
    {
        npcRigidbody.drag = groundDrag;
    }

    void Rotate()
    {
        if (canRotate == false)
            return;
        Vector3 lookPos;
        if (rotateTarget != null)
        {
            //Debug.Log("Steering to target");
            lookPos = rotateTarget.transform.position - this.transform.position;
        }
        else
        {
            lookPos = agent.nextPosition - this.transform.position;
        }
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, npcStats.currentRotationSpeed * Time.deltaTime);
    }
    void Move()
    {
        if (canMove != true)
            return;
        moveDir = this.transform.forward * 1f;
        npcRigidbody.AddForce(FinalForceMod * npcStats.currentSpeed * npcRigidbody.mass * moveDir, ForceMode.Force);
    }
}
