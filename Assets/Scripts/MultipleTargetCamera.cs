using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour {
    public List<Transform> targets;
    public Vector3 offset;
    public float smoothTime = 0.5f;
    Vector3 velocity;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;
    private Camera cam;
    private void Start() {
        cam = GetComponent<Camera>();
    }
    public void AddTargets(List<Transform> targets) {
        this.targets = targets;
    }
    private void LateUpdate() {
        var targetsCount = targets.Count;
        if(targetsCount == 0 || GameManager.Instance.CurrentState!=GameManager.GameState.Playing) {
            return;
        }
        Move();
        Zoom();
    }
    void Move() {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
    void Zoom() {
        float averageDistance = GetAverageDistance();
        float newZoom = Mathf.Lerp(maxZoom, minZoom, averageDistance / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    float GetAverageDistance() {
        float totalDistance = 0f;
        for (int i = 0; i < targets.Count - 1; i++) {
            for (int j = i + 1; j < targets.Count; j++) {
                totalDistance += Vector3.Distance(targets[i].position, targets[j].position);
            }
        }
        float averageDistance = totalDistance / (targets.Count * (targets.Count - 1) * 0.5f);
        return averageDistance;
    }
    Vector3 GetCenterPoint() {
        var targetsCount = targets.Count;
        if(targetsCount==0) {
            return Vector3.zero;
        }
        if(targetsCount==1) {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targetsCount; i++) {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
    
}
