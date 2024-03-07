using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    public float lineLifetime = 0.1f;
    public void StartTimer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        StartCoroutine(ShootEffect(lineRenderer));
    }
    IEnumerator ShootEffect(LineRenderer line)
    {
        line.enabled = true;
        yield return new WaitForSeconds(lineLifetime);
        line.enabled = false;
        Destroy(gameObject);
    }
}
