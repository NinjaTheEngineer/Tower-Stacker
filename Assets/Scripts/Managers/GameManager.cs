using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;

public class GameManager : NinjaMonoBehaviour {
    [SerializeField] MultipleTargetCamera multipleTargetCamera;

    public PieceGenerator PieceGenerator { get; private set;}
    public PieceGenerator AIPieceGenerator { get; private set;}
    [SerializeField] PieceGenerator pieceGenerator;

    public AIController AIController { get; private set; }
    [SerializeField] AIController aiController;

    public PieceController PieceController { get; private set;}
    [SerializeField] PieceController pieceController;

    public TowerHeightManager TowerHeightManager { get; private set;}
    public TowerHeightManager AITowerHeightManager { get; private set;}
    [SerializeField] TowerHeightManager towerHeightManager;

    public TimeManager TimeManager { get; private set;}
    [SerializeField] TimeManager timeManager;

    public ChainsManager AIChainsManager { get; private set;}
    public ChainsManager ChainsManager { get; private set;}
    [SerializeField] ChainsManager chainsManager;

    [SerializeField] TouchInputController touchInputController;

    public enum GameState { Initializing, Playing, CheckingWinCondition, Paused, GameOver, AIGameOver };
    GameState _currentState;
    public GameState CurrentState { 
        get =>_currentState;
        private set {
            var logId = "CurrentState_set";
            logd(logId, "Setting CurrentState from "+_currentState+" to "+value);
            _currentState = value;
        }
    }
    public static Action OnGameStart;
    public static Action OnGameOver;
    public static Action OnGamePaused;
    public float goalHeight = 40;
    public float aiDistance;
    public static GameManager Instance {get; private set;}
    private void Awake() {
        if(Instance==null) {
            Instance = this;
        } else if(Instance!=this) {
            Destroy(gameObject);
        }
        Utils.InitRandom();
        Application.targetFrameRate = 60;
    }
    bool isGameWithAI = false;
    private void Start() {
        var logId = "Start";
        CurrentState = GameState.Initializing;
        logd(logId, "Starting with "+touchInputController.logf());
        StartCoroutine(InitializeRoutine());
    }
    IEnumerator InitializeRoutine() {
        var logId = "InitializeRoutine";
        logd(logId, "Starting Initialize Routine!");
        var waitForSeconds = new WaitForSecondsRealtime(0.02f);
        TowerHeightManager = Instantiate(towerHeightManager);
        yield return waitForSeconds;
        touchInputController = TouchInputController.Instance;
        yield return waitForSeconds;
        TimeManager = Instantiate(timeManager);
        yield return waitForSeconds;
        isGameWithAI = aiController!=null;
        if(isGameWithAI) {
            StartCoroutine(InitializeAIRoutine());
        } else {
            ChainsManager = Instantiate(chainsManager);
            yield return waitForSeconds;
        }
        
        PieceController = Instantiate(pieceController);
        yield return waitForSeconds;
        PieceGenerator = Instantiate(pieceGenerator);
        PieceGenerator.SetPieceController(PieceController);
        yield return waitForSeconds;
        SetUpCameraTargets();
        InitGame();
    }
    void SetUpCameraTargets() {
        if(!isGameWithAI) {
            multipleTargetCamera.AddTargets(new(){PieceGenerator.transform, TowerHeightManager.transform});
        } else {
            multipleTargetCamera.AddTargets(new(){PieceGenerator.transform, TowerHeightManager.transform, AIPieceGenerator.transform, AITowerHeightManager.transform});
        }
    }
    IEnumerator InitializeAIRoutine() {
        var logId = "InitializeAIRoutine";
        var waitForSeconds = new WaitForSecondsRealtime(0.02f);

        if(!isGameWithAI) {
            logw(logId, "Game is not with AI => no-op");
            yield break;
        }
        
        AITowerHeightManager = Instantiate(towerHeightManager);
        var newPos = new Vector3(aiDistance, towerHeightManager.transform.position.y, towerHeightManager.transform.position.z);
        AITowerHeightManager.transform.position = newPos;
        
        AIController = aiController;
        AIController.transform.position = newPos;

        yield return waitForSeconds;
        
        AIPieceGenerator = Instantiate(pieceGenerator);
        AIPieceGenerator.SetPieceController(AIController);
        newPos = new Vector3(aiDistance, pieceGenerator.transform.position.y, pieceGenerator.transform.position.z);
        AIPieceGenerator.transform.position = newPos;
    }
    public void InitGame() {
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
        TimeManager.StartTimer();
        TowerHeightManager.Initialize();
        PieceGenerator.Initialize();
        ChainsManager?.Initialize();
        InitAI();
        StartCoroutine(CheckWinConditionRoutine());
    }
    public void InitAI() {
        var logId = "InitAI";
        if(!isGameWithAI) {
            logw(logId, "Game is not with AI => no-op");
            return;
        }
        AIPieceGenerator.Initialize();
        AITowerHeightManager.Initialize(true);
        StartCoroutine(AICheckWinConditionRoutine());
    }
    IEnumerator CheckWinConditionRoutine() {
        var logId = "CheckWinConditionRoutin";
        var waitForSeconds = new WaitForSeconds(0.2f);
        logd(logId, "Starting CheckWinCondition Routine");
        while(CurrentState!=GameState.GameOver) {
            if(TowerHeightManager.CurrentHeight >= goalHeight) {
                GameOver();
            }
            yield return waitForSeconds;
        }
    }
    IEnumerator AICheckWinConditionRoutine() {
        var logId = "AICheckWinConditionRoutin";
        var waitForSeconds = new WaitForSeconds(0.2f);
        logd(logId, "Starting AICheckWinCondition Routine");
        while(CurrentState!=GameState.AIGameOver) {
            if(AITowerHeightManager.CurrentHeight >= goalHeight) {
                AIGameOver();
            }
            yield return waitForSeconds;
        }
    }
    public void GameOver() {
        CurrentState = GameState.GameOver;
        OnGameOver?.Invoke();
    }
    public void AIGameOver() {
        CurrentState = GameState.AIGameOver;
        OnGameOver?.Invoke();
    }
    void Update() {
        var logId = "Update";
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(CurrentState==GameState.Playing) {
                PauseGame();
            } else {
                ResumeGame();
            }
        }
        if(touchInputController==null) {
            logw(logId, "TouchInputController is null => no-op", true);
            return;
        }
        if(touchInputController.IsUITap && CurrentState==GameState.Paused) {
            ResumeGame();
        }  
    }
    public void ResumeGame() {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
    }
    public void PauseGame() {
        CurrentState = GameState.Paused;
        OnGamePaused?.Invoke();
        Time.timeScale = 0f;
    }
    public void RestartGame() {
        var logId = "RestartGame";
        logd(logId, "Restarting Game...");
        StopAllCoroutines();
        InitGame();
    }
}
