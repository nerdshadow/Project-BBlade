using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, IKillable
{
    [SerializeField]
    Rigidbody rigidBody;
    [SerializeField]
    GameObject fracturedPrefab;
    [SerializeField]
    float explosiveForce = 1000f;
    [SerializeField]
    float explosiveRad = 2f;
    [SerializeField]
    float pieceFadeSpeed = 0.25f;
    [SerializeField]
    float pieceDestroyDelay = 5f;
    [SerializeField]
    float pieceSleepCheckDelay = 0.5f;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
    [ContextMenu("Destroy")]
    public void Die()
    {
        Explode();
    }
    void Explode()
    {
        if(rigidBody != null)
            Destroy(rigidBody);
        if(TryGetComponent<Collider>(out Collider collider))
            collider.enabled = false;
        if(TryGetComponent<Renderer>(out Renderer renderer))
            renderer.enabled = false;
        GameObject brokenInstance = Instantiate(fracturedPrefab, transform.position, transform.rotation);
        Rigidbody[] rigBodies = brokenInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rbody in rigBodies)
        {
            rbody.AddExplosionForce(explosiveForce, transform.position, explosiveRad);
        }
        StartCoroutine(FadeOutRigidbodies(rigBodies));
    }
    IEnumerator FadeOutRigidbodies(Rigidbody[] _rigBodies)
    {
        WaitForSeconds wait = new WaitForSeconds(pieceSleepCheckDelay);
        int activeRigBodies = _rigBodies.Length;

        while (activeRigBodies > 0)
        {
            yield return wait;
            foreach (Rigidbody rBody in _rigBodies)
            {
                if (rBody.IsSleeping())
                {
                    activeRigBodies--;
                }
            }

        }
        yield return new WaitForSeconds(pieceDestroyDelay);
        float time = 0;

        Renderer[] renders = Array.ConvertAll(_rigBodies, GetRendererFromRigBody);

        foreach (Rigidbody rBody in _rigBodies)
        {
            Destroy(rBody.GetComponent<Collider>());
            Destroy(rBody);
        }
        while(time < 1)
        {
            float step = Time.deltaTime * pieceFadeSpeed;
            foreach (Renderer renderer in renders)
            {
                renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
            }
            time += step;
            yield return null;
        }

        foreach (Renderer renderer in renders)
        {
            Destroy(renderer.gameObject);
        }

        Destroy(gameObject);
    }

    Renderer GetRendererFromRigBody(Rigidbody _rigBody)
    {
            return _rigBody.GetComponent<Renderer>();

    }
}
