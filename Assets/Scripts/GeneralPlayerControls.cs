using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralPlayerControls : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ResetAndReloadScene()
    {
        CarController.ResetAllCarsBeforeSceneWipe();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
