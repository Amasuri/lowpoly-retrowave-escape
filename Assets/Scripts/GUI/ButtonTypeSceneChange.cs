using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonTypeSceneChange : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public Button button;

    public ChangeSceneTo ChangeSceneToValue;

    private float lastPointerDownTimestamp;
    public enum ChangeSceneTo
    {
        Runner,
        Credits
    }

    private void Start()
    {
        //button.onClick.AddListener(ChangeScene);
    }

    private void ChangeScene()
    {
        switch(ChangeSceneToValue)
        {
            case ChangeSceneTo.Credits:
                SceneManager.LoadScene("Credits");
                SceneManager.UnloadSceneAsync("Runner");
                break;

            case ChangeSceneTo.Runner:
                SceneManager.LoadScene("Runner");
                SceneManager.UnloadSceneAsync("Credits");
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var timeNow = Time.realtimeSinceStartup;

        //The idea: long press -> only display tooltip. Quick press -> do the function
        if(timeNow - lastPointerDownTimestamp < 1f)
            ChangeScene();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPointerDownTimestamp = Time.realtimeSinceStartup;
    }
}
