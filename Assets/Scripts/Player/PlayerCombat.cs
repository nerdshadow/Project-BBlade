using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    MainControls playerControls;
    [SerializeField]
    MainControls.GameplayActions mainControlsMap;
    public UnityEvent<bool> playerAttack = new UnityEvent<bool>();
    public UnityEvent cancelAttack = new UnityEvent();
    [SerializeField]
    ObjectPoolManager objectPoolManager = ObjectPoolManager.instance;
    private void Start()
    {
        if (playerControls == null)
            playerControls = InputManager.inputActions;
        mainControlsMap = playerControls.Gameplay;

        mainControlsMap.Enable();
        EnableControls();

        if (objectPoolManager == null)
            objectPoolManager = ObjectPoolManager.instance;
    }
    private void OnEnable()
    {
        if (playerControls != null)
            EnableControls();
        if (objectPoolManager == null)
            objectPoolManager = ObjectPoolManager.instance;
    }
    private void OnDisable()
    {
        if (playerControls != null)
            DisableControls();
    }
    private void Update()
    {
        IncreasingDashDistance();
    }
    void EnableControls()
    {
        mainControlsMap.Attack.performed += StartChargingAttack;
        mainControlsMap.Attack.canceled += EndChargingAttack;
        mainControlsMap.Attack.Enable();

        mainControlsMap.CancelAttack.performed += CancelAttack;
        mainControlsMap.CancelAttack.Enable();
    }
    void DisableControls()
    {
        mainControlsMap.Attack.performed -= StartChargingAttack;
        mainControlsMap.Attack.canceled -= EndChargingAttack;
        mainControlsMap.Attack.Disable();

        mainControlsMap.CancelAttack.performed -= CancelAttack;
        mainControlsMap.CancelAttack.Disable();
    }
    bool startedAttack = false;
    void StartChargingAttack(InputAction.CallbackContext context)
    {
        if (canAttack == false)
            return;
        startPosOfDash = transform.position + new Vector3(0, attackPosition.transform.position.y, 0);
        //Debug.Log("Started atk");
        playerAttack.Invoke(true);
        startedAttack = true;
    }
    void EndChargingAttack(InputAction.CallbackContext context)
    {
        if (canAttack == false)
            return;
        if (startedAttack == false)
            return;
        //Debug.Log("Stopped atk");
        playerAttack.Invoke(false);
        startedAttack = false;
        ReloadDash();
        ReloadAttack(missAttackDelay);
    }
    void CancelAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel atk");
        startedAttack = false;
        cancelAttack.Invoke();
        ReloadDash();
        ReloadAttack(cancelAttackDelay);
    }
    [SerializeField]
    float cancelAttackDelay = 1f;
    [SerializeField]
    float missAttackDelay = 3f;
    [SerializeField]
    float hitAttackDelay = 0.1f;
    bool canAttack = true;
    Coroutine reloadAttack = null;
    void ReloadAttack(float _delay)
    {
        if (reloadAttack != null)
            StopCoroutine(reloadAttack);
        reloadAttack = StartCoroutine(DelayBetweenAttacks(_delay));
    }
    public UnityEvent<float, float> AttackRechargeEvent = new UnityEvent<float, float>();
    IEnumerator DelayBetweenAttacks(float _delay)
    {
        canAttack = false;
        float currentDelay = 0;
        AttackRechargeEvent.Invoke(_delay, currentDelay);
        while (currentDelay < _delay)
        {
            currentDelay += Time.deltaTime;
            AttackRechargeEvent.Invoke(_delay, currentDelay);
            yield return null;
        }
        currentDelay = _delay;
        AttackRechargeEvent.Invoke(_delay, currentDelay);
        canAttack = true;
    }
    public void SlashVFX()
    {
        GetComponentInChildren<VisualEffect>().Play();
    }
    [SerializeField]
    float dashDistance = 0f;
    [SerializeField]
    float maxDashDistance = 10f;
    [SerializeField]
    float dashIncreaseMod = 3f;
    public UnityEvent<float> ChangeDashDistance = new UnityEvent<float>();
    bool canIncreaseDashDistance = false;
    public void ChangeCanIncreaseDashDistance(bool _can)
    {
        canIncreaseDashDistance = _can;
    }
    void IncreasingDashDistance()
    {
        if (startedAttack == false || canIncreaseDashDistance == false)
            return;
        CheckCollisionBeforeDashWithRaycasts();
        if (dashDistance > maxDashDistance)
        {
            dashDistance = maxDashDistance;
            ChangeDashDistance.Invoke(dashDistance);
            return;
        }
        dashDistance += dashIncreaseMod * Time.deltaTime;
        ChangeDashDistance.Invoke(dashDistance);
    }
    Coroutine reloadDash = null;
    void ReloadDash()
    {
        if (reloadDash != null)
            StopCoroutine(reloadDash);
        reloadDash = StartCoroutine(ResetDashDistance());
    }
    IEnumerator ResetDashDistance()
    {
        Debug.Log("Reseting Dash");
        yield return new WaitForFixedUpdate();
        dashDistance = 0f;
        ChangeDashDistance.Invoke(dashDistance);
    }
    [SerializeField]
    GameObject target;
    [SerializeField]
    LayerMask collisionLayerMask = 0;
    [SerializeField]
    Transform[] raycastPositions;
    void CheckCollisionBeforeDashWithRaycasts()
    {
        RaycastHit hit;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < raycastPositions.Length; i++)
        {
            Vector3 dir = raycastPositions[i].forward;
            //Debug.DrawRay(raycastPositions[i].position, dir * maxDashDistance);
            if (Physics.Raycast(raycastPositions[i].position, dir, out hit, maxDashDistance, collisionLayerMask))
            {
                Vector3 direction = hit.point -raycastPositions[i].position;
                Debug.DrawRay(raycastPositions[i].position, direction, Color.red);
                float distance = direction.magnitude;
                distance = Mathf.Abs(distance);
                if (distance <= minDistance)
                    minDistance = distance;
            }
        }
        if (dashDistance > minDistance)
        {
            dashDistance = minDistance;
        }
    }

    [SerializeField]
    GameObject attackPosition;
    public void TryAttack()
    {
        Debug.Log("tried to attack");
        StartCoroutine(TryAttackAfterDelay());
    }
    IEnumerator TryAttackAfterDelay()
    {
        yield return new WaitForFixedUpdate();
        bool hitOnTipTarget = TryDealDamageOnEndOfDash();
        bool hitThroughTarget = TryDealDamageOnLengthOfDash();
        if (hitOnTipTarget == true || hitThroughTarget == true)
            ReloadAttack(hitAttackDelay);
    }
    bool TryDealDamageOnEndOfDash()
    {
        bool hitTarget = false;
        Collider[] colliders = Physics.OverlapSphere(attackPosition.transform.position, 2f);
        foreach (Collider collider in colliders)
        {
            if (collider.tag != "Player" 
                && collider.GetComponent<IKillable>() != null)
            {
                collider.GetComponent<IKillable>().Die();
                hitTarget = true;
                //Play BloodVFX
                Vfx_TryBloodsplash(collider);
            }
        }
        return hitTarget;
    }
    Vector3 startPosOfDash = Vector3.zero;
    bool TryDealDamageOnLengthOfDash()
    {
        bool hitTarget = false;
        Debug.DrawLine(startPosOfDash, attackPosition.transform.position, Color.blue, 3f);
        Collider[] colliders = Physics.OverlapCapsule(startPosOfDash, attackPosition.transform.position, 1.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.tag != "Player"
                && collider.GetComponent<IKillable>() != null)
            {
                collider.GetComponent<IKillable>().Die();
                hitTarget = true;
                //Play BloodVFX
                Vfx_TryBloodsplash(collider);
            }
        }
        return hitTarget;
    }
    void Vfx_TryBloodsplash(Collider targetColl)
    {
        BloodStreamParticle currentBloodStram = objectPoolManager.bloodStreamPool.Get();
        currentBloodStram.transform.position = targetColl.bounds.center;
        currentBloodStram.transform.rotation = Quaternion.LookRotation(transform.forward);
        currentBloodStram.PlayAndTryReturnToPool();
    }
}
