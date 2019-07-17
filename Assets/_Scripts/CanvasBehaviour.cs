using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasBehaviour : MonoBehaviour
{
    public RectTransform credits;
    public RectTransform buttons;

    public void OnLeave()
    {
        Debug.Log("OnLeave");
        SceneManager.LoadScene("Intro");
    }

    public void OnStart()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnWeightPainting()
    {
        SceneManager.LoadScene("WeightVisualization");
    }

    public void OnSkinning()
    {
        SceneManager.LoadScene("SkeletonTest");
    }

    public void OnCredits()
    {
        buttons.gameObject.SetActive(false);
        credits.gameObject.SetActive(true);
    }

    public void OnLeaveCredits()
    {
        credits.gameObject.SetActive(false);
        buttons.gameObject.SetActive(true);
    }
}
