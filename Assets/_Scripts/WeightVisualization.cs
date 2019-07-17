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
                color += boneIndex2Color[weight.boneIndex0] * weight.weight0;
                color += boneIndex2Color[weight.boneIndex1] * weight.weight1;
                color += boneIndex2Color[weight.boneIndex2] * weight.weight2;
                color += boneIndex2Color[weight.boneIndex3] * weight.weight3;

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

    public static readonly Dictionary<int, Color> boneIndex2Color = new Dictionary<int, Color>()
    {
        { 0, Color.red }, // SPINE BASE 
        { 1, Color.green }, // SPINE MID 

        { 2, Color.yellow }, // SHOULDER RIGHT
        { 3, Color.blue }, // ELLBOW RIGHT
        { 4, Color.yellow }, // WRIST RIGHT
        { 5, Color.red }, // HAND RIGHT
        { 6, Color.red }, // HAND TIP RIGHT
        { 7, Color.blue }, // THUMB RIGHT

        { 8, Color.blue }, // SHOULDER LEFT
        { 9, Color.yellow }, // ELLBOW LEFT
        { 10, Color.green }, // WRIST LEFT
        { 11, Color.red }, // HAND LEFT
        { 12, Color.red }, // HAND TIP LEFT
        { 13, Color.blue }, // THUMB LEFT

        { 14, Color.red }, // SPINE SHOULDER
        { 15, Color.blue }, // NECK
        { 16, Color.white }, // Maybe HEAD

        { 17, Color.white }, // KNEE LEFT
        { 18, Color.red }, // ANKLE LEFT
        { 19, Color.blue }, // FOOT LEFT
        { 20, Color.yellow }, // HIP LEFT

        { 21, Color.yellow }, // KNEE RIGHT
        { 22, Color.blue }, // ANKLE RIGHT
        { 23, Color.red }, // FOOT RIGHT
        { 24, Color.blue }, // HIP RIGHT
    };
}