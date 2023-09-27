using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using UnityEngine.UI;
using System;

public class NextPieceIndicator : NinjaMonoBehaviour {
    [SerializeField] Image pieceBlockImage;
    [SerializeField] Transform pieceVisualizer;
    [SerializeField] float pieceDistanceMultiplier = 33f;
    List<Image> images = new List<Image>();
    private void Start() {
        if(images.Contains(pieceBlockImage)) {
            return;
        }
        images.Add(pieceBlockImage);
    }
    public void SetNextPiece(Piece piece) {
        var logId = "SetNextPiece";
        if(piece==null) {
            loge(logId, "Next piece is null => no-op");
            return;
        }
        var blockPositions = piece.PieceConfiguration.blockPositions;
        var blocksCount = blockPositions.Length;
        var pieceSprite = piece.PieceConfiguration.sprite;
        var imagesCount = images.Count;
        for (int i = 0; i < imagesCount; i++) {
            images[i].gameObject.SetActive(false);
        }
        Image currentImage;
        for (int currentImageIndex = 0; currentImageIndex < blocksCount; currentImageIndex++) {
            var currentBlockPos = blockPositions[currentImageIndex];
            
            if(currentImageIndex < imagesCount) {
                currentImage = images[currentImageIndex];
            } else {
                currentImage = Instantiate(pieceBlockImage, pieceVisualizer);  
                images.Add(currentImage);
            }
            currentImage.gameObject.SetActive(true);
            currentImage.sprite = pieceSprite;
            currentImage.transform.position = (Vector2)pieceVisualizer.position + new Vector2(currentBlockPos.x, -currentBlockPos.y)*pieceDistanceMultiplier;
        }

    }
}
