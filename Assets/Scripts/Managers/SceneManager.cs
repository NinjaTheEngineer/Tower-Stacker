using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NinjaTools;
public enum SceneName {
    MainMenu,
    Casual,
    AI
}
public class SceneManager : NinjaMonoBehaviour {
    public static SceneManager Instance {get; private set;}
    private void Awake() {
        if(Instance==null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else if(Instance!=this) {
            Destroy(gameObject);
        }
    }

    public void OpenScene(SceneName scene) {
        var logId = "OpenScene";
        string sceneName = scene.ToString();
        logd(logId, "Opening Scene="+sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void OpenScene(string sceneName) {
        var logId = "OpenScene";
        if(sceneName==null) {
            logw(logId, "Tried to open null scene => no-op");
            return;
        }
        logd(logId, "Opening Scene="+sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void RestartScene()  {
        var logId = "RestartScene";
        logd(logId, "Restarting Scene");
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(activeScene.buildIndex);
    }
}
