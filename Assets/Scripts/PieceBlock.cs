using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PieceBlock : NinjaMonoBehaviour {
    Piece parentPiece;
    [SerializeField] SpriteRenderer spriteRenderer;
    void Start() {
        parentPiece = GetComponentInParent<Piece>();
        spriteRenderer.color = parentPiece.PieceConfiguration.color;
    }

}