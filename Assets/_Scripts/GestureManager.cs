﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GestureManager : MonoBehaviour, KinectGestures.GestureListenerInterface
{
    private static readonly float upDelta = 0.5f;

    public GameObject ball;
    public GameObject ballSpawn;

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

    public void UserDetected(uint userId, int userIndex)
    {
        // as an example - detect these user specific gestures
        KinectManager manager = KinectManager.Instance;

        manager.DetectGesture(userId, KinectGestures.Gestures.Jump);
        manager.DetectGesture(userId, KinectGestures.Gestures.Squat);

        //		manager.DetectGesture(userId, KinectGestures.Gestures.Push);
        //		manager.DetectGesture(userId, KinectGestures.Gestures.Pull);

        //		manager.DetectGesture(userId, KinectWrapper.Gestures.SwipeUp);
        //		manager.DetectGesture(userId, KinectWrapper.Gestures.SwipeDown);

        if (GestureInfo != null)
        {
            GestureInfo.text = "SwipeLeft, SwipeRight, Jump or Squat.";
        }
    }

    public void UserLost(uint userId, int userIndex)
    {
        if (GestureInfo != null)
        {
            GestureInfo.text = string.Empty;
        }
    }

    public void GestureInProgress(uint userId, int userIndex, KinectGestures.Gestures gesture,
                                  float progress, KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
    {
        GestureInfo.text = string.Format("{0} Progress: {1:F1}%", gesture, (progress * 100));
        if (gesture == KinectGestures.Gestures.Click && progress > 0.3f)
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

    public bool GestureCompleted(uint userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
    {
        string sGestureText = gesture + " detected";
        if (gesture == KinectGestures.Gestures.Click)
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

        progressDisplayed = false;

        return true;
    }

    public bool GestureCancelled(uint userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectWrapper.NuiSkeletonPositionIndex joint)
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
        SpawnBall(true);
    }

    public void OnSwipeRight()
    {
        SpawnBall(false);
    }

    private void SpawnBall(bool leftFoot)
    {
        if (showSpawnMessage)
        {
            spawnMessage.gameObject.SetActive(false);
            showSpawnMessage = false;
        }

        goalMessage.gameObject.SetActive(false);
        missMessage.gameObject.SetActive(false);
        
        ball.transform.position = ballSpawn.transform.position + ballSpawn.transform.up * upDelta;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        audioSource.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (Color.red);
        Gizmos.DrawLine(ballSpawn.transform.position, ballSpawn.transform.position + ballSpawn.transform.up * upDelta);
    }
}