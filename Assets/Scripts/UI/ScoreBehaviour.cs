using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBehaviour : MonoBehaviour
{
    TextMeshPro textMeshPro;
    int currentScore = 0;
    string currentText = "Score: ";
    void Start()
    {
        if(textMeshPro == null)
            textMeshPro = GetComponent<TextMeshPro>();
        ChangeScore(0);
    }

    public void ChangeText(string _text)
    {
        currentText = _text;
        ChangeScore(currentScore);
    }
    public void ChangeScore(int _score)
    {
        textMeshPro.text = currentText + _score;
    }
    public void AddScore(int _score)
    {
        currentScore += _score;
        ChangeScore(currentScore);
    }
}
