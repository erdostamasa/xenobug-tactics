using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsSlider : MonoBehaviour
{
    [SerializeField] Slider _slider;

    void Start() {
        _slider.value = SoundManager.instance.EffectsVolume;
        _slider.onValueChanged.AddListener(val => SoundManager.instance.ChangeEffectsVolume(val));
    }
}
