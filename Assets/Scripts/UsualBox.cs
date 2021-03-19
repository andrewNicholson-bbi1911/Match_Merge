using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsualBox : ChangebleAbst
{
    private float senceInRadius = 2 * 1.2f;
    private float senceOutRadius = 3 * 1.3f;

    [SerializeField]private SpriteRenderer _spriteRenderer;

    private Vector3 _newTargetForward;
    [SerializeField] private Vector3 _moveTargetPosition;
    private Vector3 _scaleTargetPosition;
    private Color _colorTargetPosition;

    internal void Start()
    {
        startPos = _transform.position;
        _startScale = _transform.localScale;
        ResetPosition();
    }

    private bool _destroyed = false;

    public override void ChangeIn(Vector3 position)
    {

        var dist = Vector3.Distance(position, startPos);

        if (dist < senceInRadius * 3)
        {
            var interpolCoef = (Mathf.Cos(dist / (senceInRadius * 3) * Mathf.PI) + 1) * 0.5f;

            _newTargetForward = Vector3.Lerp(Vector3.forward, (position - startPos).normalized, interpolCoef);
        }
        else
        {
            _newTargetForward = Vector3.forward;
        }

        if (dist < senceInRadius)
        {
            var interpolCoef = (Mathf.Cos(dist / senceInRadius * Mathf.PI ) + 1) * 0.7f;
            if(interpolCoef > 0.65f)
            {
                interpolCoef = 0.65f;
            }

            _moveTargetPosition = Vector3.Lerp(startPos, position, interpolCoef);
            _scaleTargetPosition = _startScale * (1 - interpolCoef);
            _colorTargetPosition = Color.Lerp(startColor, Color.black, interpolCoef);

        }
        else
        {
            ResetPosition();
        }
    }

    public bool swapped = false;

    public override void ChangeOut(Vector3 position)
    {
        if (_destroyed)
            return;
        if (ignorOut)
        {
            ResetPosition();
            if (_gameCell.IsSelected)
            {
                _moveTargetPosition = position + Vector3.up;
                _transform.position = position + Vector3.up;
            }
            else
            {
                var distance = Vector3.Distance(position, startPos);
                if (distance <= 0.65f & !swapped)
                {
                    GameBoard.SwapSelectedCell(GameCell);
                    swapped = true;
                }
                else if(distance > 0.65f)
                {
                    swapped = false;
                }
            }
            return;
        }
        var backInterpolate = Time.deltaTime * _t * 5;
        var dist = Vector3.Distance(position, startPos);
        
        if (dist < senceOutRadius * 3)
        {
            _newTargetForward = (position - startPos).normalized;
        }
        else
        {
            _newTargetForward = Vector3.forward;
        }

        if (dist < senceOutRadius-0.4f)
        {
            var interpolCoef = 0.6f * (Mathf.Cos((dist) / (senceOutRadius) * Mathf.PI * 0.5f ));
           
            _moveTargetPosition = Vector3.Lerp(position - _newTargetForward * (senceOutRadius - 0.7f) , startPos, interpolCoef);
            _scaleTargetPosition = _startScale * (0.4f + interpolCoef * 0.6f );
            _colorTargetPosition = Color.Lerp(startColor, Color.black, interpolCoef);

        }
        else
        {
            ResetPosition();
        }
        
    }

    public override void ResetPosition(bool resetForward = false)
    {
        if (resetForward)
        {
            _newTargetForward = Vector3.forward;
        }
        _moveTargetPosition = startPos;
        _scaleTargetPosition = _startScale;
        _colorTargetPosition = startColor;
    }

    public override void DestroyCell()
    {
        startColor = new Color(0, 0, 0, 0);
        ResetPosition();
        _spriteRenderer.color = startColor;
        _destroyed = true;
    }

    public override void RepairCell()
    {
        _destroyed = false;
        ResetPosition();
        _spriteRenderer.color = startColor;
    }

    private void Update()
    {
        var delta = _t * Time.deltaTime;
        //_transform.forward = Vector3.Lerp(_transform.forward, _newTargetForward, delta);
        transform.localScale = Vector3.Lerp(_transform.localScale, _scaleTargetPosition, delta);
        transform.position = Vector3.Lerp(_transform.position, _moveTargetPosition, delta);
        if (!_destroyed) _spriteRenderer.color = Color.Lerp(_colorTargetPosition, startColor, delta);
    }


}
