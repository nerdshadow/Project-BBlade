using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFinalDoor : MonoBehaviour
{
    [SerializeField]
    GameObject door;
    void Start()
    {
        GameManager.OpenExit.AddListener(OpenDoor);
    }

    void OpenDoor()
    {
        StartCoroutine(RotateDoor(door, 2f));
    }
    IEnumerator RotateDoor(GameObject door, float time)
    {
        float timePassed = 0f;
        float yRotation = 200f;
        Quaternion intRot = door.transform.rotation;
        Quaternion desRotation = Quaternion.Euler(door.transform.rotation.x,
                yRotation,
                door.transform.rotation.z);
        while (timePassed < time)
        {
            door.transform.rotation = Quaternion.Lerp(intRot, desRotation, timePassed/time);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }

}
