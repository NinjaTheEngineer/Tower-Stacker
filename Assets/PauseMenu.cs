using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PauseMenu : MenuController {
    [SerializeField]GameObject pauseButton;
    protected override void Initialize() {
        base.Initialize();
        StartCoroutine(HandleStateRoutine());
    }

    IEnumerator HandleStateRoutine(){
        var logId = "HandleStateRoutine";
        logd(logId, "HandleStateRoutine Started!");
        var waitForSeconds = new WaitForSecondsRealtime(0.1f);
        while (true) {
            var currentGameState = GameManager.Instance.CurrentState;
            var isActive = IsActive;
            if (!isHiding && isActive && currentGameState!=GameManager.GameState.Paused) {
                pauseButton.SetActive(true);
                Hide();
            } else if (!isActive && currentGameState==GameManager.GameState.Paused) {
                pauseButton.SetActive(false);
                Show();
            }
            logd(logId, "CurrentGameState=" + currentGameState + " ActiveSelf=" + isActive, true);
            yield return waitForSeconds;
        }
    }

    public void OnRestartButtonClick() {
        var logId = "OnRestartButtonClick";
        logd(logId, "Restart button clicked!");
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.RestartGame();
    }
    public void OnResumeButtonClick() {
        var logId = "OnResumeButtonClick";
        logd(logId, "Resume button clicked!");
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.ResumeGame();
    }
    public void OnPauseButtonClick() {
        var logId = "OnPauseButtonClick";
        logd(logId, "Pause button clicked!");
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.PauseGame();
    }
    public void OnBackButtonClick() {
        var logId = "OnBackButtonClick";
        logd(logId, "Back button clicked => Returning to MainMenu");
        AudioManager.Instance.PlayButtonClick();
        SceneManager.Instance.OpenScene(SceneName.MainMenu);
    }
}