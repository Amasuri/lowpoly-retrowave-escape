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

    private const float deltaTextUpdateS = 0.25f;
    private float nextTextUpdateInS = deltaTextUpdateS;
    private string cachedText = "";

    // Start is called before the first frame update
    private void Start()
    {
        nextTextUpdateInS = 0f;

        terminal = GetComponent<TextMeshProUGUI>();
        terminal.text = "";

        fpsBuffer = new Queue<float>(fpsBufferSize);
        for (int i = 0; i < fpsBufferSize; i++)
            fpsBuffer.Enqueue(60f);

        WipeTerminalTextIfPlayerHasntBeenSpawned();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!DEBUG)
            return;

        cachedText = "";

        CalculateAndDisplayFPSAverage();
        DisplayFinalResAndCloseCallTimes();
        WipeTerminalTextIfPlayerHasntBeenSpawned();

        nextTextUpdateInS -= Time.deltaTime;
        if(nextTextUpdateInS <= 0f)
        {
            terminal.text = cachedText;
            nextTextUpdateInS = deltaTextUpdateS;
        }
    }

    private void CalculateAndDisplayFPSAverage()
    {
        fpsBuffer.Dequeue();
        fpsBuffer.Enqueue(1.0f / Time.deltaTime);
        var fpsAverage = fpsBuffer.Sum() / fpsBuffer.Count;

        cachedText = "FPS: " + (fpsAverage).ToString("000");
        cachedText += "       " + ScoreCounter.current.GetLastCloseCallDebug().ToString("0000");
    }

    private void DisplayFinalResAndCloseCallTimes()
    {
        cachedText += "\n" + Screen.width + "x" + Screen.height;
        cachedText += "    CC: " + ScoreCounter.current.GetCloseCallTimesDebug();
        cachedText += "\n                  " + ScoreCounter.current.GetAllCloseCallDebug().ToString("000000");
    }

    private void WipeTerminalTextIfPlayerHasntBeenSpawned()
    {
        if (LaneController.current == null)
            return;

        if (!LaneController.current.HasPlayerBeenSpawned)
            cachedText = "";
    }
}
