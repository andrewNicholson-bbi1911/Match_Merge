using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastMagic : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private List<ChangebleAbst> _blocks;

    [SerializeField] private Image _activatingPanel;
    private BoxCollider _collider;

    bool reset = true;
    

    private void Start()
    {
        _blocks = new List<ChangebleAbst>();
        foreach (GameCell cell in GameBoard.GameCells)
        {
            _blocks.Add(cell.ChangebleAbst);
        }
    }

    private bool _gameFieldActivated = false;
    public void Update()
    {
        if (!_gameFieldActivated)
            return;
        if (Input.GetMouseButton(0) | Input.touchCount > 0) // if screen touched
        {
            Cast();
        }

    }

    public static bool casting = false;

    public void Cast() {
        reset = false;
        RaycastHit hit;
        Ray ray;
        if (Input.touchCount > 0)
        {
            ray = _camera.ScreenPointToRay(Input.GetTouch(0).position);
        }
        else
        {
            ray = _camera.ScreenPointToRay(Input.mousePosition);
        }

        var hitPoint = ray.origin - ray.direction * (ray.origin.y / ray.direction.y);

        //animationPartOfChanges;
        if (GameController.InCast == true)
        {
            CastIn(hitPoint);
        }
        else if (GameController.InCast == false)
        {
            CastOut(hitPoint);
        }

        casting = true;
    }

    public void EndCast()
    {
        if (reset)
            return;
        casting = false;
        foreach (ChangebleAbst cell in _blocks)
        {
            cell.ResetPosition();
        }
        reset = true;
        GameBoard.RemoveSelectedCell(true);


        if (GameController.InCast == false)
        {
            foreach (GameBoardOutMoveStruct gBOMove in GameBoard.GameBoardOutMoves)
            {
                GameController.Move(gBOMove);
            }
        }

        GameController.lastChangedCoords = new int[2] { -1, -1 };
        ActivateUIElement(false);
    }



    public void CastIn(Vector3 castPosition)
    {
        foreach (ChangebleAbst cell in _blocks)
        {
            cell.ChangeIn(castPosition);
        }
    }

    public void CastOut(Vector3 castPosition)
    {
        foreach (ChangebleAbst cell in _blocks)
        {
            cell.ChangeOut(castPosition);
        }
    }
    
    public void ActivateUIElement(bool activate = true)
    {
        _gameFieldActivated = activate;
    }

}
