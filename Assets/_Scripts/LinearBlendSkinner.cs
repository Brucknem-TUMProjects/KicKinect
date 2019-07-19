using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(MeshFilter))]
public class LinearBlendSkinner : MonoBehaviour
{
    public bool completeRendering = true;

    public GameObject objectPool;

    [Range(0, (int)Kinect.JointType.ThumbRight)]
    public int currentBone = 0;

    [Range(0, (int)Kinect.JointType.ThumbRight)]
    public int followingBones = 0;

    public static readonly Dictionary<Kinect.JointType, Kinect.JointType> _KinectBoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
        { Kinect.JointType.Head, Kinect.JointType.Head },
    };

    /// <summary>
    /// A mapping between the rigged mesh joint indices and the Kinect joint types.
    /// </summary>
    public static readonly Dictionary<int, Kinect.JointType> boneIndex2JointType = new Dictionary<int, Kinect.JointType>
    {
        {(int)Kinect.JointType.SpineBase,  Kinect.JointType.SpineBase },
        {(int)Kinect.JointType.SpineMid,  Kinect.JointType.SpineMid },

        {(int)Kinect.JointType.Head,  Kinect.JointType.ShoulderLeft },
        {(int)Kinect.JointType.Neck,  Kinect.JointType.SpineShoulder },
        {(int)Kinect.JointType.SpineShoulder,  Kinect.JointType.HipRight },

        {(int)Kinect.JointType.ShoulderRight,  Kinect.JointType.ThumbLeft },
        {(int)Kinect.JointType.ElbowRight,  Kinect.JointType.ShoulderRight },
        {(int)Kinect.JointType.WristRight,  Kinect.JointType.ElbowRight },
        {(int)Kinect.JointType.HandRight,  Kinect.JointType.WristRight },
        {(int)Kinect.JointType.HandTipRight,  Kinect.JointType.FootLeft },
        {(int)Kinect.JointType.ThumbRight,  Kinect.JointType.Head },

        {(int)Kinect.JointType.ShoulderLeft,  Kinect.JointType.ElbowLeft },
        {(int)Kinect.JointType.ElbowLeft,  Kinect.JointType.WristLeft },
        {(int)Kinect.JointType.WristLeft,  Kinect.JointType.HandLeft },
        {(int)Kinect.JointType.HandLeft,  Kinect.JointType.HandTipLeft },
        {(int)Kinect.JointType.HandTipLeft,  Kinect.JointType.KneeLeft },
        {(int)Kinect.JointType.ThumbLeft,  Kinect.JointType.AnkleLeft },

        {(int)Kinect.JointType.FootRight,  Kinect.JointType.FootRight },
        {(int)Kinect.JointType.AnkleRight,  Kinect.JointType.AnkleRight },
        {(int)Kinect.JointType.KneeRight,  Kinect.JointType.KneeRight },
        {(int)Kinect.JointType.HipRight,  Kinect.JointType.HipLeft },

        {(int)Kinect.JointType.FootLeft,  Kinect.JointType.Neck },
        {(int)Kinect.JointType.AnkleLeft,  Kinect.JointType.ThumbRight },
        {(int)Kinect.JointType.KneeLeft,  Kinect.JointType.HandTipRight },
        {(int)Kinect.JointType.HipLeft,  Kinect.JointType.HandRight },
    };

    public static readonly Dictionary<Kinect.JointType, bool> jointToFlipOrientation = new Dictionary<Kinect.JointType, bool>()
    {
        { Kinect.JointType.FootLeft, true },
        { Kinect.JointType.AnkleLeft, false },
        { Kinect.JointType.KneeLeft, false },
        { Kinect.JointType.HipLeft, false },

        { Kinect.JointType.FootRight, true },
        { Kinect.JointType.AnkleRight, false },
        { Kinect.JointType.KneeRight, false },
        { Kinect.JointType.HipRight, false },

        { Kinect.JointType.HandTipLeft, true },
        { Kinect.JointType.ThumbLeft, true },
        { Kinect.JointType.HandLeft, false },
        { Kinect.JointType.WristLeft, false },
        { Kinect.JointType.ElbowLeft, false },
        { Kinect.JointType.ShoulderLeft, false },

        { Kinect.JointType.HandTipRight, true },
        { Kinect.JointType.ThumbRight, true },
        { Kinect.JointType.HandRight, false },
        { Kinect.JointType.WristRight, false },
        { Kinect.JointType.ElbowRight, false },
        { Kinect.JointType.ShoulderRight, false },

        { Kinect.JointType.SpineBase, true },
        { Kinect.JointType.SpineMid, true },
        { Kinect.JointType.SpineShoulder, true },
        { Kinect.JointType.Neck, true },
        { Kinect.JointType.Head, true },
    };

    /// <summary>
    /// The rest pose holding the raw mesh data.
    /// </summary>
    private Mesh restPose;

    /// <summary>
    /// The rest pose vertices.
    /// </summary>
    private Vector3[] restPoseVertices;

    /// <summary>
    /// The bone weights.
    /// </summary>
    private BoneWeight[] weights;

    /// <summary>
    /// Mesh renderer component to which the skinned mesh gets assigned.
    /// </summary>
    private MeshRenderer rend;

    /// <summary>
    /// The skinned mesh.
    /// </summary>
    private Mesh mesh;

    private static int usedIndices = 0;
    private Dictionary<Kinect.JointType, GameObject> restPoseGameObjects;
    private Dictionary<Kinect.JointType, GameObject> currentPoseGameObjects;

    private Dictionary<Kinect.JointType, Vector3> restPosePositions = new Dictionary<Kinect.JointType, Vector3>();
    private Dictionary<Kinect.JointType, Vector3> restPoseOrientations = new Dictionary<Kinect.JointType, Vector3>();

    private Dictionary<Kinect.JointType, Kinect.Vector4> jointToKinectVector4Map;
    private Dictionary<Kinect.JointType, Quaternion> jointToRotationsMap;
    private Dictionary<Kinect.JointType, Matrix4x4> jointToRodriguesMap;
    private Dictionary<Kinect.JointType, Vector3> jointToPositionsMap;
    private Dictionary<Kinect.JointType, Vector3> jointToOrientationsMap;

    private List<Dictionary<Kinect.JointType, float>> contributingJointsMap = new List<Dictionary<Kinect.JointType, float>>();

    private Matrix4x4 correctiveRotation = Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));

    // Start is called before the first frame update
    void Start()
    {
        SetRestPoseValues();
        rend = GetComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        restPose = mesh;
        weights = restPose.boneWeights;
        restPoseVertices = new Vector3[restPose.vertexCount];
        for (int i = 0; i < restPose.vertexCount; i++)
        {
            //restPoseVertices[i] = transform.TransformPoint(restPose.vertices[i].x, restPose.vertices[i].y, restPose.vertices[i].z);
            restPoseVertices[i] = new Vector3(restPose.vertices[i].x, restPose.vertices[i].y, restPose.vertices[i].z);
            contributingJointsMap.Add(MapContributingJointsToWeights(weights[i]));
        }
    }

    private void SetRestPoseOrientations()
    {
        foreach (KeyValuePair<Kinect.JointType, Vector3> joint in restPosePositions)
        {
            Kinect.JointType child = _KinectBoneMap[joint.Key];
            Vector3 orientation = restPosePositions[child] - joint.Value;
            restPoseOrientations.Add(joint.Key, orientation);
        }
        //Debug.Log(restPoseOrientations.Count);
    }

    private void SetRestPoseValues()
    {
        restPoseGameObjects = new Dictionary<Kinect.JointType, GameObject>();
        currentPoseGameObjects = new Dictionary<Kinect.JointType, GameObject>();
        restPosePositions = new Dictionary<Kinect.JointType, Vector3>();

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
                    GameObject o = objectPool.transform.GetChild(usedIndices++).gameObject;
                    o.transform.localScale = Vector3.one * 0.05f;
                    GameObject p = objectPool.transform.GetChild(usedIndices++).gameObject;
                    p.transform.localScale = Vector3.one * 0.05f;
                    restPoseGameObjects.Add(joint, p);
                    currentPoseGameObjects.Add(joint, o);
                    restPosePositions.Add(joint, bone.gameObject.transform.position);
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
        SetRestPoseOrientations();
    }


    /// <summary>
    /// Calculates the linear blend skinning based on the given Kinect body data.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    public void SetParameters(Kinect.IBody body)
    {
        //Vector3 root = SetRoot(body.Joints[Kinect.JointType.SpineBase]);
        //transform.localRotation = Quaternion.Euler(90, 0, 0);
        //Debug.DrawRay(root, Vector3.up * 10000, Color.green);

        foreach (KeyValuePair<Kinect.JointType, GameObject> currentPoseGameObject in currentPoseGameObjects)
        {
            currentPoseGameObject.Value.SetActive(false);
        }
        foreach (KeyValuePair<Kinect.JointType, GameObject> currentPoseGameObject in restPoseGameObjects)
        {
            currentPoseGameObject.Value.SetActive(false);
        }

        Vector3[] skinnedVertices = new Vector3[restPoseVertices.Length];

        MapJointsToKinectVector4(body);
        MapJointsToRotations(body);
        MapJointsToPositions(body);
        MapJointsToOrientations(body);
        //MapJointsToRodriguez(body);

        //Kinect.JointType selectedJoint = boneIndex2JointType[currentBone];
        foreach (Kinect.JointType selectedJoint in boneIndex2JointType.Values)
        //if(completeRendering)
        {
            Kinect.JointType n = _KinectBoneMap[selectedJoint];
            //Debug.DrawRay(currentPoseGameObject[c], currentPoseGameObject[c], Color.red);

            currentPoseGameObjects[selectedJoint].SetActive(true);
            currentPoseGameObjects[selectedJoint].GetComponent<MeshRenderer>().material.color = Color.green;
            currentPoseGameObjects[selectedJoint].transform.position = jointToPositionsMap[selectedJoint];
            currentPoseGameObjects[selectedJoint].transform.rotation = Quaternion.FromToRotation(Vector3.up, jointToOrientationsMap[selectedJoint]);

            Vector3 orientation = (jointToRotationsMap[selectedJoint] * Vector3.up).normalized * 0.1f;
            //if (!jointToFlipOrientation[selectedJoint])
            {
                orientation *= -1;
            }
            Debug.DrawRay(currentPoseGameObjects[selectedJoint].transform.position, orientation, Color.cyan);
        }

        //List<String> debug = new List<string>();
        //debug.Add("");
        for (int i = 0; i < restPoseVertices.Length; i++)
        {
            //Debug.Log("RestPoseVertex[" + i + "] = " + restPoseVertices[i]);
            skinnedVertices[i] = Vector3.zero;

            //string debug = "restPoseVertices[" + i + "] = " + restPoseVertices[i] + "\n\n";

            //debug[debug.Count - 1] += ("skinnedVertices[" + i + "]: " + skinnedVertices[i] + " - " + "restPoseVertices[" + i + "]: " + restPoseVertices[i] + "\n");
            foreach (Kinect.JointType joint in contributingJointsMap[i].Keys)
            {
                //Debug.Log(joint.ToString() + " = " + jointToPositionsMap[joint]);
                if (!completeRendering && ((int)joint < currentBone || (int)joint >= (int)Kinect.JointType.ThumbRight || (int)joint > currentBone + followingBones))
                {
                    continue;
                }

                Matrix4x4 rotation = Matrix4x4.identity;
                //Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.up, jointToOrientationsMap[joint]));
                //Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.FromToRotation(restPoseOrientations[joint], jointToOrientationsMap[joint]));
                //Matrix4x4 rotation = Matrix4x4.Rotate(jointToRotationsMap[joint]);
                Matrix4x4 translation = Matrix4x4.Translate(jointToPositionsMap[joint]);
                Matrix4x4 scaling = Matrix4x4.Scale(new Vector3(1, 1, 1));

                Vector3 r = restPoseVertices[i];
                r = correctiveRotation.MultiplyPoint3x4(rotation.MultiplyPoint3x4(r));
                switch (joint)
                {
                    case Kinect.JointType.SpineBase:
                    case Kinect.JointType.SpineMid:
                    case Kinect.JointType.SpineShoulder:
                    case Kinect.JointType.Neck:
                        r = Quaternion.Euler(0, 180, 0) * r;
                        r = jointToRotationsMap[joint] * r;
                        break;
                    case Kinect.JointType.HipLeft:
                    case Kinect.JointType.KneeLeft:
                    case Kinect.JointType.AnkleLeft:
                        //r = Quaternion.Euler(0, 90, 180) * r;
                        //r = jointToRotationsMap[joint] * r;
                        //r = Quaternion.Euler(180, 180, 0) * r;
                        break;
                }

                switch (joint)
                {
                    case Kinect.JointType.KneeLeft:
                        if (!jointToFlipOrientation[joint])
                        {
                            //r *= -1;
                        }
                        break;
                }

                Vector3 preAssignment = (Vector3)(contributingJointsMap[i][joint] *  translation.MultiplyPoint3x4(r));

                skinnedVertices[i] += preAssignment;
                //debug[debug.Count - 1] += joint.ToString() + "Weight: " + contributingJoints[joint] +
                //    "\nRotation: \n" + rotation.ToString() +
                //    "\nTranslation: \n" + jointToPositionsMap[joint].ToString() +
                //    "\nScaling\n" + scaling.ToString() + "\n";
                //debug += "\n\n ************************************************************************************* \n\n";
            }
            //skinnedVertices[i] /= 4;
            if(i % 100 == 0)
            {
                //debug.Add("");
            }
            //Debug.Log(debug);
        }
        //UnityEditor.EditorApplication.isPlaying = false;
        //foreach (string s in debug)
        {
           // Debug.Log(s);
        }
        mesh.vertices = skinnedVertices;
    }

    /// <summary>
    /// Maps the joint types to their measured Kinect joint rotation.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    private void MapJointsToRotations(Kinect.IBody body)
    {
        jointToRotationsMap = new Dictionary<Kinect.JointType, Quaternion>();
        jointToRodriguesMap = new Dictionary<Kinect.JointType, Matrix4x4>();
        for (int type = 0; type <= (int)Kinect.JointType.ThumbRight; type++)
        {
            Quaternion rotation = GetUnityMatrix4x4FromKinectVector4(body.JointOrientations[(Kinect.JointType)type].Orientation);
            if (!jointToFlipOrientation[(Kinect.JointType)type])
            {
                //rotation = Quaternion.Euler(-rotation.eulerAngles);
            }
            jointToRotationsMap.Add((Kinect.JointType)type, rotation);
            jointToRodriguesMap.Add((Kinect.JointType)type,Rodrigues(rotation * Vector3.up));
        }
    }

    /// <summary>
    /// Maps the joint types to their measured Kinect joint orientation.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    private void MapJointsToOrientations(Kinect.IBody body) { 
        jointToOrientationsMap = new Dictionary<Kinect.JointType, Vector3>();
        for (int type = 0; type <= (int)Kinect.JointType.ThumbRight; type++)
        {
            Vector3 parentPosition = jointToPositionsMap[(Kinect.JointType)type];
            Vector3 childPosition = jointToPositionsMap[_KinectBoneMap[(Kinect.JointType)type]];

            Vector3 currentOrientation = childPosition - parentPosition;
            jointToOrientationsMap.Add((Kinect.JointType)type, currentOrientation);
        }

    }


    /// <summary>
    /// Maps the joint types to their measured Kinect joint orientation.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    private void MapJointsToKinectVector4(Kinect.IBody body)
    {
         jointToKinectVector4Map = new Dictionary<Kinect.JointType, Kinect.Vector4>();
        for (int type = 0; type <= (int)Kinect.JointType.ThumbRight; type++)
        {
            jointToKinectVector4Map.Add((Kinect.JointType)type, (body.JointOrientations[(Kinect.JointType)type].Orientation));
        }
    }

    /// <summary>
    /// Maps the joint types to their measured Kinect joint positions.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    private void MapJointsToPositions(Kinect.IBody body)
    {
        string debug = "";
        jointToPositionsMap = new Dictionary<Kinect.JointType, Vector3>();
        for (int type = 0; type <= (int)Kinect.JointType.ThumbRight; type++)
        {
            Vector3 pos = GetVector3FromJoint(body.Joints[(Kinect.JointType)type]);
            jointToPositionsMap.Add((Kinect.JointType)type, pos);

            debug += pos + "\t - " + (Kinect.JointType)type + "\n";
        }
        Debug.Log(debug);
    }

    /// <summary>
    /// Extracts the contributing bone indices from the bone weight and maps their respective Kinect joint type to their weight.
    /// </summary>
    /// <param name="weight">The bone weight of a vertex</param>
    /// <returns></returns>
    private Dictionary<Kinect.JointType, float> MapContributingJointsToWeights(BoneWeight weight)
    {
        Dictionary<Kinect.JointType, float> joints = new Dictionary<Kinect.JointType, float>();

        if (boneIndex2JointType.ContainsKey(weight.boneIndex0))
        {
            if (!joints.ContainsKey(boneIndex2JointType[weight.boneIndex0]))
            {
                joints.Add(boneIndex2JointType[weight.boneIndex0], weight.weight0);
            }
        }
        if (boneIndex2JointType.ContainsKey(weight.boneIndex1))
        {
            if (!joints.ContainsKey(boneIndex2JointType[weight.boneIndex1]))
            {
                joints.Add(boneIndex2JointType[weight.boneIndex1], weight.weight1);
            }
        }
        if (boneIndex2JointType.ContainsKey(weight.boneIndex2))
        {
            if (!joints.ContainsKey(boneIndex2JointType[weight.boneIndex2]))
            {
                joints.Add(boneIndex2JointType[weight.boneIndex2], weight.weight2);
            }
        }
        if (boneIndex2JointType.ContainsKey(weight.boneIndex3))
        {
            if (!joints.ContainsKey(boneIndex2JointType[weight.boneIndex3]))
            {
                joints.Add(boneIndex2JointType[weight.boneIndex3], weight.weight3);
            }
        }
        return joints;
    }

    /// <summary>
    /// Sets the position of the root bone
    /// </summary>
    /// <param name="rootBone"></param>
    private Vector3 SetRoot(Kinect.Joint rootBone)
    {
        Vector3 root = GetVector3FromJoint(rootBone);
        transform.parent.position = root;
        return root;
    }
    
    //public Matrix4x4 GTrafo(Vector3 rotation, Vector3 joint_position)
    //{
    //    Matrix3x3 expomap = ExpoMap(rotation);
    //    Matrix4x4 G = new Matrix4x4();
    //    for (int i = 0; i < 3; i++)
    //    {
    //        G.SetColumn(i, new Vector4(expomap.Column(i).x, expomap.Column(i).y, expomap.Column(i).z, 0));
    //    }
    //    G.SetColumn(3, new Vector4(joint_position.x, joint_position.y, joint_position.z, 1));
    //    return G;
    //}
    //private Matrix3x3 ExpoMap(Vector3 rotation)
    //{
    //    //v_rot = R*v
    //    //where
    //    //R = I_3 + (sin w)K + (1 - cos w)K^2
    //    //where 
    //    //K = 
    //    //     0   - k_z    k_y
    //    //   k_z       0   -k_x
    //    // - k_y     k_x      0
    //    //implying K*v =  k x v
    //    //and k is the rotation axis vector (unit vector)
    //    //and w is the rotation angle.
    //    //k is the normalized vector of "rotation"
    //    //w is the norm of "rotation"
    //    Vector3 k = new Vector3(rotation.x, rotation.y, rotation.z);
    //    k.Normalize();
    //    float w = norm(rotation);

    //    Matrix3x3 I_3 = Matrix3x3.identity;
    //    Matrix3x3 K = new Matrix3x3(
    //          0f,  -k.z,   k.y,
    //         k.z,    0f,  -k.z,
    //        -k.y,   k.x,    0f
    //        );
    //    Matrix3x3 K2 = K * K;
    //    Matrix3x3 R = I_3 + (K * Mathf.Sin(w)) + (K2 * (1 - Mathf.Cos(w)));

    //    return R;
    //}
    //private static float norm(Vector3 v)
    //{
    //    return Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
    //}


    /// <summary>
    /// Extracts a unity vector from a Kinect joint
    /// </summary>
    /// <param name="joint"></param>
    /// <returns></returns>
    private Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return transform.TransformVector(new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z));
    }

    /// <summary>
    /// Converts a Kinect vector to a unity vector.
    /// https://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/index.htm
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private static Quaternion GetUnityMatrix4x4FromKinectVector4(Kinect.Vector4 vector)
    {
        return new Quaternion(vector.X, vector.Y, vector.Z, vector.W);
    }

    /// <summary>
    /// Rodrigues formula
    /// https://en.wikipedia.org/wiki/Rodrigues%27_rotation_formula
    /// </summary>
    /// <param name="_k"></param>
    /// <returns></returns>
    private static Matrix4x4 Rodrigues(Vector3 _k)
    {
        Vector3 k = _k.normalized;
        float theta = _k.magnitude;

        Matrix4x4 K = new Matrix4x4(
            new Vector4(0, k.z, -k.y,0),
            new Vector4(-k.z, 0, k.x,0),
            new Vector4(k.y, -k.x, 0,0),
            new Vector4(0,0,0,1)
            );

        Matrix4x4 sin = Matrix4x4.Scale(Vector3.one * ((float) Math.Sin(theta)));
        Matrix4x4 cos = Matrix4x4.Scale(Vector3.one * ((float)Math.Cos(1 - theta)));
        Matrix4x4 R = Add(Add(Matrix4x4.identity, (sin * K)), cos * K * K);

        return R;
    }

    /// <summary>
    /// Matrix addition
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static Matrix4x4 Add(Matrix4x4 a, Matrix4x4 b)
    {
        Matrix4x4 result = Matrix4x4.zero;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                result[i,j] = a[i, j] + b[i, j];
            }
        }
        return result;
    }
}