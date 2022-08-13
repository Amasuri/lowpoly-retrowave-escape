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

    // Start is called before the first frame update
    private void Start()
    {
        terminal = GetComponent<TextMeshProUGUI>();
        terminal.text = "";
        nextTextUpdateInS = 0f;

        WipeTerminalTextIfPlayerHasntBeenSpawned();
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
    }

    private void WipeTerminalTextIfPlayerHasntBeenSpawned()
    {
        if (LaneController.current == null)
            return;

        if (!LaneController.current.HasPlayerBeenSpawned)
            cachedText = "";
    }
}
