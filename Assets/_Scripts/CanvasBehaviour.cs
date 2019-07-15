using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasBehaviour : MonoBehaviour
{
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
}
