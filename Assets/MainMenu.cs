using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class MainMenu : NinjaMonoBehaviour {
    public void OnCasualButtonClick() {
        AudioManager.Instance.PlayButtonClick();
        SceneManager.Instance.OpenScene(SceneName.Casual);
    }
    public void OnAIButtonClick() {
        AudioManager.Instance.PlayButtonClick();
        SceneManager.Instance.OpenScene(SceneName.AI);
    }
}