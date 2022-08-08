using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RunGUIDebugText : MonoBehaviour
{
    public TextMeshProUGUI terminal;

    private const int fpsBufferSize = 100;
    private Queue<float> fpsBuffer;

    private const bool DEBUG = true;

    // Start is called before the first frame update
    private void Start()
    {
        terminal = GetComponent<TextMeshProUGUI>();
        fpsBuffer = new Queue<float>(fpsBufferSize);
        for (int i = 0; i < fpsBufferSize; i++)
            fpsBuffer.Enqueue(60f);
        WipeTerminalTextIfPlayerHasntBeenSpawned();
    }

    // Update is called once per frame
    private void Update()
    {
        CalculateAndDisplayFPSAverage();
        DisplayFinalResAndCloseCallTimes();
        WipeTerminalTextIfPlayerHasntBeenSpawned();
    }

    private void CalculateAndDisplayFPSAverage()
    {
        fpsBuffer.Dequeue();
        fpsBuffer.Enqueue(1.0f / Time.deltaTime);
        var fpsAverage = fpsBuffer.Sum() / fpsBuffer.Count;

        terminal.text = "FPS: " + (fpsAverage).ToString("000");
        terminal.text += "       " + ScoreCounter.current.GetLastCloseCallDebug().ToString("0000");
    }

    private void DisplayFinalResAndCloseCallTimes()
    {
        terminal.text += "\n" + Screen.width + "x" + Screen.height;
        terminal.text += "    CC: " + ScoreCounter.current.GetCloseCallTimesDebug();
        terminal.text += "\n                  " + ScoreCounter.current.GetAllCloseCallDebug().ToString("000000");
    }

    private void WipeTerminalTextIfPlayerHasntBeenSpawned()
    {
        if (LaneController.current == null)
            return;

        if (!LaneController.current.HasPlayerBeenSpawned)
            terminal.text = "";
    }
}
