using System.Collections;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField]
    TrailRenderer trailRenderer;
    public float trailLifetime = 0.1f;
    public void StartMovingTo(Vector3 transform)
    {
        trailRenderer = GetComponent<TrailRenderer>();
        StartCoroutine(ShootEffect(trailRenderer, transform));
    }
    IEnumerator ShootEffect(TrailRenderer _trail, Vector3 _position)
    {
        _trail.enabled = true;
        float time = 0;
        Vector3 initPos = transform.position;
        while (time < trailLifetime)
        {
            transform.position = Vector3.Lerp(initPos, _position, time / trailLifetime);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = _position;
        yield return new WaitForFixedUpdate();
        _trail.enabled = false;
        Destroy(gameObject);
    }
}
