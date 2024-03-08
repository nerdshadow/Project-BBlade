using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBehaviour : MonoBehaviour
{
    [SerializeField]
    TMP_Text textMeshPro;
    int currentScore = 0;
    string currentText = "Score: ";
    void Start()
    {
        if(textMeshPro == null)
            textMeshPro = GetComponent<TMP_Text>();
        ChangeScore(0);
    }

    public void ChangeText(string _text)
    {
        currentText = _text;
        ChangeScore(currentScore);
    }
    public void ChangeScore(int _score)
    {
        textMeshPro.text = "Score: " + _score;
    }
    public void AddScore(int _score)
    {
        currentScore += _score;
        ChangeScore(currentScore);
    }
}
