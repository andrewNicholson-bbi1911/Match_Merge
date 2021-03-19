using UnityEngine;
using UnityEngine.UI;

public class TranslationStatusController : MonoBehaviour
{
    [SerializeField] private CircleIndicator _circleIndicator;

    public void Awake()
    {
        UpdateMaxValue(GameController.MaxMovesToDestroyedCellTranslations);
        UpdateValue(0);
    }

    public void UpdateMaxValue(int maxValue)
    {
        if (maxValue <= 1)
        {
            gameObject.SetActive(false);
        }
        _circleIndicator.SetMaxValue(maxValue);
    }

    public void UpdateValue(int value)
    {
        _circleIndicator.UpdateValue(value);
    }

    public void SetMode(bool In)
    {
        if (In)
        {
            _circleIndicator.enableMod = 1;
            _circleIndicator.UpdateValue();
        }
        else
        {
            _circleIndicator.enableMod = 0;
            _circleIndicator.UpdateValue();

        }
    }
}
