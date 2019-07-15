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
    public bool record = false;

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

    void Update()
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

                if (record)
                {
                    if (HasTrackedBody(_Data))
                    {
                        i++;
                        File.WriteAllText(standardDirectory + "frame (" + i + ").json", JsonConvert.SerializeObject(_Data));
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

    public static bool HasTrackedBody(ICollection<IBody> bodies)
    {
        foreach (IBody body in bodies)
        {
            if (body.IsTracked)
            {
                return true;
            }
        }
        return false;
    }
}
