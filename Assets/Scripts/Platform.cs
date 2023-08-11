using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class Platform : NinjaMonoBehaviour {
    Vector3 initialPosition;
    Quaternion initialRotation;
    [SerializeField] Rigidbody2D rb;
    private void Start() {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        StartCoroutine(HandleStateRoutine());
    }

    IEnumerator HandleStateRoutine() {
        var logId = "HandleStateRoutine";
        var waitForSeconds = new WaitForSeconds(0.1f);
        logd(logId, "Starting HandleState Routine");
        while(true) {
            var currentGameState = GameManager.Instance.CurrentState;
            var rbType = rb.bodyType;
            if(currentGameState==GameManager.GameState.GameOver && rbType!=RigidbodyType2D.Dynamic) {
                Release();
            } else if(currentGameState==GameManager.GameState.Playing && rbType!=RigidbodyType2D.Static) {
                Restart();
            }
            yield return waitForSeconds;
        }
    }

    void Restart() {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.bodyType = RigidbodyType2D.Static;
    }

    void Release() {
        AudioManager.Instance.PlayPlatformReleaseSound();
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
