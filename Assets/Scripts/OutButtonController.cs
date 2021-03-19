using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutButtonController : MonoBehaviour
{
    [SerializeField] private CircleIndicator _circleIndicator;
    [SerializeField] private Text _needMore;
    [SerializeField] private Button _outButton;
    [SerializeField] private Button _inButton;
    private int _maxValue;
    private int _curValue;

    public void UpdateMaxValue(int maxValue)
    {
        _maxValue = maxValue;
        _circleIndicator.SetMaxValue(_maxValue);
    }

    public void UpdateValue(int value)
    {
        _curValue = value;
        _circleIndicator.UpdateValue(_curValue);
        if (_curValue >= _maxValue)
        {
            maxValueReached();
        }
        else
        {
            maxValueNotReached();
        }
    }

    public void SetMode(bool In)
    {
        if (In)
        {
            _circleIndicator.enableMod = 1;
        }
        else
        {
            _circleIndicator.enableMod = 0;
            _circleIndicator.SetMaxValue(1);
            _circleIndicator.UpdateValue(1);
        }
    }

    private void maxValueReached()
    {
        _needMore.text = "";
        _outButton.interactable = true;
    }

    private void maxValueNotReached()
    {
        _outButton.interactable = false;
        _needMore.text = (_maxValue - _curValue).ToString();
    }

}
