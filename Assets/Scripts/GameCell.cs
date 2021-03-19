using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCell : MonoBehaviour
{
    [SerializeField] private DrawController _drawController;
    [SerializeField] internal ChangebleAbst ChangebleAbst;



    public int[] Coordinates { get => _xy; private set { _xy = value; } }
    public int X { get => Coordinates[0]; }
    public int Y { get => Coordinates[1]; }
    [SerializeField] private int[] _xy;


    public int Value { get => _pow; }
    private int _pow = 0;
    private bool _valueChanged = false;

    public bool IsSelected { get => _isSelected; }
    [SerializeField] private bool _isSelected = false;

    private List<GameCell> _neighbourCells = new List<GameCell>();


    private bool Pressed
    {
        get => _pressed;
        set
        {
            _pressed = value;
            if (value == false)
            {
                _valueChanged = false;
            }
        }
    }
    private bool _pressed = false;



    internal void SetCoords(int x, int y)
    {
        _xy = new int[2] { x, y };
    }
    internal void SetNeighbours(List<GameCell> neighbours) => _neighbourCells = neighbours;


    public void SetSelected(bool isSelected)
    {
        Pressed = isSelected;
        _isSelected = isSelected;
        ChangebleAbst.ignorOut = isSelected;

        foreach (GameCell cell in _neighbourCells)
        {
            cell.ChangebleAbst.ignorOut = isSelected;
        }
    }

    internal void DestroyCell()
    {
        _pow = -127;
        _drawController.SetValue(_pow);
        GetComponent<BoxCollider>().enabled = false;
    }
    internal void RepairCell()
    {
        GetComponent<BoxCollider>().enabled = true;

    }

    internal void Decrease(bool destroed = false)
    {
        if (destroed)
        {
            DestroyCell();
            return;
        }
        _pow--;
        if (_pow <= 0)
            _pow = 0;

        _drawController.SetValue(_pow);
    }

    internal void Increase(bool destroyed = false)
    {
        if (destroyed)
            RepairCell();
        _pow++;
        _drawController.SetValue(_pow);
    }

    private void OnMouseOver()
    {
        if (CastMagic.casting & !_pressed)
        {
            GameBoard.SetSelectedCell(this);

            if (GameController.InCast == true && !(GameController.lastChangedCoords[0]==X && GameController.lastChangedCoords[1] == Y))
            {
                GameController.ChangeCellIn(this);
            }
        }
    }

    private void OnMouseExit() => Pressed = false;

    private void OnMouseUp() => Pressed = false;

}
