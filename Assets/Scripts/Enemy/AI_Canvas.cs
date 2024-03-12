using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Canvas : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    SliderBehaviour visionSlider;
    [SerializeField]
    GameObject deathMarkGO;
    [SerializeField]
    GameObject alertGO;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public void ActivateDeathMark(bool _activate)
    {
        deathMarkGO.SetActive(_activate);
    }
    public void ActivateAlert(bool _activate)
    {
        ActivateVisionSlider(false);
        alertGO.SetActive(_activate);
    }
    public void ActivateVisionSlider(bool _activate)
    {
        visionSlider.gameObject.SetActive(_activate);
    }
    public void ChangeVisionValue(float _value)
    {
        visionSlider.ChangeSliderValues(_value);
    }
}
