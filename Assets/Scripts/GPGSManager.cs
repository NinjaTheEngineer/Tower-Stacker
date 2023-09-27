using UnityEngine;
using NinjaTools;
using GooglePlayGames;
using TMPro;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class GPGSManager : NinjaMonoBehaviour {
    public TextMeshProUGUI text;
    public string highScoreLeaderboardId = "CgkI0cqO5N4fEAIQAw";
    public static GPGSManager Instance;
    bool isAuthenticated = false;
    public string Token;
    public string Error;
    public void AddHighscore(int highscore) {
        var logId = "AddHighscore";
        if (!isAuthenticated) {
            logd(logId, "Not authenticated => no-op");
            return;
        }
        Social.ReportScore(highscore, highScoreLeaderboardId, (bool success) => {
            logd(logId, "Reporting highscore of " + highscore + " => " + (success ? "Successfully" : "Failed"));
        });
    }
    public int GetHighscore() {
        var logId = "GetHighscore";
        int userScore = HighscoreManager.Instance.Highscore;
        if (!isAuthenticated) {
            logd(logId, "User is not authenticated => returning player prefs");
            return userScore;
        }
        Social.LoadScores(highScoreLeaderboardId, (IScore[] scores) => {
            var scoresCount = scores.Length;
            for (int i = 0; i < scoresCount; i++) {
                var currentScore = scores[i];
                if (currentScore != null) {
                    if (currentScore.userID == Social.localUser.id) {
                        userScore = (int)currentScore.value;
                        break;
                    }
                }
            }
        });
        return userScore;
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        PlayGamesPlatform.Activate();
        text.gameObject.SetActive(false);
    }


    //public void Start() {
    //    PlayGamesPlatform.Activate();
    //    Social.localUser.Authenticate(ProcessAuthentication);
    //}
    public void ShowHighscoreLeaderboard() {
        var logId = "ShowHighscoreLeaderboard";
        if (!isAuthenticated) {
            logd(logId, "Not authenticated => no-op");
            return;
        }
        PlayGamesPlatform.Instance.ShowLeaderboardUI(highScoreLeaderboardId);
    }
    void ProcessAuthentication(bool success) {
        var logId = "ProcessAuthentication";
        isAuthenticated = success;
        var highscore = GetHighscore();
        var authenticationText = success ? "Login with Google Play games successful. Highscore=" + highscore : "Login Unsuccessful.";
        HighscoreManager.Instance.Highscore = highscore;
        logd(logId, authenticationText);
//#if UNITY_EDITOR || !UNITY_STANDALONE
        text.gameObject.SetActive(true);
        text.SetText(authenticationText);
//#endif
    }

    async void Start() {
        await UnityServices.InitializeAsync();
        await LoginGooglePlayGames();
        await SignInWithGooglePlayGamesAsync(Token);
    }
    //Fetch the Token / Auth code
    public Task LoginGooglePlayGames() {
        var tcs = new TaskCompletionSource<object>();
        string authenticationText = "";
        PlayGamesPlatform.Instance.Authenticate((success) => {
            if (success == SignInStatus.Success) {
                Debug.Log("Login with Google Play games successful.");
                var highscore = GetHighscore();
                isAuthenticated = true;
                authenticationText = "Login with Google Play games successful. Highscore=" + highscore;
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => {
                    Debug.Log("Authorization code: " + code);
                    Token = code;
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                    tcs.SetResult(null);
                });
            } else {
                authenticationText = "Login Unsuccessful.";
                isAuthenticated = false;
                Error = "Failed to retrieve Google play games authorization code";
                Debug.Log("Login Unsuccessful");
                tcs.SetException(new Exception("Failed"));
            }
        });
//#if UNITY_EDITOR || !UNITY_STANDALONE
        text.gameObject.SetActive(true);
        text.SetText(authenticationText);
//#endif
        return tcs.Task;
    }


    async Task SignInWithGooglePlayGamesAsync(string authCode) {
        try {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            var debugText = $"PlayerID: {AuthenticationService.Instance.PlayerId}"; //Display the Unity Authentication PlayerID
            text.SetText(debugText);
            Debug.Log("SignIn is successful.");
        } catch (AuthenticationException ex) {
            text.SetText(ex.StackTrace);
            Debug.LogException(ex);
        } catch (RequestFailedException ex) {
            text.SetText(ex.StackTrace);
            Debug.LogException(ex);
        }
    }
}
