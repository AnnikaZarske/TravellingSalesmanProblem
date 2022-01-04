using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public string experimentScene;
    public string visualScene;
    
    public void OnExperimentClick()
    {
        SceneManager.LoadScene(experimentScene);
    }

    public void OnVisualClick()
    {
        SceneManager.LoadScene(visualScene);
    }
}
