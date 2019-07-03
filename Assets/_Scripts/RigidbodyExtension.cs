using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyExtension : MonoBehaviour
{
    public bool debugVelocity = false;

    private Rigidbody rb;
    private Vector3 lastPosition;
    private Vector3 _velocity;

    public Vector3 Velocity
    {
        get
        {
            return _velocity;
        }
        private set
        {
            _velocity = value;
            Velocities.Add(_velocity.magnitude);
        }
    }

    public List<float> Velocities { get; private set; }

    private void Start()
    {
        Velocities = new List<float>();
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        Vector3 distance = transform.position - lastPosition;
        lastPosition = transform.position;

        Velocity = distance / Time.deltaTime;

        if (debugVelocity)
        {
            Debug.Log(Velocity.magnitude);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float m1 = rb.mass;
        Vector3 v1 = Velocity;
        float m2 = collision.rigidbody.mass;
        Vector3 v2 = collision.rigidbody.velocity;

        Vector3 u2 = (2 * m1 * v1 + (m2 - m1) * v2) / (m1 + m2);
        collision.rigidbody.velocity = u2;
    }

    private void OnApplicationQuit()
    {
        if (debugVelocity)
        {
            VelocitiesSaver.Save(Velocities);
        }
    }
}