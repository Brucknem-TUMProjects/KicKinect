using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class BallBehaviour : MonoBehaviour
{
    public Text DebugText;
    public AudioSource audioSource;

    public AudioClip ballHitSound;
    public AudioClip missSound;
    public AudioClip goalSound;

    public Image missMessage;
    public Image goalMessage;

    public RectTransform scoreCounter;
    public RectTransform shotCounter;

    public Sprite[] digitSprites;

    private Rigidbody rb;

    private Image[] scoreDigits = new Image[2];
    private Image[] shotDigits = new Image[2];

    private int currentScore = 0;
    private int currentShot = 0;

    private GameStates currentState = GameStates.RESPAWN;

    private enum GameStates
    {
        RESPAWN,
        SHOT,
        GOAL
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        scoreDigits[0] = scoreCounter.GetChild(0).GetComponent<Image>();
        scoreDigits[1] = scoreCounter.GetChild(1).GetComponent<Image>();
        shotDigits[0] = shotCounter.GetChild(0).GetComponent<Image>();
        shotDigits[1] = shotCounter.GetChild(1).GetComponent<Image>();
    }

    void Awake()
    {
        AllMessagesOff();
    }

    public void Respawn()
    {
        AllMessagesOff();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentState = GameStates.RESPAWN;
    }

    private void ResetScore()
    {
        currentScore = 0;
        SetCounter(shotDigits, 0);
        SetCounter(scoreDigits, 1);
    }

    private void OnTriggerEnter(Collider other)
    {        
        if(other.tag == "ScoreReceiver")
        {
            OnGoal();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            audioSource.clip = ballHitSound;
            audioSource.Play();
            StartCoroutine(OnShot());
        }
    }

    private void OnGoal()
    {
        currentState = GameStates.GOAL;
        AllMessagesOff();
        goalMessage.gameObject.SetActive(true);
        audioSource.clip = goalSound;
        audioSource.Play();
        currentScore++;
        SetCounter(scoreDigits, currentScore);
    }

    private IEnumerator OnShot()
    {
        currentState = GameStates.SHOT;
        currentShot++;
        SetCounter(shotDigits, currentShot);

        for(int i = 300; i > 0; i--)
        {
            if (currentState == GameStates.SHOT)
            {
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                break;
            }
        }

        if (currentState == GameStates.SHOT)
        {
            AllMessagesOff();
            missMessage.gameObject.SetActive(true);
            audioSource.clip = missSound;
            audioSource.Play();
        }
    }

    private void AllMessagesOff()
    {
        missMessage.gameObject.SetActive(false);
        goalMessage.gameObject.SetActive(false);
    }

    private void SetCounter(Image[] digits, int count)
    {
        digits[0].sprite = digitSprites[count / digitSprites.Length];
        digits[1].sprite = digitSprites[count % digitSprites.Length];
    }
}
