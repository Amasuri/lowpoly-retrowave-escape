using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonTypeSceneChange : MonoBehaviour
{
    public Button button;

    public ChangeSceneTo ChangeSceneToValue;
    public enum ChangeSceneTo
    {
        Runner,
        Credits
    }

    private void Start()
    {
        button.onClick.AddListener(ChangeScene);
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
}
