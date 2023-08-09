using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class GameOverMenu : NinjaMonoBehaviour {
    public GameObject visu;
    private RectTransform visuRectTransform;
    private Vector3 targetPosition;
    private Vector3 initPosition;
    public float animationDuration = 2f;

    void Start() {
        visuRectTransform = visu.GetComponent<RectTransform>();
        StartCoroutine(HandleStateRoutine());
        initPosition = transform.position;
        visu?.SetActive(false);
        targetPosition = CalculateTargetPosition();
    }

    private Vector3 CalculateTargetPosition()
    {
        RectTransform pauseMenuRectTransform = GetComponent<RectTransform>();

        // Calculate the initial position outside the top part of the screen
        float yPosition = pauseMenuRectTransform.rect.yMax + visuRectTransform.rect.height;

        return new Vector3(initPosition.x, yPosition, initPosition.z);
    }

    IEnumerator HandleStateRoutine() {
        var logId = "HandleStateRoutine";
        var waitForSeconds = new WaitForSeconds(0.1f);
        while (true) {
            var currentGameState = GameManager.Instance.CurrentState;
            var isActive = visu.activeSelf;
            if (isActive && currentGameState!=GameManager.GameState.Paused) {
                Hide();
            } else if (!isActive && currentGameState==GameManager.GameState.Paused) {
                Show();
            }
            logd(logId, "CurrentGameState=" + currentGameState + " ActiveSelf=" + isActive, true);
            yield return waitForSeconds;
        }
    }

    void Show()
    {
        visu?.SetActive(true);
        StartCoroutine(AnimateVisu(targetPosition, initPosition));
    }

    void Hide()
    {
        visu?.SetActive(false);
        StartCoroutine(AnimateVisu(initPosition, targetPosition));
    }

    IEnumerator AnimateVisu(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration) {
            elapsedTime += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            visu.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        visu.transform.position = end;
    }
    public void OnRestartButtonClick()
    {
        var logId = "OnRestartButtonClick";
        logd(logId, "Restart button clicked!");
        GameManager.Instance.RestartGame();
    }
}