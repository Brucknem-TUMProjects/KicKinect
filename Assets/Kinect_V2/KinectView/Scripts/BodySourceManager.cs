using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class BodySourceManager : MonoBehaviour 
{
    public bool mock = false;
    private string standardDirectory;

    private IKinectSensor _Sensor;
    private IBodyFrameReader _Reader;
    private IBody[] _Data = null;
    
    private int i = -1;

    public IBody[] GetData()
    {
        return _Data;
    }


    void Start()
    {
        standardDirectory = Application.dataPath + "/_Recordings/_Skeleton/";
        Directory.CreateDirectory(standardDirectory);

        if (mock)
        {
            _Sensor = KinectSensor.GetMock();
        }
        else
        {
            _Sensor = KinectSensor.GetDefault();
        }

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader(standardDirectory);

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }
    
    void Update () 
    {
        if (_Reader != null)
        {
            IBodyFrame frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new IBody[_Sensor.BodyFrameSource.BodyCount];
                }
                
                frame.GetAndRefreshBodyData(_Data);



                if (!mock)
                {
                    if (PrintBodyTrackingStatus((int)(Time.time * 1000), _Data))
                    {
                        IBody[] currentBodies = (IBody[])_Data.Clone();
                        i++;
                        File.WriteAllText(standardDirectory + i + ".json", JsonConvert.SerializeObject(currentBodies));
                    }
                }

                frame.Dispose();
                frame = null;
            }
        }    
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
    
    private bool PrintBodyTrackingStatus(int i, ICollection<IBody> bodies)
    {
        string s = "Frame: " + i + " --- ";
        bool hasTrackedBody = false;
        foreach (IBody body in bodies)
        {
            if (body.IsTracked)
            {
                Debug.Log("Found a tracked one!!!");
                hasTrackedBody = true;
            }
            s += " - " + body.IsTracked;
        }
        return hasTrackedBody;
    }
}
