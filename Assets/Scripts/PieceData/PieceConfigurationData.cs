using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

[CreateAssetMenu(fileName = "PieceConfiguration", menuName = "Pieces/Create New Piece")]
public class PieceConfigurationData : ScriptableObject {
    public int gridSize = 5;
    public Sprite sprite;
    public Vector2 pivotPosition; 
    public Vector2[] blockPositions;
    public override string ToString() => "{GridSize="+gridSize+" PivotPosition="+pivotPosition+" BlocksPositions="+blockPositions+"}";
}
