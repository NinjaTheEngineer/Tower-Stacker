using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class AIController : PieceController {
    [SerializeField] protected float movementWidth;
    bool controllingPiece = false;
    [SerializeField] int minRotateAttempts = 0;
    [SerializeField] int maxRotateAttempts = 4;
    [SerializeField] float minAttemptCooldown = 0.6f;
    [SerializeField] float maxAttemptCooldown = 1.2f;
    [Tooltip("The lower the value the higher the change to rotate per attempt.")]
    [Range(2, 10)]
    [SerializeField] int rotateChange = 4;
    [SerializeField] float minControlCycleDelay = 1;
    [SerializeField] float maxControlCycleDelay = 3;
    [SerializeField] float moveThresholdDistance = 0.15f;

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
            if(!controllingPiece) {
                StartCoroutine(ControlPieceRoutine());
            }
            MoveHorizontally();
        }
    }
    IEnumerator ControlPieceRoutine() {
        var logId = "ControlPieceRoutine";
        var waitForSeconds = Utils.WaitForSeconds(minControlCycleDelay, maxControlCycleDelay);
        logd(logId, "Starting ControlPiece Routine");
        controllingPiece = true;
        while(controlledPiece!=null && controlledPiece.CurrentState == Piece.PieceState.Controlled) {
            targetX = GetTargetX();
            int rotateAttempts = Random.Range(minRotateAttempts, maxRotateAttempts);
            while(rotateAttempts>0) {
                yield return Utils.WaitForSeconds(minAttemptCooldown, maxAttemptCooldown);
                bool rotate = Random.Range(0, rotateChange)==0;
                if(rotate) {
                    RotatePiece(90f);
                }
                rotateAttempts--;
                logd(logId, "Rotating="+rotate+" RotateAttemptsLeft="+rotateAttempts);
            }
            yield return waitForSeconds;
        }
        controllingPiece = false;
    }
    float targetX;
    float GetTargetX() {
        var logId = "GetTargetX";
        var posX = transform.position.x;
        var halfWidth = movementWidth/2;
        var randomX = Random.Range(posX-halfWidth, posX+halfWidth);
        logd(logId, "Returning X="+randomX+" while halfWidth="+halfWidth+" PosX="+posX);
        return randomX;
    }

    private void MovePieceDown() {
        controlledPiece.transform.position += -transform.up * Time.deltaTime * pieceFallSpeed;
    }

    private void MoveHorizontally() {
        var targetPosition = new Vector3(targetX, controlledPiece.transform.position.y, controlledPiece.transform.position.z);
        float distanceToTarget = Vector3.Distance(controlledPiece.transform.position, targetPosition);
        if (distanceToTarget > moveThresholdDistance) {
            controlledPiece.transform.position = Vector3.MoveTowards(controlledPiece.transform.position, targetPosition, pieceMoveSpeed * Time.deltaTime);
        }
    }
    private void OnDestroy() {
        GameManager.OnGameOver -= ReleaseControlledPiece;
    }
}
