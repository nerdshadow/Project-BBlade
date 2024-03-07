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
    public float maxDetectTime;

    public Transform eyeLocation;

    public bool isDead = false;

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
            components.enabled = false;
        }
        GetComponent<NavMeshAgent>().enabled = false;
    }
}
