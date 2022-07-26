using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RunGUITimeScoreText : MonoBehaviour
{
    public TextMeshProUGUI terminal;

    // Start is called before the first frame update
    private void Start()
    {
        terminal = GetComponent<TextMeshProUGUI>();

        WipeTerminalTextIfPlayerHasntBeenSpawned();
    }

    // Update is called once per frame
    private void Update()
    {
        if(!LaneController.current.HasPlayerBeenSpawned)
        {
            WipeTerminalTextIfPlayerHasntBeenSpawned();
            return;
        }

        int score = 0; //score is STUB. score calculation to be done...
        int seconds = (int)RunTimer.TimeSinceLastRunStartSec;

        int dispSeconds = seconds % 60;
        int dispMinutes = seconds / 60;

        terminal.text = dispMinutes.ToString("00") + ":" + dispSeconds.ToString("00");
        terminal.text += "\n" + score.ToString("00000");
    }

    private void WipeTerminalTextIfPlayerHasntBeenSpawned()
    {
        if (LaneController.current == null)
            return;

        if (!LaneController.current.HasPlayerBeenSpawned)
            terminal.text = "";
    }
}
