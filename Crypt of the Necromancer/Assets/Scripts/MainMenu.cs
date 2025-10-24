using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        ClearStats();

        SceneManager.LoadScene("Tutorial-Test"); // SceneManager.GetActiveScene().buildIndex + 1
        
    }

    public void QuitGame()
    {
        ClearStats();

        #if UNITY_EDITOR
        // If in Unity Editor, stop Play Mode
        EditorApplication.isPlaying = false;
        #else
        Debug.Log("Quit");  // Message for knowing qit button(s) work in editor
        Application.Quit();
        #endif
    }

    // On game restart/replay, stats will be set to default value
    private void ClearStats()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ClearSave();
    }
}
