using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RunGUITimeScoreText : MonoBehaviour
{
    public TextMeshProUGUI terminal;
    static public RunGUITimeScoreText current;

    private const float deltaTextUpdateS = 0.25f;
    private float nextTextUpdateInS = deltaTextUpdateS;
    private string cachedText = "";

    private float CloseCallTimerLeftSec = 0f;
    private float CloseCallTimerMax = 1.5f;

    public bool HadCloseCallRecently => CloseCallTimerLeftSec > 0f;

    // Start is called before the first frame update
    private void Start()
    {
        terminal = GetComponent<TextMeshProUGUI>();
        terminal.text = "";
        nextTextUpdateInS = 0f;

        current = this;

        WipeTerminalTextIfPlayerHasntBeenSpawned();

        CloseCallEvent.OnCloseCall += StartCloseCallText;
        CloseCallFailureEvent.OnCloseCallFail += StartCloseCallFailText;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!LaneController.current.HasPlayerBeenSpawned)
        {
            WipeTerminalTextIfPlayerHasntBeenSpawned();
            return;
        }

        if(CloseCallTimerLeftSec > 0f)
        {
            CloseCallTimerLeftSec -= Time.deltaTime;
        }

        nextTextUpdateInS -= Time.deltaTime;
        if (nextTextUpdateInS <= 0f)
        {
            cachedText = "";
            CalculateAndUpdateScoreString();
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

        var lastCCval = ScoreCounter.current.GetLastCloseCallDebug();
        var sign = lastCCval > 0f ? "+" : "-";
        if(CloseCallTimerLeftSec > 0f)
            cachedText += "\n" + sign + lastCCval.ToString("0000") + "!";
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

    private void StartCloseCallFailText()
    {
        //The context is as such: if failed CC happened ~immediately after CC, don't display it, it's probably a side bump thing
        if (CloseCallTimerMax - CloseCallTimerLeftSec <= 0.05f) //~50ms; backbumps happen correctly on 0-60s speed, head2head bumps displayed on 10s but not on 60s speed
        {
            CloseCallTimerLeftSec = 0f;
            return;
        }

        CloseCallTimerLeftSec = CloseCallTimerMax;
    }

    private void OnDestroy()
    {
        CloseCallEvent.OnCloseCall -= StartCloseCallText;
        CloseCallFailureEvent.OnCloseCallFail -= StartCloseCallFailText;
    }

    private void OnDisable()
    {
        CloseCallEvent.OnCloseCall -= StartCloseCallText;
        CloseCallFailureEvent.OnCloseCallFail -= StartCloseCallFailText;
    }
}
