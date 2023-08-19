using UnityEngine;
using NinjaTools;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using System;
using UnityEngine.SocialPlatforms;

public class GPGSManager : MonoBehaviour {
	public TextMeshProUGUI text;
    public string Token;
    public string Error;
    
    public void Start() {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
            text.SetText("Login with Google Play games successful.");
            // Continue with Play Games Services
        } else {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            text.SetText("Login Unsuccessful.");
            Debug.Log("Login Unsuccessful");
        }
    }
}
