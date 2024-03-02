using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    MainControls playerControls;
    [SerializeField]
    MainControls.GameplayActions mainControlsMap;
    public UnityEvent<bool> playerAttack = new UnityEvent<bool>();
    public UnityEvent cancelAttack = new UnityEvent();
    private void Start()
    {
        if (playerControls == null)
            playerControls = InputManager.inputActions;
        mainControlsMap = playerControls.Gameplay;

        mainControlsMap.Enable();
        EnableControls();
    }
    private void OnEnable()
    {
        if (playerControls != null)
            EnableControls();
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
        StartCoroutine(ResetDashDistance());
        StartCoroutine(DelayBetweenAttacks());
    }
    void CancelAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel atk");
        cancelAttack.Invoke();
        StartCoroutine(ResetDashDistance());
        StartCoroutine(DelayBetweenAttacks());
    }
    bool canAttack = true;
    IEnumerator DelayBetweenAttacks()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.1f);
        canAttack = true;
    }
    public void Slash()
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
    void IncreasingDashDistance()
    {
        if (startedAttack == false)
            return;
        if (dashDistance > maxDashDistance)
        {
            dashDistance = maxDashDistance;
            ChangeDashDistance.Invoke(dashDistance);
            return;
        }
        dashDistance += dashIncreaseMod * Time.deltaTime;
        ChangeDashDistance.Invoke(dashDistance);
    }
    IEnumerator ResetDashDistance()
    {
        yield return new WaitForFixedUpdate();
        dashDistance = 0f;
        ChangeDashDistance.Invoke(dashDistance);
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
        Collider[] colliders = Physics.OverlapSphere(attackPosition.transform.position, 2f);
        foreach (Collider collider in colliders)
        {
            Debug.Log(collider.name);
            if (collider.GetComponent<SimpleEnemy>() != null)
                collider.GetComponent<SimpleEnemy>().Die();
        }
    }
}
