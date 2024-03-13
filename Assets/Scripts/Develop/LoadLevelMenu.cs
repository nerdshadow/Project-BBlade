using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadLevelMenu : MonoBehaviour
{
    public List<LevelsSO> levels;
    public TMP_Text textBox;
    public void TryLoadLevel(int levelN)
    {
        if(levels.Count == 0)
            return;
        if (levels.Count > levelN &&
            levels[levelN].isUnlocked == true)
        {
            GameManager.instance.LoadLevel(levels[levelN].level);  
        }
        else 
        {
            textBox.text = "Level " + levelN + " is locked";
        }
    }
}
