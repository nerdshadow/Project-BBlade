using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLights : MonoBehaviour
{
    [ContextMenu("TurnLights")]
    void TurnLights()
    {
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights) 
        {

            light.enabled = !light.enabled;
        }
    }
}
