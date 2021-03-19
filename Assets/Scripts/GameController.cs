using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //[SerializeField] private UIController _uiController;
    private static List<PlayerMove> Moves = new List<PlayerMove>();

    private static int[] _startVlues = { 2, 1, 1, 1, -127, -127 };

    public static int DestroyedCellsAmount { get => _destroyedCellsAmmount; }
    private static int _destroyedCellsAmmount;

    public static int Score
    {
        get => _score;
        private set
        {
            _score = value;
            UIController.UpdateScore(_score);
            if (_score > _highScore)
            {
                _highScore = _score;
                PlayerPrefs.SetInt("HighScore", _highScore);
                UIController.UpdateHighScore(_highScore);
            }
        }
    }
    private static int _score;

    public static int HighScore { get => _highScore; }
    private static int _highScore;

    public static int[] lastChangedCoords = new int[2] { -1, -1 };
    public static bool InCast
    {
        get => _inCast;
    }
    private static bool _inCast = true;

    public static int MaxMovesToOutEnabled
    {
        get => _maxMovesToOutEnabled;
        private set
        {
            if (value >= 24)
            {
                value = 24;
            }
            _maxMovesToOutEnabled = value;
            UIController.UpdateNewOUTMaxStepsStatus();

        }
    }
    private static int _maxMovesToOutEnabled = 12;
    public static int SpentMoves
    {
        get => _spentMoves;
        private set
        {
            _spentMoves = value;
            UIController.UpdateOUTStatus(_spentMoves);
        }
    }
    private static int _spentMoves = 0;

    public static int MovesToDestroyedCellTranslation
    {
        get => _movesToDestroyedCellMoves;
        private set
        {
            if(value >= MaxMovesToDestroyedCellTranslations)
            {
                _movesToDestroyedCellMoves = 0;
                TranslateNextDCellMove();
            }
            else if(value < 0)
            {
                _movesToDestroyedCellMoves = MaxMovesToDestroyedCellTranslations;
                BackMove();
            }
            else
            {
                _movesToDestroyedCellMoves = value;
            }
            UIController.UpdateDestroyedCellStatus(_movesToDestroyedCellMoves);
        }
    }
    private static int _movesToDestroyedCellMoves = 0;
    public static int MaxMovesToDestroyedCellTranslations { get => _maxMovesToDestroyedCellTranslations; }
    public static int _maxMovesToDestroyedCellTranslations = 1;

    private static Stack<PlayerMove> _destroyedCellMovesStack;


    private void Start()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            _highScore = PlayerPrefs.GetInt("HighScore");
            UIController.UpdateHighScore(_highScore);

        }
        else
        {
            PlayerPrefs.SetInt("HighScore", 0);
            _highScore = 0;
            UIController.UpdateHighScore(_highScore);

        }

        Score = 0;
        SpentMoves = 0;
        
        _destroyedCellMovesStack = new Stack<PlayerMove>();
        GenerateTranslationInfoDestroyedCell();
    }

    internal static void SetSettings(GameSettings gameSettings)
    {
        GenerateRandomStartCellValues();
        MovesToDestroyedCellTranslation = 0;
        MaxMovesToOutEnabled = 12;
    }


    public static void ChangeCellIn(int[] coords)
    {
        ChangeCellIn(GameBoard.GameCells[coords[0], coords[1]]);
    }

    public static void ChangeCellIn(GameCell changingCell)
    {

        var _startVal = changingCell.Value;
        var sameNeighbours = GameBoard.GetCellNeighbours(changingCell.Coordinates).FindAll(x => x.Value == _startVal);

        var inreasingCells = new List<int[]>();
        var decreasingCells = new List<int[]>();

        if (sameNeighbours.Count == 2 || sameNeighbours.Count == 4)
        {
            foreach (GameCell cell in sameNeighbours)
            {
                if (cell.Value > 0)
                {
                    decreasingCells.Add(cell.Coordinates);
                }
            }
            inreasingCells.Add(changingCell.Coordinates);
            if (sameNeighbours.Count == 4)
            {
                inreasingCells.Add(changingCell.Coordinates);
            }
            var addScore = Mathf.CeilToInt(Mathf.Pow(2, changingCell.Value + sameNeighbours.Count / 2)) * sameNeighbours.Count / 2;
            Score += addScore;

            Move(new PlayerMove(inreasingCells, decreasingCells, addScore));
            lastChangedCoords = changingCell.Coordinates;
        }
        else if (sameNeighbours.Count == 3)
        {
            var changingIndex1 = 1;
            var changingIndex2 = 2;

            if (sameNeighbours[0].X == sameNeighbours[1].X || sameNeighbours[0].Y == sameNeighbours[1].Y)
            {
                changingIndex2 = 0;
            }
            else if (sameNeighbours[0].X == sameNeighbours[2].X || sameNeighbours[0].Y == sameNeighbours[2].Y)
            {
                changingIndex1 = 0;
            }

            if (sameNeighbours[changingIndex1].Value > 0) decreasingCells.Add(sameNeighbours[changingIndex1].Coordinates);
            if (sameNeighbours[changingIndex2].Value > 0) decreasingCells.Add(sameNeighbours[changingIndex2].Coordinates);

            inreasingCells.Add(changingCell.Coordinates);
            var addScore = Mathf.CeilToInt(Mathf.Pow(2, changingCell.Value + 1));
            Score += addScore;

            Move(new PlayerMove(inreasingCells, decreasingCells, addScore));
            lastChangedCoords = changingCell.Coordinates;

        }
    }

    internal static void Move(PlayerMove playerMove)
    {
        Moves.Add(playerMove);

        foreach (int[] cellCoords in playerMove.increasedCells)
        {
            GameBoard.GameCells[cellCoords[0], cellCoords[1]].Increase();
        }

        foreach (int[] cellCoords in playerMove.decreasedCells)
        {
            GameBoard.GameCells[cellCoords[0], cellCoords[1]].Decrease();
        }

        SpentMoves++;
        MovesToDestroyedCellTranslation++;

        CheckAbleMooves();
    }

    internal static void Move(GameBoardOutMoveStruct gbOUTMove, bool destroyed = false)
    {


        PlayerMove newMove = new PlayerMove(false);
        if (gbOUTMove.deltaVal >= 0)
        {
            for (int i = 0; i < gbOUTMove.deltaVal; i++)
            {
                newMove.AddDecreaseCell(gbOUTMove.newCellCoords);
                newMove.AddIncreaseCell(gbOUTMove.oldCellCoords);
            }
        }
        else
        {
            for (int i = 0; i < -gbOUTMove.deltaVal; i++)
            {
                newMove.AddIncreaseCell(gbOUTMove.newCellCoords);
                newMove.AddDecreaseCell(gbOUTMove.oldCellCoords);
            }
        }

        newMove.destroyed = destroyed;

        if (_destroyedCellMovesStack.Count > 0)
        {
            Moves.Add(_destroyedCellMovesStack.Pop());
        }
        else {
            Moves.Add(newMove);
        }
    }


    public void ChangeMode()
    {
        if (true)
        {
            _inCast = !_inCast;
            
            if (!_inCast && (_spentMoves < _maxMovesToOutEnabled))
            {
                _inCast = true;
            }
            else if(_inCast)
            {
                CheckAbleMooves();
                MaxMovesToOutEnabled += 4;
                SpentMoves = 0;
            }
        }
        else
        {
            return;
        }
        UIController.SetMode(_inCast);
    }

    public void BackNMoves(int n)
    {
        for (int i = 0; i < n & Moves.Count > 0; i++)
        {
            BackMove();
        }

        if(n == 10)
        {
            SpentMoves+=10;
        }
    }

    public void FuckGoBack()
    {
        BackMove();
    }

    private static void BackMove()
    {

        if (Moves.Count <= 0 | _score <= 0) return;

        var playerMove = Moves[Moves.Count - 1];
        

        foreach (int[] cellCoords in playerMove.decreasedCells)
        {
            GameBoard.GameCells[cellCoords[0], cellCoords[1]].Increase(playerMove.destroyed);
        }

        foreach (int[] cellCoords in playerMove.increasedCells)
        {
            GameBoard.GameCells[cellCoords[0], cellCoords[1]].Decrease(playerMove.destroyed);
        }

        Score -= playerMove.amount;

        SpentMoves--;

        if (playerMove.In)
        {
            MovesToDestroyedCellTranslation--;
        }
        Moves.Remove(playerMove);

        if (playerMove.destroyed)
        {
            _destroyedCellMovesStack.Push(playerMove);
            _movesToDestroyedCellMoves = MaxMovesToDestroyedCellTranslations;
            BackMove();
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(0);
    }

    private static void GenerateRandomStartCellValues()
    {
        foreach (int val in _startVlues)
        {
            while (true)
            {
                var x = Random.Range(0, GameBoard.Len);
                var y = Random.Range(0, GameBoard.Len);
                if (GameBoard.GameCells[x, y].Value == 0)
                {
                    var a = val;
                    if (a == -127)
                    {
                        GameBoard.GameCells[x, y].DestroyCell();
                        _destroyedCellsAmmount++;
                    }
                    else
                    {
                        while (a > 0)
                        {
                            GameBoard.GameCells[x, y].Increase();
                            a--;
                        }
                    }
                    break;
                }
            }
        }
    }

    private static void CheckAbleMooves()
    {
        bool found = false;
        foreach (GameCell cell in GameBoard.GameCells)
        {
            if (GameBoard.GetCellNeighbours(cell.Coordinates).FindAll(x => x.Value == cell.Value).Count >= 2)
            {
                found = true;
                break;
            }
        }
        if (!found & _maxMovesToOutEnabled > _spentMoves)
        {
            UIController.SetLooseScreen();
        }
        
    }


    private static int[] changing;
    private static void TranslateNextDCellMove()
    {
        GameBoard.MoveDestroyedCell(changing[0], changing[1]);
        GenerateTranslationInfoDestroyedCell();
    }
    private static void GenerateTranslationInfoDestroyedCell()
    {
        changing = new int[2] { Random.Range(0, DestroyedCellsAmount), Random.Range(0, 8) };
    }
}

