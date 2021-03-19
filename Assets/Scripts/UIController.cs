using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    private static Text _stScoretxt;

    [SerializeField] private Text _highScoreText;
    private static Text _stHStxt;

    [SerializeField] private Image _trophyIMG;
    private static Image _stTrophyIMG;

    [SerializeField] private OutButtonController _outButton;
    private static OutButtonController _stOutButton;

    [SerializeField] private TranslationStatusController _destCellStatus;
    private static TranslationStatusController _stDestCellStatus;

    [Header("Game Screen")]
    [SerializeField] private GameObject _gameScreen;
    private static GameObject _stGameScreen;
    [SerializeField] private Image _scoreBG;
    private static Image _stScoreBG;

    [Header("Loose Screen")]
    [SerializeField] private LooseScreenController _looseScreen;
    private static LooseScreenController _stLooseScr;

    [Header("Camera Effects")]
    [SerializeField] private Camera _camera;
    private static Camera _stCamera;
    [SerializeField] private Color _inModeBGColor;
    [SerializeField] private Color _outModeBGColor;
    private static Color _stInModeBGColor;
    private static Color _stOutModeBGColor;

    [Header("otherColors")]
    [SerializeField] private Color _inModeColor;
    [SerializeField] private Color _outModeColor;
    public static Color _StInModeColor;
    public static Color _StOutModeColor;

    public void Awake()
    {
        _stScoretxt = _scoreText;
        _stHStxt = _highScoreText;
        _stLooseScr = _looseScreen;
        _stCamera = _camera;
        _stInModeBGColor = _inModeBGColor;
        _stOutModeBGColor = _outModeBGColor;
        _stTrophyIMG = _trophyIMG;
        _stOutButton = _outButton;
        _stOutButton.UpdateMaxValue(GameController.MaxMovesToOutEnabled);
        _stOutButton.UpdateValue(0);
        _stGameScreen = _gameScreen;
        _stDestCellStatus = _destCellStatus;
        _stDestCellStatus.UpdateValue(0);
        _stScoreBG = _scoreBG;

        _StInModeColor = _inModeColor;
        _StOutModeColor = _outModeColor;



        SetMode(true);
    }

    public static void UpdateScore(int score)
    {
        _stTrophyIMG.gameObject.SetActive(score >= GameController.HighScore);
        _stScoretxt.text = $"{score}";
    }

    public static void UpdateHighScore(int hs) => _stHStxt.text = $"{hs}";

    public static void SetLooseScreen()
    {
        _stLooseScr.gameObject.SetActive(true);
        _stGameScreen.SetActive(false);
    }

    public static void SetMode(bool isInMode)
    {
        if (isInMode)
        {
            _stCamera.backgroundColor = _stInModeBGColor;
            _stScoreBG.color = _StInModeColor;
        }
        else
        {
            _stCamera.backgroundColor = _stOutModeBGColor;
            _stScoreBG.color = _StOutModeColor;
        }
        _stOutButton.SetMode(isInMode);
        _stDestCellStatus.SetMode(isInMode);
    }

    public static void UpdateOUTStatus(int value)
    {
        _stOutButton.UpdateValue(value);
    }

    public static void UpdateNewOUTMaxStepsStatus()
    {
        _stOutButton.UpdateMaxValue(GameController.MaxMovesToOutEnabled);
        _stOutButton.UpdateValue(0);
    }

    public static void UpdateDestroyedCellStatus(int value)
    {
        _stDestCellStatus.UpdateValue(value);
    }

}
