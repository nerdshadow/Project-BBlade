using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
        Debug.Log("Started atk");
        playerAttack.Invoke(true);
        startedAttack = true;
    }
    void EndChargingAttack(InputAction.CallbackContext context)
    {
        if (canAttack == false)
            return;
        if (startedAttack == false)
            return;
        Debug.Log("Stopped atk");
        playerAttack.Invoke(false);
        startedAttack = false;
        StartCoroutine(DelayBetweenAttacks());
    }
    void CancelAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel atk");
        cancelAttack.Invoke();
        StartCoroutine(DelayBetweenAttacks());
    }
    bool canAttack = true;
    IEnumerator DelayBetweenAttacks()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.1f);
        canAttack = true;
    }
}
