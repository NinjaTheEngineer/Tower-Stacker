using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using TMPro;

public class AIGameOverMenu : GameOverMenu {
    protected override void SetHeightText() {
        var logId = "SetHeightText";
        var currentHeight = gameManager.TowerHeightManager.CurrentHeight;
        logd(logId, "Setting HeightAmount to "+currentHeight);
        heightAmountText.text = currentHeight.ToString();
    }
    protected override IEnumerator HandleStateRoutine() {
        var logId = "HandleStateRoutine";
        logd(logId, "HandleStateRoutine Started!");
        var waitForSeconds = new WaitForSeconds(0.1f);
        while (true) {
            var currentGameState = GameManager.Instance.CurrentState;
            var isActive = IsActive;
            if (!isHiding && isActive && currentGameState!=GameManager.GameState.AIGameOver) {
                Hide();
            } else if (!isActive && currentGameState==GameManager.GameState.AIGameOver) {
                SetMenu();
                Show();
            }
            logd(logId, "CurrentGameState=" + currentGameState + " ActiveSelf=" + isActive, true);
            yield return waitForSeconds;
        }
    }
}