using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Pool;

public class BloodDecal : MonoBehaviour
{
    [SerializeField]
    List<Material> decalMaterials = new List<Material>();
    ObjectPool<BloodDecal> pool;
    private void OnEnable()
    {
        if (decalMaterials.Count == 1)
        {
            GetComponentInChildren<DecalProjector>().material = decalMaterials[0];
            return;
        }
        int randI = Random.Range(0, decalMaterials.Count);
        GetComponentInChildren<DecalProjector>().material = decalMaterials[randI];
    }
    public void SetPool(ObjectPool<BloodDecal> _pool)
    {
        pool = _pool;
    }
}
