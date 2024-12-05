using System;
using Newtonsoft.Json;
using OVRSimpleJSON;
using UnityEngine;

public class PlayScript : MonoBehaviour
{
    public static event Action<Note> PlayNote;
    public static event Action<float> StartProgressBar;

    private Note _notePlaying;
    private Note _noteToPlay;
    private int _cycles = 0;
    private int _countdown = 3;
    // private float _poseTime = 2f;
    // private const float _feedbackTime = 1.5f;

    [SerializeField]
    private HandReader _handReader;
    [SerializeField]
    private ServerGateway _serverGateway;
    [SerializeField]
    private PlayCanvasUIManager _playCanvasUIManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        _cycles = 0;
        _countdown = 3;
        _playCanvasUIManager.SetModelGuess("..");
        _playCanvasUIManager.SetStatus("..");
    }

    public void StartPlayCountdown()
    {
        _playCanvasUIManager.SetStatus("Get ready..");
        StartProgressBar?.Invoke(3f);
        InvokeRepeating("Tick", 0, 1);
    }

    private void Tick()
    {
        _playCanvasUIManager.SetStatus("Start playing in " + _countdown.ToString() + "...");
        _countdown--;
        if (_countdown < 0)
        {
            CancelInvoke();
            Invoke("Play", 0f);
        }
    }

    public void Play()
    {
        // Tell him what to do
        // _playCanvasUIManager.SetModelGuess("..");
        // _playCanvasUIManager.SetStatus("Keep steady pose..");
        // StartProgressBar?.Invoke(_poseTime);

        // Collect data and send it to the server
        Invoke("CollectDataAndSendToServer", 0f);
    }

    private void CollectDataAndSendToServer()
    {

        if (!_handReader.IsHandTracking())
        {
            Debug.LogError("Hand tracking lost!");
            _playCanvasUIManager.SetModelGuess("No hand data, retrying..");
            Invoke("Play", 1.5f);
        }
        else
        {
            // Tell the user we're collecting data
            // _playCanvasUIManager.SetStatus("Acquiring pose..");

            // Init RequestWrapper
            RequestWrapper requestWrapper = new RequestWrapper();

            // Get the hand data
            for (int i = 0; i < 32; i++)
            {
                HandWrapper leftHandWrapper = _handReader.ReadHand(HandSide.Left);
                HandWrapper rightHandWrapper = _handReader.ReadHand(HandSide.Right);
                requestWrapper.LeftHandWrappers.Add(leftHandWrapper);
                requestWrapper.RightHandWrappers.Add(rightHandWrapper);
            }

            SendToServerAndHandle(requestWrapper);
        }
    }

    private void SendToServerAndHandle(RequestWrapper requestWrapper)
    {
        // Send the hand data to the server
        _serverGateway.SendHandDataPlayMode(requestWrapper, (response) =>
        {
            // Parse the response
            Note notePredicted = ParseResponse(response);

            // If the note is not the same as the previous one, play it
            if (notePredicted != null && _notePlaying != notePredicted)
            {
                // Update the playing note
                _notePlaying = notePredicted;

                // Do the business logic
                _playCanvasUIManager.SetModelGuess(notePredicted.ToString());
                _playCanvasUIManager.SetStatus("Playing..");
                PlayNote?.Invoke(notePredicted);
                // UpdatePoseTime();
            }
            // Repeat!
            Invoke("Play", 0.1f);
        });
    }

    private static Note ParseResponse(string response)
    {
        try
        {
            int messageAsInt = int.Parse(JSON.Parse(response)["message"]);
            Note notePredicted = NotesList.Notes[messageAsInt];
            return notePredicted;
        }
        catch (FormatException e)
        {
            Debug.Log("Error parsing response: " + e.Message);
            return null;
        }
    }

    // private void UpdatePoseTime()
    // {
    //     if (_poseTime > 1.0f && _cycles % 5 == 0)
    //     {
    //         _poseTime = _poseTime - 0.25f;
    //     }
    //     _cycles++;
    // }

    public void StopPlaying()
    {
        CancelInvoke("Play");
        CancelInvoke("CollectDataAndSendToServer");
        CancelInvoke("Tick");
        _playCanvasUIManager.SetModelGuess("..");
    }

}
