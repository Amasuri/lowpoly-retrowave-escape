using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipHighscoreText : MonoBehaviour
{
    /// <summary>
    /// One of the ways to know if settings menu is activated or not
    /// </summary>
    public GameObject refToFlashButton;
    public TextMeshProUGUI terminal;
    public GameObject refToTooltipObject;

    private void Start()
    {
    }

    private void Update()
    {
        var SettingsClosed = !refToFlashButton.activeSelf;
        var HasData = SystemSave.HighestScore > 0f || SystemSave.CumulativeScore > 0f;

        if (SettingsClosed && HasData && (!refToTooltipObject.activeSelf || terminal.text == ""))
        {
            refToTooltipObject.SetActive(true);
            refToTooltipObject.GetComponent<TextFadeOut>().ResetToOblique();
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
