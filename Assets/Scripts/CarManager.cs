using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Car Spawn and Movement Controller, Gets Sensor data
 */

[Serializable]
public class CarManager {

    public Transform spawnPoint;
    [HideInInspector] public GameObject instance;

    private CarController carController;

	// Use this for initialization
	public void Setup () {
        carController = instance.GetComponent<CarController>();
	}
	
    public bool HasFailed()
    {
        float[] sensorData = carController.PollSensors();
        for(int i=0; i<sensorData.Length; i++)
        {
            //Debug.Log("Sensor [" + i + "] = " + sensorData[i]);
            if(sensorData[i] < 3 && sensorData[i]>0)
            {
                return true;
            }
        }
        return false;
    }

    public void MoveForward()
    {
        carController.acc = 1f;
    }

    public void MoveBackward()
    {
        carController.acc = -1f;
    }

    public void TurnRight()
    {
        carController.steer = 1f;
    }

    public void TurnLeft()
    {
        carController.steer = -1f;
    }

    public void Stop()
    {
        carController.acc = 0f;
        carController.steer = 0f;
    }

	public void Reset()
    {
        instance.transform.position = spawnPoint.position;
        instance.transform.rotation = spawnPoint.rotation;

        instance.SetActive(false);
        instance.SetActive(true);
    }
}
