using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameBoard))]
[CanEditMultipleObjects]
public class GameBoardEditor : Editor
{
    private SerializedProperty _length;
    private SerializedProperty _width;
    private SerializedProperty _cellSize;
    private SerializedProperty _cellSpace;
    private SerializedProperty _cellObject;

    private bool _autoUpdate = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        _length = serializedObject.FindProperty("_length");
        _width = serializedObject.FindProperty("_width");
        _cellSize = serializedObject.FindProperty("_cellSize");
        _cellSpace = serializedObject.FindProperty("_cellSpace");
        _cellObject = serializedObject.FindProperty("_cellObject");
    }

    public override void OnInspectorGUI()
    {
        GameBoard gameBoard = (GameBoard)target;

        ShowGameBoardFields();

        if (_width.intValue <= 1)
            _width.intValue = 1;

        if (_length.intValue <= 1)
            _length.intValue = 1;

        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("UpdateGameBoard") | _autoUpdate)
        {
            gameBoard.UpdateGameBoard();
        }

        if (GUILayout.Button("RebuildGameBoard"))
        {
            gameBoard.InitializeGameBoard();
        }

        if (GameBoard.GameCells == null)
            Debug.LogWarning("Pls update or rebuild map!");
        else
            GUILayout.Box(GameBoard.GameCells.Length.ToString());
    }

    private void ShowGameBoardFields()
    {
        EditorGUILayout.PropertyField(_length);
        EditorGUILayout.PropertyField(_width);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_cellSize);
        EditorGUILayout.PropertyField(_cellSpace);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_cellObject);
        EditorGUILayout.Space();
        _autoUpdate = EditorGUILayout.Toggle("enable autoupdate", _autoUpdate);

    }
}
