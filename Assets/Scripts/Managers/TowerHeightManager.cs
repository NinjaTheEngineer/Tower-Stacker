using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using TMPro;

public class TowerHeightManager : NinjaMonoBehaviour {
    [SerializeField] TextMeshProUGUI towerHeightText;
    [SerializeField] TextMeshProUGUI aiTowerHeightText;
    [SerializeField] LayerMask towerLayer;
    [SerializeField] float towerWidth = 5f;
    [SerializeField] float raycastDistance = 50f;
    [SerializeField] float heightMultiplier = 2f;
    float rawHeight;
    public float RawHeight => rawHeight;
    public int CurrentHeight => Mathf.RoundToInt(rawHeight * heightMultiplier);
    float highestHeightReached;
    public int HighestHeightReached => Mathf.RoundToInt(highestHeightReached * heightMultiplier);
    GameManager gameManager;
    bool isAIControlled = false;
    public void Initialize(bool isAI=false) {
        StopAllCoroutines();
        isAIControlled = isAI;
        rawHeight = 0;
        gameManager = GameManager.Instance;
        towerHeightText.gameObject.SetActive(false);
        aiTowerHeightText.gameObject.SetActive(false);
        StartCoroutine(CalculateTowerHeightRoutine());
        StartCoroutine(HandleTextStateRoutine());
    }
    IEnumerator CalculateTowerHeightRoutine() {
        var logId = "CalculateTowerHeightRoutine";
        var waitForSeconds = new WaitForSeconds(0.2f);
        while (true) {
            yield return waitForSeconds;
            
            if(gameManager.CurrentState!=GameManager.GameState.Playing) {
                continue;
            }
            float highestHitY = 0; // Initialize to a low value

            Vector2 raycastOrigin = transform.position;
            Vector2 raycastDirection = Vector2.up;

            RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastOrigin, new Vector2(towerWidth, raycastDistance), 0f, raycastDirection, raycastDistance, towerLayer);
            foreach (var hit in hits) {
                var hitCollider = hit.collider;
                logd(logId, "HitCollider=" + hitCollider.logf(), true);
                if (hitCollider != null) {
                    if (hit.point.y > highestHitY) {
                        logt(logId, "PointHitY="+hit.point.y+" HighestHitY="+highestHitY, true);
                        highestHitY = hit.point.y;
                    }
                }
            }
            if(highestHeightReached<highestHitY) {
                highestHeightReached = highestHitY;
            }
            rawHeight = highestHitY;
        }
    }
    IEnumerator HandleTextStateRoutine() {
        var logId = "HandleTextStateRoutine";
        logd(logId, "Starting HandleTextState Routine.");
        var waitForSeconds = new WaitForSecondsRealtime(0.2f);
        while (true) {
            var currentState = gameManager.CurrentState;
            var textVisible = towerHeightText.isActiveAndEnabled || aiTowerHeightText.isActiveAndEnabled;
            var isPlaying = currentState==GameManager.GameState.Playing || currentState==GameManager.GameState.CheckingWinCondition;
            var heightScore = Mathf.RoundToInt(highestHeightReached * heightMultiplier).ToString();
            towerHeightText.text = heightScore;
            aiTowerHeightText.text = heightScore;
            if(isPlaying && !textVisible) {
                towerHeightText.gameObject.SetActive(!isAIControlled && true);
                aiTowerHeightText.gameObject.SetActive(isAIControlled && true);
            } else if(!isPlaying && textVisible) {
                towerHeightText.gameObject.SetActive(false);
                aiTowerHeightText.gameObject.SetActive(false);
            }
            yield return waitForSeconds;
        }
    }
}