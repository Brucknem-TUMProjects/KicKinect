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

    // Start is called before the first frame update
    void Start()
    {
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
                lr.startColor = WeightVisualization.jointToColor[jt];
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
}