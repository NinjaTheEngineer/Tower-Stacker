using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using TMPro;

public class MainMenu : NinjaMonoBehaviour {
    public TextMeshProUGUI highscoreText; //Can decouple this so the HighscoreText manages itself later on
    private void Awake() {
        highscoreText.gameObject.SetActive(false);
    }
    private void OnEnable() {
        HighscoreManager.OnHighscoreChanged += OnHighScoreChanged;
        highscoreText.gameObject.SetActive(true);
    }
    private void OnHighScoreChanged(int highscore) {
        highscoreText.text = highscore.ToString();

    }
    public void OnCasualButtonClick() {
        AudioManager.Instance.PlayButtonClick();
        SceneManager.Instance.OpenScene(SceneName.Casual);
    }
    public void OnAIButtonClick() {
        AudioManager.Instance.PlayButtonClick();
        SceneManager.Instance.OpenScene(SceneName.AI);
    }
    private void OnDisable() {
        HighscoreManager.OnHighscoreChanged -= OnHighScoreChanged;
    }
}