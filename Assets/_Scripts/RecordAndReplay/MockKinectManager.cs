using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine;

public class MockKinectManager : KinectManager
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string psz1, string psz2);

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);
    }

    private sealed class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return SafeNativeMethods.StrCmpLogicalW(a, b);
        }
    }

    private NaturalStringComparer comparer = new NaturalStringComparer();

    public string path;

    private List<Texture2D> frames = new List<Texture2D>();
    private int currentFrame = 0;

    private static MockKinectManager instance;
    // returns the single KinectManager instance

    public new static MockKinectManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;

        LoadFrames();
    }

    // checks if Kinect is initialized and ready to use. If not, there was an error during Kinect-sensor initialization
    public override bool IsInitialized()
    {
        return true;
    }


    // returns the depth image/users histogram texture,if ComputeUserMap is true
    public override Texture2D GetUsersLblTex2D()
    {
        currentFrame++;
        currentFrame %= frames.Count;

        return frames[currentFrame];
    }

    // returns true if at least one user is currently detected by the sensor
    public override bool IsUserDetected()
    {
        return true;
    }

    private void LoadFrames()
    {
        string[] rawFilePaths = Directory.GetFiles(path);
        List<string> filePaths = new List<string>();
        foreach(string filePath in rawFilePaths)
        {
            if (filePath.EndsWith(".png"))
            {
                filePaths.Add(filePath);
            }
        }

        filePaths.Sort(comparer);

        foreach(string filePath in filePaths)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D frame = new Texture2D(2, 2);
            frame.LoadImage(bytes);
            frames.Add(frame);
        }
    }
}
