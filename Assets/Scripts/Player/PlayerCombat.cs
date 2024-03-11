using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    GameManager gameManager;
    private void Start()
    {
        if (playerControls == null)
            playerControls = InputManager.inputActions;
        mainControlsMap = playerControls.Gameplay;

        mainControlsMap.Enable();
        EnableControls();

        if (objectPoolManager == null)
            objectPoolManager = ObjectPoolManager.instance;
        if(gameManager == null)
            gameManager = GameManager.instance;

        PlayerStats.playerDied.AddListener(StopCombat);
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
    private void FixedUpdate()
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

        mainControlsMap.SheathSword.performed += StartSheath;
        mainControlsMap.SheathSword.Enable();
    }
    void DisableControls()
    {
        mainControlsMap.Attack.performed -= StartChargingAttack;
        mainControlsMap.Attack.canceled -= EndChargingAttack;
        mainControlsMap.Attack.Disable();

        mainControlsMap.CancelAttack.performed -= CancelAttack;
        mainControlsMap.CancelAttack.Disable();

        mainControlsMap.SheathSword.performed -= StartSheath;
        mainControlsMap.SheathSword.Disable();
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
        //Debug.Log("Cancel atk");
        startedAttack = false;
        cancelAttack.Invoke();
        ReloadDash();
        ReloadAttack(cancelAttackDelay);
    }
    public UnityEvent startedSheath = new UnityEvent();
    void StartSheath(InputAction.CallbackContext context)
    {
        //Debug.Log("Sheath weapon");
        startedAttack = false;
        startedSheath.Invoke();
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
        if (CheckCollisionBeforeDashWithRaycasts() == true)
        {
            ChangeDashDistance.Invoke(dashDistance);
            return;
        }
        if (dashDistance >= maxDashDistance)
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
        //Debug.Log("Reseting Dash");
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
    bool CheckCollisionBeforeDashWithRaycasts()
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
        if (dashDistance >= minDistance)
        {
            dashDistance = minDistance;
            return true;
        }
        return false;
    }
    [SerializeField]
    GameObject attackPosition;
    List<Collider> markedColliders = new List<Collider>();
    public void TryMark()
    {
        //Debug.Log("tried to attack");
        StartCoroutine(TryMarkAfterDelay());
    }
    IEnumerator TryMarkAfterDelay()
    {
        yield return new WaitForFixedUpdate();
        bool hitOnTipTarget = TryMarkOnEndOfDash();
        bool hitThroughTarget = TryMarkOnLengthOfDash();
        if (hitOnTipTarget == true || hitThroughTarget == true)
            ReloadAttack(hitAttackDelay);
    }
    bool TryMarkOnEndOfDash()
    {
        bool hitTarget = false;
        Collider[] colliders = Physics.OverlapSphere(attackPosition.transform.position, 2f);
        foreach (Collider collider in colliders)
        {
            //if (collider.tag != "Player" 
            //    && collider.GetComponent<IKillable>() != null)
            //{
            //    if (markedColliders.Contains(collider) == true)
            //        markedColliders.Remove(collider);
            //    markedColliders.Add(collider);
            //    if (collider.GetComponent<AI_Canvas>() != null)
            //        collider.GetComponent<AI_Canvas>().ActivateDeathMark(true);
            //    Vfx_TryPoolBloodsplash(collider);
            //    hitTarget = true;
            //}
            //if (collider.GetComponent<AI_StateBehaviour>() != null)
            //    collider.GetComponent<AI_StateBehaviour>().GetStunned();
            CheckColliderForMarking(collider);
        }
        return hitTarget;
    }
    Vector3 startPosOfDash = Vector3.zero;
    bool TryMarkOnLengthOfDash()
    {
        bool hitTarget = false;
        Debug.DrawLine(startPosOfDash, attackPosition.transform.position, Color.blue, 3f);
        Collider[] colliders = Physics.OverlapCapsule(startPosOfDash, attackPosition.transform.position, 1.5f);
        foreach (Collider collider in colliders)
        {
            //if (collider.tag != "Player"
            //    && collider.gameObject.layer == LayerMask.NameToLayer("CharacterLayer")
            //    && collider.GetComponent<IKillable>() != null)
            //{
            //    if (markedColliders.Contains(collider) == true)
            //    {
            //        collider.GetComponentInChildren<BloodStreamParticle>().ReturnToPool();
            //        markedColliders.Remove(collider);
            //    }
            //    markedColliders.Add(collider);
            //    collider.GetComponent<AI_Canvas>().ActivateDeathMark(true);
            //    Vfx_TryPoolBloodsplash(collider);
            //    hitTarget = true;
            //    if (collider.GetComponent<AI_StateBehaviour>() != null)
            //        collider.GetComponent<AI_StateBehaviour>().GetStunned();
            //}
            CheckColliderForMarking(collider);

        }
        return hitTarget;
    }
    [SerializeField]
    GameObject deathMarkCanvas;
    bool CheckColliderForMarking(Collider collider)
    {
        if (collider.tag != "Player"
                && collider.gameObject.layer == LayerMask.NameToLayer("CharacterLayer")
                && collider.GetComponent<IKillable>() != null)
        {
            if (markedColliders.Contains(collider) == true)
            {
                collider.GetComponentInChildren<BloodStreamParticle>().ReturnToPool();
                markedColliders.Remove(collider);
            }
            markedColliders.Add(collider);
            collider.GetComponent<AI_Canvas>().ActivateDeathMark(true);
            Vfx_TryPoolBloodsplash(collider);
            if (collider.GetComponent<AI_StateBehaviour>() != null)
                collider.GetComponent<AI_StateBehaviour>().GetStunned();
            return true;
        }
        if (collider.tag != "Player"
            && collider.GetComponent<IKillable>() != null)
        {
            if (markedColliders.Contains(collider) == true)
            {
                return false;
            }
            GameObject tempCanvas = Instantiate(deathMarkCanvas, collider.transform);
            tempCanvas.transform.position = collider.bounds.center;
            markedColliders.Add(collider);
            return true;
        }
        return false;
    }
    public void TryBlowMarks()
    {
        if (markedColliders.Count == 0)
            return;
        foreach (Collider collider in markedColliders)
        {
            //BloodStreamParticle vfx = collider.GetComponentInChildren<BloodStreamParticle>();
            //vfx.transform.SetParent(null);
            //vfx.PlayAndTryReturnToPool();
            //collider.GetComponent<AI_Canvas>().ActivateDeathMark(false);
            //collider.GetComponent<IKillable>().Die();
            CheckMarkedColliders(collider);
        }
        markedColliders.Clear();
    }
    void CheckMarkedColliders(Collider collider) 
    {
        BloodStreamParticle vfx = collider.GetComponentInChildren<BloodStreamParticle>();
        if (vfx != null)
        {
            vfx.transform.SetParent(null);
            vfx.PlayAndTryReturnToPool();
        }
        if (collider.GetComponent<AI_Canvas>() != null)
            collider.GetComponent<AI_Canvas>().ActivateDeathMark(false);
        else
            collider.GetComponentInChildren<Canvas>().enabled = false;
        collider.GetComponent<IKillable>().Die();
    }
    void StopCombat()
    {
        StopAllCoroutines();
        markedColliders.Clear();
        canAttack = false;
    }
    void Vfx_TryPoolBloodsplash(Collider targetColl)
    {
        BloodStreamParticle currentBloodStram = objectPoolManager.bloodStreamPool.Get();
        currentBloodStram.transform.position = targetColl.bounds.center;
        currentBloodStram.transform.rotation = Quaternion.LookRotation(transform.forward);
        currentBloodStram.transform.parent = targetColl.transform;
        //currentBloodStram.transform.rotation = Quaternion.LookRotation(-targetColl.transform.forward);
        //currentBloodStram.PlayAndTryReturnToPool();
    }
}
