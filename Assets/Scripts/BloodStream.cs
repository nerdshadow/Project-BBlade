using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodStream : MonoBehaviour
{
    ParticleSystem particleS;
    public List<ParticleCollisionEvent> collisions;
    [SerializeField]
    GameObject bloodDecal;
    private void Start()
    {
        particleS = GetComponent<ParticleSystem>();
        collisions = new List<ParticleCollisionEvent>();
    }
    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = particleS.GetCollisionEvents(other, collisions);

        int i = 0;

        while (i < numCollisionEvents)
        {
            Vector3 pos = collisions[i].intersection;
            Quaternion rot = Quaternion.LookRotation(collisions[i].normal);

            //Spawn Decals
            if (bloodDecal != null)
            {
                GameObject curDecal = Instantiate(bloodDecal, pos, rot);
                Vector3 rotation = curDecal.transform.eulerAngles;
                rotation.z = Random.Range(0, 361);
                curDecal.transform.eulerAngles = rotation;
            }
            i++;
        }
    }
}
