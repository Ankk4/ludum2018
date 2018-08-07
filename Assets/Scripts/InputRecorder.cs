using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRecorder : MonoBehaviour
{
    private enum InputRecorderState
    {
        Recording,
        Playback,
        Off
    }
    private InputRecorderState currentState;

    private List<InputData> inputBuffer;
    private int currentIndex;

	void Start ()
    {
        currentState = InputRecorderState.Recording;
        inputBuffer = new List<InputData>();
	}
	
    public void ResolveInput(bool canJump = false)
    {
        if (currentState == InputRecorderState.Recording)
        {
            InputData data = new InputData();
            data.index = currentIndex;
            data.horizontal = Input.GetAxis("Horizontal");
            data.vertical = Input.GetAxis("Vertical");
            data.jump = canJump ? Input.GetButton("Jump") : false;
            inputBuffer.Add(data);
            currentIndex++;
        }
    }

    public List<InputData> GetRecording()
    {
        return inputBuffer;
    }

    public InputData GetRecordByIndex(int index)
    {
        return inputBuffer[index];
    }
}
