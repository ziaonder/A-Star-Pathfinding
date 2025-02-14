using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    private Slider slider;
    public bool isWidth;
    public TextMeshProUGUI text;
    public static event Action OnUpdateData;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { UpdateData(); });
    }

    private void UpdateData()
    {
        if(isWidth)
            Main.width = (int)slider.value;
        else
            Main.height = (int)slider.value;

        text.text = slider.value.ToString();
        OnUpdateData?.Invoke();
    }
}
