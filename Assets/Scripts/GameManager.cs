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

    static ZSocketManager zSocketManager;
    private string dataReceived;

    Thread thread;
    static readonly object lockObject = new object();
    bool processData = false;
    Google.Protobuf.ByteString image;
    float[] sensorData = new float[10] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};

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
        zSocketManager = new ZSocketManager("tcp://127.0.0.1:10000");
        string recieved = "";
        while (true)
        {
            GameState gameState = new GameState();
            lock (lockObject)
            {
                gameState.Sensor0 = sensorData[0];
                gameState.Sensor1 = sensorData[1];
                gameState.Sensor2 = sensorData[2];
                gameState.Sensor3 = sensorData[3];
                gameState.VelX = sensorData[4];
                gameState.VelY = sensorData[5];
                gameState.VelZ = sensorData[6];
                gameState.RotX = sensorData[7];
                gameState.RotY = sensorData[8];
                gameState.RotZ = sensorData[9];
                gameState.Image = image;
                
            }
            zSocketManager.SendReq(gameState, out recieved);
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
                    sensorData = carManager.PollSensors();
                    
                    /*
                    Debug.Log("data: ");
                    for(int i=0; i<10; i++)
                    {
                        Debug.Log("Sensor " + i + " : " + sensorData[i]);
                    }
                    */
                    
                }
            }
            yield return null;
        }
    }

    public void ResetCar()
    {
        carManager.Reset();
    }

    void OnApplicationQuit()
    {
       
    }

}
