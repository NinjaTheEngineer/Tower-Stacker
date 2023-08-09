using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;
public class Piece : NinjaMonoBehaviour {
    [SerializeField] LayerMask settingLayers;
    public Rigidbody2D rb;
    public enum PieceState { Initializing, Controlled, Released, OutOfBounds }
    [field: SerializeField] public GameObject BlockPrefab { get; private set; }
    [field: SerializeField] public PieceConfigurationData PieceConfiguration { get; private set; }
    [field: SerializeField] public Vector2 PivotPosition { get; private set;}
    public PieceState CurrentState { get; private set; }
    public Action OnPieceReleased;
    public static Action<Piece> OnOutOfBounds;
    public float destroyHeight;

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
    [SerializeField] float minControlledHeight = -1.5f;
    IEnumerator HandleOutOfBoundsRoutine() {
        var logId = "HandleOutOfBoundsRoutine";
        var waitForSeconds = new WaitForSeconds(0.1f);
        while(CurrentState!=PieceState.OutOfBounds) {
            if (transform.position.y <= destroyHeight) {
                CurrentState = PieceState.OutOfBounds;
                OnOutOfBounds?.Invoke(this);
                logd(logId, "Piece="+this+" is out of bounds! Destroying.");
                Destroy(gameObject);
            } else if(CurrentState==PieceState.Controlled && transform.position.y <= minControlledHeight) {
                ReleasePiece();
            }
            yield return waitForSeconds;
        }
    }

    public void Initialize(float destroyHeight = -10f) {
        this.destroyHeight = destroyHeight;
        PivotPosition = PieceConfiguration.pivotPosition;
        CurrentState = PieceState.Controlled;
    }

    public void ReleasePiece() {
        var logId = "ReleasePiece";
        if(CurrentState==PieceState.Released) {
            logw(logId, "Piece already released!");
            return;
        }
        logd(logId, "Releasing Piece!");
        CurrentState = PieceState.Released;
        rb.isKinematic = false;
        rb.gravityScale = 1;
        gameObject.layer = LayerMask.NameToLayer("Tower");
        if(transform.childCount > 0) {
            foreach(Transform t in transform) {
                t.gameObject.layer = LayerMask.NameToLayer("Tower");
            }
        }
        OnPieceReleased?.Invoke();
    }

    void OnCollisionEnter2D(Collision2D collision){
        var logId = "OnTriggerEnter2D";
        var hitCollisionLayer = (settingLayers & 1<<collision.gameObject.layer) == 1<<collision.gameObject.layer;
        logd(logId, "HitCollision="+hitCollisionLayer+" Piece="+this.logf()+" Trigger Enter With "+collision.gameObject);
        if (hitCollisionLayer) {
            ReleasePiece();
        }
    }
}