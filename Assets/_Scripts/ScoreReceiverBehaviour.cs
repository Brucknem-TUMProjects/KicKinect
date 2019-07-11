using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreReceiverBehaviour : MonoBehaviour
{
    public RectTransform goalMessage;
    
    public AudioSource audioSource;
    public AudioClip goalCheerSound;

    void Start()
    {
        audioSource.clip = goalCheerSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ontrigger");
        if(other.tag == "SoccerBall")
        {
            goalMessage.gameObject.SetActive(true);
            audioSource.Play();
        }
    }
}
