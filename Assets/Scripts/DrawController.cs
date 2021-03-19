using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawController : MonoBehaviour
{
    [SerializeField] private TextMesh _text;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private ChangebleAbst _changebleAbst;

    private bool valSet = false; 

    private void Awake()
    {
        if (!valSet) SetValue(0);
    }

    private void SetText(string text)
    {
        _text.text = text;
        if(text.Length >= 3)
        {
            _text.characterSize = 0.2f / Mathf.Log(text.Length, 2);
        }
    }


    private void SetText(int intVal) => SetText(intVal.ToString());

    private void SetHole()
    {
        _changebleAbst.DestroyCell();
        SetText("");
    }

    public void SetValue(int power)
    {
        if (power == -127)
        {
            SetHole();
            return;
        }
        if (power < 0) power = 0;
        var num = Mathf.CeilToInt(Mathf.Pow(2, power));
        _changebleAbst.startColor = _gradient.Evaluate(Mathf.Clamp((float)power / 11f, 0, 1));
        _changebleAbst.RepairCell();
        if (num > 1000000)
        {
            num /= 1000000;
            SetText(num.ToString() + "M");
        }else if(num > 10000){

            num /= 1000;
            SetText(num.ToString() + "k");
        }
        else
        {
            SetText(num);
        }
        _changebleAbst.ResetPosition();
        valSet = true;
    }
}
