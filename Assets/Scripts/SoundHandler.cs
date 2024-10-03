using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundHandler : MonoBehaviour
{
    public static SoundHandler sound;

    private AudioSource audioSource;

    void Awake() {// Singleton pattern
        if (sound == null) {
            sound = this;
        } else {
            print("Warning: Doublicate SoundHandler detected!");
            Destroy(this.gameObject);
        }

    }

    public void Play() {
    
    }
}
