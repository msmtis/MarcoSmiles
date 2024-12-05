using System;
using Newtonsoft.Json;
using OVRSimpleJSON;
using UnityEngine;

public class TrainingScript : MonoBehaviour
{
    public static event Action<Note> PlayNote;
    public static event Action<int> UpdateAccuracy;
    public static event Action<float> StartProgressBar;

    [SerializeField]
    private HandReader _handReader;
    [SerializeField]
    private TrainingCanvasUIManager _trainingCanvasUIManager;
    [SerializeField]
    private ServerGateway _serverGateway;

    private Note _noteToPlay;
    private int _cycles = 0;
    private float _poseTime = 5f;
    private const float _fedbackTime = 1.5f;

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
        _trainingCanvasUIManager.SetNoteToPlay("..");
        _trainingCanvasUIManager.SetModelGuess("..");
        _trainingCanvasUIManager.SetStatus("..");
        NextFourNotes.GenerateNextFourNotes();
    }

    public void StartTraining()
    {
        Invoke("Train", 0);
    }

    private void Train()
    {

        // Get a random note to play
        _noteToPlay = NextFourNotes.GetNextNote();

        // Tell the user to play it
        _trainingCanvasUIManager.SetNoteToPlay(_noteToPlay.ToString());
        _trainingCanvasUIManager.SetModelGuess("...");
        _trainingCanvasUIManager.SetStatus("Keep steady pose..");
        StartProgressBar?.Invoke(_poseTime);

        // Collect data and send it to the server
        Invoke("CollectDataAndSendToServer", _poseTime);

    }

    public void StopTraining()
    {
        CancelInvoke("Train");
        CancelInvoke("CollectDataAndSendToServer");
        _serverGateway.EndTraining((response) =>
        {
            _trainingCanvasUIManager.SetModelGuess("...");
            _trainingCanvasUIManager.SetNoteToPlay("...");
            _trainingCanvasUIManager.SetStatus("...");
        });
    }

    private void CollectDataAndSendToServer()
    {

        if (!_handReader.IsHandTracking())
        {
            Debug.LogError("Hand tracking lost!");
            _trainingCanvasUIManager.SetModelGuess("No hand data, retrying..");
            Invoke("Train", 1.5f);
        }
        else
        {
            // Tell the user we're collecting data
            _trainingCanvasUIManager.SetStatus("Acquiring pose..");

            // Init RequestWrapper
            RequestWrapper requestWrapper = new RequestWrapper(_noteToPlay.ComputeDistance());

            // Get the hand data
            for (int i = 0; i < 128; i++)
            {
                HandWrapper leftHandWrapper = _handReader.ReadHand(HandSide.Left);
                HandWrapper rightHandWrapper = _handReader.ReadHand(HandSide.Right);
                requestWrapper.LeftHandWrappers.Add(leftHandWrapper);
                requestWrapper.RightHandWrappers.Add(rightHandWrapper);
            }
            Debug.LogError("Note sent: " + JsonConvert.SerializeObject(_noteToPlay.ToString() + " - " + _noteToPlay.ComputeDistance()));

            SendToServer(requestWrapper);
        }
    }

    private void SendToServer(RequestWrapper requestWrapper)
    {
        // Send the hand data to the server
        _serverGateway.SendHandData(requestWrapper, (response) =>
        {
            // Parse the response
            (int accuracy, Note notePredicted) = ParseResponse(response);

            // Do the business logic
            _trainingCanvasUIManager.SetModelGuess(notePredicted.ToString());
            UpdateStatus(notePredicted);
            PlayNote?.Invoke(notePredicted);
            UpdateAccuracy?.Invoke(accuracy);
            UpdatePoseTime();

            // Repeat!
            Invoke("Train", _fedbackTime);
        });
    }

    private static (int, Note) ParseResponse(string response)
    {
        int accuracy = int.Parse(JSON.Parse(response)["accuracy"]);
        int messageAsInt = int.Parse(JSON.Parse(response)["message"]);
        Note notePredicted = NotesList.Notes[messageAsInt];
        return (accuracy, notePredicted);
    }

    private void UpdateStatus(Note modelGuess)
    {
        if (modelGuess == _noteToPlay)
        {
            _trainingCanvasUIManager.SetStatus("Correct!");
            _noteToPlay.GuessedCounter++;
        }
        else
        {
            _trainingCanvasUIManager.SetStatus("Incorrect, keep going!");
        }
    }

    private void UpdatePoseTime()
    {
        if (_poseTime >= 2f && _cycles % 10 == 0)
        {
            _poseTime = _poseTime - 0.25f;
        }
        _cycles++;
    }

}
