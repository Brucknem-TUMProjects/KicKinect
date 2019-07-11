using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallBehaviour : MonoBehaviour
{
    //private Rigidbody rb;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //}

    public AudioSource audioSource;
    public AudioClip ballHitSound;

    void Start()
    {
        audioSource.clip = ballHitSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ontrigger");
        if(other.tag == "Player")
        {
            audioSource.Play();
        }
    }
}
