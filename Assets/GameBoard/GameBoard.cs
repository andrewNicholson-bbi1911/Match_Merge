using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GameBoard : MonoBehaviour
{
    [Header("Board")]

    [SerializeField] private int _length;
    private static int _statLength;
    [SerializeField] private int _width;
    private static int _statWidth;
    public static int Len { get => _statLength; }
    public static int Wid { get => _statWidth; }
    private Vector2 _lastSize;
    [Space]
    [SerializeField] private float _cellSize = 1;
    [SerializeField] private float _cellSpace = 0;
    [Space]
    [SerializeField] private GameObject _cellObject;

    public static GameCell[,] GameCells
    {
        get
        {
            return _statCells;
        }
    }
    private static GameCell[,] _statCells;

    public GameController GameController { get => _gameController; }
    private GameController _gameController;

    private static GameCell _selectedCell = null;

    private void Awake()
    {
        InitializeGameBoard();
    }

    #region Sellecting cell
    public static void SetSelectedCell(GameCell newSelectedCell)
    {
        RemoveSelectedCell();
        _selectedCell = newSelectedCell;
        _selectedCell.SetSelected(true);

    }

    public static void RemoveSelectedCell(bool all = false)
    {
        if (_selectedCell == null)
            return;
        _selectedCell.SetSelected(false);
        _selectedCell = null;
        /*
        if (all)
        {
            foreach(GameCell cell in GameCells)
            {
                cell.SetSelected(false);
            } 
        }*/
    }
    #endregion

    #region Building Board Methods
    public void InitializeGameBoard(bool inGame = false, int w = 5, int h = 6)
    {
        DestroyCells();

        if (inGame)
        {
            _length = Mathf.Max(4,h);
            _width = Mathf.Max(4,h);
        }

        _statLength = _length;
        _statWidth = _width;

        _statCells = new GameCell[_statLength, _statWidth];
        var zeroCell = new Vector3(-(_cellSize * (_length - 1) + _cellSpace * (_length - 1)) / 2
            , 0
            , -(_cellSize * (_width - 1) + _cellSpace * (_width - 1)) / 2);

        for (int i = 0; i < _length; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                CreateCell(i, j, zeroCell);
            }
        }

        foreach (GameCell cell in GameCells)
        {
            cell.SetNeighbours(GetCellNeighbours(cell.Coordinates));
        }
    }
    

    private void CreateCell(int x, int y, Vector3 zeroCell)
    {
        var position = zeroCell + Vector3.right * (_cellSize + _cellSpace) * x + Vector3.forward * (_cellSize + _cellSpace) * y;
        var newCell = Instantiate(_cellObject, transform).GetComponent<GameCell>();
        newCell.transform.localPosition = position;
        newCell.name = $"cell [{x}, {y}]";
        newCell.SetCoords(x, y);
        _statCells[x, y] = newCell;
    }

    public void UpdateGameBoard()
    {
        DestroyCells(false);

        _statCells = CopyCells(_statCells, new GameCell[_length, _width]);


        var zeroCell = new Vector3(-(_cellSize * (_length - 1) + _cellSpace * (_length - 1)) / 2
            , 0
            , -(_cellSize * (_width - 1) + _cellSpace * (_width - 1)) / 2);

        for (int i = 0; i < _length; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                var position = zeroCell + Vector3.right * (_cellSize + _cellSpace) * i + Vector3.forward * (_cellSize + _cellSpace) * j;
                try
                {
                    var updatedCell = GameCells[i, j];
                    updatedCell.transform.localPosition = position;
                }
                catch
                {
                    CreateCell(i, j, zeroCell);
                }

            }
        }

        _statCells = new GameCell[_length, _width];
    }

    public void DestroyCells(bool totalDestroy = true)
    {
        _statLength = _length;
        _statWidth = _width;

        try
        {
            var cells_ = GetComponentsInChildren<GameCell>();
            if (!totalDestroy)
            {

            }
            for (int i = cells_.Length - 1; i >= 0; i--)
            {
                if (cells_[i].Coordinates[0] >= _length | cells_[i].Coordinates[1] >= _width | totalDestroy)
                    DestroyImmediate(cells_[i].gameObject);

            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private static GameCell[,] CopyCells(GameCell[,] sourceCells, GameCell[,] destinationCells)
    {
        for (int i = 0; i < Math.Min(sourceCells.GetLength(0), destinationCells.GetLength(0)); i++)
        {
            for (int j = 0; j < Math.Min(sourceCells.GetLength(1), destinationCells.GetLength(1)); j++)
            {
                destinationCells[i, j] = sourceCells[i, j];
            }
        }
        return destinationCells;
    }
    #endregion

    internal static List<GameBoardOutMoveStruct> GameBoardOutMoves
    {
        get
        {
            var returning = _gameBoardOutMoveStructs;
            _gameBoardOutMoveStructs = new List<GameBoardOutMoveStruct>();
            return returning;
        }
    }

    private static List<GameBoardOutMoveStruct> _gameBoardOutMoveStructs = new List<GameBoardOutMoveStruct>();
    public static void SwapSelectedCell(GameCell swappingCell) => SwapCells(_selectedCell, swappingCell);
    public static void SwapCells(GameCell newCell, GameCell oldCell)
    {
        if (newCell.Value == -127 || oldCell.Value == -127)
            return;

        ChangeCells(newCell, oldCell);
    }


    internal static void MoveDestroyedCell(int index, int neighbourIndex)
    {
        GameCell destroyedCell = null;
        foreach(GameCell cell in GameCells)
        {
            if (cell.Value != -127)
                continue;
            if (index <= 0)
            {
                destroyedCell = cell;
                break;
            }
            else
            {
                index--;
            }
        }

        if (index > 0)
        {
            MoveDestroyedCell(index, neighbourIndex);
            return;
        }

        if (destroyedCell == null)
            return;
        var changingNeighbour = GetCellNeighbours(destroyedCell.Coordinates)[neighbourIndex % GetCellNeighbours(destroyedCell.Coordinates).Count];
        if (changingNeighbour.Value != -127)
        {
            ChangeCells(destroyedCell, changingNeighbour, true);
        }
        else
        {
            MoveDestroyedCell(index + 1, neighbourIndex + 1);
        }
    }

    private static void ChangeCells(GameCell newCell, GameCell oldCell, bool destroyed = false)
    {

        bool haveSelectedCell = true;
        var selectedCell = newCell;
        if (!newCell.IsSelected)
        {
            selectedCell = oldCell;
            if (!oldCell.IsSelected)
            {
                haveSelectedCell = false;
            }
        }
        RemoveSelectedCell();

        if (_gameBoardOutMoveStructs.Count > 0
            && (
            ((newCell.X == _gameBoardOutMoveStructs[_gameBoardOutMoveStructs.Count - 1].oldCellCoords[0] && newCell.Y == _gameBoardOutMoveStructs[_gameBoardOutMoveStructs.Count - 1].oldCellCoords[1])
            && (oldCell.X == _gameBoardOutMoveStructs[_gameBoardOutMoveStructs.Count - 1].newCellCoords[0] && oldCell.Y == _gameBoardOutMoveStructs[_gameBoardOutMoveStructs.Count - 1].newCellCoords[1]))
            ||
            ((oldCell.X == _gameBoardOutMoveStructs[_gameBoardOutMoveStructs.Count - 1].oldCellCoords[0] && oldCell.Y == _gameBoardOutMoveStructs[_gameBoardOutMoveStructs.Count - 1].oldCellCoords[1])
            && (newCell.X == _gameBoardOutMoveStructs[_gameBoardOutMoveStructs.Count - 1].newCellCoords[0] && newCell.Y == _gameBoardOutMoveStructs[_gameBoardOutMoveStructs.Count - 1].newCellCoords[1]))
            ))
        {
            _gameBoardOutMoveStructs.RemoveAt(_gameBoardOutMoveStructs.Count - 1);
        }
        else
        {
            _gameBoardOutMoveStructs.Add(new GameBoardOutMoveStruct(newCell, oldCell));
        }


        //changing in GameCells
        var oldCoords = oldCell.Coordinates;
        _statCells[oldCoords[0], oldCoords[1]] = newCell;
        _statCells[newCell.X, newCell.Y] = oldCell;
        oldCell.SetCoords(newCell.X, newCell.Y);
        newCell.SetCoords(oldCoords[0], oldCoords[1]);
        //changing _startPoint
        var oldStartPoint = oldCell.ChangebleAbst.startPos;
        oldCell.ChangebleAbst.startPos = newCell.ChangebleAbst.startPos;
        newCell.ChangebleAbst.startPos = oldStartPoint;

        //changing neighbours
        UpdateNeighbours();

        if (haveSelectedCell)
        {
            SetSelectedCell(selectedCell);
        }
        if (destroyed)
        {
            foreach(GameBoardOutMoveStruct outMoveStruct in GameBoardOutMoves)
            {
                GameController.Move(outMoveStruct, true);
            }
            oldCell.ChangebleAbst.ResetPosition();
            newCell.ChangebleAbst.ResetPosition();

        }
    }

    #region Helper Methods
    public static List<GameCell> GetCellNeighbours(int[] cellCoords)
    {
        //Debug.Log(cellCoords[0] + " " + _length + " " + cellCoords[1] + " " + _width + " " + BoardCells.Length);
        var neighbours = new List<GameCell>();
        if (cellCoords[0] > 0) neighbours.Add(_statCells[cellCoords[0] - 1, cellCoords[1]]);
        if (cellCoords[0] < _statLength - 1) neighbours.Add(_statCells[cellCoords[0] + 1, cellCoords[1]]);
        if (cellCoords[1] > 0) neighbours.Add(_statCells[cellCoords[0], cellCoords[1] - 1]);
        if (cellCoords[1] < _statWidth - 1) neighbours.Add(_statCells[cellCoords[0], cellCoords[1] + 1]);
        return neighbours;
    }

    private static void UpdateNeighbours()
    {
        foreach (GameCell cell in _statCells)
        {
            cell.SetNeighbours(GetCellNeighbours(cell.Coordinates));
        }
    }
    #endregion

    public Vector2Int FindCellIndex(Vector3 pos)
    {
        Vector2Int index = new Vector2Int(-1, -1);
        int i = 0;
        var dist = Single.PositiveInfinity;
        foreach (var cell in _statCells)
        {
            i++;
            var result = Vector3.Distance(cell.transform.position, pos);
            if (result <= dist)
            {
                dist = result;
                index = new Vector2Int(cell.Coordinates[0], cell.Coordinates[1]);
            }
        }

        return index;
    }

    
}

internal struct GameBoardOutMoveStruct
{
    internal int[] newCellCoords;
    internal int[] oldCellCoords;
    /// <summary>
    /// new Value of cell - old Value Of Cell;
    /// </summary>
    internal int deltaVal;
    internal bool destroyed;

    internal GameBoardOutMoveStruct(GameCell newCell, GameCell oldCell)
    {
        newCellCoords = newCell.Coordinates;
        oldCellCoords = oldCell.Coordinates;
        deltaVal = newCell.Value - oldCell.Value;
        destroyed = false;
    }
}