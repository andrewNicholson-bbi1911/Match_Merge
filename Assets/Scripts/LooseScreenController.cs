using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooseScreenController : MonoBehaviour
{
    [SerializeField] private Text _scoreTXT;
    [SerializeField] private Text _hsTXT;
    [SerializeField] private Text _message;
    [SerializeField] private Color _hsColor;
    private static string[] _enMessages = { "New Highscore!", "Out of moves" };
    private static List<string[]> _messages = new List<string[]>();

    private void OnEnable()
    {
        _scoreTXT.text = GameController.Score.ToString();
        _hsTXT.text = GameController.HighScore.ToString();
        if(GameController.Score >= GameController.HighScore)
        {
            _message.color = _hsColor;
            _message.text = _enMessages[0];
        }
        else
        {
            _message.text = _enMessages[1];
        }
    }
}
