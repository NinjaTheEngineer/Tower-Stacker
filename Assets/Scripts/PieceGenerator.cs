using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PieceGenerator : NinjaMonoBehaviour {
    [SerializeField] TowerHeightManager towerHeightChecker;
    [SerializeField] List<Piece> pieces; 
    [SerializeField] PieceController pieceController;
    [SerializeField] float pieceScale = 0.35f;
    [SerializeField] float initHeight = 10;
    Piece lastGeneratedPiece;
    private void Awake() {
        var logId = "Awake";
        if(pieceController==null) {
            pieceController = FindObjectOfType<PieceController>();
            if(pieceController==null) {
                loge(logId, "No PieceController in the scene! No-op");
                Destroy(gameObject);
            }
        }
        if(towerHeightChecker==null) {
            towerHeightChecker = FindObjectOfType<TowerHeightManager>();
        }
    }
    public void Initialize() {
        GenerateRandomPiece();
        StartCoroutine(AdjustHeightRoutine());
        Piece.OnOutOfBounds += OnPieceOutOfBounds;
    }

    void OnPieceOutOfBounds(Piece piece) {
        var logId = "OnPieceOutOfBounds";
        logd(logId, "Piece="+piece);
        if(piece==lastGeneratedPiece && piece.CurrentState==Piece.PieceState.Controlled) {
            GenerateRandomPiece();
        }
    }

    IEnumerator AdjustHeightRoutine() {
        var logId = "AdjustHeightRoutine";
        var waitForSeconds = new WaitForSeconds(0.5f);
        while(true) {
            transform.position = new Vector2(0, initHeight + towerHeightChecker.CurrentHeight);
            yield return waitForSeconds;
        }
    }
    public void GenerateRandomPiece() {
        var logId = "GenerateRandomPiece";
        var piecesCount = pieces.Count;
        
        if(piecesCount==0) {
            logw(logId, "No pieces => no-op");
            return;
        }

        int randomPieceIndex = Random.Range(0, piecesCount);
        Vector2 pivotPosition = transform.position;
        if(lastGeneratedPiece) {
            lastGeneratedPiece.OnPieceReleased -= OnPieceReleased;
        }
        lastGeneratedPiece = Instantiate(pieces[randomPieceIndex], pivotPosition, Quaternion.identity);

        logd(logId, "Generating LastGeneratedPiece="+lastGeneratedPiece.logf()+" with index="+randomPieceIndex);
        var pieceBlockPositions = lastGeneratedPiece.PieceConfiguration.blockPositions;

        foreach (var position in pieceBlockPositions) {
            Vector2 blockPosition = pivotPosition + new Vector2(position.x, -position.y);
            GameObject newBlock = Instantiate(lastGeneratedPiece.BlockPrefab, blockPosition, Quaternion.identity);
            newBlock.transform.parent = lastGeneratedPiece.transform;
        }
        logd(logId, "Setting Controlled Piece to Piece="+lastGeneratedPiece.logf()+".");
        lastGeneratedPiece.transform.localScale = new Vector2(pieceScale, pieceScale);
        lastGeneratedPiece.OnPieceReleased += OnPieceReleased;
        pieceController.SetControlledPiece(lastGeneratedPiece);
    }
    public void OnPieceReleased() {
        GenerateRandomPiece();
    }
    private void OnDisable() {
        StopAllCoroutines();
        Piece.OnOutOfBounds -= OnPieceOutOfBounds;
    }
}