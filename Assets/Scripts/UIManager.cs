using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI widthText, heightText, widthCount, heightCount;
    public Slider widthSlider, heightSlider;

    public void OnMatrixChanged()
    {
        widthText.text = widthSlider.value.ToString();
        heightText.text = heightSlider.value.ToString();

        widthCount.text = "";
        heightCount.text = "";
        for(int i = 0; i < widthSlider.value; i++)
        {
            widthCount.text += $"{i} \n";
        }

        for(int j = 0; j < heightSlider.value; j++)
        {
            heightCount.text += $"{j} " ;
        }
    }
}
