using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    Transform playerBody;
    [SerializeField]
    float followSpeed;
    [SerializeField]
    float followDistance;
    [SerializeField]
    Vector3 offset;
    [SerializeField]
    Quaternion rotation;
    [SerializeField]
    GameObject aimTarget;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        Vector3 pos = Vector3.Lerp(transform.position, playerBody.position + offset + (-transform.forward * followDistance), followSpeed * Time.deltaTime);
        transform.position = pos;

        transform.rotation = rotation;
    }
    private void FixedUpdate()
    {
        MoveAimTarget();
    }

    void MoveAimTarget()
    {
        if (aimTarget == null)
            return;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            aimTarget.transform.position = hit.point;
        }
        else
        {
            aimTarget.transform.position = ray.direction * 30;
        }
    }

    [ContextMenu("RefreshCamera")]
    void RefreshCameraEdit()
    {
        Vector3 pos = playerBody.position + offset + (-transform.forward * followDistance);
        transform.position = pos;

        transform.rotation = rotation;
    }
}
