using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Stats")]
public class BaseStats : ScriptableObject
{
    public string npcName = "NPC_Name";
    public float basicAtkRange = 1;
    public float basicAtkRachargeTime = 1;
    public float basicDetectDistance = 1;
    public float basicDetectAngle = 1;
    public float basicDetectTime = 1;
    public float basicSpeed = 1;
    public float basicRotationSpeed = 1;
}