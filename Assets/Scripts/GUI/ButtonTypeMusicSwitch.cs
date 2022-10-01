using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTypeMusicSwitch : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    public Button button;
    public TextMeshProUGUI terminal;
    public TextFadeOut textFadeComponent;

    private void Start()
    {
        //Most of the listeners are pre-added in Editor UI, but some tasks can't be done in Editor...
        button.onClick.AddListener(ToggleMusicOnClick);
    }

    private void ToggleMusicOnClick()
    {
        SystemSettings.MUSIC_ON = !SystemSettings.MUSIC_ON;

        var setting = SystemSettings.MUSIC_ON ? "yes" : "no";
        terminal.text = "Music: " + setting;
        textFadeComponent.ResetToOblique();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var setting = SystemSettings.MUSIC_ON ? "yes" : "no";
        terminal.text = "Music: " + setting;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var setting = SystemSettings.MUSIC_ON ? "yes" : "no";
        terminal.text = "Music: " + setting;
        textFadeComponent.ResetToOblique();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var setting = SystemSettings.MUSIC_ON ? "yes" : "no";
        terminal.text = "Music: " + setting;
        textFadeComponent.ResetToOblique();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var setting = SystemSettings.MUSIC_ON ? "yes" : "no";
        terminal.text = "Music: " + setting;
    }
}
