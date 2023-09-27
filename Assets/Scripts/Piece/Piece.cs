using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;
public class Piece : NinjaMonoBehaviour {
    [SerializeField] public LayerMask settingLayers;
    public Rigidbody2D rb;
    public enum PieceState { Initializing, Controlled, Released, OutOfBounds }
    [field: SerializeField] public GameObject BlockPrefab { get; private set; }
    [field: SerializeField] public GameObject GhostPrefab { get; private set; }
    [field: SerializeField] public PieceConfigurationData PieceConfiguration { get; private set; }
    [field: SerializeField] public Vector2 PivotPosition { get; private set;}
    public List<GameObject> GhostBlocks { get; private set; } = new List<GameObject>();
    public PieceState CurrentState { get; private set; }
    public Action OnPieceReleased;
    public static Action<Piece> OnOutOfBounds;
    public float destroyHeight;
    public float gravityScale;
    [SerializeField] float minControlledHeight = -1.5f;

    public override string ToString() => "{PieceConfiguration=" + PieceConfiguration.logf() + " CurrentState=" + CurrentState.logf() + " DestroyHeight=" + destroyHeight + " PivotPosition=" + PivotPosition + "}";

    private void Awake() {
        CurrentState = PieceState.Initializing;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.gravityScale = 0;
    }
    private void Start() {
        StartCoroutine(HandleOutOfBoundsRoutine());
    }
    IEnumerator HandleOutOfBoundsRoutine() {
        var logId = "HandleOutOfBoundsRoutine";
        var waitForSeconds = new WaitForSeconds(0.1f);
        while(CurrentState!=PieceState.OutOfBounds) {
            if (transform.position.y <= destroyHeight) {
                CurrentState = PieceState.OutOfBounds;
                AudioManager.Instance.PlayPieceFellSound();
                OnOutOfBounds?.Invoke(this);
                logd(logId, "Piece="+this+" is out of bounds! Destroying.");
                Destroy(gameObject);
            } else if(CurrentState==PieceState.Controlled && transform.position.y <= minControlledHeight) {
                ReleasePiece();
            }
            yield return waitForSeconds;
        }
    }
    public GameObject Ghost { get; private set; } = null;
    public void InstantiateGhost() {
        var logId = "InstantiateGhostBlocks";
        var ghostBlockCount = GhostBlocks.Count;
        if (Ghost != null || ghostBlockCount != 0) {
            logw(logId, "Ghost="+Ghost.logf()+" BlocksCount="+ghostBlockCount+" => no-op");
            return;
        }
        Vector2 pivotPosition = transform.position;
        GhostBlocks = new List<GameObject>();
        Ghost = new GameObject("Ghost");
        var ghostTransform = Ghost.transform;
        ghostTransform.position = pivotPosition;

        var pieceBlockPositions = PieceConfiguration.blockPositions;
        foreach (var position in pieceBlockPositions) {
            Vector2 blockPosition = new Vector2(position.x, -position.y);
            GameObject ghostBlock = Instantiate(GhostPrefab, ghostTransform);
            logd(logId, "Instantiating GhostBlock at " + blockPosition);
            ghostBlock.transform.parent = ghostTransform;
            ghostBlock.transform.localPosition = blockPosition;
            GhostBlocks.Add(ghostBlock);
        }
        ghostTransform.localScale = new Vector3(0.5f, 0.5f, 1);
    }
    public List<GameObject> PieceBlocks = new List<GameObject>();
    public void Initialize(List<GameObject> pieceBlocks, float destroyHeight = -10f) {
        var logId = "Initialize";
        logd(logId, "Initializing Piece!");
        if(pieceBlocks.Count==0) {
            logw(logId, "Piece="+this.logf()+" Doesn't have PieceBlocks! Please check piece configuration data => no-op");
            Destroy(gameObject);
            return;
        }
        this.destroyHeight = destroyHeight;
        PivotPosition = PieceConfiguration.pivotPosition;
        PieceBlocks = pieceBlocks;
        CurrentState = PieceState.Controlled;
    }

    public void ReleasePiece() {
        var logId = "ReleasePiece";
        if(CurrentState==PieceState.Released) {
            logw(logId, "Piece already released!", true);
            return;
        }
        logd(logId, "Releasing Piece!");
        CurrentState = PieceState.Released;
        rb.isKinematic = false;
        rb.gravityScale = gravityScale;
        gameObject.layer = LayerMask.NameToLayer("Tower");
        if(transform.childCount > 0) {
            foreach(Transform t in transform) {
                t.gameObject.layer = LayerMask.NameToLayer("Tower");
            }
        }
        OnPieceReleased?.Invoke();
        if(Ghost) {
            Destroy(Ghost);
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        var logId = "OnTriggerEnter2D";
        var hitCollisionLayer = (settingLayers & 1<<collision.gameObject.layer) == 1<<collision.gameObject.layer;
        logd(logId, "HitCollision="+hitCollisionLayer+" Piece="+this.logf()+" Trigger Enter With "+collision.gameObject);
        if (hitCollisionLayer) {
            AudioManager.Instance.PlayPieceCollidedSound();
            ReleasePiece();
        }
    }
    public void UpdateGhostPosition() {
        var logId = "UpdateGhostPosition";
        var ghostBlocksCount = GhostBlocks.Count;
        for (int i = 0; i < ghostBlocksCount; i++) {
            var currentBlock = GhostBlocks[i];
            RaycastHit2D hit = Physics2D.Raycast(currentBlock.transform.position, Vector2.down, 10, settingLayers);
            var collider = hit.collider;
            var hitCollisionLayer = collider != null && (settingLayers & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer;
            if (hitCollisionLayer)
            {
                currentBlock.transform.position = hit.point;
                logt(logId, "Moving currentBlock=" + currentBlock.logf() + " to " + hit.point, true);
            }
        }
    }
    private void OnDestroy() {
        if(Ghost) {
            Destroy(Ghost);
        }
    }
}