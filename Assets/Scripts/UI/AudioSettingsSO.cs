using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudiSettingSO", menuName = "SO/AudiSettingSO", order = 1)]
public class AudioSettingsSO : ScriptableObject
{
    [Range(0.0001f, 1.0f)]
    public float masterVolume = 1.0f;
    [Range(0.0001f, 1.0f)]
    public float musicVolume = 1.0f;
    [Range(0.0001f, 1.0f)]
    public float sfxVolume = 1.0f;
}
