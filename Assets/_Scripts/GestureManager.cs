using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GestureManager : MonoBehaviour, KinectGestures.GestureListenerInterface
{
    private static readonly float upDelta = 0.5f;

    public BallBehaviour ball;
    public GameObject ballSpawn;
    public GameObject player;

    public Image spawnMessage;
    public Image goalMessage;
    public Image missMessage;

    public AudioSource audioSource;
    public AudioClip refreeWhistleSound;

    //// GUI Text to display the gesture messages.
    public Text GestureInfo;

    // private bool to track if progress message has been displayed
    private bool progressDisplayed;

    private bool showSpawnMessage = true;

    private void Start()
    {
        // Manually swipe to dismiss spawn message and start game
        // OnSwipeLeft();
        audioSource.clip = refreeWhistleSound;
    }

    public void UserDetected(long userId, int userIndex)
    {
        KinectManager manager = KinectManager.Instance;

        manager.DetectGesture(userId, KinectGestures.Gestures.SwipeLeft);
        manager.DetectGesture(userId, KinectGestures.Gestures.SwipeRight);
        //manager.DetectGesture(userId, KinectGestures.Gestures.SwipeDown);
        manager.DetectGesture(userId, KinectGestures.Gestures.SwipeUp);
        manager.DetectGesture(userId, KinectGestures.Gestures.Squat);

        if (GestureInfo != null)
        {
            GestureInfo.text = "SwipeLeft, SwipeRight, Jump or Squat.";
        }
    }

    // This method is called when the user is lost
    public void UserLost(long userId, int userIndex)
    {
        if (GestureInfo != null)
        {
            GestureInfo.text = string.Empty;
        }
        ShowStartMessage();
    }

    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
        GestureInfo.text = string.Format("{0} Progress: {1:F1}%", gesture, (progress * 100));
        if (gesture == KinectGestures.Gestures.RaiseRightHand && progress > 0.3f)
        {
            string sGestureText = string.Format("{0} {1:F1}% complete", gesture, progress * 100);
            if (GestureInfo != null)
                GestureInfo.text = sGestureText;

            progressDisplayed = true;
        }
        else if ((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
        {
            string sGestureText = string.Format("{0} detected, zoom={1:F1}%", gesture, screenPos.z * 100);
            if (GestureInfo != null)
                GestureInfo.text = sGestureText;

            progressDisplayed = true;
        }
        else if (gesture == KinectGestures.Gestures.Wheel && progress > 0.5f)
        {
            string sGestureText = string.Format("{0} detected, angle={1:F1} deg", gesture, screenPos.z);
            if (GestureInfo != null)
                GestureInfo.text = sGestureText;

            progressDisplayed = true;
        }
    }

    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint, Vector3 screenPos)
    {
        string sGestureText = gesture + " detected";
        if (gesture == KinectGestures.Gestures.RaiseRightHand)
            sGestureText += string.Format(" at ({0:F1}, {1:F1})", screenPos.x, screenPos.y);

        if (GestureInfo != null)
            GestureInfo.text = sGestureText;

        if (gesture == KinectGestures.Gestures.SwipeLeft)
        {
            OnSwipeLeft();
        }
        else if (gesture == KinectGestures.Gestures.SwipeRight)
        {
            OnSwipeRight();
        }
        else if (gesture == KinectGestures.Gestures.SwipeUp)
        {
            OnStart();
        }
        else if(gesture == KinectGestures.Gestures.Squat)
        {
            OnResetCounters();
        }

        progressDisplayed = false;

        return true;
    }

    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint)
    {
        if (progressDisplayed)
        {
            // clear the progress info
            if (GestureInfo != null)
                GestureInfo.text = string.Empty;

            progressDisplayed = false;
        }

        return true;
    }

    public void OnSwipeLeft()
    {
        SpawnBall();
    }

    public void OnSwipeRight()
    {
        SpawnBall();
    }

    public void OnStart()
    {
        spawnMessage.gameObject.SetActive(false);
    }

    public void ShowStartMessage()
    {
        ball.AllMessagesOff();
        spawnMessage.gameObject.SetActive(true);
        OnResetCounters();
    }

    public void OnResetCounters()
    {
        ball.ResetCounters();
    }

    private void ResetPlayerPosition()
    {
        player.transform.position = ballSpawn.transform.position;
    }

    private void SpawnBall()
    {
        if (showSpawnMessage)
        {
            spawnMessage.gameObject.SetActive(false);
            showSpawnMessage = false;
        }
        
        ball.transform.position = ballSpawn.transform.position + ballSpawn.transform.up * upDelta;
        ball.Respawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (Color.red);
        Gizmos.DrawLine(ballSpawn.transform.position, ballSpawn.transform.position + ballSpawn.transform.up * upDelta);
    }
}