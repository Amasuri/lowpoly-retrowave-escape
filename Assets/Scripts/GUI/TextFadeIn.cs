using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFadeIn : MonoBehaviour
{
    public TextMeshProUGUI terminal;

    public static bool HasTitleTextFadedIn;
    public bool IsTitleText;

    // Start is called before the first frame update
    private void Start()
    {
        if (IsTitleText)
            HasTitleTextFadedIn = false;

        terminal = GetComponent<TextMeshProUGUI>();
        terminal.alpha = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        terminal.alpha += 0.25f * Time.deltaTime;

        if (IsTitleText && terminal.alpha >= 1f)
            HasTitleTextFadedIn = true;
    }
}
