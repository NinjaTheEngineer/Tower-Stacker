using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;

public class TimeManager : NinjaMonoBehaviour {
    float startTime = 0;
    float elapsedTimeInGame = 0;
    GameManager gameManager;
    bool isCounting = false;
    private void Start() {
        gameManager = GameManager.Instance;
    }

    public void StartTimer() {
        startTime = Time.deltaTime;
        isCounting = true;
        StartCoroutine(ElapseTimeRoutine());
    }
    IEnumerator ElapseTimeRoutine() {
        var logId = "ElapseTimeRoutine";
        logd(logId, "Starting ElapseTime Routine");
        var currentState = gameManager.CurrentState;
        var waitForSeconds = new WaitForSecondsRealtime(0.01f);
        while(isCounting) {
            if(currentState==GameManager.GameState.Playing) {
                elapsedTimeInGame = Time.deltaTime - startTime;
            } else if(currentState==GameManager.GameState.GameOver || currentState==GameManager.GameState.AIGameOver) {
                StopTimer();
            }
            yield return waitForSeconds;
        }
    }
    public void StopTimer() {
        isCounting = false;
        StopAllCoroutines();
    }
    public float ElapsedTimeInGame => elapsedTimeInGame;
}