using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpell : MonoBehaviour
{
    [SerializeField]private GameBoard _gameBoard;
    private Transform _transform;
    
    private void Awake()
    {
        _transform = transform;
    }

    

}
