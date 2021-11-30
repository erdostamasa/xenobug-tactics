using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SoundDescriptor : ScriptableObject {
    public AudioClip audioClip;
    public float volume = 1f;
    public Vector2 pitchRange = Vector2.one;
}
