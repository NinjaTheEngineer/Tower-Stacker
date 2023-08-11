using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
public class AudioManager : NinjaMonoBehaviour {
    private static AudioManager _instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip defeatSound;
    [SerializeField] private AudioClip platformReleaseSound;
    [SerializeField] private AudioClip pieceRotateSound;
    [SerializeField] private List<AudioClip> pieceCollidedSounds;
    [SerializeField] private List<AudioClip> pieceFellSounds;
    [SerializeField] private List<AudioClip> chainsBreakSounds;
    [SerializeField] private AudioClip backgroundMusic;
    public static AudioManager Instance {
        get {
            if(_instance == null) {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null) {
                    _instance = new GameObject("AudioManager").AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }
    private void Awake() {
        if (_instance==null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    private void Start() {
        PlayBackgroundMusic();
    }

    public void PlaySFX(AudioClip clip) {
        var logId = "PlaySFX";
        if(clip==null) {
            logw(logId, "Clip="+clip.logf()+" => no-op");
            return;
        }
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick() {
        PlaySFX(buttonClickSound);
    }
    public void PlayPieceFellSound() {
        var random = Random.Range(0, pieceFellSounds.Count);
        PlaySFX(pieceFellSounds[random]);
    }
    public void PlayPieceRotateSound() {
        PlaySFX(pieceRotateSound);
    }
    public void PlayPieceCollidedSound() {
        var random = Random.Range(0, pieceCollidedSounds.Count);
        PlaySFX(pieceCollidedSounds[random]);
    }
    public void PlayChainBreakSound() {
        var random = Random.Range(0, chainsBreakSounds.Count);
        PlaySFX(chainsBreakSounds[random]);
    }
    public void PlayPlatformReleaseSound() {
        PlaySFX(platformReleaseSound);
    }
    public void PlayVictorySound() {
        PlaySFX(victorySound);
    }
    public void PlayDefeatSound() {
        PlaySFX(defeatSound);
    }
    public void PlayBackgroundMusic() {
        var logId = "PlayBackgroundMusic";
        if(backgroundMusic==null) {
            logw(logId, "BackgroundMusic="+backgroundMusic.logf()+" => no-op");
            return;
        }
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopBackgroundMusic() {
        musicSource.Stop();
    }
}