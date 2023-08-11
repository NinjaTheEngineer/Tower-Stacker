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
        int seconds = Mathf.FloatToHalf(elapsedTimeInGame % 60f);
        var text = "In "+(minutes==0?"":minutes+" minutes")+ "and "+((minutes==0) ? ((seconds==0) ? "": (seconds+" seconds")) : (" and "+seconds+" seconds")) + "!";
        logd(logId, "Setting Time to "+minutes+" minutes and "+seconds+" while ElapsedTimeInGame="+elapsedTimeInGame, true);
        timePassedText.text = text;
    }

    public void OnRestartButtonClick() {
        var logId = "OnRestartButtonClick";
        logd(logId, "Restart button clicked => Restarting game");
        GameManager.Instance.RestartGame();
    }
    public void OnBackButtonClick() {
        var logId = "OnBackButtonClick";
        logd(logId, "Back button clicked => Returning to MainMenu");
        SceneManager.Instance.OpenScene(SceneName.MainMenu);
    }
}