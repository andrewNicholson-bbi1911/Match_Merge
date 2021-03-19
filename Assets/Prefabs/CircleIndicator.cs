using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CircleIndicator : MonoBehaviour
{
    [SerializeField] private Image _healthPartObject;
    [SerializeField] private Transform _healthIndicatorRoot;
    [SerializeField] private float _angleBetweenParts = 20;
    [SerializeField] private bool _controllerIsonUIElement = false;
    private int _maxValue;
    private int _curValue;

    private List<Image> _partsImgCompList;
    [Header("Colors")]
    [SerializeField] private List<Color> _enabledColors;
    public int enableMod = 1;
    [SerializeField] private Color _disabledColor;

    public void SetMaxValue(int maxValue)
    {
        DeliteParts();

        _maxValue = maxValue;
        _partsImgCompList = new List<Image>();

        var delta = 360.0f / _maxValue;
        var activePart = ((float)(delta - _angleBetweenParts)) / 360.0f;
        for (int i = 0; i < _maxValue; i++)
        {
            var part = Instantiate(_healthPartObject, _healthIndicatorRoot);
            part.enabled = true;
            part.color = _disabledColor;
            part.rectTransform.localEulerAngles = Vector3.forward * delta * i;
            part.fillAmount = activePart;
            _partsImgCompList.Add(part);
        }

    }

    private void DeliteParts()
    {
        var delitable = _healthIndicatorRoot.GetComponentsInChildren<Image>();
        for (int i = delitable.Length - 1; i >= 0; i--)
        {
            if (_controllerIsonUIElement)
            {
                if (delitable[i] == GetComponent<Image>())
                {
                    continue;
                }
            }
            try
            {
                Destroy(delitable[i].gameObject);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                DestroyImmediate(delitable[i].gameObject);
            }
        }
    }


    public void UpdateValue() => UpdateValue(_curValue);

    public void UpdateValue(int newValue)
    {
        _curValue = newValue;
        for (int i = 0; i < _maxValue; i++)
        {
            if (i < _curValue)
            {
                _partsImgCompList[i].color = _enabledColors[enableMod];
            }
            else
            {
                _partsImgCompList[i].color = _disabledColor;
            }
        }
    }
}
