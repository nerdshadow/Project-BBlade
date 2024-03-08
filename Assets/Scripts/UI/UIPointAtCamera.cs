using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPointAtCamera : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    Camera uiCam;
    [SerializeField]
    GameManager gameManager;
    public bool canLook = true;
    public bool beInBounds = true;
    [SerializeField]
    Transform targetTransform;
    public float borderSize = 0;
    private void Start()
    {
        gameManager = GameManager.instance;
        cam = gameManager.playerCameraRef;
        uiCam = cam.GetComponentInChildren<Camera>();
    }
    private void LateUpdate()
    {
        //BeInBounds();
        LookAtCamera();
    }
    void LookAtCamera()
    {
        if (canLook != true)
            return;
        transform.LookAt(cam.transform.position);
        transform.rotation = cam.transform.rotation;
    }
    void BeInBounds()
    {
        bool outOfBounds = false;
        Vector3 targetPositionOnScreenPoint = cam.WorldToScreenPoint(targetTransform.position);
        if (targetPositionOnScreenPoint.x <= borderSize ||
            targetPositionOnScreenPoint.x >= Screen.width - borderSize ||
            targetPositionOnScreenPoint.y <= borderSize ||
            targetPositionOnScreenPoint.y >= Screen.height - borderSize)
        {
            outOfBounds = true;
        }
        Debug.Log("out of screen " + outOfBounds);

        if (outOfBounds == true)
        {
            Vector3 cappedTargetPositionOnScreen = targetPositionOnScreenPoint;
            if (cappedTargetPositionOnScreen.x <= borderSize)
                cappedTargetPositionOnScreen.x = borderSize;
            if (cappedTargetPositionOnScreen.x >= Screen.width - borderSize)
                cappedTargetPositionOnScreen.x = Screen.width - borderSize;
            if (cappedTargetPositionOnScreen.y <= borderSize)
                cappedTargetPositionOnScreen.y = borderSize;
            if (cappedTargetPositionOnScreen.y <= Screen.height - borderSize)
                cappedTargetPositionOnScreen.y = Screen.height - borderSize;

            Vector3 pointerWorldPosition = cam.ScreenToWorldPoint(cappedTargetPositionOnScreen);
            this.transform.position = pointerWorldPosition;
            //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0f);
        }

    }
}
