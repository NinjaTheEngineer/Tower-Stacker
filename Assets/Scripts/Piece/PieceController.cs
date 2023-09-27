using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System.Linq;

public class PieceController : NinjaMonoBehaviour {
    [SerializeField] protected float pieceFallSpeed = 2f;
    [SerializeField] protected float pieceMoveSpeed = 5f;
    public LayerMask settingLayers;

    protected Piece controlledPiece; 
    protected TouchInputController touchInputController;

    public void Start() {
        GameManager.OnGameOver += ReleaseControlledPiece;
    }  
    public void SetControlledPiece(Piece piece, List<GameObject> piecesBlocks) {
        var logId = "SetControllerPiece";
        logd(logId, "Setting ControlledPiece from "+controlledPiece.logf()+" to "+piece.logf()+" => Initializing Piece", true);
        touchInputController = TouchInputController.Instance;
        controlledPiece = piece;
        controlledPiece?.Initialize(piecesBlocks);
        StopAllCoroutines();
        StartCoroutine(UpdateGhostPositionRoutine());
    }
    public void ReleaseControlledPiece() {
        var logId = "ReleaseControlledPiece";
        if(controlledPiece==null) {
            logw(logId, "No ControlledPiece found => no-op");
            return;
        }
        logw(logId, "Destroying ControlledPiece="+controlledPiece.logf());
        Destroy(controlledPiece.gameObject);
        StopAllCoroutines();
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

            if (touchInputController.IsTap) {
                RotatePiece(90f);
            }
        }
    }
    public float ghostHeightOffset;
    public GameObject highestHitObject = null; 
    public GameObject pieceBlockThatHit = null;
    public float boxCastWidth = 1f;
    public float boxCastHeight = 10f;
    public float currentBoxCastHeight;
    public IEnumerator UpdateGhostPositionRoutine() {
        var logId = "UpdateGhostPositionRoutine";

        highestHitObject = null;
        pieceBlockThatHit = null;
        currentBoxCastHeight = boxCastHeight;
        while (controlledPiece) {
            var pieceState = controlledPiece.CurrentState;
            var ghost = controlledPiece?.Ghost;
            if (ghost == null || pieceState != Piece.PieceState.Controlled) {
                logw(logId, "No ghost found => no-op", true);
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            float highestY = float.MinValue;
            float lastCollisionDistance = 10;

            foreach (var currentPieceBlock in controlledPiece.PieceBlocks) {
                Vector2 currentPieceBlockPos = currentPieceBlock.transform.position;
                RaycastHit2D[] hits = Physics2D.BoxCastAll(currentPieceBlockPos, new Vector2(boxCastWidth, currentBoxCastHeight), 0f, Vector2.down, 20, controlledPiece.settingLayers);
                if (hits.Length == 0) {
                    continue;
                }
                var collider = hits[0].collider;
                var colliderGameObject = collider?.gameObject;
                if (pieceBlockThatHit == null && colliderGameObject != null) {
                    pieceBlockThatHit = currentPieceBlock;
                    highestHitObject = colliderGameObject;
                }

                if (pieceBlockThatHit == null) {
                    continue;
                }

                var hitHeight = hits[0].point.y;
                var blockHeight = currentPieceBlockPos.y;

                var collisionDistance = Vector2.Distance(hits[0].point, currentPieceBlockPos); 

                var hasHighestHeight = (lastCollisionDistance > collisionDistance);

                logd(logId, "Hit collider=" + collider.logf() + " HitHeight=" + hitHeight.logf() + " HitGameObject=" + colliderGameObject.logf(), true);

                if (hasHighestHeight) {
                    highestY = hitHeight;
                    logd(logId, "Setting referenceBlock from " + highestHitObject.logf() + " to " + colliderGameObject, true);
                    highestHitObject = colliderGameObject;
                    pieceBlockThatHit = currentPieceBlock;
                    lastCollisionDistance = collisionDistance;
                    currentBoxCastHeight = Vector2.Distance(highestHitObject.transform.position, pieceBlockThatHit.transform.position);
                }
            }

            if (highestHitObject != null) {
                var pieceBlockPos = pieceBlockThatHit.transform.position;
                var controlledPiecePos= controlledPiece.transform.position;
                var hitObjectPos= highestHitObject.transform.position;
                ghostHeightOffset = (pieceBlockPos.y - controlledPiecePos.y) * -1;
                var bounds = highestHitObject.GetComponent<Collider2D>().bounds;
                hitBoundsY = bounds.size.y;
                Vector2 ghostPosition = new Vector2(controlledPiece.transform.position.x, hitObjectPos.y + ghostHeightOffset + hitBoundsY);
                logd(logId, "Setting ghost to position="+ghostPosition+" while PieceBlockPos="+pieceBlockPos+ " HitObjectPos="+hitObjectPos+" ControlledPiecePos = " + controlledPiecePos + " HitBounds="+bounds+" GhostHeightOffset="+ghostHeightOffset, true);
                ghost.transform.position = ghostPosition;
            }
            yield return true;
        }
    }   public float hitBoundsY = 0;
        public Vector2 pieceBlockPos = Vector2.zero;
        public Vector2 controlledPiecePos = Vector2.zero;
        public Vector2 hitObjectPos = Vector2.zero;
    private void OnDrawGizmos() {
        if(pieceBlockThatHit==null) {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pieceBlockThatHit.transform.position, new Vector2(0.5f, 0.5f));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(highestHitObject.transform.position, new Vector2(0.5f, 0.5f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controlledPiece.transform.position, new Vector2(0.5f, 0.5f));
        Vector2 collisionCheckCenter = new Vector2(pieceBlockThatHit.transform.position.x, pieceBlockThatHit.transform.position.y - currentBoxCastHeight / 2f);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(collisionCheckCenter, new Vector3(boxCastWidth, currentBoxCastHeight, 1));
        Gizmos.DrawRay(pieceBlockThatHit.transform.position, Vector3.down*10f);
    }

    private void MovePieceDown() {
        var logId = "MovePieceDown";
        logd(logId, "Moving Piece Down", true);
        controlledPiece.transform.position += -transform.up * Time.deltaTime * pieceFallSpeed;
    }

    private void MoveHorizontally() {
        var logId = "MoveHorizontally";

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A)) {
            controlledPiece.transform.position += -transform.right * pieceMoveSpeed * 10f * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.D)) {
            controlledPiece.transform.position += transform.right * pieceMoveSpeed * 10f * Time.deltaTime;
        }
#endif
        if (touchInputController==null) {
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
        var logId = "RotatePiece";
        var currentState = GameManager.Instance.CurrentState;
        if (currentState == GameManager.GameState.Paused || currentState == GameManager.GameState.GameOver) {
            logd(logId, "GameManager Current State="+currentState);
            return;
        }
        if (controlledPiece != null) {
            var rotationPivot = controlledPiece.PivotPosition;
            if (rotationPivot != new Vector2(-1, -1)) {
                rotationPivot.y *= -1;
                rotationPivot = controlledPiece.transform.TransformPoint(rotationPivot);
                controlledPiece.transform.RotateAround(rotationPivot, Vector3.forward, angle);
                controlledPiece.Ghost.transform.RotateAround(rotationPivot, Vector3.forward, angle);
                AudioManager.Instance.PlayPieceRotateSound();
            }
        }
    }
    private void OnDestroy() {
        GameManager.OnGameOver -= ReleaseControlledPiece;
    }
}