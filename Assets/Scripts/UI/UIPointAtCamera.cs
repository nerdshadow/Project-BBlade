using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPointAtCamera : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    GameManager gameManager;
    public bool canLook = true;
    private void Start()
    {
        gameManager = GameManager.instance;
        cam = gameManager.playerCameraRef;
    }
    private void LateUpdate()
    {
        LookAtCamera();
    }
    void LookAtCamera()
    {
        if (canLook != true)
            return;
        transform.LookAt(cam.transform.position);
    }
}
