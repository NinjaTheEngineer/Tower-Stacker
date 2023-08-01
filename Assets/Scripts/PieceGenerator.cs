using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PieceGenerator : NinjaMonoBehaviour {
    [SerializeField] List<Piece> pieces; 
    [SerializeField] PieceController pieceController;
    private void Awake() {
        var logId = "Awake";
        if(pieceController==null) {
            pieceController = FindObjectOfType<PieceController>();
            if(pieceController==null) {
                loge(logId, "No PieceController in the scene! No-op");
                Destroy(gameObject);
            }
        }
    }
    private void Start() {
        var logId = "Start";
        var piecesCount = pieces.Count;
        if(piecesCount==0) {
            logd(logId, "No pieces => no-op");
            return;
        }
        GenerateRandomPiece();
    }

    private void Update() {
        var logId = "Update";

        var piecesCount = pieces.Count;
        if(piecesCount==0) {
            logd(logId, "No pieces => no-op");
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            GenerateRandomPiece();
        }
    }

    // Generate a new piece based on the specified configuration index
    public void GenerateRandomPiece() {
        var logId = "GenerateRandomPiece";
        var piecesCount = pieces.Count;
        
        if(piecesCount==0) {
            logw(logId, "No pieces => no-op");
            return;
        }

        int randomPieceIndex = Random.Range(0, piecesCount);
        Vector2 pivotPosition = transform.position;
        var randomPiece = Instantiate(pieces[randomPieceIndex], pivotPosition, Quaternion.identity);

        logd(logId, "Generating randomPiece="+randomPiece.logf()+" with index="+randomPieceIndex);
        var randomPieceBlockPositions = randomPiece.PieceConfiguration.blockPositions;

        foreach (var position in randomPieceBlockPositions) {
            Vector2 blockPosition = pivotPosition + new Vector2(position.x, -position.y);
            GameObject newBlock = Instantiate(randomPiece.BlockPrefab, blockPosition, Quaternion.identity);
            newBlock.transform.parent = randomPiece.transform;
        }
        logd(logId, "Setting Controlled Piece to Piece="+randomPiece.logf()+".");
        pieceController.SetControlledPiece(randomPiece);
    }
}