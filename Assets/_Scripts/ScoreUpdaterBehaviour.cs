using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdaterBehaviour : MonoBehaviour
{
    public Sprite[] images;
    public Image scoreHolder;

    private int currentScore = 0;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ontrigger");
        if(other.tag == "SoccerBall")
        {
            updateScore();
        }
    }
    
    private void updateScore()
    {
        scoreHolder.sprite = images[(++currentScore % 10)];
    }
}