using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SkeletonMock : MonoBehaviour
{
    [JsonObject]
    public class SerializableTransform
    {
        [JsonProperty]
        public float x, y, z = float.NaN;

        [JsonProperty]
        public float rx, ry, rz, rw = float.NaN;

        [JsonProperty]
        public float sx, sy, sz = float.NaN;

        public SerializableTransform(Transform transform) : this(transform.position, transform.rotation, transform.localScale) { }

        public SerializableTransform(Vector3 position, Quaternion rotation, Vector3 localScale) : this(
            position.x, position.y, position.z,
            rotation.x, rotation.y, rotation.z, rotation.w,
            localScale.x, localScale.y, localScale.z)
        { }

        [JsonConstructor]
        public SerializableTransform(float x, float y, float z, float rx, float ry, float rz, float rw, float sx, float sy, float sz)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.rx = rx;
            this.ry = ry;
            this.rz = rz;
            this.rw = rw;
            this.sx = sx;
            this.sy = sy;
            this.sz = sz;
        }

        [JsonIgnore]
        public Vector3 Position { get => new Vector3(x, y, z); }
        [JsonIgnore]
        public Vector3 LocalScale { get => new Vector3(sx, sy, sz); }
        [JsonIgnore]
        public Quaternion Rotation { get => new Quaternion(rx, ry, rz, rw); }
    }

    [JsonObject]
    public class SerializableJoint
    {
        [JsonProperty]
        public string name;

        [JsonProperty]
        public SerializableTransform transform;

        public SerializableJoint(string name, Transform transform) : this(name, new SerializableTransform(transform)) { }

        [JsonConstructor]
        public SerializableJoint(string name, SerializableTransform transform)
        {
            this.name = name;
            this.transform = transform;
        }
    }
    
    [JsonObject]
    public class SerializableJointFrame
    {
        [JsonProperty]
        public List<SerializableJoint> joints;
        
        public SerializableJointFrame() : this(new List<SerializableJoint>()) { }

        [JsonConstructor]
        public SerializableJointFrame(List<SerializableJoint> joints)
        {
            this.joints = joints;
        }
      
        public void Add(string name, Transform transform)
        {
            joints.Add(new SerializableJoint(name, transform));
        }

        public IEnumerator<SerializableJoint> GetEnumerator()
        {
            return joints.GetEnumerator();
        }

        [JsonIgnore]
        public int Count { get => joints.Count; }
    }

    private List<SerializableJointFrame> jointFrames = new List<SerializableJointFrame>();

    private string standardDirectory;
    private static readonly string filename = "Body.skeleton";
    private int frameIndex = -1;

    public bool record = true;
    public GameObject body;
    public Dictionary<string, GameObject> mockJoints;


    // Start is called before the first frame update
    void Start()
    {
        standardDirectory = Application.dataPath + "/_Recordings/_Skeleton/";
        if (record)
        {
            Directory.CreateDirectory(standardDirectory);
        }
        else
        {
            mockJoints = new Dictionary<string, GameObject>();
            foreach (Transform child in body.GetComponentInChildren<Transform>()) {
                mockJoints[child.gameObject.name] = child.gameObject;
            }

            jointFrames = Deserialize(standardDirectory + filename);
        }
    }

    public static List<SerializableJointFrame> Deserialize(string path)
    {
        Debug.Log(path);
        List<SerializableJointFrame> frames = new List<SerializableJointFrame>();

        object root = JsonConvert.DeserializeObject(File.ReadAllText(path));

        return frames;
    }

    private void Update()
    {
        if (record)
        {
            SerializableJointFrame currentFrame = new SerializableJointFrame();
            foreach (Transform child in gameObject.transform.GetComponentsInChildren<Transform>())
            {
                currentFrame.Add(child.gameObject.name, child);
            }
            jointFrames.Add(currentFrame);
        }
        else
        {
            frameIndex++;
            frameIndex %= jointFrames.Count;

            SerializableJointFrame frame = jointFrames[frameIndex];

            foreach(SerializableJoint joint in frame)
            {
                mockJoints[joint.name].transform.position = joint.transform.Position;
                mockJoints[joint.name].transform.rotation = joint.transform.Rotation;
                mockJoints[joint.name].transform.localScale = joint.transform.LocalScale;
            }
        }
    }

    private void OnDestroy()
    {
        //if (record)
        //{
        //    Debug.Log("Destroying");
        //    File.WriteAllText(standardDirectory + "Body.skeleton", JsonConvert.SerializeObject(jointFrames));
        //}
    }
}
