using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DepthImageReplayer : MonoBehaviour
{
    // the KinectManager instance
    private MockKinectManager manager;

    // the foreground texture
    private Texture2D foregroundTex;

    // rectangle taken by the foreground texture (in pixels)
    private Rect foregroundRect;
    private Vector2 foregroundOfs;

    // game objects to contain the joint colliders
    private GameObject[] jointColliders = null;
    
    void Start()
    {
        // calculate the foreground rectangle
        Rect cameraRect = Camera.main.pixelRect;
        float rectHeight = cameraRect.height;
        float rectWidth = cameraRect.width;

        if (rectWidth > rectHeight)
            rectWidth = rectHeight * KinectWrapper.Constants.DepthImageWidth / KinectWrapper.Constants.DepthImageHeight;
        else
            rectHeight = rectWidth * KinectWrapper.Constants.DepthImageHeight / KinectWrapper.Constants.DepthImageWidth;

        foregroundOfs = new Vector2((cameraRect.width - rectWidth) / 2, (cameraRect.height - rectHeight) / 2);
        foregroundRect = new Rect(foregroundOfs.x, cameraRect.height - foregroundOfs.y, rectWidth, -rectHeight);

        // create joint colliders
        int numColliders = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;
        jointColliders = new GameObject[numColliders];

        for (int i = 0; i < numColliders; i++)
        {
            string sColObjectName = ((KinectWrapper.NuiSkeletonPositionIndex)i).ToString() + "Collider";
            jointColliders[i] = new GameObject(sColObjectName);
            jointColliders[i].transform.parent = transform;

            SphereCollider collider = jointColliders[i].AddComponent<SphereCollider>();
            collider.radius = 1f;
        }
    }

    void Update()
    {
        if (manager == null)
        {
            manager = MockKinectManager.Instance;
        }

        // get the users texture
        if (manager && manager.IsInitialized())
        {
            foregroundTex = manager.GetUsersLblTex();
        }
    }

    void OnGUI()
    {
        if (foregroundTex)
        {
            GUI.DrawTexture(foregroundRect, foregroundTex);
        }
    }
}