using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/*
 * Configured for single car training. Can be easily changed for multiple cars.
 */

public class GameManager : MonoBehaviour {

    public GameObject carPrefab;
    public CarManager carManager;

    public Text messageText;

    private double trialNumber;
    private double totalTrials;

    static TCPManager tcpManager;
    private string dataToSend;
    private string dataReceived;

	// Use this for initialization
	void Start () {
        trialNumber = 0;
        totalTrials = 10;
        messageText.text = "Trial No: " + trialNumber;
        SpawnCar();

        tcpManager = new TCPManager("127.0.0.1", 10000);

        dataToSend = "FWD";
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
        yield return StartCoroutine(TrialStart());

        yield return StartCoroutine(TrialRunning());

        if(trialNumber <= totalTrials)
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator TrialStart()
    {
        ResetCar();
        trialNumber++;

        messageText.text = "Trial No: " + trialNumber;
 
        yield return null;
    }

    private IEnumerator TrialRunning()
    {
        while (!carManager.HasFailed())
        {
            tcpManager.SendReq(dataToSend,out dataReceived);
            if(dataReceived == "FD")
            {
                carManager.MoveForward();
            }
            else if(dataReceived == "BK")
            {
                carManager.MoveBackward();
            }
            else if (dataReceived == "RT")
            {
                carManager.TurnRight();
            }
            else if (dataReceived == "LT")
            {
                carManager.TurnLeft();
            }
            else
            {
                carManager.Stop();
            }
            yield return null;
        }
    }

    public void ResetCar()
    {
        carManager.Reset();
    }
}
