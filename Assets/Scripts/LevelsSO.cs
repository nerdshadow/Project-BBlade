using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "SO/LevelSO", order = 1)]
public class LevelsSO : ScriptableObject
{
    public SceneField level;
    public bool isUnlocked = false;
}
