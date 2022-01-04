using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public string menuScene;

    public void OnBackClick()
    {
        SceneManager.LoadScene(menuScene);
    }
}
