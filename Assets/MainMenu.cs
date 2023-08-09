using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class MainMenu : NinjaMonoBehaviour {
    [SerializeField] SceneManager sceneManager;
    void Start() {
        sceneManager ??= SceneManager.Instance;
    }

    public void OnCasualButtonClick() {
        if(sceneManager==null) {
            return;
        }
        sceneManager.OpenScene(SceneName.Casual);
    }
}