using System;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(MeshFilter))]
public class LinearBlendSkinner : MonoBehaviour
{
    [Range(0, (int)Kinect.JointType.ThumbRight)]
    public int currentBone = 0;

    [Range(0, (int)Kinect.JointType.ThumbRight)]
    public int followingBones = 0;

    private Dictionary<Kinect.JointType, Kinect.JointType[]> _RealBoneMap = new Dictionary<Kinect.JointType, Kinect.JointType[]>()
    {
        { Kinect.JointType.HipLeft , new Kinect.JointType[]{ Kinect.JointType.KneeLeft } },
        { Kinect.JointType.KneeLeft , new Kinect.JointType[]{ Kinect.JointType.AnkleLeft } },
        { Kinect.JointType.AnkleLeft , new Kinect.JointType[]{ Kinect.JointType.FootLeft } },

        { Kinect.JointType.HipRight , new Kinect.JointType[]{ Kinect.JointType.KneeRight } },
        { Kinect.JointType.KneeRight , new Kinect.JointType[]{ Kinect.JointType.AnkleRight } },
        { Kinect.JointType.AnkleRight , new Kinect.JointType[]{ Kinect.JointType.FootRight } },

        { Kinect.JointType.SpineBase , new Kinect.JointType[]{ Kinect.JointType.SpineMid, Kinect.JointType.HipLeft, Kinect.JointType.HipRight } },
        { Kinect.JointType.SpineMid , new Kinect.JointType[]{ Kinect.JointType.SpineShoulder } },
        { Kinect.JointType.SpineShoulder , new Kinect.JointType[]{ Kinect.JointType.ShoulderLeft, Kinect.JointType.ShoulderRight, Kinect.JointType.Neck } },
        { Kinect.JointType.Neck , new Kinect.JointType[]{ Kinect.JointType.Head } },
        { Kinect.JointType.Head , new Kinect.JointType[]{ Kinect.JointType.Head } },

        { Kinect.JointType.ShoulderLeft , new Kinect.JointType[]{ Kinect.JointType.ElbowLeft } },
        { Kinect.JointType.ElbowLeft , new Kinect.JointType[]{ Kinect.JointType.WristLeft } },
        { Kinect.JointType.WristLeft , new Kinect.JointType[]{ Kinect.JointType.HandLeft } },
        { Kinect.JointType.HandLeft , new Kinect.JointType[]{ Kinect.JointType.HandTipLeft, Kinect.JointType.ThumbLeft } },

        { Kinect.JointType.ShoulderRight , new Kinect.JointType[]{ Kinect.JointType.ElbowRight } },
        { Kinect.JointType.ElbowRight , new Kinect.JointType[]{ Kinect.JointType.WristRight } },
        { Kinect.JointType.WristRight , new Kinect.JointType[]{ Kinect.JointType.HandRight } },
        { Kinect.JointType.HandRight , new Kinect.JointType[]{ Kinect.JointType.HandTipRight, Kinect.JointType.ThumbRight } },

        //{ Kinect.JointType.ShoulderLeft , new Kinect.JointType[]{ Kinect.JointType.ElbowLeft } },
        //{ Kinect.JointType.ElbowLeft , new Kinect.JointType[]{ Kinect.JointType.WristLeft } },
        //{ Kinect.JointType.WristLeft , new Kinect.JointType[]{ Kinect.JointType.HandLeft } },
        //{ Kinect.JointType.HandLeft , new Kinect.JointType[]{ Kinect.JointType.HandTipLeft, Kinect.JointType.ThumbLeft } },

        //{ Kinect.JointType.ShoulderRight , new Kinect.JointType[]{ Kinect.JointType.ElbowRight } },
        //{ Kinect.JointType.ElbowRight , new Kinect.JointType[]{ Kinect.JointType.WristRight } },
        //{ Kinect.JointType.WristRight , new Kinect.JointType[]{ Kinect.JointType.HandRight } },
        //{ Kinect.JointType.HandRight , new Kinect.JointType[]{ Kinect.JointType.HandTipRight, Kinect.JointType.ThumbRight } },
    };

