using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour {
    public static SoundManager instance;

    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioSource _effectsSource;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void PlaySound(SoundDescriptor sd) {
        _effectsSource.pitch = Random.Range(sd.pitchRange.x, sd.pitchRange.y);
        _effectsSource.PlayOneShot(sd.audioClip, sd.volume);
    }

    public void ChangeEffectsVolume(float volume) {
        _effectsSource.volume = volume;
    }

    public void ChangeMusicVolume(float volume) {
        _musicSource.volume = volume;
    }

    public void ToggleEffects() {
        _effectsSource.mute = !_effectsSource.mute;
    }

    public void ToogleMusic() {
        _musicSource.mute = !_musicSource.mute;
    }
}