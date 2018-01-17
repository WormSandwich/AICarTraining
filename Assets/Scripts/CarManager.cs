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
	
	public void Reset()
    {
        instance.transform.position = spawnPoint.position;
        instance.transform.rotation = spawnPoint.rotation;

        instance.SetActive(false);
        instance.SetActive(true);
    }
}
