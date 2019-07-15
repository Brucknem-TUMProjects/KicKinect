using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(WeightVisualization))]
public class RestPoseSkeletonViewer : MonoBehaviour
{
    public GameObject armature;
    public Material boneMaterial;
    public Transform renderParent;
    private Dictionary<Kinect.JointType, GameObject> restPoseGameObjects;
    private WeightVisualization wv;

    // Start is called before the first frame update
    void Start()
    {
        wv = GetComponent<WeightVisualization>();
        SetRestPoseValues();
    }

    private void Update()
    {
        SetLines();
    }

    private void SetLines()
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject sourceJoint = restPoseGameObjects[jt];
            GameObject targetJoint = null;

            if (LinearBlendSkinner._KinectBoneMap.ContainsKey(jt))
            {
                targetJoint = restPoseGameObjects[LinearBlendSkinner._KinectBoneMap[jt]];
            }
            
            LineRenderer lr = sourceJoint.GetComponent<LineRenderer>();
            if (targetJoint != null)
            {
                lr.SetPosition(0, sourceJoint.transform.position);
                lr.SetPosition(1, targetJoint.transform.position);
                //lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                lr.startColor = wv.boneIndex2ColorMap[jointType2BoneIndex[jt]];
                lr.gameObject.GetComponent<MeshRenderer>().material.color = lr.startColor;
                lr.endColor = lr.startColor;
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    private void SetRestPoseValues()
    {
        restPoseGameObjects = new Dictionary<Kinect.JointType, GameObject>();

        Transform[] bones = transform.parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i <= (int)Kinect.JointType.ThumbRight; i++)
        {
            Kinect.JointType joint = (Kinect.JointType)i;
            string s = joint.ToString();
            bool found = false;
            foreach (Transform bone in bones)
            {
                if (bone.name.Equals(s))
                {
                    GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    jointObj.transform.position = bone.position + Vector3.right * 2;
                    LineRenderer lr = jointObj.AddComponent<LineRenderer>();
                    lr.positionCount = 2;
                    lr.material = boneMaterial;
                    lr.startWidth = 0.01f;
                    lr.endWidth = 0.005f;
                    jointObj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                    jointObj.name = joint.ToString();
                    jointObj.transform.parent = renderParent;
                    restPoseGameObjects.Add(joint, jointObj);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw new KeyNotFoundException("Bone: " + s + " not found");
            }
            else
            {
                Debug.Log("Bone: " + s + " found");
            }
        }
    }

    public static readonly Dictionary<Kinect.JointType, int> jointType2BoneIndex = new Dictionary<Kinect.JointType, int>
    {
        {Kinect.JointType.SpineBase, 0 },
        {Kinect.JointType.SpineMid , 1},

        {Kinect.JointType.ShoulderRight, 2 },
        {Kinect.JointType.ElbowRight ,3},
        {Kinect.JointType.WristRight ,4},
        {Kinect.JointType.HandRight,5 },
        {Kinect.JointType.HandTipRight,6 },
        {Kinect.JointType.ThumbRight ,7},

        { Kinect.JointType.ShoulderLeft,8 },
        { Kinect.JointType.ElbowLeft ,9},
        { Kinect.JointType.WristLeft ,10},
        { Kinect.JointType.HandLeft ,11},
        { Kinect.JointType.HandTipLeft,12 },
        { Kinect.JointType.ThumbLeft,13 },

        {Kinect.JointType.SpineShoulder,14 },
        {Kinect.JointType.Neck ,15},
        {Kinect.JointType.Head ,16},

        {Kinect.JointType.KneeLeft,17 },
        {Kinect.JointType.AnkleLeft ,18},
        {Kinect.JointType.FootLeft ,19},
        {Kinect.JointType.HipLeft ,20},

        {Kinect.JointType.KneeRight ,21},
        {Kinect.JointType.AnkleRight ,22},
        {Kinect.JointType.FootRight,23 },
        {Kinect.JointType.HipRight ,24},
    };
}
