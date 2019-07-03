using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreReceiverBehaviour : MonoBehaviour
{
    public RectTransform goalMessage;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ontrigger");
        if(other.tag == "SoccerBall")
        {
            goalMessage.gameObject.SetActive(true);
        }
    }
}
