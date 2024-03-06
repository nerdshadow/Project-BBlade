using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_StateBehaviour : MonoBehaviour
{
    [Header("Variables: ")]
    [SerializeField]
    protected NavMeshAgent agent;
    [SerializeField]
    protected GameObject target = null;
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    public AI_Base_State currentState;
    [SerializeField]
    protected CharacterStats characterStats;
    [SerializeField]
    protected AI_Movement npcMovement;

    [SerializeField]
    Collider characterColl;

    public bool canMeleeAttack = true;

    protected Coroutine ChangeAnimMoveX;

    public List<GameObject> wayPoints = null;
    public bool canPatrol = false;

    private void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0.1f;

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (characterStats != null)
            agent.speed = 60;

        if(npcMovement == null)
            npcMovement = GetComponent<AI_Movement>();

        IgnoreCollider(characterColl);
    }
    protected virtual void Update()
    {
        currentState = currentState.Process();
    }

    protected virtual void IgnoreCollider(Collider coll)
    {
        Collider[] colls = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i] != coll)
                Physics.IgnoreCollision(coll, colls[i]);
        }

        colls = GetComponents<Collider>();
        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i] != coll)
                Physics.IgnoreCollision(coll, colls[i]);
        }
    }
    protected virtual IEnumerator IEnum_Change_Anim_MoveX_Weight(float var_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            anim.SetFloat("Move_X", Mathf.Lerp(anim.GetFloat("Move_X"), var_end, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetFloat("Move_X", var_end);
        ChangeAnimMoveX = null;
    }
    public void Change_Anim_MoveX_Weight(float var_end, float duration)
    {
        if (var_end == anim.GetFloat("Move_X"))
            return;
        //Debug.Log(gameObject.name + " changing dir x");
        if (ChangeAnimMoveX != null)
        {
            StopCoroutine(ChangeAnimMoveX);
            ChangeAnimMoveX = null;
        }
        if (anim.GetFloat("Move_X") != var_end)
        {
            ChangeAnimMoveX = StartCoroutine(IEnum_Change_Anim_MoveX_Weight(var_end, duration));
        }
        else return;
    }
    protected Coroutine ReloadCoroutine;
    IEnumerator ReloadCD(float reload)
    {
        float x = 0;
        while (x <= reload)
        {
            x += Time.deltaTime;
            yield return null;
        }
        canMeleeAttack = true;
        ReloadCoroutine = null;
    }
    public void ReloadAtk()
    {
        if (ReloadCoroutine == null)
        {
            canMeleeAttack = false;
            ReloadCoroutine = StartCoroutine(ReloadCD(characterStats.currentAtkRechargeTine));
        }
        else return;
    }
    public void DoAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(/*attackPosition.*/transform.position, 2f);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Player"
                && collider.GetComponent<IKillable>() != null)
            {
                collider.GetComponent<IKillable>().Die();
                //Play BloodVFX
                //Vfx_TryBloodsplash(collider);
            }
        }
    }
}
