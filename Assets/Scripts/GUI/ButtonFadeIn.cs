using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFadeIn : MonoBehaviour
{
    public Image thisImage;
    public Button thisButton;

    // Start is called before the first frame update
    private void Start()
    {
        thisImage.color = new Color(1, 1, 1, 0);
        thisButton.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        thisImage.color = new Color(1, 1, 1, thisImage.color.a + (0.25f * Time.deltaTime));
        if (thisImage.color.a >= 1f && !thisButton.enabled)
            thisButton.enabled = true;
    }
}
