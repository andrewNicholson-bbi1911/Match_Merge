using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChangebleAbst : MonoBehaviour
{
    [SerializeField]protected Transform _transform;
    public GameCell GameCell { get => _gameCell; }
    [SerializeField]protected GameCell _gameCell;
    internal Vector3 startPos;
    protected Vector3 _startScale;
    public Color startColor;
    protected float _t = 3.4f;
    public bool ignorOut = false;
    

    public abstract void ChangeIn(Vector3 position);
    public abstract void ChangeOut(Vector3 position);
    public abstract void ResetPosition(bool addInfo = false);
    public abstract void DestroyCell();
    public abstract void RepairCell();
}
