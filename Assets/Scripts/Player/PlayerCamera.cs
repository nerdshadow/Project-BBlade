using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public GameObject aimTarget;
    [SerializeField]
    LayerMask layerMask;
    public Volume cameraVolume;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        if(cameraVolume == null)
            cameraVolume = GetComponentInChildren<Volume>();
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
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            aimTarget.transform.position = hit.point;
        }
        else
        {
            aimTarget.transform.position = ray.direction * 30;
        }
    }
    [ContextMenu("try access chAb")]
    public void ChangeAberration()
    {
        cameraVolume.profile.TryGet(out ChromaticAberration chAb);
        StartCoroutine(ChAb(chAb));
    }
    IEnumerator ChAb(ChromaticAberration _chAb)
    {
        _chAb.active = true;
        while (_chAb.intensity.value < 0.9f)
        {
            _chAb.intensity.value += Time.deltaTime;
            yield return null;
        }
        _chAb.intensity.value = 1f;
    }
    public void ChangeVignette()
    {
        cameraVolume.profile.TryGet(out Vignette chAb);
        StartCoroutine(Vign(chAb));
    }
    IEnumerator Vign(Vignette _chAb)
    {
        _chAb.active = true;
        while (_chAb.intensity.value < 0.48f)
        {
            _chAb.intensity.value += Time.deltaTime;
            yield return null;
        }
        _chAb.intensity.value = 0.5f;
    }
    [ContextMenu("RefreshCamera")]
    void RefreshCameraEdit()
    {
        Vector3 pos = playerBody.position + offset + (-transform.forward * followDistance);
        transform.position = pos;

        transform.rotation = rotation;
    }
}
