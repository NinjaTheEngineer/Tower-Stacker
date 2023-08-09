using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using TMPro;

public class TowerHeightManager : NinjaMonoBehaviour {
    [SerializeField] TextMeshProUGUI towerHeightText;
    [SerializeField] LayerMask towerLayer;
    public float TargetHeight { get; private set; }
    public float CurrentHeight { get; private set; }
    public float heightMultiplier = 2f;
    private void Start() {
        StartCoroutine(CalculateTowerHeightRoutine());
    }
    Vector2 raycastOrigin;
    Vector2 raycastDirection;
    float towerWidth = 5f; // You may need to adjust this to match your platform's width
    float raycastDistance = 50f;

    IEnumerator CalculateTowerHeightRoutine() {
    var logId = "CalculateTowerHeightRoutine";
    var waitForSeconds = new WaitForSeconds(0.2f);
    while (true) {
        float highestHitY = 0; // Initialize to a low value

        raycastOrigin = transform.position;
        raycastDirection = Vector2.up;

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

        CurrentHeight = highestHitY;
        towerHeightText.text = Mathf.RoundToInt(highestHitY * heightMultiplier).ToString();
        yield return waitForSeconds;
    }
}
    private void OnDrawGizmos() {
        Gizmos.DrawLine(raycastOrigin, raycastDirection * 10);
    }
}