using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RunGUITimeScoreText : MonoBehaviour
{
    public TextMeshProUGUI terminal;

    private const float deltaTextUpdateS = 0.25f;
    private float nextTextUpdateInS = deltaTextUpdateS;
    private string cachedText = "";

    private float CloseCallTimerLeftSec = 0f;
    private float CloseCallTimerMax = 1.5f;

    // Start is called before the first frame update
    private void Start()
    {
        terminal = GetComponent<TextMeshProUGUI>();
        terminal.text = "";
        nextTextUpdateInS = 0f;

        WipeTerminalTextIfPlayerHasntBeenSpawned();

        CloseCallEvent.OnCloseCall += StartCloseCallText;
    }

    // Update is called once per frame
    private void Update()
    {
        cachedText = "";

        if (!LaneController.current.HasPlayerBeenSpawned)
        {
            WipeTerminalTextIfPlayerHasntBeenSpawned();
            return;
        }

        if(CloseCallTimerLeftSec > 0f)
        {
            CloseCallTimerLeftSec -= Time.deltaTime;
        }

        CalculateAndUpdateScoreString();

        nextTextUpdateInS -= Time.deltaTime;
        if (nextTextUpdateInS <= 0f)
        {
            terminal.text = cachedText;
            nextTextUpdateInS = deltaTextUpdateS;
        }
    }

    private void CalculateAndUpdateScoreString()
    {
        int score = ScoreCounter.current.TotalScore;
        int seconds = (int)RunTimer.TimeSinceLastRunStartSec;

        int dispSeconds = seconds % 60;
        int dispMinutes = seconds / 60;

        cachedText = dispMinutes.ToString("00") + ":" + dispSeconds.ToString("00");
        cachedText += "\n" + score.ToString("000000");

        if(CloseCallTimerLeftSec > 0f)
            cachedText += "\n+" + ScoreCounter.current.GetLastCloseCallDebug().ToString("0000") + "!";
    }

    private void WipeTerminalTextIfPlayerHasntBeenSpawned()
    {
        if (LaneController.current == null)
            return;

        if (!LaneController.current.HasPlayerBeenSpawned)
            cachedText = "";
    }

    private void StartCloseCallText()
    {
        CloseCallTimerLeftSec = CloseCallTimerMax;
    }

    private void OnDestroy()
    {
        CloseCallEvent.OnCloseCall -= StartCloseCallText;
    }

    private void OnDisable()
    {
        CloseCallEvent.OnCloseCall -= StartCloseCallText;
    }
}
