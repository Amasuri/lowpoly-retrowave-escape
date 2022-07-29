using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFadeIn : MonoBehaviour
{
    public TextMeshProUGUI terminal;

    // Start is called before the first frame update
    private void Start()
    {
        terminal = GetComponent<TextMeshProUGUI>();
        terminal.alpha = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        terminal.alpha += 0.25f * Time.deltaTime;
    }
}
