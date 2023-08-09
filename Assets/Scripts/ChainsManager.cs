using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class ChainsManager : NinjaMonoBehaviour {
    [SerializeField] List<ChainHolder> chainHolders;
    int chainsRemaining;
    private void Start() {
        Piece.OnOutOfBounds += OnPieceOutOfBounds;
        Initialize();
    }
    public void Initialize() {
        gameObject.SetActive(true);
        chainsRemaining = chainHolders.Count;
        for (int i = 0; i < chainsRemaining; i++) {
            chainHolders[i].EnableChains();
        }
    }

    void OnPieceOutOfBounds(Piece piece) {
        var logId = "OnPieceOutOfBounds";
        logd(logId, "Piece="+piece.logf()+" is out of bounds!");   
        if(chainsRemaining>0) {
            chainsRemaining--;
            chainHolders[chainsRemaining].DisableChains();
        }
        if(chainsRemaining<=0) {
            GameManager.Instance.GameOver();
        }
    }
    
    private void OnDestroy() {
        Piece.OnOutOfBounds -= OnPieceOutOfBounds;
    }
}
