using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TestVisualEffect : MonoBehaviour
{
    VisualEffect testvfx;
    void Start()
    {
        testvfx = GetComponent<VisualEffect>();
    }
    [ContextMenu("Play VFX")]
    void PlayVFX()
    {
        testvfx.Play();
    }
}
