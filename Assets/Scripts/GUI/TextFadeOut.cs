using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFadeOut : MonoBehaviour
{
    public TextMeshProUGUI terminal;

    public bool isTooltipText;

    private void Start()
    {
        terminal = GetComponent<TextMeshProUGUI>();

        if (isTooltipText)
        {
            terminal.alpha = 0f;
            terminal.text = "";
        }
    }

    private void Update()
    {
        if (terminal.alpha > 0f)
            terminal.alpha -= 0.5f * Time.deltaTime;
    }

    public void ResetToOblique()
    {
        terminal.alpha = 1f;
    }
}
