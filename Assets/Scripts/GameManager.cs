using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TestCode;
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
    private GameState gameState;
    private string dataReceived;

    Thread thread;
    static readonly object lockObject = new object();
    string returnData = "";
    bool processData = false;
    Google.Protobuf.ByteString image;

    // Use this for initialization
	void Start () {
        trialNumber = 0;
        totalTrials = 10;
        messageText.text = "Trial No: " + trialNumber;
        image = Google.Protobuf.ByteString.Empty;

        SpawnCar();

        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
        StartCoroutine(GameLoop());
	}

	// Update is called once per frame
	void Update () {
		
	}

    private void ThreadMethod()
    {
        tcpManager = new TCPManager("127.0.0.1", 10000);
        string recieved = "";
        while (true)
        {
           lock (lockObject)
            {
                gameState = new GameState
                {
                    Sensor0 = 1.2f,
                    Sensor1 = 1.2f,
                    Sensor2 = 1.2f,
                    Sensor4 = 1.2f,
                    VelX = 1.2f,
                    VelY = 1.2f,
                    VelZ = 1.2f,
                    RotX = 1.2f,
                    RotY = 1.2f,
                    RotZ = 1.2f,
                    Image = image
                };
            }
            tcpManager.SendReq(gameState, out recieved);
            lock (lockObject) { 
                dataReceived = recieved;
                processData = true;
            }
        }
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

        //tcpManager.Dispose();
    }

    private IEnumerator TrialStart()
    {
        ResetCar();
        trialNumber++;
        /*
        if(tempFlag)
        {
            string fileName = Application.dataPath + "/screenshots/shot.png";
            System.IO.File.WriteAllBytes(fileName, carManager.GetImage(512,512));
            Debug.Log(string.Format("Took screenshot to: {0}", fileName));
            tempFlag = false;
        }
        */
        messageText.text = "Trial No: " + trialNumber;
 
        yield return null;
    }

    private IEnumerator TrialRunning()
    {
        while (!carManager.HasFailed())
        {
            if (processData)
            {
                lock (lockObject)
                {
                    if (dataReceived == "FD")
                    {
                        carManager.MoveForward();
                    }
                    else if (dataReceived == "BK")
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
                    processData = false;
                    image = Google.Protobuf.ByteString.CopyFrom(carManager.GetImage(256, 256));
                    Debug.Log("Size: " + gameState.CalculateSize());
                }
            }
            yield return null;
        }
    }

    public void ResetCar()
    {
        carManager.Reset();
    }
}
