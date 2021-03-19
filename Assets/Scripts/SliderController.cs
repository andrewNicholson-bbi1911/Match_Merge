using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private int[] _intervalOvValues = new int[2] { 10, 16 };
    private int curMaxVal = 10;
    [Header("Bonus Part")]
    [SerializeField] private Button _modeButton;
    [SerializeField] private Text _infoText;

    public void IncreaceValue()
    {
        slider.value += 1;
    }

    public void ResetValue()
    {
        UpdateSliderMaxValue();
        slider.value = 0;
    }

    private void UpdateSliderMaxValue()
    {
        curMaxVal = Random.Range(_intervalOvValues[0], _intervalOvValues[1] + 1);
        slider.maxValue = curMaxVal;
    }

    public void OnValueChange()
    {
        _infoText.text = $"{slider.value}/curMaxVal";
        if (slider.value >= curMaxVal)
        {
            _modeButton.interactable = true;
        }
        else
        {
            _modeButton.interactable = false;
        }
    }
}
