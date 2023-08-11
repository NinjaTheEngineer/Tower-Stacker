using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
public class TouchInputController : NinjaMonoBehaviour {
    [SerializeField] private LayerMask uiLayerMask;
    public enum TouchState { None, Tap, UITap, Drag }
    public static TouchInputController Instance { get; private set;}
    private void Awake() {
        if(Instance==null) {
            Instance = this;
        } else if(Instance!=this) {
            Destroy(gameObject);
        }
    }
    static TouchState _currentState;
    [field: SerializeField] public TouchState CurrentState { 
        get => _currentState;
        private set {
            var logId = "CurrentState_set";
            logd(logId, "Setting CurrentState from"+_currentState+" to "+value, true);
            _currentState = value;
        }
    }
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
    public bool IsUITap { 
        get { 
            bool isUITap = CurrentState==TouchState.UITap;
            //CurrentState = TouchState.None;
            return isUITap;
        }
    }
    void Update() {
        var logId = "Update";
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Began:
                    logd(logId, "Touch Began => Setting CurrentState to None.");
                    CurrentState = TouchState.None;
                    tapStartTime = Time.realtimeSinceStartup; // Record the touch start time
                    break;
                case TouchPhase.Moved:
                    Vector3 touchDelta = touch.deltaPosition;
                    touchDelta.y = 0f;
                    logd(logId, "Touch Moving!",true);

                    if (!IsDragging && touchDelta.magnitude >= 1f) {
                        logd(logId, "Isn't Dragging and touchDeltaMagnitude="+touchDelta.magnitude+" => Setting CurrentState to Drag.");
                        CurrentState = TouchState.Drag;
                    }
                    break;
                case TouchPhase.Ended:
                    logd(logId, "Touch Ended!");
                    var timeElapsed = Time.realtimeSinceStartup - tapStartTime;
                    logd(logId, "TimeElapsed="+timeElapsed);
                    if(timeElapsed <= tapDelay) {
                        CurrentState = IsPointerOverUIObject(touch.position)?TouchState.UITap:TouchState.Tap;
                        logd(logId, "Setting CurrentState to "+CurrentState);
                    } else {
                        logd(logId, "Setting CurrentState to None.");
                        CurrentState = TouchState.None;
                    }
                    break;
            }
        }
    }
    private bool IsPointerOverUIObject(Vector2 screenPosition) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        return Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, uiLayerMask);
    }
    public override string ToString() => "{TouchInputController - CurrentState="+CurrentState.logf()+"}";
}
