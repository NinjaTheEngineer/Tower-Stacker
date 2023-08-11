using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;

public class MenuController : NinjaMonoBehaviour {
    RectTransform visuRectTransform;
    Vector3 targetPosition;
    Vector3 initPosition;
    [SerializeField] GameObject visu;
    [SerializeField] float animationDuration = 0.5f;
    protected bool isHiding = false;
    public bool IsActive => isHiding && visu==null?gameObject.activeSelf:visu.activeSelf;
    void Start() {
        Initialize();
    }

    protected virtual void Initialize() {
        visuRectTransform = visu?.GetComponent<RectTransform>();
        initPosition = transform.position;
        visu?.SetActive(false);
        targetPosition = CalculateTargetPosition();
    }

    Vector3 CalculateTargetPosition() {
        RectTransform menuRectTransform = GetComponent<RectTransform>();

        float yPosition = menuRectTransform.rect.yMax + visuRectTransform.rect.height;

        return new Vector3(initPosition.x, yPosition, initPosition.z);
    }

    public void Show() {
        var logId = "Show";
        isHiding = false;
        if(visu==null) {
            logd(logId, "No Visu => Activating Menu="+this.logf());
            gameObject.SetActive(true);
        } else {
            logd(logId, "Animating Menu="+this.logf()+" From "+targetPosition+" to "+initPosition);
            StartCoroutine(AnimateVisu(targetPosition, initPosition, () => visu.SetActive(true)));
        }
    }

    public void Hide() {
        var logId = "Hide";
        isHiding = true;
        if(visu==null) {
            logd(logId, "No Visu => Deactivating Menu="+this.logf());
            gameObject.SetActive(false);
        } else {
            logd(logId, "Animating Menu="+this.logf()+" From "+initPosition+" to "+targetPosition);
            StartCoroutine(AnimateVisu(initPosition, targetPosition, null, () => visu.SetActive(false)));
        }
    }

    IEnumerator AnimateVisu(Vector3 start, Vector3 end, Action startAction=null, Action endAction=null) {
        float elapsedTime = 0f;
        transform.position = start;
        startAction?.Invoke();
        while (elapsedTime < animationDuration) {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            visu.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        visu.transform.position = end;
        endAction?.Invoke();
    }
}