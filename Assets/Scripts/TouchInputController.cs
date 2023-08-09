using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
public class TouchInputController : NinjaMonoBehaviour {
    public enum TouchState { None, Tap, Drag }
    public static TouchInputController Instance { get; private set;}
    private void Awake() {
        if(Instance==null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else if(Instance!=this) {
            Destroy(gameObject);
        }
    }
    [field: SerializeField] public static TouchState CurrentState { get; private set; }
    float tapStartTime;
    [SerializeField] float tapDelay = 0.02f; // The delay in seconds before considering a touch as a tap
    public bool IsDragging => CurrentState==TouchState.Drag;
    public bool IsTap { 
        get { 
            bool isTap = CurrentState==TouchState.Tap;
            CurrentState = TouchState.None;
            return isTap;
        }
    }
    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Began:
                    CurrentState = TouchState.None;
                    tapStartTime = Time.realtimeSinceStartup; // Record the touch start time
                    break;
                case TouchPhase.Moved:
                    Vector3 touchDelta = touch.deltaPosition;
                    touchDelta.y = 0f;

                    if (!IsDragging && touchDelta.magnitude >= 1f) {
                        CurrentState = TouchState.Drag;
                    }
                    break;
                case TouchPhase.Ended:
                    if(Time.realtimeSinceStartup - tapStartTime <= tapDelay) {
                        CurrentState = TouchState.Tap;
                    } else {
                        CurrentState = TouchState.None;
                    }
                    break;
            }
        }
    }
}
