using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class HomePageManager : MonoBehaviour
{
    public void StartDrumSession()
    {
        SceneManager.LoadScene("Drum Studio");
    }

    public void QuitApp()
    {
        Debug.Log("Attempting to quit the app...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
