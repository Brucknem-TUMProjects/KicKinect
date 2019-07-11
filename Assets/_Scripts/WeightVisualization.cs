using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(MeshRenderer))]
public class WeightVisualization : MonoBehaviour
{
    private MeshRenderer rend;
    private Mesh restPose;
    private BoneWeight[] weights;
    private Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        restPose = GetComponent<MeshFilter>().mesh;
        weights = restPose.boneWeights;
        colors = new Color[weights.Length];

        Debug.Log(weights.Length + " - " + restPose.vertices.Length + " - " + colors.Length);

        for (int i = 0; i < weights.Length; i++)
        {
            BoneWeight weight = weights[i];

            Color color = new Color(0, 0, 0, 1);
            if (boneIndex2Color.ContainsKey(weight.boneIndex0))
            {
                color += boneIndex2Color[weight.boneIndex0] * weight.weight0;
            }
            if (boneIndex2Color.ContainsKey(weight.boneIndex1))
            {
                color += boneIndex2Color[weight.boneIndex1] * weight.weight1;
            }
            if (boneIndex2Color.ContainsKey(weight.boneIndex2))
            {
                color += boneIndex2Color[weight.boneIndex2] * weight.weight2;
            }
            if (boneIndex2Color.ContainsKey(weight.boneIndex3))
            {
                color += boneIndex2Color[weight.boneIndex3] * weight.weight3;
            }

            colors[i] = color;
        }
        GetComponent<MeshFilter>().mesh.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {

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

        { 21, Color.white }, // KNEE RIGHT
        { 22, Color.red }, // ANKLE RIGHT
        { 23, Color.blue }, // FOOT RIGHT
        { 24, Color.blue }, // HIP RIGHT
    };
}