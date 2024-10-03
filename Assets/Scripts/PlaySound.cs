using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private AudioSource audioSource;

    public void Play() {
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.Play();
    }
}
