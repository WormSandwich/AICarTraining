using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    public float maxTorque = 200f;
    public float brakeTorque = 90f;
    public float steerForce = 2f;

    public Transform centreOfMass;

    public float maxSensorDistance = 10f;

    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform[] tireMeshes = new Transform[4];

    private Rigidbody m_rigidBody;
    private float m_brakeTorque = 0f;

	// Use this for initialization
	void Start ()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.centerOfMass = centreOfMass.localPosition;
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

    private void FixedUpdate()
    {
        float steer = Input.GetAxis("Horizontal");
        //float finalAngle = steer * 45f;

        float acc = Input.GetAxis("Vertical");
        float finalAcc = acc * maxTorque;

        if(acc == 0f)
        {
            m_brakeTorque = brakeTorque;
        }
        else
        {
            m_brakeTorque = 0f;
        }


        //wheelColliders[i].motorTorque = finalAcc;
        // accelerate
        if (steer == 0f)
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].motorTorque = finalAcc;
                wheelColliders[i].brakeTorque = m_brakeTorque;
            }
        }
        // steer
        else
        {
            m_brakeTorque = 0f;

            wheelColliders[0].motorTorque = steer * maxTorque;
            wheelColliders[3].motorTorque = steer * maxTorque;

            wheelColliders[1].motorTorque = -1f * steer * maxTorque;
            wheelColliders[2].motorTorque = -1f * steer * maxTorque;

            for(int i=0; i<4; i++)
            {
                wheelColliders[i].brakeTorque = m_brakeTorque;
            }
        }
        //wheelColliders[0].steerAngle = finalAngle;
        //wheelColliders[1].steerAngle = finalAngle;
    }

    // Update is called once per frame
    void Update () {
        UpdateMeshPositions();
	}
}
