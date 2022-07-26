using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunButtonController : MonoBehaviour
{
    static public RunButtonController current;

    // Start is called before the first frame update
    private void Start()
    {
        current = this;
    }

    // Update is called once per frame
    private void Update()
    {
        if (LaneController.HasPlayerCollided)
            TurnOnRetryButton();
    }

    public void TurnOnRetryButton()
    {
        var n = transform.Find("Retry Button");
        if (n == null)
            return;

        n.gameObject.active = true;
    }
}
