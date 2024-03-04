using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRandSlider : MonoBehaviour
{
    Slider slider;
    private void Start()
    {
        slider = GetComponent<Slider>();
    }
    // Update is called once per frame
    void Update()
    {
        if(slider.value <=0.95)
            slider.value += 0.01f * Time.deltaTime;
        else
            slider.value = 0.0f;
    }
}
