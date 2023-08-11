using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using TMPro;
using System;

public class GameOverMenu : MenuController {
    [SerializeField] protected TextMeshProUGUI heightAmountText;
    [SerializeField] protected TextMeshProUGUI timePassedText;
    protected GameManager gameManager;
    protected override void Initialize() {
        base.Initialize();
        gameManager = GameManager.Instance;
        StartCoroutine(HandleStateRoutine());
    }

    IEnumerator HandleStateRoutine() {
        var logId = "HandleStateRoutine";
        logd(logId, "HandleStateRoutine Started!");
        var waitForSeconds = new WaitForSeconds(0.1f);
        while (true) {
            var currentGameState = GameManager.Instance.CurrentState;
            var isActive = IsActive;
            if (!isHiding && isActive && currentGameState!=GameManager.GameState.GameOver) {
                Hide();
            } else if (!isActive && currentGameState==GameManager.GameState.GameOver) {
                SetMenu();
                Show();
            }
            logd(logId, "CurrentGameState=" + currentGameState + " ActiveSelf=" + isActive, true);
            yield return waitForSeconds;
        }
    }
    void SetMenu() {
        var logId = "SetMenu";
        logd(logId, "Setting Menu..",true);
        if(heightAmountText) {
            SetHeightText();
        }
        if(timePassedText) {
            SetTimeText();
        }
    }
    protected virtual void SetHeightText() {
        var logId = "SetHeightText";
        var currentHeight = gameManager.TowerHeightManager.CurrentHeight;
        logd(logId, "Setting HeightAmount to "+currentHeight);
        heightAmountText.text = currentHeight.ToString();
    }
    void SetTimeText() {
        var logId = "SetTimeText";
        var elapsedTimeInGame = gameManager.TimeManager.ElapsedTimeInGame;
        int minutes = Mathf.FloorToInt(elapsedTimeInGame / 60f);
        int seconds = Mathf.FloorToInt(elapsedTimeInGame % 60f);
        var text = "In ";
        logd(logId, "Setting Time text with Minutes="+minutes+" Seconds="+seconds);
        if(minutes > 0) {
            text += minutes+" minute"+(minutes > 1 ? "s" : "");
            if (seconds > 0) {
                text += " and ";
            }
        }
        
        if(seconds > 0) {
            text += seconds+" second"+(seconds > 1 ? "s" : "");
        }
        timePassedText.text = text;
    }

    public void OnRestartButtonClick() {
        var logId = "OnRestartButtonClick";
        logd(logId, "Restart button clicked => Restarting game");
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.RestartGame();
    }
    public void OnBackButtonClick() {
        var logId = "OnBackButtonClick";
        logd(logId, "Back button clicked => Returning to MainMenu");
        AudioManager.Instance.PlayButtonClick();
        SceneManager.Instance.OpenScene(SceneName.MainMenu);
    }
}