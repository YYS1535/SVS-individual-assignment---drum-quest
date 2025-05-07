using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumstickVelocityTracker : MonoBehaviour
{
    private Vector3 previousPosition;
    public Vector3 Velocity { get; private set; }

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        Velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }
}
