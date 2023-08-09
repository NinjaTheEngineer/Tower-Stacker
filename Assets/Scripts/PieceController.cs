using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PieceController : NinjaMonoBehaviour {
    [SerializeField] float pieceFallSpeed = 2f;
    [SerializeField] float pieceMoveSpeed = 5f;

    Piece controlledPiece; 
    TouchInputController touchInputController;

    public void Start() {
        touchInputController = TouchInputController.Instance;
        GameManager.OnGameOver += ReleaseControlledPiece;
    }  
    public void SetControlledPiece(Piece piece) {
        controlledPiece = piece;
        controlledPiece?.Initialize(); // Safe invocation in case controlledPiece is null
    }
    public void ReleaseControlledPiece() {
        var logId = "ReleaseControlledPiece";
        if(controlledPiece==null) {
            return;
        }
        Destroy(controlledPiece.gameObject);
        controlledPiece = null;
    }

    private void Update() {
        var logId = "Update";
        if (controlledPiece == null) {
            Debug.Log("No ControlledPiece assigned => no-op");
            return;
        }

        var pieceState = controlledPiece.CurrentState;
        if (pieceState == Piece.PieceState.Released) {
            Debug.Log($"Piece={controlledPiece.logf()} is Free => Freeing Controller");
            controlledPiece = null;
            return;
        }

        if (pieceState == Piece.PieceState.Controlled) {
            logd(logId, $"Piece={controlledPiece.logf()} is being Controlled", true);
            MovePieceDown();
            MoveHorizontally();

            if (touchInputController.IsTap) {
                RotatePiece(90f);
            }
        }
    }

    private void MovePieceDown() {
        controlledPiece.transform.position += -transform.up * Time.deltaTime * pieceFallSpeed;
    }

    private void MoveHorizontally() {
        if (touchInputController.IsDragging) {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Moved:
                    Vector3 touchDelta = touch.deltaPosition;
                    touchDelta.y = 0f;
                    controlledPiece.transform.position += touchDelta * pieceMoveSpeed * Time.deltaTime;
                    break;
            }
        }
    }

    public void RotatePiece(float angle) {
        if (controlledPiece != null) {
            var rotationPivot = controlledPiece.PivotPosition;
            if (rotationPivot != new Vector2(-1, -1)) {
                rotationPivot.y *= -1;
                rotationPivot = controlledPiece.transform.TransformPoint(rotationPivot);
                controlledPiece.transform.RotateAround(rotationPivot, Vector3.forward, angle);
            }
        }
    }
    private void OnDestroy() {
        GameManager.OnGameOver -= ReleaseControlledPiece;
    }
}