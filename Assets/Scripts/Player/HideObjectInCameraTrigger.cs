using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectInCameraTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.TryGetComponent<MeshRenderer>(out MeshRenderer mshRend))
        {
            mshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;    
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MeshRenderer>(out MeshRenderer mshRend))
        {
            mshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }
}
