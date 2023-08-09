using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NinjaTools;

[CustomEditor(typeof(PieceConfigurationData))]
public class PieceConfigurationDataEditor : Editor {
    private SerializedProperty colorProperty;
    private SerializedProperty gridSizeProperty;
    private SerializedProperty pivotPositionProperty;
    private SerializedProperty blockPositionsProperty;

    private int gridSize;
    private bool gridInitialized = false;
    private Dictionary<Vector2Int, Vector2> selectedBlockPositions = new Dictionary<Vector2Int, Vector2>();

    private void OnEnable() {
        colorProperty = serializedObject.FindProperty("color");
        gridSizeProperty = serializedObject.FindProperty("gridSize");
        pivotPositionProperty = serializedObject.FindProperty("pivotPosition");
        blockPositionsProperty = serializedObject.FindProperty("blockPositions");
        gridSize = gridSizeProperty.intValue;
        InitializeGrid();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(colorProperty);
        EditorGUILayout.PropertyField(gridSizeProperty);
        EditorGUILayout.PropertyField(pivotPositionProperty);
        if (EditorGUI.EndChangeCheck()) {
            gridSize = Mathf.Max(gridSize, 2);
            gridSizeProperty.intValue = gridSize;
            InitializeGrid();
        }

        DrawBlockPositions();

        EditorGUILayout.Space();
        if (GUILayout.Button("Save")) {
            SaveToScriptableObject();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void InitializeGrid()
    {
        if (!gridInitialized)
        {
            // Set the selected block positions based on the existing blockPositions array
            selectedBlockPositions.Clear();
            for (int i = 0; i < blockPositionsProperty.arraySize; i++)
            {
                SerializedProperty positionProperty = blockPositionsProperty.GetArrayElementAtIndex(i);
                Vector2 position = positionProperty.vector2Value;
                int x = Mathf.RoundToInt(position.x);
                int y = Mathf.RoundToInt(position.y);
                selectedBlockPositions[new Vector2Int(x, y)] = position;
            }
            gridInitialized = true;
        }
    }

    private void DrawBlockPositions() {
        EditorGUILayout.LabelField("Block Positions", EditorStyles.boldLabel);
        
        if(EditorGUI.EndChangeCheck()) {
            // Update the grid when the blockPositions array changes
            InitializeGrid();
        }
        // Draw the grid and handle cell selections
        int cellSize = 30;
        Rect gridRect = GUILayoutUtility.GetRect(cellSize * gridSize, cellSize * gridSize, GUILayout.ExpandWidth(false));
        int maxCellIndex = gridSize * gridSize - 1;

        EditorGUI.DrawRect(gridRect, Color.black);

        for (int i = 0; i <= maxCellIndex; i++) {
            int x = i % gridSize;
            int y = i / gridSize;
            Rect cellRect = new Rect(gridRect.x + x * cellSize, gridRect.y + y * cellSize, cellSize, cellSize);
            Vector2 pivotPosition = pivotPositionProperty.vector2Value;
            bool isSelectBlock = selectedBlockPositions.ContainsKey(new Vector2Int(x, y));
            bool isPivot = x == Mathf.RoundToInt(pivotPosition.x) && y == Mathf.RoundToInt(pivotPosition.y);
            if (isSelectBlock && isPivot) {
                EditorGUI.DrawRect(cellRect, Color.magenta);
            } else if(isPivot) {
                EditorGUI.DrawRect(cellRect, Color.blue);
            } else if(isSelectBlock) {
                EditorGUI.DrawRect(cellRect, Color.green);
            } else {
                EditorGUI.DrawRect(cellRect, Color.white);
            }

            // Handle cell selection
            if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition)) {
                if (Event.current.button == 0) {
                    if(isSelectBlock) {
                        selectedBlockPositions.Remove(new Vector2Int(x, y));
                    } else {
                        Vector2 position = new Vector2(x, y);
                        selectedBlockPositions[new Vector2Int(x, y)] = position;
                    }
                } else if (Event.current.button == 1) {
                    Vector2 clickedPosition = new Vector2(x, y);
                    pivotPosition = pivotPositionProperty.vector2Value;
                    if (clickedPosition == pivotPosition) {
                        pivotPositionProperty.vector2Value = new Vector2(-1, -1);
                    } else {
                        pivotPositionProperty.vector2Value = clickedPosition;
                    }
                }
                GUI.changed = true;
            }

            blockPositionsProperty.arraySize = selectedBlockPositions.Count;
            int index = 0;
            foreach (var position in selectedBlockPositions.Values) {
                SerializedProperty positionProperty = blockPositionsProperty.GetArrayElementAtIndex(index);
                positionProperty.vector2Value = position;
                index++;
            }
        }
        EditorGUILayout.PropertyField(blockPositionsProperty, true);
    }

    private void SaveToScriptableObject() {
        serializedObject.ApplyModifiedProperties();
    }
}