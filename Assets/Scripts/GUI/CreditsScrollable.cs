using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsScrollable : MonoBehaviour
{
    public bool IsMobile { get; private set; }

    public TextMeshProUGUI terminal;

    private Vector3 lastTouchPos;
    private Vector3 lastDefaultTextPos;

    private void Start()
    {
        IsMobile = SystemInfo.operatingSystem.Contains("Android");

        lastDefaultTextPos = terminal.transform.position;
    }

    private void Update()
    {
        //Works with touch in the same way. MouseUp = TouchEnded, MouseDown = TouchMoving (sorta), MouseJC = TouchStart
        //Used the united mouse+touch controls for easier debug

        var mouseDown = Input.GetMouseButton(0);
        var mouseJustClicked = Input.GetMouseButtonDown(0);
        var mouseUp = Input.GetMouseButtonUp(0);

        if (mouseJustClicked)
        {
            lastTouchPos = Input.mousePosition;
        }
        else if (mouseDown)
        {
            var delta = Input.mousePosition - lastTouchPos;

            terminal.transform.position = lastDefaultTextPos + new Vector3(0, delta.y, 0);
        }
        else if (mouseUp)
        {
            lastDefaultTextPos = new Vector3(lastDefaultTextPos.x, terminal.transform.position.y, lastDefaultTextPos.z);
        }
    }
}
