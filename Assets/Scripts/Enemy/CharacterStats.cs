using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStats : MonoBehaviour, IKillable
{
    [SerializeField]
    protected BaseStats baseStats;
    public string currentName;
    public float currentAtkRange;
    public float currentAtkRechargeTine;
    public float currentSpeed;
    public float currentRotationSpeed;
    public float currentDetectDistance;
    public float currentDetectAngle;
    public float currentDetectTime;

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
        currentAtkRange = baseStats.basicAtkRange;
        currentAtkRechargeTine = baseStats.basicAtkRachargeTime;
        currentSpeed = baseStats.basicSpeed;
        currentRotationSpeed = baseStats.basicRotationSpeed;
        currentDetectAngle = baseStats.basicDetectAngle;
        currentDetectDistance = baseStats.basicDetectDistance;
        currentDetectTime = baseStats.basicDetectTime;
    }

    public virtual void ChangeSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }
    [ContextMenu("Die")]
    public void Die()
    {
        if (isDead == true)
            return;
        //Death
        DisableComponents();
        isDead = true;
        Debug.Log(currentName + " is dead");
    }
    public void DisableComponents()
    {
        Rigidbody[] arrayOfRb = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody components in arrayOfRb)
        {
            components.freezeRotation = true;
        }

        Collider[] arrayOfColliders = GetComponentsInChildren<Collider>();
        foreach (Collider components in arrayOfColliders)
        {
            if(components.gameObject.layer == 6)
                components.enabled = false;
            LayerMask layer = components.excludeLayers;
            layer |= (1 << 10);
            components.excludeLayers = layer;
        }
        GetComponent<NavMeshAgent>().enabled = false;
    }
}
