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
    protected GameObject playerRef = null;
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    public AI_Base_State currentState;
    [SerializeField]
    protected CharacterStats characterStats;
    [SerializeField]
    protected AI_Movement npcMovement;
    [SerializeField]
    protected AI_Canvas npcCanvas;
    [SerializeField]
    public Collider characterColl;
    public bool canAttack = true;
    public Transform meleeAtkPoint;
    public Transform rangeAtkPoint;
    public List<GameObject> wayPoints = null;
    public bool canPatrol = false;
    public GameManager gameManager;
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
        if (gameManager == null)
            gameManager = GameManager.instance;

        playerRef = gameManager.playerRef;

        if(npcCanvas == null)
            npcCanvas = GetComponent<AI_Canvas>();

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
    protected Coroutine ChangeAnimMoveX;
    public void Change_Anim_MoveX_Weight(float var_end, float duration)
    {
        if (var_end == anim.GetFloat("Move_X"))
            return;
        //Debug.Log(gameObject.name + " changing dir x");
        if (ChangeAnimMoveX != null)
        {
            if (ChangeAnimMoveX == StartCoroutine(IEnum_Change_Anim_MoveX_Weight(var_end, duration)))
                return;
            StopCoroutine(ChangeAnimMoveX);
            ChangeAnimMoveX = null;
        }
        if (anim.GetFloat("Move_X") != var_end)
        {
            ChangeAnimMoveX = StartCoroutine(IEnum_Change_Anim_MoveX_Weight(var_end, duration));
        }
        else return;
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
    protected Coroutine ChangeAnimCombatValue;
    public void Change_Anim_CombatValue(float var_end, float duration)
    {
        if (var_end == anim.GetFloat("CombatValue"))
            return;
        //Debug.Log(gameObject.name + " changing dir x");
        if (ChangeAnimCombatValue != null)
        {
            StopCoroutine(ChangeAnimCombatValue);
            ChangeAnimCombatValue = null;
        }
        if (anim.GetFloat("CombatValue") != var_end)
        {
            ChangeAnimCombatValue = StartCoroutine(IEnum_Change_Anim_CombatValue(var_end, duration));
        }
        else return;
    }
    protected virtual IEnumerator IEnum_Change_Anim_CombatValue(float var_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            anim.SetFloat("CombatValue", Mathf.Lerp(anim.GetFloat("CombatValue"), var_end, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetFloat("CombatValue", var_end);
        ChangeAnimCombatValue = null;
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
        canAttack = true;
        ReloadCoroutine = null;
    }
    public void ReloadAtk()
    {
        if (ReloadCoroutine == null)
        {
            canAttack = false;
            ReloadCoroutine = StartCoroutine(ReloadCD(characterStats.currentAtkRechargeTine));
        }
        else return;
    }
    public void DoMeleeAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(meleeAtkPoint.position, 1f);
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

    [SerializeField]
    GameObject bulletTrailVFX;
    GameObject currentBulletTrail;
    [SerializeField]
    float trailLifetime = 0.1f;
    public void DoRangedAttack()
    {
        if(playerRef == null)
            playerRef = gameManager.playerRef;
        StartCoroutine(ShootOnNextUpdate());
        
    }
    IEnumerator ShootOnNextUpdate()
    {
        yield return new WaitForFixedUpdate();
        rangeAtkPoint.LookAt(playerRef.GetComponent<Collider>().bounds.center);
        currentBulletTrail = Instantiate(bulletTrailVFX, rangeAtkPoint.position, rangeAtkPoint.rotation);
        currentBulletTrail.GetComponent<BulletTrail>().lineLifetime = trailLifetime;
        currentBulletTrail.GetComponent<LineRenderer>().SetPosition(0, rangeAtkPoint.position);
        currentBulletTrail.GetComponent<BulletTrail>().StartTimer();
        currentBulletTrail.SetActive(true);

        Vector3 shootDir = rangeAtkPoint.forward + new Vector3(Random.Range(-0.1f, 0.1f),
                                                        Random.Range(-0.1f, 0.1f),
                                                        Random.Range(-0.1f, 0.1f));
        shootDir.Normalize();
        if (Physics.Raycast(rangeAtkPoint.position, shootDir, out RaycastHit hit, characterStats.currentRangeAtkRange * 2))
        {
            currentBulletTrail.GetComponent<LineRenderer>().SetPosition(1, hit.point);
            hit.collider.GetComponent<IKillable>()?.Die();
        }
        else
        {
            currentBulletTrail.GetComponent<LineRenderer>().SetPosition(1, rangeAtkPoint.position + (shootDir * characterStats.currentRangeAtkRange * 2));
        }
    }
}
