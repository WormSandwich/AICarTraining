using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Configured for single car training. Can be easily changed for multiple cars.
 */

public class GameManager : MonoBehaviour {

    public GameObject carPrefab;
    public CarManager carManager;

    private double trialNumber;

	// Use this for initialization
	void Start () {
        SpawnCar();
        StartCoroutine(GameLoop());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnCar()
    {
        carManager.instance = Instantiate(carPrefab, carManager.spawnPoint.position, carManager.spawnPoint.rotation) as GameObject;
        carManager.Setup();
    }

    IEnumerator GameLoop()
    {
        yield return null;
    }
}
