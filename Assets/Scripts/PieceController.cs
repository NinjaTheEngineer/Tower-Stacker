using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PieceController : NinjaMonoBehaviour {
    private Piece controlledPiece; // Reference to the controlled piece
    [SerializeField] float pieceScale = 0.35f;
    [SerializeField] float pieceFallSpeed = 2f;
    [SerializeField] float pieceMoveSpeed = 5f;

    public void SetControlledPiece(Piece piece) {
        controlledPiece = piece;
        controlledPiece.Initialize(pieceScale);
    }

    // Update is called once per frame
    private void Update() {
        var logId = "Update";
        
        if(controlledPiece==null) {
            logd(logId, "No ControlledPiece asigned => no-op", true);
            return;
        }

        var pieceState = controlledPiece.CurrentState;
        if(pieceState==Piece.PieceState.Free) {
            logd(logId, "Piece="+controlledPiece.logf()+" is Free => Freeing Controller", true);
            controlledPiece = null;
            return;
        } else if (pieceState==Piece.PieceState.Controlled) {
            logd(logId, "Piece="+controlledPiece.logf()+" is being Controlled", true);
            MovePieceDown();
            if(Input.GetKeyDown(KeyCode.R)) {
                RotatePiece(90f);
            }
            MoveHorizontally();
        }
    }

    private void MovePieceDown() {
        // Move the piece downwards at a constant rate
        controlledPiece.transform.position += -transform.up * Time.deltaTime * pieceFallSpeed;
    }
    private void MoveHorizontally() {
        if(Input.GetKeyDown(KeyCode.D)) {
            controlledPiece.transform.position += transform.right * Time.deltaTime * pieceMoveSpeed;
        } else if(Input.GetKeyDown(KeyCode.A)) {
            controlledPiece.transform.position += -transform.right * Time.deltaTime * pieceMoveSpeed;
        }
    }
    public void RotatePiece(float angle) {
        if (controlledPiece != null) {
            var rotationPivot = controlledPiece.PivotPosition;
            if(rotationPivot == new Vector2(-1, -1)) {
                return;
            }
            rotationPivot.y *= -1;
            rotationPivot = controlledPiece.transform.TransformPoint(rotationPivot);
            controlledPiece.transform.RotateAround(rotationPivot, Vector3.forward, angle);
        }
    }
}