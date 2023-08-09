using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;

public class GameManager : NinjaMonoBehaviour {
    [SerializeField] MultipleTargetCamera multipleTargetCamera;
    [SerializeField] PieceGenerator pieceGeneratorPf;
    public PieceGenerator PieceGenerator { get; private set;}
    [SerializeField] PieceController pieceControllerPf;
    public PieceController PieceController { get; private set;}
    [SerializeField] TowerHeightManager towerHeightManagerPf;
    public TowerHeightManager TowerHeightManager { get; private set;}
    [SerializeField] ChainsManager chainsManagerPf;
    public ChainsManager ChainsManager { get; private set;}

    public float timeScaleDuration;
    public enum GameState { Playing, Paused, GameOver };
    public GameState CurrentState { get; private set;}
    public static Action OnGameStart;
    public static Action OnGameOver;
    public static Action OnGamePaused;
    public static GameManager Instance {get; private set;}
    private void Awake() {
        if(Instance==null) {
            Instance = this;
        } else if(Instance!=this) {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;
    }

    private void Start() {
        StartCoroutine(InitializeRoutine());
    }
    IEnumerator InitializeRoutine() {
        var logId = "InitializeRoutine";
        var waitForSeconds = new WaitForSeconds(0.01f);
        TowerHeightManager = Instantiate(towerHeightManagerPf);
        yield return waitForSeconds;
        ChainsManager = Instantiate(chainsManagerPf);
        yield return waitForSeconds;
        PieceController = Instantiate(pieceControllerPf);
        yield return waitForSeconds;
        PieceGenerator = Instantiate(pieceGeneratorPf);
        PieceGenerator.Initialize();
        yield return waitForSeconds;
        multipleTargetCamera.AddTargets(new(){PieceGenerator.transform, ChainsManager.transform});
        CurrentState = GameState.Playing;
    }
    public void GameOver() {
        CurrentState = GameState.GameOver;
        OnGameOver?.Invoke();
    }
    void Update() {
        if(CurrentState==GameState.Playing) {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                PauseGame();
            }
        }    
    }
    void PauseGame() {
        CurrentState = GameState.Paused;
        OnGamePaused?.Invoke();
    }
    public void RestartGame() {
        var logId = "RestartGame";
        logd(logId, "Restart button clicked!");
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        PieceGenerator.Initialize();
        ChainsManager.Initialize();
    }
}
