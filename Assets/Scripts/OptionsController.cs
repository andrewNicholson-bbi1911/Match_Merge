using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsController : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    private bool _settingsOpened = false;
    private float _maxHeight = 310;
    private float _openTime = 0.05f;

    private void Awake()
    {
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    public void ShowSettings()
    {
        _settingsOpened = !_settingsOpened;
        StopAllCoroutines();
        if (_settingsOpened)
        {
            StartCoroutine(OpenSettings());
        }
        else
        {
            StartCoroutine(CloseSettings());
        }
    }

    private IEnumerator OpenSettings()
    {
        float spentTime = 0f;
        while (_rectTransform.rect.height <= _maxHeight)
        {
            yield return null;
            spentTime += Time.deltaTime;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _maxHeight * spentTime / _openTime);

        }
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _maxHeight);

    }

    private IEnumerator CloseSettings()
    {
        float spentTime = 0f;
        while (_rectTransform.rect.height > 0)
        {
            yield return null;
            spentTime += Time.deltaTime;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _maxHeight * (1 - spentTime / _openTime));

        }
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

    }
}
