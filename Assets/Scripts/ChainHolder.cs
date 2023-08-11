using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class ChainHolder : NinjaMonoBehaviour {
    [SerializeField] List<GameObject> chains;
    int numberOfChains;
    private void Start() {
        numberOfChains = chains.Count;
    }

    public void EnableChains() {
        var logId = "EnableChains";
        logd(logId, "Enabling Chains");
        for (int i = 0; i < numberOfChains; i++) {
            chains[i].gameObject.SetActive(true);
        }
    }
    
    public void DisableChains() {
        var logId = "DisableChains";
        logd(logId, "Disabling Chains");
        for (int i = 0; i < numberOfChains; i++) {
            chains[i].gameObject.SetActive(false);
        }
        AudioManager.Instance.PlayChainBreakSound();
    }
}
