using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class PlayerAnimRig : MonoBehaviour
{
    [SerializeField]
    Camera mainCam;
    #region Animations
    Animator anim;
    public Vector3 moveDirection = Vector3.zero;
    public float moveX = 0f, moveZ = 0f;
    float playerMagnitude = 0f;
    [SerializeField]
    float expectedAnimationSpeed = 4f;
    [SerializeField]
    float setFloatSpeed = 1f;
    [SerializeField]
    float setFloatDamp = 0.1f;
    [SerializeField]
    float heightMod = -0.05f;
    public bool canRotate = true;
    bool inAttack = false;
    #endregion Animations

    [SerializeField]
    GameObject targetPositionGO;
    [SerializeField]
    GameObject aimTargetGO;
    bool lookingDirectly = true;
    public float bottomRotationSpeed = 3f;
    private void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
        PlayerStats.playerDied.AddListener(StopAnim);
        PlayerStats.turnPlayerToKiller.AddListener(ForceRotate);
    }
    private void Update()
    {
        CopyPostion(targetPositionGO);
    }
    private void FixedUpdate()
    {
        TryToRotate();
        ManageAnimator();
        ReadyToDash();
    }
    void CopyPostion(GameObject _positionTarget)
    {
        transform.position = new Vector3(_positionTarget.transform.position.x, _positionTarget.transform.position.y + heightMod, _positionTarget.transform.position.z);
    }
    public void TryGetPlayerMagnitude(float _magni)
    {
        playerMagnitude = _magni;
    }
    public void TryGetMoveDirection(Vector3 _moveDir)
    {
        moveDirection = _moveDir;
    }
    void TryToRotate()
    {
        if (canRotate == false)
            return;
        //if (moveDirection == Vector3.zero)
        //{
        //    CheckRotationTowardTarget(aimTargetGO);
        //    RotateTowards(aimTargetGO);
        //}
        else
        {
            RotateInMove(aimTargetGO);
        }
    }
    void CheckRotationTowardTarget(GameObject _aimTarget)
    {
        if (lookingDirectly == false)
            return;
        Vector3 forward = transform.forward;
        Vector3 toTarget = _aimTarget.transform.position - transform.position;
        toTarget.y = 0 + heightMod;
        //Debug.Log(Vector3.Angle(forward, toTarget));
        if (Vector3.Angle(forward, toTarget) >= 60)
        {
            lookingDirectly = false;
        }
    }
    void RotateTowards(GameObject _aimTarget)
    {
        if (lookingDirectly == true)
            return;
        Vector3 forward = transform.forward;
        Vector3 toTarget = _aimTarget.transform.position - transform.position;
        if (Vector3.Angle(forward, toTarget) >= 1)
        {
            toTarget.y = 0 + heightMod;
            Quaternion rotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, bottomRotationSpeed * Time.deltaTime);
        }
        else
        {
            toTarget.y = 0 + heightMod;
            Quaternion rotation = Quaternion.LookRotation(toTarget);
            transform.rotation = rotation;
            lookingDirectly = true;
        }

    }
    void ForceRotate(Transform rotateTo)
    {
        Vector3 forward = transform.forward;
        Vector3 toTarget = rotateTo.position - transform.position;
        toTarget.y = 0 + heightMod;
        Quaternion rotation = Quaternion.LookRotation(toTarget);
        transform.rotation = rotation;
    }
    void RotateInMove(GameObject _aimTarget)
    {
        Vector3 toTarget = _aimTarget.transform.position - transform.position;
        toTarget.y = 0 + heightMod;
        Quaternion rotation = Quaternion.LookRotation(toTarget);
        transform.rotation = rotation;
    }
    public void TryGetMoveXZ(float _moveX, float _moveZ)
    {
        moveX = _moveX;
        moveZ = _moveZ;
    }
    void ManageAnimator()
    {
        if (moveDirection == Vector3.zero)
        {
            anim.SetFloat("MovementSpeedMulti", 1f);
            ChangeAnimValue("MoveX", 0);
            ChangeAnimValue("MoveZ", 0);
            return;
        }

        Vector3 moveDir = new(Mathf.Round(moveX), 0, Mathf.Round(moveZ));

        if (moveDir.magnitude > 1.0f)
        {
            moveDir = moveDir.normalized;
        }
        moveDir = Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0) * moveDir;
        moveDir = transform.InverseTransformDirection(moveDir);

        if (playerMagnitude < 0)
            playerMagnitude = 0f;
        anim.SetFloat("MovementSpeedMulti", playerMagnitude / expectedAnimationSpeed);
        ChangeAnimValue("MoveX", moveDir.x);
        ChangeAnimValue("MoveZ", moveDir.z);
    }
    public void ChangeAnimValue(string name, float value)
    {
        float res = Mathf.Abs(value) - Mathf.Abs(anim.GetFloat(name));
        if (Mathf.Abs(res) <= 0.05)
        {
            anim.SetFloat(name, value);
            return;
        }
        anim.SetFloat(name, value, setFloatDamp, setFloatSpeed * Time.deltaTime);
    }
    [SerializeField]
    float normAnimTimeForAttack = 0.1f;
    public void ChangeInAttack(bool inatk)
    {
        inAttack = inatk;
    }
    public void Attack(bool isAttacking)
    {
        if (isAttacking == false)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("PrepareAtk") != true)
            {
                anim.CrossFade("MovementTree", 0.2f, 0);
                anim.CrossFade("MovementTree", 0.2f, 1);
                return;
            }
            float nTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (nTime >= normAnimTimeForAttack)
            {
                anim.Play("EndAtk", 0);
                anim.Play("EndAtk", 1);
            }
            else
            {
                anim.CrossFade("MovementTree", 0.2f, 0);
                anim.CrossFade("MovementTree", 0.2f, 1);
            }
        }
        else
        {
            anim.CrossFade("PrepareAtk", 0.1f, 0);
            anim.CrossFade("PrepareAtk", 0.1f, 1);
        }
    }
    public void StartSheath()
    {
        anim.CrossFade("SheathFast", 0.1f);
    }
    public void ReleaseAtk()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("PrepareAtk"))
            return;
        anim.CrossFade("MovementTree", 0.2f, 0);
        anim.CrossFade("MovementTree", 0.2f, 1);
    }
    public UnityEvent<bool> ReadyToDashEvent = new UnityEvent<bool>();
    public void ReadyToDash()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("PrepareAtk") != true)
        {
            ReadyToDashEvent.Invoke(false);
            return;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= normAnimTimeForAttack)
            ReadyToDashEvent.Invoke(true);
    }
    [SerializeField]
    GameObject weapon;
    [SerializeField]
    GameObject sheath;
    public UnityEvent attackAnimSlashed = new UnityEvent();
    public void AttackAnimSlashed()
    {
        SheathWeapon(0);
        attackAnimSlashed.Invoke();
    }
    public UnityEvent sheathedIn = new UnityEvent();
    public void SheathWeapon(int toSheath)
    {
        //Debug.Log(toSheath);
        if (toSheath == 1)
        {
            weapon.SetActive(false);
            sheath.SetActive(true);
            ChangeRArmAnimLayerWeght(false);
            sheathedIn.Invoke();

        }
        else
        {
            weapon.SetActive(true);
            sheath.SetActive(false);
            ChangeRArmAnimLayerWeght(true);
        }
    }
    //Coroutine changeAnimLayerWeight;
    [ContextMenu("RArmLayer")]
    void TestLayer()
    {
        ChangeRArmAnimLayerWeght(true);
    }
    public void ChangeRArmAnimLayerWeght(bool toChange)
    {
        if (toChange == true)
        {
            anim.SetLayerWeight(1, 1);
        }
        if (toChange == false)
        {
            anim.SetLayerWeight(1, 0);
        }
    }
    void StopAnim()
    {
        anim.Play("Death");
        canRotate = false;
    }
    [SerializeField]
    AudioClip swordSheathSFX;
    public void SheathSwordSFX()
    {
        AudioManager.instance.PlayOneShotSoundFXClip(swordSheathSFX, transform, 1f);
    }
    [SerializeField]
    AudioClip[] footStepDirt;
    bool canFootStep = true;
    public void FootStepSFX()
    {
        if (canFootStep == true)
        {
            AudioManager.instance.PlayRandomOneShotSoundFXClip(footStepDirt, transform, 0.5f);
            canFootStep = false;
            StartCoroutine(WaitFootStep());
            //Debug.Log("Step");
        }
    }
    IEnumerator WaitFootStep()
    {
        yield return new WaitForSeconds(0.1f);
        canFootStep = true;
    }
}
