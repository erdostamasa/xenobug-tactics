using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour {
    [SerializeField] Slider _slider;

    void Start() {
        _slider.value = SoundManager.instance.MusicVolume;
        _slider.onValueChanged.AddListener(val => SoundManager.instance.ChangeMusicVolume(val));
    }
}