internal struct PlayerMove
{
    public List<int[]> increasedCells;
    public List<int[]> decreasedCells;
    internal int amount;
    internal bool In;
    internal bool destroyed;

    public PlayerMove(bool isIn)
    {
        increasedCells = new List<int[]>();
        decreasedCells = new List<int[]>();
        amount = 0;
        In = isIn;
        destroyed = false;
    }

    public PlayerMove(List<int[]> increasedCells, List<int[]> decreasedCells, int bonusValue, bool inMove = true, bool destroyed = false)
    {
        this.increasedCells = increasedCells;
        this.decreasedCells = decreasedCells;
        amount = bonusValue;
        In = inMove;
        this.destroyed = destroyed;
    }

    public void AddIncreaseCell(int[] coords) => increasedCells.Add(coords);
    public void AddDecreaseCell(int[] coords) => decreasedCells.Add(coords);
}


internal class GameSettings
{
    private int _width = 5;
    private int _height = 6;
    private int[] _startPows = { 2, 1, 1, 1, -127, -127 };

    private int _minOut = 12;
    private int _maxOut = 24;
    private int _stepOut = 4;

    private int _dCellTrMoves = 16;

    /// <summary>
    /// Sets settings about gameboard
    /// </summary>
    /// <param name="w">the width of gameboard</param>
    /// <param name="h">the height of gameboard</param>
    /// <param name="_startPowsOfrandomCells">the start powers of gameboard Cells [the negativ values of any indexes will generate a destroyed cells]</param>
    internal void SetBoardSettings(int w, int h, int[] _startPowsOfrandomCells)
    {
        _width = Mathf.Max(4,w);
        _height = Mathf.Max(4, h);
        for (int i = _startPowsOfrandomCells.Length; i>0; i--)
        {
            if (_startPowsOfrandomCells[i] < 0)
            {
                _startPowsOfrandomCells[i] = -127;
            }
        }
    }

    /// <summary>
    /// Sets settings about activation of Out Mode
    /// </summary>
    /// <param name="_minOutModMovesActivator">how many moves the player should made before the Out mode become enabled firstly</param>
    /// <param name="_maxOutModMovesActivator">maximum of player moves before Out mode become enabled</param>
    /// <param name="_stepOutModMovesActivator">the step of increacing cost of activating Out mode</param>
    internal void SetOutModSettings(int _minOutModMovesActivator, int _maxOutModMovesActivator, int _stepOutModMovesActivator)
    {
        _minOut = Mathf.Max(5, _minOutModMovesActivator);
        _maxOut = Mathf.Max(_minOut+1, _maxOutModMovesActivator);
        _stepOut = Mathf.Max(1, _stepOutModMovesActivator);
    }

    /// <summary>
    /// Sets settings about translation of destroyed cells
    /// </summary>
    /// <param name="_movesToDCellTranslation">how many moves the player should made before the destroyed cell translate</param>
    internal void SetDestroyedCellsTranslationSettings(int _movesToDCellTranslation, bool shouldTranslate = true)
    {
        _dCellTrMoves = Mathf.Max(1, _movesToDCellTranslation);
    }

}