using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using TMPro;

public class TowerHeightManager : NinjaMonoBehaviour {
    [SerializeField] TextMeshProUGUI towerHeightText;
    [SerializeField] LayerMask towerLayer;
    [SerializeField] float towerWidth = 5f;
    [SerializeField] float raycastDistance = 50f;
    [SerializeField] float heightMultiplier = 2f;
    public float TargetHeight { get; private set; }
    float rawHeight;
    public float RawHeight => rawHeight;
    public float CurrentHeight => Mathf.RoundToInt(rawHeight * heightMultiplier);
    GameManager gameManager;
    bool isAIControlled = false;
    public void Initialize(bool isAI=false) {
        StopAllCoroutines();
        isAIControlled = isAI;
        rawHeight = 0;
        gameManager = GameManager.Instance;
        StartCoroutine(CalculateTowerHeightRoutine());
        if(isAIControlled) {
            towerHeightText.gameObject.SetActive(false);
            return;
        }
        StartCoroutine(HandleTextStateRoutine());
    }
    IEnumerator HandleTextStateRoutine() {
        var logId = "HandleTextStateRoutine";
        logd(logId, "Starting HandleTextState Routine.");
        var waitForSeconds = new WaitForSecondsRealtime(0.2f);
        while (true) {
            var currentState = gameManager.CurrentState;
            var textVisible = towerHeightText.isActiveAndEnabled;
            var isPlaying = currentState==GameManager.GameState.Playing || currentState==GameManager.GameState.CheckingWinCondition;
            towerHeightText.text = Mathf.RoundToInt(rawHeight * heightMultiplier).ToString();
            if(isPlaying && !textVisible) {
                towerHeightText.gameObject.SetActive(true);
            } else if(!isPlaying && textVisible) {
                towerHeightText.gameObject.SetActive(false);
            }
            yield return waitForSeconds;
        }
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

            rawHeight = highestHitY;
        }
    }
}