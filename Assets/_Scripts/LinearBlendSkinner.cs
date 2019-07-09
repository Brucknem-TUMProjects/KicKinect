using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(MeshFilter))]
public class LinearBlendSkinner : MonoBehaviour
{
    public static readonly Dictionary<Windows.Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
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

    private MeshFilter filter;
    private Mesh restPose;
    

    private void Start()
    {
        filter = GetComponent<MeshFilter>();
        restPose = filter.mesh;
    }
    
    //TODO calculate linear blend skinning based on parameters
    public void SetParameters(Kinect.IBody body)
    {
        SetRoot(body.Joints[Kinect.JointType.SpineBase]);
        //BoneWeight[] weights = restPose.boneWeights;

        //for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        //{
        //    Kinect.Joint sourceJoint = body.Joints[jt];
        //    Kinect.Joint? targetJoint = null;

        //    if (_BoneMap.ContainsKey(jt))
        //    {
        //        targetJoint = body.Joints[_BoneMap[jt]];
        //    }
        //}
    }

    private void SetRoot(Kinect.Joint rootBone)
    {
        transform.position = GetVector3FromJoint(rootBone);
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

    //TODO calculate pose parameters
    public List<Quaternion> CalculatePoseParameters(List<Kinect.Joint> jointLocations)
    {
        List<Quaternion> poseParameters = new List<Quaternion>();

        for(int i = 0; i < jointLocations.Count; i++)
        {
            poseParameters.Add(Quaternion.identity);
        }

        return poseParameters;
    }

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }

    private static readonly Dictionary<Kinect.JointType, int> joint2CharacterBoneMap = new Dictionary<Kinect.JointType, int>
    {

    };
}