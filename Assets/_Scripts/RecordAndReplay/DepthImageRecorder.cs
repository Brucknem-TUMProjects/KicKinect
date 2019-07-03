using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DepthImageRecorder : MonoBehaviour
{

    // the KinectManager instance
    private KinectManager manager;
    
    // the foreground texture
    private Texture2D foregroundTex;

    // rectangle taken by the foreground texture (in pixels)
    private Rect foregroundRect;
    private Vector2 foregroundOfs;

    // game objects to contain the joint colliders
    private GameObject[] jointColliders = null;

    private string standardDirectory;

    private List<Texture2D> frames = new List<Texture2D>();

    void Start()
    {
        standardDirectory = Application.dataPath + "/_Recordings/" + DateTime.Now.ToString("d-MMM-yyyy-HH-mm-ss");
        Debug.Log(standardDirectory);
        Directory.CreateDirectory(standardDirectory);
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
            manager = KinectManager.Instance;
        }

        // get the users texture
        if (manager && manager.IsInitialized())
        {
            foregroundTex = manager.GetUsersLblTex();
        }

        if (manager.IsUserDetected())
        {
            Texture2D current = new Texture2D(foregroundTex.width, foregroundTex.height);
            current.SetPixels32(foregroundTex.GetPixels32());
            frames.Add(current);
        }
    }

    void OnGUI()
    {
        if (foregroundTex)
        {
            GUI.DrawTexture(foregroundRect, foregroundTex);
        }
    }

    private void OnApplicationQuit()
    {
            SaveImages();      
    }

    public void SaveImages()
    {
        int i = 0;
        foreach(Texture2D frame in frames)
        {
            //SaveTextureAsPNG(i++, RotateTexture(GrayScale(frame)));
            SaveTextureAsPNG(i++, GrayScale(frame));
        }
    }

    public void SaveTextureAsPNG(int frameIndex, Texture2D texture)
    {
        SaveTextureAsPNG(texture, standardDirectory + "/" + frameIndex + ".png");
    }

    public static void SaveTextureAsPNG(Texture2D texture, string _fullPath)
    {
        byte[] _bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        //Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }
    
    public static Texture2D RotateTexture(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        Color32[] rotated = new Color32[pixels.Length];

        for(int yy = 0; yy < texture.height; yy++)
        {
            for(int xx = 0; xx < texture.width; xx++)
            {
                int x = -xx + texture.width - 1;
                int y = -yy + texture.height - 1;

                rotated[y * texture.width + x] = pixels[yy * texture.width + xx];
            }
        }

        Texture2D final = new Texture2D(texture.width, texture.height);
        final.SetPixels32(rotated);
        return final;
    }

    public static Texture2D GrayScale(Texture2D texture)
    {
        Texture2D final = new Texture2D(texture.width, texture.height);
        Color32[] pixels = texture.GetPixels32();
        for(int i = 0; i < pixels.Length; i++)
        {
            byte g = (byte)(((int)pixels[i].r + (int)pixels[i].g + (int)pixels[i].b) / 3);
            pixels[i] = new Color32(g, g, g, pixels[i].a);
        }
        final.SetPixels32(pixels);
        return final;
    }
}