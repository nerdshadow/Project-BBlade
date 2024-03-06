using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPointAtCamera : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject testLook;
    private void Start()
    {
        gameManager = GameManager.instance;
        cam = gameManager.playerCameraRef;
    }
    private void LateUpdate()
    {
        //transform.LookAt(testLook.transform.position);
        transform.LookAt(cam.transform.position);
        //transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
