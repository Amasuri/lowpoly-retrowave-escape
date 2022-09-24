using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFlash : MonoBehaviour, IPointerEnterHandler
{
    public Button button;
    public TextMeshProUGUI terminal;

    private void Start()
    {
        //Most of the listeners are pre-added in Editor UI, but some tasks can't be done in Editor...
        button.onClick.AddListener(ToggleFlash);
    }

    private void ToggleFlash()
    {
        SystemSettings.FLASHES_ON = !SystemSettings.FLASHES_ON;

        var setting = SystemSettings.FLASHES_ON ? "yes" : "no";
        terminal.text = "Flashing lights: " + setting;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var setting = SystemSettings.FLASHES_ON ? "yes" : "no";
        terminal.text = "Flashing lights: " + setting;
    }
}
