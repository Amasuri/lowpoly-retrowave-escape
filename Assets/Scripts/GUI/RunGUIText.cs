using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RunGUIText : MonoBehaviour
{
    public TextMeshProUGUI terminal;

    private const int fpsBufferSize = 100;
    private Queue<float> fpsBuffer;

    // Start is called before the first frame update
    private void Start()
    {
        terminal = GetComponent<TextMeshProUGUI>();
        fpsBuffer = new Queue<float>(fpsBufferSize);
        for (int i = 0; i < fpsBufferSize; i++)
            fpsBuffer.Enqueue(60f);
    }

    // Update is called once per frame
    private void Update()
    {
        CalculateAndDisplayFPSAverage();
    }

    private void CalculateAndDisplayFPSAverage()
    {
        fpsBuffer.Dequeue();
        fpsBuffer.Enqueue(1.0f / Time.deltaTime);
        var fpsAverage = fpsBuffer.Sum() / fpsBuffer.Count;

        terminal.text = "FPS: " + (fpsAverage).ToString("000");
    }
}
