using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PieceGenerator : NinjaMonoBehaviour {
    [SerializeField] TowerHeightManager towerHeightManager;
    [SerializeField] List<Piece> pieces; 
    [SerializeField] PieceController pieceController;
    [SerializeField] float pieceScale = 0.35f;
    [SerializeField] float initHeight = 10;
    float initX;
    Piece lastGeneratedPiece;
    List<Piece> generatedPieces = new List<Piece>();
    private void Awake() {
        if(towerHeightManager==null) {
            towerHeightManager = FindObjectOfType<TowerHeightManager>();
        }
        Piece.OnOutOfBounds += OnPieceOutOfBounds;
        GameManager.OnGameOver += StopGenerationn;
        GameManager.OnGameStart += Initialize;
    }
    public void SetTowerHeightManager(TowerHeightManager towerHeightManager) {
        var logId = "SetTowerHeightManager";
        if(towerHeightManager==null) {
            logw(logId, "Tried to set TowerHeightManager to null => no-op");
            return;
        }
        this.towerHeightManager = towerHeightManager;
    } 
    public void SetPieceController(PieceController pc) {
        var logId = "SetPieceController";
        pieceController = pc;
        if(pieceController==null) {
            loge(logId, "No PieceController in the scene! No-op");
            Destroy(gameObject);
            return;
        }
    }
    public void Initialize() {
        var logId = "Initialize";
        logd(logId, "Initializing PieceGenerator");
        DestroyGeneratedPieces();
        GenerateRandomPiece();
        initX = transform.position.x;
        StartCoroutine(AdjustHeightRoutine());
    }
    public void StopGenerationn() {
        StopAllCoroutines();
    }
    void DestroyGeneratedPieces() {
        var logId = "Reset";
        var generatedPiecesCount = generatedPieces.Count;
        if(generatedPiecesCount==0) {
            logd(logId, "No generated pieces found.");
            return;
        }
        for (int i = 0; i < generatedPiecesCount; i++) {
            var currentPiece = generatedPieces[i];
            if(currentPiece==null) {
                continue;
            }
            Destroy(generatedPieces[i].gameObject);
        }
        logd(logId, "Finished destroying "+generatedPiecesCount+" pieces.");
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
        logd(logId, "Starting AdjustHeight Routine");
        var waitForSeconds = new WaitForSeconds(0.5f);
        while(true) {
            transform.position = new Vector2(initX, initHeight + towerHeightManager.RawHeight);
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
        logd(logId, "Setting Controlled Piece for PieceController="+pieceController.logf()+" to Piece="+lastGeneratedPiece.logf()+".");
        lastGeneratedPiece.transform.localScale = new Vector2(pieceScale, pieceScale);
        lastGeneratedPiece.OnPieceReleased += OnPieceReleased;
        generatedPieces.Add(lastGeneratedPiece);
        pieceController.SetControlledPiece(lastGeneratedPiece);
    }
    public void OnPieceReleased() {
        GenerateRandomPiece();
    }
    private void OnDisable() {
        StopAllCoroutines();
        Piece.OnOutOfBounds -= OnPieceOutOfBounds;
        GameManager.OnGameOver -= StopGenerationn;
        GameManager.OnGameStart -= Initialize;
    }
}