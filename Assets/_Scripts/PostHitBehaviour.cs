using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostHitBehaviour : MonoBehaviour
{
    public Image missMessage;

    public AudioSource audioSource;
    public AudioClip postHitSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = postHitSound;       
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ontrigger");
        if(other.tag == "SoccerBall")
        {
            missMessage.gameObject.SetActive(true);
            audioSource.Play();
        }
    }
}
