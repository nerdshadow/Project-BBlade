using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDash : MonoBehaviour
{
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    [ContextMenu("Dash")]
    public void Dash()
    {
        Vector3 newPos = (transform.forward - transform.position).normalized;
        Debug.Log(newPos);
        transform.position = transform.position + newPos * 1;
    }
}
