using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStats : MonoBehaviour, IKillable
{
    [SerializeField]
    protected BaseStats baseStats;
    public string currentName;
    public float currentMeleeAtkRange;
    public float currentRangeAtkRange;
    public float currentAtkRechargeTine;
    public float currentCalmSpeed;
    public float currentCombatSpeed;
    public float currentSpeed;
    public float currentRotationSpeed;
    public float currentDetectDistance;
    public float currentDetectAngle;
    public float currentDetectSpeed;
    public float maxDetectTime;
    public Transform eyeLocation;
    public bool isDead = false;
    GameManager gameManager;
    [SerializeField]
    AudioClip[] deathSounds;
    [SerializeField]
    public AudioClip alertSound;
    private void Start()
    {
        if (gameManager == null)
            gameManager = GameManager.instance;
        gameManager.AddEnemyToList(this.gameObject);
    }
    public LayerMask layersToDetect;
    protected virtual void OnEnable()
    {
        InitializeStats();
    }
    public virtual void InitializeStats()
    {
        currentName = baseStats.npcName;
        currentMeleeAtkRange = baseStats.basicMeleeAtkRange;
        currentRangeAtkRange = baseStats.basicRangeAtkRange;
        currentAtkRechargeTine = baseStats.basicAtkRachargeTime;
        currentCalmSpeed = baseStats.basicCalmSpeed;
        currentCombatSpeed = baseStats.basicCombatSpeed;
        currentRotationSpeed = baseStats.basicRotationSpeed;
        currentDetectAngle = baseStats.basicDetectAngle;
        currentDetectDistance = baseStats.basicDetectDistance;
        maxDetectTime = baseStats.basicDetectTime;
        currentDetectSpeed = baseStats.basicDetectSpeed;
        currentSpeed = currentCalmSpeed;
    }

    public virtual void ChangeSpeed(float newSpeed)
    {
        currentCalmSpeed = newSpeed;
    }
    [ContextMenu("Die")]
    public void Die()
    {
        if (isDead == true)
            return;
        //Death
        DisableComponents();
        isDead = true;
        AlertOthers();
        AudioManager.instance.PlayRandomOneShotSoundFXClip(deathSounds, transform, 1f, 30f);
        gameManager.RemoveEnemyFromList(this.gameObject);
        gameManager.AddScore(100);

        //Debug.Log(currentName + " is dead");
    }
    public void DisableComponents()
    {
        Rigidbody[] arrayOfRb = GetComponents<Rigidbody>();
        foreach (Rigidbody components in arrayOfRb)
        {
            components.freezeRotation = true;
            components.useGravity = false;
            components.isKinematic = true;
        }

        Collider[] arrayOfColliders = GetComponents<Collider>();
        foreach (Collider components in arrayOfColliders)
        {
            components.gameObject.layer = 30;
        }
        GetComponent<NavMeshAgent>().enabled = false;
    }
    void AlertOthers()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f, layersToDetect);
        if (colliders.Length == 0)
            return;
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Player")
                return;
            Vector3 hitDir = collider.GetComponent<Collider>().bounds.center - this.GetComponent<Collider>().bounds.center;
            RaycastHit hit;
            if (Physics.Raycast(this.GetComponent<Collider>().bounds.center, hitDir, out hit, 10f))
            {
                if (hit.collider.GetComponent<AI_StateBehaviour>()
                    && hit.collider.GetComponent<CharacterStats>().isDead != true)
                    hit.collider.GetComponent<AI_StateBehaviour>().currentState.InstantPlayerDetect();
            }
        }
    }
}
