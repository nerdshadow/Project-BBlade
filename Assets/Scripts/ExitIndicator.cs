using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitIndicator : MonoBehaviour
{
    private void Awake()
    {
        gameObject.GetComponentInChildren<Renderer>().enabled = false;
    }
    private void Start()
    {
        GameManager.OpenExit.AddListener(ActicateIdicator);
    }
    private void FixedUpdate()
    {
        Rotate();
    }
    void Rotate()
    {
        GameObject _aimTarget = FindObjectOfType<LevelChanger>().gameObject;
        Vector3 toTarget = _aimTarget.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(toTarget);
        transform.rotation = rotation;
    }
    void ActicateIdicator()
    {
        gameObject.GetComponentInChildren<Renderer>().enabled = true;
    }
}
