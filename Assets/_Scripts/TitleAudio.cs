using UnityEngine;

public class TitleAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip titleMusic;
    public bool enable = true;

    void Start()
    {
        audioSource.clip = titleMusic;

        if (enable)
            audioSource.Play();
    }
}