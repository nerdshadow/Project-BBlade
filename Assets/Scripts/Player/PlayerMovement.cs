using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static AI_Base_State;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigbody;
    [SerializeField]
    Camera mainCam;
    [SerializeField]
    PlayerStats playerStats;

    #region "Movement"
    [SerializeField]
    MainControls playerControls;
    [SerializeField]
    MainControls.GameplayActions mainControlsMap;
    [Header("Movement: ")]
    [SerializeField]
    Vector2 movementInput;
    [SerializeField]
    private Vector3 playerVelocity;
    [SerializeField]
    float finalForceMod = 8f;
    [SerializeField]
    float playerMagnitude = 0f;
    public UnityEvent<float> TellPlayerMagnitude = null;
    public Vector3 moveDir;
    public UnityEvent<Vector3> TellMoveDir = null;
    public float moveX = 0, moveZ = 0;
    public UnityEvent<float, float> TellMoveInput = null;
    [SerializeField]
    float groundDrag = 5f;
    public bool controlSpeed = true;
    public bool canMove = true;
    public bool canRotate = true;
    #endregion Movement
    #region "Colliders"
    [SerializeField]
    CapsuleCollider characterColl;
    [SerializeField]
    Collider damageableColl;
    #endregion Colliders

    private void Start()
    {
        IgnoreCollider(characterColl);
        IgnoreCollider(damageableColl);

        if (playerControls == null)
            playerControls = InputManager.inputActions;
        mainControlsMap = playerControls.Gameplay;
        //Redo
        mainControlsMap.Enable();
        PlayerStats.playerDied.AddListener(StopMovement);
        EnableControls();

    }
    private void OnEnable()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (playerControls != null)
            EnableControls();
    }
    private void OnDisable()
    {
        DisableControls();
    }
    void IgnoreCollider(Collider coll)
    {
        if (coll == null)
            return;
        Collider[] colls = transform.parent.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i] != coll)
                Physics.IgnoreCollision(coll, colls[i]);
        }
    }
    void EnableControls()
    {
        mainControlsMap.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        mainControlsMap.Movement.canceled += i => movementInput = i.ReadValue<Vector2>();
    }
    void DisableControls()
    {
        //mainControlsMap.Movement.performed -= i => movementInput = i.ReadValue<Vector2>();
        //mainControlsMap.Movement.canceled -= i => movementInput = i.ReadValue<Vector2>();
    }
    private void Update()
    {
        ApplyDrag();
        ShowDashDistance();
    }
    private void FixedUpdate()
    {
        Movement();
        RotateTowardsMouse();
        FinalMove();
    }
    void ControlSpeed()
    {
        if (!controlSpeed)
            return;
        if (rigbody.velocity.magnitude > playerStats.currentMovSpeed)
        {
            rigbody.velocity = rigbody.velocity.normalized * playerStats.currentMovSpeed;
        }
    }
    void Movement()
    {
        if (canMove == false)
            return;
        moveX = movementInput.x;
        moveZ = movementInput.y;
        TellMoveInput.Invoke(moveX, moveZ);
        moveDir = transform.parent.forward * moveZ + transform.parent.right * moveX;
        moveDir = moveDir.normalized;
        moveDir = Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0) * moveDir;
    }
    void ApplyDrag()
    {
        if (!controlSpeed)
            return;
        rigbody.drag = groundDrag;
    }
    void RotateTowardsMouse()
    {
        if (canRotate == false)
            return;
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
        else
        {
            transform.LookAt(new Vector3(ray.direction.x * 100, transform.position.y,
                                                ray.direction.z * 100));
        }
    }
    void FinalMove()
    {
        if (canMove == false)
        {
            TellMoveDir.Invoke(Vector3.zero);
            TellPlayerMagnitude.Invoke(0f);
            return;
        }
        TellMoveDir.Invoke(moveDir);
        rigbody.AddForce(finalForceMod * playerStats.currentMovSpeed * rigbody.mass * moveDir, ForceMode.Force);
        playerMagnitude = rigbody.velocity.magnitude;
        TellPlayerMagnitude.Invoke(playerMagnitude);
        ControlSpeed();
    }
    public void StopMovement()
    {
        groundDrag = 1f;
        moveDir = Vector3.zero;
        canMove = false;
        canRotate = false;
        controlSpeed = false;
        //this.enabled = false;
    }
    public void ChangeAbilityToMove(bool isAttacking)
    {
        canMove = !isAttacking;
        moveX = 0f;
        moveZ = 0f;
        rigbody.velocity = Vector3.zero;
    }
    [SerializeField]
    GameObject target;
    public float currentDashDistance = 0;
    public void ChangeDashDistance(float distance)
    {
        currentDashDistance = distance;
    }
    public void DashForward()
    {
        if (currentDashDistance <= 1.5f)
            return;
        Vector3 pos = transform.position;
        Vector3 dir = (target.transform.position - pos).normalized;
        //Debug.Log("Dashed for " + currentDashDistance);
        rigbody.MovePosition(pos + dir * currentDashDistance);
    }
    [SerializeField]
    GameObject distanceIdicator;
    void ShowDashDistance()
    {
        Vector3 pos = transform.position + new Vector3(0, 0.05f, 0);
        Vector3 dir = (target.transform.position - pos).normalized;
        dir = new Vector3(dir.x, pos.y, dir.z);
        
        distanceIdicator.transform.position = pos + dir * currentDashDistance;
    }

}
