using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class MainMenu : NinjaMonoBehaviour {
    public void OnCasualButtonClick() {
        SceneManager.Instance.OpenScene(SceneName.Casual);
    }
    public void OnAIButtonClick() {
        SceneManager.Instance.OpenScene(SceneName.AI);
    }
}