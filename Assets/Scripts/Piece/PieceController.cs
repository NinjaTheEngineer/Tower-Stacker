using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PieceController : NinjaMonoBehaviour {
    [SerializeField] protected float pieceFallSpeed = 2f;
    [SerializeField] protected float pieceMoveSpeed = 5f;

    protected Piece controlledPiece; 
    protected TouchInputController touchInputController;

    public void Start() {
        GameManager.OnGameOver += ReleaseControlledPiece;
    }  
    public void SetControlledPiece(Piece piece) {
        var logId = "SetControllerPiece";
        logd(logId, "Setting ControlledPiece from "+controlledPiece.logf()+" to "+piece.logf()+" => Initializing Piece", true);
        touchInputController = TouchInputController.Instance;
        controlledPiece = piece;
        controlledPiece?.Initialize(); 
    }
    public void ReleaseControlledPiece() {
        var logId = "ReleaseControlledPiece";
        if(controlledPiece==null) {
            logw(logId, "No ControlledPiece found => no-op");
            return;
        }
        logw(logId, "Destroying ControlledPiece="+controlledPiece.logf());
        Destroy(controlledPiece.gameObject);
        controlledPiece = null;
    }

    private void Update() {
        var logId = "Update";
        if (controlledPiece == null) {
            logd(logId, "No ControlledPiece assigned => no-op", true);
            return;
        }

        var pieceState = controlledPiece.CurrentState;
        if (pieceState == Piece.PieceState.Released) {
            logd(logId, "Piece="+controlledPiece.logf()+" is Free => Freeing Controller");
            controlledPiece = null;
            return;
        }

        if (pieceState==Piece.PieceState.Controlled) {
            logd(logId, "Piece="+controlledPiece.logf()+" is being Controlled", true);
            MovePieceDown();
            MoveHorizontally();

            if(touchInputController.IsTap) {
                RotatePiece(90f);
            }
        }
    }

    private void MovePieceDown() {
        var logId = "MovePieceDown";
        logd(logId, "Moving Piece Down", true);
        controlledPiece.transform.position += -transform.up * Time.deltaTime * pieceFallSpeed;
    }

    private void MoveHorizontally() {
        var logId = "MoveHorizontally";
        if(touchInputController==null) {
            loge(logId, "TouchInputController is null => no-op");
            return;
        }
        if (touchInputController.IsDragging) {
            logd(logId, "Is Dragging! touchInputController="+touchInputController.logf()+" is dragging="+touchInputController?.IsDragging.logf(),true);
            Touch touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Moved:
                    Vector3 touchDelta = touch.deltaPosition;
                    touchDelta.y = 0f;
                    controlledPiece.transform.position += touchDelta * pieceMoveSpeed * Time.deltaTime;
                    break;
            }
        } else {
            logd(logId, "NOT Dragging! touchInputController="+touchInputController.logf()+" is dragging="+touchInputController?.IsDragging.logf(),true);
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