    private Dictionary<Kinect.JointType, Kinect.JointType> _KinectBoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
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
    };

    /// <summary>
    /// A mapping between the rigged mesh joint indices and the Kinect joint types.
    /// </summary>
    public static readonly Dictionary<int, KeyValuePair<Kinect.JointType, Kinect.JointType>> boneIndex2JointType = new Dictionary<int, KeyValuePair<Kinect.JointType, Kinect.JointType>>
    {
        {0, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.SpineBase,Kinect.JointType.SpineMid )},
        {1, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.SpineMid,Kinect.JointType.SpineShoulder )},

        //{2, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.SpineShoulder ,Kinect.JointType.Neck )},

        {2, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.SpineShoulder ,Kinect.JointType.ShoulderRight )},
        {3, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.ShoulderRight ,Kinect.JointType.ElbowRight)},
        {4, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.ElbowRight ,Kinect.JointType.WristRight )},
        {5, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.WristRight ,Kinect.JointType.HandRight )},
        {6, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.HandRight ,Kinect.JointType.HandTipRight )},
        {7, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.HandRight ,Kinect.JointType.ThumbRight )},

        {8, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.SpineShoulder , Kinect.JointType.ShoulderLeft)},
        {9, new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.ShoulderLeft , Kinect.JointType.ElbowLeft )},
        {10,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.ElbowLeft , Kinect.JointType.WristLeft )},
        {11,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.WristLeft , Kinect.JointType.HandLeft)},
        {12,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.HandLeft , Kinect.JointType.HandTipLeft)},
        {13,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.HandLeft , Kinect.JointType.ThumbLeft  )},

        {14,new KeyValuePair<Kinect.JointType, Kinect.JointType>( Kinect.JointType.SpineShoulder,Kinect.JointType.Neck )},
        {15,new KeyValuePair<Kinect.JointType, Kinect.JointType>( Kinect.JointType.Neck,Kinect.JointType.Head )},
        {16,new KeyValuePair<Kinect.JointType, Kinect.JointType>( Kinect.JointType.Head,Kinect.JointType.Head )},

        {17,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.HipLeft , Kinect.JointType.KneeLeft)},
        {18,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.KneeLeft , Kinect.JointType.AnkleLeft)},
        {19,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.AnkleLeft , Kinect.JointType.FootLeft)},
        {20,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.SpineBase , Kinect.JointType.HipLeft)},

        {21,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.HipRight , Kinect.JointType.KneeRight)},
        {22,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.KneeRight , Kinect.JointType.AnkleRight)},
        {23,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.AnkleRight , Kinect.JointType.FootRight )},
        {24,new KeyValuePair<Kinect.JointType, Kinect.JointType>(Kinect.JointType.SpineBase , Kinect.JointType.HipRight)},
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
    
    private Dictionary<Kinect.JointType, GameObject> bonePositions;

    public GameObject SpineBase;
    public GameObject SpineMid;
    public GameObject Neck;
    public GameObject Head;
    public GameObject ShoulderLeft;
    public GameObject ElbowLeft;
    public GameObject WristLeft;
    public GameObject HandLeft;
    public GameObject ShoulderRight;
    public GameObject ElbowRight;
    public GameObject WristRight;
    public GameObject HandRight;
    public GameObject HipLeft;
    public GameObject KneeLeft;
    public GameObject AnkleLeft;
    public GameObject FootLeft;
    public GameObject HipRight;
    public GameObject KneeRight;
    public GameObject AnkleRight;
    public GameObject FootRight;
    public GameObject SpineShoulder;
    public GameObject HandTipLeft;
    public GameObject ThumbLeft;
    public GameObject HandTipRight;
    public GameObject ThumbRight;

    private Dictionary<Kinect.JointType, Vector3> restPosePositions = new Dictionary<Kinect.JointType, Vector3>();
    private Dictionary<KeyValuePair<Kinect.JointType, Kinect.JointType>, Vector3> restPoseOrientations = new Dictionary<KeyValuePair<Kinect.JointType, Kinect.JointType>, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        SetRestPoseValues();
        rend = GetComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        restPose = mesh;
        weights = restPose.boneWeights;
        restPoseVertices = restPose.vertices;
        SetBonePositions();
    }

    private void SetRestPoseOrientations()
    {
        foreach (KeyValuePair<Kinect.JointType, Kinect.JointType[]> pair in _RealBoneMap)
        {
            foreach (Kinect.JointType child in pair.Value)
            {
                Vector3 orientation = restPosePositions[child] - restPosePositions[pair.Key];
                restPoseOrientations.Add(new KeyValuePair<Kinect.JointType, Kinect.JointType>(pair.Key, child), orientation);
            }
        }
        Debug.Log(restPoseOrientations.Count);
    }

    private void SetRestPoseValues()
    {
        restPosePositions.Add(Kinect.JointType.SpineBase,       SpineBase    .transform.position  );
        restPosePositions.Add(Kinect.JointType.SpineMid        ,SpineMid     .transform.position  );
        restPosePositions.Add(Kinect.JointType.Neck            ,Neck         .transform.position  );
        restPosePositions.Add(Kinect.JointType.Head            ,Head         .transform.position  );
        restPosePositions.Add(Kinect.JointType.ShoulderLeft    ,ShoulderLeft .transform.position  );
        restPosePositions.Add(Kinect.JointType.ElbowLeft       ,ElbowLeft    .transform.position  );
        restPosePositions.Add(Kinect.JointType.WristLeft       ,WristLeft    .transform.position  );
        restPosePositions.Add(Kinect.JointType.HandLeft        ,HandLeft     .transform.position  );
        restPosePositions.Add(Kinect.JointType.ShoulderRight   ,ShoulderRight.transform.position  );
        restPosePositions.Add(Kinect.JointType.ElbowRight      ,ElbowRight   .transform.position  );
        restPosePositions.Add(Kinect.JointType.WristRight      ,WristRight   .transform.position  );
        restPosePositions.Add(Kinect.JointType.HandRight       ,HandRight    .transform.position  );
        restPosePositions.Add(Kinect.JointType.HipLeft         ,HipLeft      .transform.position  );
        restPosePositions.Add(Kinect.JointType.KneeLeft        ,KneeLeft     .transform.position  );
        restPosePositions.Add(Kinect.JointType.AnkleLeft       ,AnkleLeft    .transform.position  );
        restPosePositions.Add(Kinect.JointType.FootLeft        ,FootLeft     .transform.position  );
        restPosePositions.Add(Kinect.JointType.HipRight        ,HipRight     .transform.position  );
        restPosePositions.Add(Kinect.JointType.KneeRight       ,KneeRight    .transform.position  );
        restPosePositions.Add(Kinect.JointType.AnkleRight      ,AnkleRight   .transform.position  );
        restPosePositions.Add(Kinect.JointType.FootRight       ,FootRight    .transform.position  );
        restPosePositions.Add(Kinect.JointType.SpineShoulder   ,SpineShoulder.transform.position  );
        restPosePositions.Add(Kinect.JointType.HandTipLeft     ,HandTipLeft  .transform.position  );
        restPosePositions.Add(Kinect.JointType.ThumbLeft       ,ThumbLeft    .transform.position  );
        restPosePositions.Add(Kinect.JointType.HandTipRight    ,HandTipRight .transform.position  );
        restPosePositions.Add(Kinect.JointType.ThumbRight      ,ThumbRight   .transform.position  );
        SetRestPoseOrientations();
        RenderRestPosePositions();
    }

    private void RenderRestPosePositions()
    {
        foreach(KeyValuePair<Kinect.JointType, Vector3> pair in restPosePositions)
        {
            GameObject prim = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            prim.transform.position = pair.Value;
            prim.name = pair.Key.ToString();
            prim.transform.localScale = Vector3.one * 0.05f;
        }
    }

    private void SetBonePositions()
    {
        bonePositions = new Dictionary<Kinect.JointType, GameObject>();

        for (int i = 0; i <= (int)Kinect.JointType.ThumbRight; i++)
        {
            bonePositions[(Kinect.JointType)i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bonePositions[(Kinect.JointType)i].transform.localScale = Vector3.one * 0.05f;

            //restPoseBonePositions[]
        }
    }

    /// <summary>
    /// Calculates the linear blend skinning based on the given Kinect body data.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    public void SetParameters(Kinect.IBody body)
    {
        Vector3 root = SetRoot(body.Joints[Kinect.JointType.SpineBase]);
        //Debug.DrawRay(root, Vector3.up * 10000, Color.green);

        foreach(KeyValuePair<Kinect.JointType, GameObject> bonePosition in bonePositions)
        {
            bonePosition.Value.SetActive(false);
        }

        Vector3[] skinnedVertices = new Vector3[restPoseVertices.Length];
        Dictionary<KeyValuePair<Kinect.JointType, Kinect.JointType>, float> contributingJoints;
        Dictionary<Kinect.JointType, Kinect.Vector4> jointToKinectVector4Map = MapJointsToKinectVector4(body);
        Dictionary<Kinect.JointType, Quaternion> jointToRotationsMap = MapJointsToRotations(body);
        Dictionary<Kinect.JointType, Vector3> jointToPositionsMap = MapJointsToPositions(body);

        KeyValuePair<Kinect.JointType, Kinect.JointType> pair = boneIndex2JointType[currentBone];
        //foreach (KeyValuePair<Kinect.JointType, Kinect.JointType> pair in boneIndex2JointType.Values)
        {
            //Debug.DrawRay(restPosePositions[pair.Key], restPoseOrientation[new KeyValuePair<Kinect.JointType, Kinect.JointType>(pair.Key, pair.Value)], Color.red);

            //bonePositions[pair.Key].SetActive(true);
            //bonePositions[pair.Key].GetComponent<MeshRenderer>().material.color = Color.green;
            //bonePositions[pair.Key].transform.position = jointToPositionsMap[pair.Key];
            //bonePositions[pair.Value].SetActive(true);
            //bonePositions[pair.Value].GetComponent<MeshRenderer>().material.color = Color.red;
            //bonePositions[pair.Value].transform.position = jointToPositionsMap[pair.Value];
            //Debug.DrawRay(bonePositions[pair.Key].transform.position, bonePositions[pair.Value].transform.position - bonePositions[pair.Key].transform.position, Color.cyan);
        }

        for (int i = 0; i < restPoseVertices.Length; i++)
        {
            contributingJoints = MapContributingJointsToWeights(weights[i]);
            skinnedVertices[i] = Vector3.zero;

            foreach (KeyValuePair<Kinect.JointType, Kinect.JointType> joint in contributingJoints.Keys)
            {
                if((int)joint.Key < currentBone || (int)joint.Key >= (int)Kinect.JointType.ThumbRight || (int)joint.Key > currentBone + followingBones)
                {
                    continue;
                }
                Vector3 parentPosition = jointToPositionsMap[joint.Key];
                Vector3 childPosition = jointToPositionsMap[joint.Value];

                Vector3 currentOrientation = childPosition - parentPosition;

                //Debug.DrawRay(parentPosition, currentOrientation, Color.red);

                Matrix4x4 rotation = Matrix4x4.identity;// Matrix4x4.Rotate(Quaternion.FromToRotation(restPoseOrientations[joint], currentOrientation));
                Matrix4x4 translation = Matrix4x4.Translate(parentPosition);

                skinnedVertices[i] += (Vector3)(contributingJoints[joint] * (translation * rotation * restPoseVertices[i]));
                //Debug.Log(skinnedVertices[i]);
                //skinnedVertices[i] = FlipXZ(skinnedVertices[i]);
                //Debug.Log(skinnedVertices[i]);

            }
        }

        mesh.vertices = skinnedVertices;
    }

    private Vector3 FlipXZ(Vector3 v)
    {
        return v;
    }

    /// <summary>
    /// Maps the joint types to their measured Kinect joint orientation.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    private Dictionary<Kinect.JointType, Quaternion> MapJointsToRotations(Kinect.IBody body)
    {
        Dictionary<Kinect.JointType, Quaternion> joints = new Dictionary<Kinect.JointType, Quaternion>();
        for (int type = 0; type <= (int)Kinect.JointType.ThumbRight; type++)
        {
            joints.Add((Kinect.JointType)type, GetUnityMatrix4x4FromKinectVector4(body.JointOrientations[(Kinect.JointType)type].Orientation));
        }

        return joints;
    }

    /// <summary>
    /// Maps the joint types to their measured Kinect joint orientation.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    private Dictionary<Kinect.JointType, Kinect.Vector4> MapJointsToKinectVector4(Kinect.IBody body)
    {
        Dictionary<Kinect.JointType, Kinect.Vector4> joints = new Dictionary<Kinect.JointType, Kinect.Vector4>();
        for (int type = 0; type <= (int)Kinect.JointType.ThumbRight; type++)
        {
            joints.Add((Kinect.JointType)type, (body.JointOrientations[(Kinect.JointType)type].Orientation));
        }

        return joints;
    }

    /// <summary>
    /// Maps the joint types to their measured Kinect joint positions.
    /// </summary>
    /// <param name="body">The Kinect body data</param>
    private Dictionary<Kinect.JointType, Vector3> MapJointsToPositions(Kinect.IBody body)
    {
        Dictionary<Kinect.JointType, Vector3> joints = new Dictionary<Kinect.JointType, Vector3>();
        for (int type = 0; type <= (int)Kinect.JointType.ThumbRight; type++)
        {
            joints.Add((Kinect.JointType)type, GetVector3FromJoint(body.Joints[(Kinect.JointType)type]));
        }

        return joints;
    }

    /// <summary>
    /// Extracts the contributing bone indices from the bone weight and maps their respective Kinect joint type to their weight.
    /// </summary>
    /// <param name="weight">The bone weight of a vertex</param>
    /// <returns></returns>
    private Dictionary<KeyValuePair<Kinect.JointType, Kinect.JointType>, float> MapContributingJointsToWeights(BoneWeight weight)
    {
        Dictionary<KeyValuePair<Kinect.JointType, Kinect.JointType>, float> joints = new Dictionary<KeyValuePair<Kinect.JointType, Kinect.JointType>, float>();

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
        transform.position = root;
        return root;
    }

    //TODO calculate G(Theta, J)
    //TODO Maybe calculate Theta first
    public Matrix4x4 PartTransformation(List<Kinect.Joint> jointLocations)
    {
        return Matrix4x4.identity;
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
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
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

}