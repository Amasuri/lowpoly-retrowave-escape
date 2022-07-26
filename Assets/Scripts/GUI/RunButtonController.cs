using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunButtonController : MonoBehaviour
{
    static public RunButtonController current;

    private bool retryButtonStartedCount;

    private const float maxRetryDelaySec = 3f;
    private float currentRetryDelaySec;

    // Start is called before the first frame update
    private void Start()
    {
        current = this;
        currentRetryDelaySec = maxRetryDelaySec;
        retryButtonStartedCount = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (LaneController.HasPlayerCollided)
            retryButtonStartedCount = true;

        if (retryButtonStartedCount)
            currentRetryDelaySec -= Time.deltaTime;

        if(currentRetryDelaySec <= 0f)
            TurnOnRetryButton();
    }

    public void TurnOnRetryButton()
    {
        var n = transform.Find("Retry Button");
        if (n == null)
            return;

        if (n.gameObject.active)
            return;

        n.gameObject.active = true;
    }
}
