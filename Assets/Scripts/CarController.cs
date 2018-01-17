﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    const int noOfSensors = 4;

    [Header("Car Physics Settings")]
    public Transform centreOfMass;
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform[] tireMeshes = new Transform[4];

    [Header("Motor Settings")]
    public float maxTorque = 200f;
    public float brakeTorque = 90f;
    public float steerForce = 2f;

    [Header("Sensor Settings")]
    public float maxSensorDistance = 10f;
    //public Transform forwardSensorMesh;
    public Transform[] sensorMeshes = new Transform[noOfSensors];
    public LineRenderer[] sensorLines = new LineRenderer[noOfSensors];

    private Rigidbody m_rigidBody;
    private float m_brakeTorque = 0f;
    private long hitCount = 0;
    private float[] sensorOutput = new float[noOfSensors];
	// Use this for initialization
	void Start ()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.centerOfMass = centreOfMass.localPosition;

        for(int i=0; i<noOfSensors; i++)
        {
            sensorOutput[i] = -1;
            sensorLines[i].startColor = Color.red;
            sensorLines[i].endColor = Color.red;
        }
	}
	
    void UpdateMeshPositions()
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion q;
            Vector3 pos;

            wheelColliders[i].GetWorldPose(out pos, out q);
            tireMeshes[i].position = pos;
            tireMeshes[i].rotation = q;

        }
    }

    private void Sensor()
    {
        for (int i = 0; i < noOfSensors; i++)
        {
            Ray ray = new Ray(sensorMeshes[i].position, sensorMeshes[i].forward);
            RaycastHit hit;
            sensorLines[i].SetPositions(new Vector3[2]
            { 
                sensorMeshes[i].position,
                (sensorMeshes[i].position + (sensorMeshes[i].forward * maxSensorDistance))
            });
            if (Physics.Raycast(ray, out hit, maxSensorDistance))
            {
                //Debug.Log("Hit on Sensor Number : " + i);
                sensorOutput[i] = Vector3.Distance(hit.point, sensorMeshes[i].position);
                sensorLines[i].startColor = Color.green;
                sensorLines[i].endColor = Color.green;
            }
            else
            {
                sensorOutput[i] = -1;
                sensorLines[i].startColor = Color.red;
                sensorLines[i].endColor = Color.red;
            }
        }
    }

    public float[] PollSensors()
    {
        float[] values = new float[noOfSensors];
        for(int i=0; i<noOfSensors; i++)
        {
            values[i] = sensorOutput[i];
        }
        return values;
    }

    public void MoveForward()
    {
        for(int i=0; i<4; i++)
        {
            wheelColliders[i].motorTorque = maxTorque;
            wheelColliders[i].brakeTorque = m_brakeTorque;
        }
    }

    public void MoveBackward()
    {
        for(int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = -maxTorque;
            wheelColliders[i].brakeTorque = m_brakeTorque;
        }
    }

    public void TurnRight()
    {
        wheelColliders[0].motorTorque = maxTorque;
        wheelColliders[3].motorTorque = maxTorque;

        wheelColliders[1].motorTorque = -1f * maxTorque;
        wheelColliders[2].motorTorque = -1f * maxTorque;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].brakeTorque = m_brakeTorque;
        }
    }

    public void TurnLeft()
    {
        wheelColliders[0].motorTorque = -1f * maxTorque;
        wheelColliders[3].motorTorque = -1f * maxTorque;

        wheelColliders[1].motorTorque = maxTorque;
        wheelColliders[2].motorTorque = maxTorque;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].brakeTorque = m_brakeTorque;
        }
    }

    private void FixedUpdate()
    {
        float steer = Input.GetAxis("Horizontal");
        float acc = Input.GetAxis("Vertical");

        if (steer == 0f && acc == 0f)
        {
            m_brakeTorque = brakeTorque;
            for(int i=0; i<4; i++)
            {
                wheelColliders[i].motorTorque = 0f;
                wheelColliders[i].brakeTorque = m_brakeTorque;
            }
        }
        else
        {
            m_brakeTorque = 0;
            if(steer < 0)
            {
                TurnLeft();
                return;
            }
            if(steer > 0)
            {
                TurnRight();
                return;
            }
            if(acc > 0)
            {
                MoveForward();
                return;
            }
            if(acc < 0)
            {
                MoveBackward();
            }
        }
    }

    private IEnumerator RunSensors()
    {
        while (true)
        {
            Sensor();
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Update is called once per frame
    void Update () {
        UpdateMeshPositions();
        StartCoroutine("Sensor");
	}
}