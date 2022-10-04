using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreText : MonoBehaviour
{
    /// <summary>
    /// One of the ways to know if settings menu is activated or not
    /// </summary>
    public GameObject refToFlashButton;
    public TextMeshProUGUI terminal;
    public GameObject refToHighscoreObject;
    public Image refToPlayButton;

    private void Start()
    {
    }

    private void Update()
    {
        var SettingsClosed = !refToFlashButton.activeSelf;
        var HasData = SystemSave.HighestScore > 0f || SystemSave.CumulativeScore > 0f;
        var HasPlaybuttonFinishedFadingIn = refToPlayButton.color.a >= 0.75f;

        //Scenarios in which tooltip seems inappropriate
        if(!SettingsClosed || !HasData || LaneController.current.HasPlayerBeenSpawned || !HasPlaybuttonFinishedFadingIn)
        {
            refToHighscoreObject.SetActive(false);
            return;
        }

        //Code below assumes that settings are closed, there is data and player isn't running, etc etc (from above)
        if (!refToHighscoreObject.activeSelf || terminal.text == "" || terminal.text == "i'm highscore!")
        {
            refToHighscoreObject.SetActive(true);
            SetDisplayData();
        }
    }

    private void SetDisplayData()
    {
        var highest = SystemSave.HighestScore.ToString();
        var total = SystemSave.CumulativeScore.ToString();

        terminal.text = "Highest:" + highest + "\nTotal:" + total;
    }
}
