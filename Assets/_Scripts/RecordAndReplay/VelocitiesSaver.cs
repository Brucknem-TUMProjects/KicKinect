using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class VelocitiesSaver
{
    public static void Save(List<float> velocities)
    {
        //KalmanFilter filter = new KalmanFilter(Time.fixedDeltaTime);

        string datapath = Application.dataPath + "/_Recordings/Velocities"; //" + DateTime.Now.ToString("d-MMM-yyyy-HH-mm-ss");
        Directory.CreateDirectory(datapath);
        List<string> lines = new List<string>() { "index,velocity,filtered" };

        for(int i = 0; i < velocities.Count; i++)
        {
            lines.Add(i + "," + velocities[i] + "," + 0);
        }

        File.WriteAllLines(datapath + "/velocities.csv", lines.ToArray());
    }
}