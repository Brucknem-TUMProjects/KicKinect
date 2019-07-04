using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Kinect = Windows.Kinect;

public class LinearBlendSkinningManager : MonoBehaviour
{
    public GameObject BodySourceManager;
    public LinearBlendSkinner[] avatars;

    private List<LinearBlendSkinner> _avatars;

    private Dictionary<ulong, LinearBlendSkinner> _Bodies = new Dictionary<ulong, LinearBlendSkinner>();
    private BodySourceManager _BodyManager;

    

    private void Start()
    {
        if (BodySourceManager == null)
        {
            return;
        }
        _avatars = new List<LinearBlendSkinner>(avatars);
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
    }

    void Update()
    {
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.IBody[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                _Bodies[trackingId].gameObject.SetActive(false);
                _Bodies[trackingId].gameObject.name = "untracked";
                _avatars.Add(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = AssociateAvatar(body.TrackingId);
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }

    private LinearBlendSkinner AssociateAvatar(ulong id)
    {
        int index = Random.Range(0, _avatars.Count - 1);
        LinearBlendSkinner avatar = _avatars[index];
        avatar.gameObject.name = "Avatar:" + id;
        avatar.gameObject.SetActive(true);
        _avatars.RemoveAt(index);
        return avatar;
    }

    private void RefreshBodyObject(Kinect.IBody body, LinearBlendSkinner bodyObject)
    {
        bodyObject.SetParameters(body);
    }
}
