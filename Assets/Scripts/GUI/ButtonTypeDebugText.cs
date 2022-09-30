using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTypeDebugText : MonoBehaviour
{
    public BoxCollider2D BoxCollider;
    public GameObject refToDebugText;

    private static int ClicksSoFar = 0;
    private const int ClicksToActive = 1; //5

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if(ClicksSoFar >= ClicksToActive && !refToDebugText.active)
        {
            refToDebugText.SetActive(true);
        }
    }

    private void OnMouseDown()
    {
        ClicksSoFar++;
    }

    public void IncClickCount()
    {
        ClicksSoFar++;
    }
}
