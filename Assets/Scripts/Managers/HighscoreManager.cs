using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using PP = NinjaTools.PlayerPrefs;
using System;

public class HighscoreManager : NinjaMonoBehaviour {
    [SerializeField] int _highscore;
    public static event Action<int> OnHighscoreChanged;
    public int Highscore {
        get => _highscore;
        set {
            var logId = "Highscore_set";
            if (value <= _highscore) {
                logd(logId, "Tried to set highscore from " + _highscore + " to " + value);
                return;
            }
            logd(logId, "Setting Highscore from " + _highscore + " to " + value);
            _highscore = value;
            OnHighscoreChanged?.Invoke(_highscore);
            PP.SetInt(PP.PPKey.HIGHSCORE, _highscore);
            GPGSManager.Instance.AddHighscore(_highscore);
        }
    }
    public static HighscoreManager Instance { get; private set; }

    private void Awake() {
        if(Instance==null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
    }
    private void Start() {
        Highscore = PP.GetInt(PP.PPKey.HIGHSCORE);
    }
}
