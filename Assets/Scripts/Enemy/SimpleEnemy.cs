using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    Animator animator;
    Collider coll;
    Rigidbody rb;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();    
        coll = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }
    public GameObject _aimTarget;
    private void Update()
    {
        Vector3 toTarget = _aimTarget.transform.position - transform.position;
        toTarget.y = 0;
        Quaternion rotation = Quaternion.LookRotation(toTarget);
        transform.rotation = rotation;
    }
    [ContextMenu("Die")]
    public void Die()
    {
        animator.CrossFade("Death", 0.05f);
        rb.useGravity = false;
        rb.isKinematic = true;
        coll.enabled = false;
    }
}