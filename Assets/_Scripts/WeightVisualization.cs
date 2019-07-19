using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(MeshRenderer))]
public class WeightVisualization : MonoBehaviour
{
    private MeshRenderer rend;
    private Mesh restPose;
    private BoneWeight[] weights;
    private Color[] colors;

    public Slider slider;
    private Material[] mats;
    
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        mats = rend.materials;
        restPose = GetComponent<MeshFilter>().mesh;
        weights = restPose.boneWeights;
        colors = new Color[weights.Length];

        Debug.Log(weights.Length + " - " + restPose.vertices.Length + " - " + colors.Length);

        for (int i = 0; i < weights.Length; i++)
        {
            BoneWeight weight = weights[i];

            Color color = new Color(0, 0, 0, 1);
            color += jointToColor[boneIndex2JointTypeForColor[weight.boneIndex0]] * weight.weight0;
            color += jointToColor[boneIndex2JointTypeForColor[weight.boneIndex1]] * weight.weight1;
            color += jointToColor[boneIndex2JointTypeForColor[weight.boneIndex2]] * weight.weight2;
            color += jointToColor[boneIndex2JointTypeForColor[weight.boneIndex3]] * weight.weight3;

            colors[i] = color;
        }
        GetComponent<MeshFilter>().mesh.colors = colors;
    }

    public void OnValueChanged(float value)
    {
        foreach (Material mat in mats)
        {
            mat.SetFloat("_WeightAmount", value);
        }
    }

    public class BoneIndex2ColorMap
    {
        Dictionary<int, Color> map = new Dictionary<int, Color>();

        private static readonly Color[] colors = { Color.red, Color.green, Color.yellow, Color.blue, Color.magenta, Color.white };
        
        public Color this[int key]
        {
            get
            {
                if (!map.ContainsKey(key))
                {
                    map.Add(key, colors[Random.Range(0, colors.Length)]);
                }
                return map[key];
            }
        }
    }

    public static readonly Dictionary<Kinect.JointType, Color> jointToColor = new Dictionary<Kinect.JointType, Color>()
    {
        { Kinect.JointType.SpineBase, Color.red }, // SPINE BASE 
        { Kinect.JointType.SpineMid, Color.blue }, // SPINE MID 

        { Kinect.JointType.ShoulderRight, Color.green }, // SHOULDER RIGHT
        { Kinect.JointType.ElbowRight, Color.red }, // ELLBOW RIGHT
        { Kinect.JointType.WristRight, Color.blue }, // WRIST RIGHT
        { Kinect.JointType.HandRight, Color.red }, // HAND RIGHT
        { Kinect.JointType.HandTipRight, Color.blue }, // HAND TIP RIGHT
        { Kinect.JointType.ThumbRight, Color.green }, // THUMB RIGHT

        { Kinect.JointType.ShoulderLeft, Color.yellow }, // SHOULDER LEFT
        { Kinect.JointType.ElbowLeft, Color.blue }, // ELLBOW LEFT
        { Kinect.JointType.WristLeft, Color.red }, // WRIST LEFT
        { Kinect.JointType.HandLeft, Color.blue }, // HAND LEFT
        { Kinect.JointType.HandTipLeft, Color.red }, // HAND TIP LEFT
        { Kinect.JointType.ThumbLeft, Color.yellow }, // THUMB LEFT

        { Kinect.JointType.SpineShoulder, Color.red }, // SPINE SHOULDER
        { Kinect.JointType.Neck, Color.blue }, // NECK
        { Kinect.JointType.Head, Color.white }, // Maybe HEAD

        { Kinect.JointType.KneeLeft, Color.blue }, // KNEE LEFT
        { Kinect.JointType.AnkleLeft, Color.red }, // ANKLE LEFT
        { Kinect.JointType.FootLeft, Color.blue }, // FOOT LEFT
        { Kinect.JointType.HipLeft, Color.white }, // HIP LEFT

        { Kinect.JointType.KneeRight, Color.green }, // KNEE RIGHT
        { Kinect.JointType.AnkleRight, Color.blue }, // ANKLE RIGHT
        { Kinect.JointType.FootRight, Color.red }, // FOOT RIGHT
        { Kinect.JointType.HipRight, Color.magenta }, // HIP RIGHT
    };

    public static readonly Dictionary<int, Kinect.JointType> boneIndex2JointTypeForColor = new Dictionary<int, Kinect.JointType>
    {
        {(int)Kinect.JointType.SpineBase,  Kinect.JointType.SpineBase },
        {(int)Kinect.JointType.SpineMid,  Kinect.JointType.SpineMid },

        {(int)Kinect.JointType.Head,  Kinect.JointType.ShoulderRight },
        {(int)Kinect.JointType.Neck,  Kinect.JointType.SpineShoulder },
        {(int)Kinect.JointType.SpineShoulder,  Kinect.JointType.HipLeft },

        {(int)Kinect.JointType.ShoulderRight,  Kinect.JointType.ThumbRight },
        {(int)Kinect.JointType.ElbowRight,  Kinect.JointType.ShoulderLeft },
        {(int)Kinect.JointType.WristRight,  Kinect.JointType.ElbowLeft },
        {(int)Kinect.JointType.HandRight,  Kinect.JointType.WristLeft },
        {(int)Kinect.JointType.HandTipRight,  Kinect.JointType.FootRight },
        {(int)Kinect.JointType.ThumbRight,  Kinect.JointType.Head },

        {(int)Kinect.JointType.ShoulderLeft,  Kinect.JointType.ElbowRight },
        {(int)Kinect.JointType.ElbowLeft,  Kinect.JointType.WristRight },
        {(int)Kinect.JointType.WristLeft,  Kinect.JointType.HandRight },
        {(int)Kinect.JointType.HandLeft,  Kinect.JointType.HandTipRight },
        {(int)Kinect.JointType.HandTipLeft,  Kinect.JointType.KneeRight },
        {(int)Kinect.JointType.ThumbLeft,  Kinect.JointType.AnkleRight },

        {(int)Kinect.JointType.FootRight,  Kinect.JointType.FootLeft },
        {(int)Kinect.JointType.AnkleRight,  Kinect.JointType.AnkleLeft },
        {(int)Kinect.JointType.KneeRight,  Kinect.JointType.KneeLeft },
        {(int)Kinect.JointType.HipRight,  Kinect.JointType.HipRight },

        {(int)Kinect.JointType.FootLeft,  Kinect.JointType.Neck },
        {(int)Kinect.JointType.AnkleLeft,  Kinect.JointType.ThumbLeft },
        {(int)Kinect.JointType.KneeLeft,  Kinect.JointType.HandTipLeft },
        {(int)Kinect.JointType.HipLeft,  Kinect.JointType.HandLeft },
    };
}