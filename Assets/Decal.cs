using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Decal : MonoBehaviour
{
    [SerializeField]
    List<Material> decalMaterials = new List<Material>();
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
}
