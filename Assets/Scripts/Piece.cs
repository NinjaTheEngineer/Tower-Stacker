using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;

public class Piece : NinjaMonoBehaviour
{
    [SerializeField] LayerMask collisionLayer;
    public Rigidbody2D rb;
    public enum PieceState { Initializing, Controlled, Free }
    [field: SerializeField] public GameObject BlockPrefab { get; private set; }
    [field: SerializeField] public PieceConfigurationData PieceConfiguration { get; private set; }
    [field: SerializeField] public Vector2 PivotPosition { get; private set;}
    public PieceState CurrentState { get; private set; }
    public float destroyHeight;

    public override string ToString() => "{PieceConfiguration=" + PieceConfiguration.logf() + " CurrentState=" + CurrentState.logf() + " DestroyHeight=" + destroyHeight + " PivotPosition=" + PivotPosition + "}";

    private void Awake()
    {
        CurrentState = PieceState.Initializing;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.gravityScale = 0;
    }

    private void Update()
    {
        if (transform.position.y <= destroyHeight)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float scale = 0.35f, float destroyHeight = -10f)
    {
        this.destroyHeight = destroyHeight;
        transform.localScale = new Vector3(scale, scale, scale);
        PivotPosition = PieceConfiguration.pivotPosition;
        CurrentState = PieceState.Controlled;
    }

    public void FreePiece() {
        var logId = "FreePiece";
        logd(logId, "Freeing Piece!");
        CurrentState = PieceState.Free;
        rb.isKinematic = false;
        rb.gravityScale = 1;
        gameObject.layer = LayerMask.NameToLayer("Solid");
    }

    void OnCollisionEnter2D(Collision2D collision){
        var logId = "OnTriggerEnter2D";
        var hitCollisionLayer = (collisionLayer.value & 1<<collision.gameObject.layer) == 1<<collision.gameObject.layer;
        logd(logId, "HitCollision="+hitCollisionLayer+" Piece="+this.logf()+" Trigger Enter With "+collision.gameObject);
        if (hitCollisionLayer) {
            FreePiece();
        }
    }
}