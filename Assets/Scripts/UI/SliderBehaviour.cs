using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBehaviour : MonoBehaviour
{
    Slider currentSlider;
    [SerializeField]
    float startValue = 0f;

    private void OnEnable()
    {
        if (currentSlider == null)
            currentSlider = GetComponent<Slider>();
        currentSlider.value = startValue;
    }
    public void ChangeSliderValues(float _currentV)
    {
        if (currentSlider.value != _currentV)
            currentSlider.value = _currentV;
    }
    public void ChangeSliderValues(float _maxV, float _currentV)
    { 
        if(currentSlider.maxValue != _maxV)
            currentSlider.maxValue = _maxV;
        if(currentSlider.value != _currentV)
            currentSlider.value = _currentV;
    }
    public void ChangeSliderValues(float _minV, float _maxV, float _currentV)
    {
        if (currentSlider.minValue != _minV)
            currentSlider.minValue = _minV;
        if (currentSlider.maxValue != _maxV)
            currentSlider.maxValue = _maxV;
        if (currentSlider.value != _currentV)
            currentSlider.value = _currentV;
    }
}
