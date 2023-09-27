using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PieceGenerator : NinjaMonoBehaviour {
    [SerializeField] TowerHeightManager towerHeightManager;
    [SerializeField] List<Piece> pieces; 
    [SerializeField] PieceController pieceController;
    NextPieceIndicator nextPieceIndicator;
    [SerializeField] float pieceScale = 0.35f;
    [SerializeField] float initHeight = 10;
    [SerializeField] float pieceDestroyHeight = -2f;
    float initX;
    Piece currentPiece;
    List<Piece> generatedPieces = new List<Piece>();
    private void Awake() {
        if(towerHeightManager==null) {
            towerHeightManager = FindObjectOfType<TowerHeightManager>();
        }
        nextPieceIndicator = FindObjectOfType<NextPieceIndicator>();
        logd("Awake", "NextPieceIndicator=" + nextPieceIndicator.logf());
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
        StartCoroutine(GeneratePieceRoutine());
        GenerateRandomPiece();
        initX = transform.position.x;
        StartCoroutine(AdjustHeightRoutine());
    }
    bool isGenerating;
    IEnumerator GeneratePieceRoutine() {
        var logId = "GeneratePieceRoutine";
        var waitForSeconds = new WaitForSeconds(0.15f);
        var waitForSecondsLong = new WaitForSeconds(2f);
        int currentPieceIndex = -1;
        int nextPieceIndex = -1;
        bool duplicatedPieces = false;
        isGenerating = true;
        while (isGenerating) {
            if(currentPiece!=null && currentPiece.CurrentState==Piece.PieceState.Controlled) {
                logd(logId, "CurrentPiece=" + currentPiece.logf() + " PieceState=" + (currentPiece == null ? "NULL" : currentPiece.CurrentState), true);
                yield return waitForSeconds;
                continue;
            }

            var piecesCount = pieces.Count;
            logd(logId, "Starting to generate new piece!");
            if (piecesCount == 0) {
                logw(logId, "No pieces => no-op");
                yield break;
            }

            Vector2 pivotPosition = transform.position;
            currentPieceIndex = currentPieceIndex==-1?Random.Range(0, piecesCount):nextPieceIndex;
            while(duplicatedPieces && currentPieceIndex==nextPieceIndex) {
                logd(logId, "Restraining duplicated pieces from new duplication case.");
                currentPieceIndex = Random.Range(0, piecesCount);
            }

            yield return null;
            
            nextPieceIndex = Random.Range(0, piecesCount);
            duplicatedPieces = currentPieceIndex == nextPieceIndex;
            if (currentPiece) {
                currentPiece.OnPieceReleased -= OnPieceReleased;
            }
            nextPieceIndicator.SetNextPiece(pieces[nextPieceIndex]);
            currentPiece = Instantiate(pieces[currentPieceIndex], pivotPosition, Quaternion.identity);

            logd(logId, "Generating CurrentPiece="+currentPiece.logf()+" with index="+currentPieceIndex+" while nextIndex="+nextPieceIndex);
            var pieceBlockPositions = currentPiece.PieceConfiguration.blockPositions;

            currentPiece.InstantiateGhost();



            List<GameObject> pieceBlocks = new List<GameObject>();
            foreach (var position in pieceBlockPositions) {
                Vector2 blockPosition = pivotPosition + new Vector2(position.x, -position.y);
                GameObject newBlock = Instantiate(currentPiece.BlockPrefab, blockPosition, Quaternion.identity);
                pieceBlocks.Add(newBlock);
                newBlock.transform.parent = currentPiece.transform;
            }
            logd(logId, "Setting Controlled Piece for PieceController=" + pieceController.logf() + " to Piece=" + currentPiece.logf() + ".");
            currentPiece.transform.localScale = new Vector2(pieceScale, pieceScale);
            currentPiece.OnPieceReleased += OnPieceReleased;
            generatedPieces.Add(currentPiece);
            pieceController.SetControlledPiece(currentPiece, pieceBlocks);
            yield return waitForSecondsLong;
        }
    }

    public void StopGenerationn() {
        isGenerating = false;
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
        if(piece==currentPiece && piece.CurrentState==Piece.PieceState.Controlled) {
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
        return;
        var logId = "GenerateRandomPiece";
        var piecesCount = pieces.Count;
        
        logd(logId, "Starting to generate new piece!");
        if (piecesCount==0) {
            logw(logId, "No pieces => no-op");
            return;
        }
        int randomPieceIndex = Random.Range(0, piecesCount);
        Vector2 pivotPosition = transform.position;
        if(currentPiece) {
            currentPiece.OnPieceReleased -= OnPieceReleased;
        }
        currentPiece = Instantiate(pieces[randomPieceIndex], pivotPosition, Quaternion.identity);

        logd(logId, "Generating LastGeneratedPiece="+currentPiece.logf()+" with index="+randomPieceIndex);
        var pieceBlockPositions = currentPiece.PieceConfiguration.blockPositions;

        currentPiece.InstantiateGhost();

        List<GameObject> pieceBlocks = new List<GameObject>();
        foreach (var position in pieceBlockPositions) {
            Vector2 blockPosition = pivotPosition + new Vector2(position.x, -position.y);
            GameObject newBlock = Instantiate(currentPiece.BlockPrefab, blockPosition, Quaternion.identity);
            pieceBlocks.Add(newBlock);
            newBlock.transform.parent = currentPiece.transform;
        }
        logd(logId, "Setting Controlled Piece for PieceController="+pieceController.logf()+" to Piece="+currentPiece.logf()+".");
        currentPiece.transform.localScale = new Vector2(pieceScale, pieceScale);
        currentPiece.OnPieceReleased += OnPieceReleased;
        generatedPieces.Add(currentPiece);
        pieceController.SetControlledPiece(currentPiece, pieceBlocks);
    }
    public List<GameObject> ghostBlocks;
    GameObject ghost;
    
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