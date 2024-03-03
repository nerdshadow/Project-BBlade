using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Pool;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class BloodStreamParticle : MonoBehaviour
{
    ParticleSystem particleS;
    public List<ParticleCollisionEvent> collisions;
    [SerializeField]
    ObjectPoolManager objectPoolManager = ObjectPoolManager.instance;
    ObjectPool<BloodStreamParticle> bloodStreamParticlePool;
    private void Awake()
    {
        particleS = GetComponent<ParticleSystem>();
        collisions = new List<ParticleCollisionEvent>();
    }
    private void OnEnable()
    {
        objectPoolManager = ObjectPoolManager.instance;
    }
    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = particleS.GetCollisionEvents(other, collisions);

        int i = 0;

        while (i < numCollisionEvents)
        {
            //if (other.GetComponentInChildren<SimpleEnemy>() || other.GetComponent<PlayerMovement>())
            //    continue;
            Vector3 pos = collisions[i].intersection;
            Quaternion rot = Quaternion.LookRotation(collisions[i].normal);

            //Spawn Decals
            if (objectPoolManager != null)
            {
                BloodDecal bloodDecal = objectPoolManager.bloodDecalPool.Get();
                bloodDecal.transform.position = pos;
                bloodDecal.transform.rotation = rot;
                Vector3 rotation = bloodDecal.transform.eulerAngles;
                rotation.z = Random.Range(0, 361);
                bloodDecal.transform.eulerAngles = rotation;
                bloodDecal.transform.SetParent(other.transform);
            }
            i++;
        }
        collisions.Clear();
    }
    public void PlayAndTryReturnToPool()
    {
        particleS.Play();
        Invoke("ReturnToPool", 1.1f);
    }
    void ReturnToPool()
    {
        bloodStreamParticlePool.Release(this);
    }
    public void SetPool(ObjectPool<BloodStreamParticle> _pool)
    {
        bloodStreamParticlePool = _pool;
    }
}
