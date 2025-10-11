using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Player.PlayerData lastSave;
    private bool hasSave;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    public void SaveFrom(Player p)
    {
        lastSave = p.ToData();
        hasSave = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!hasSave) return;   // no saved data (first scene or first playable scene after menu)

        // find the new scene's Player and apply stats
        var player = FindObjectOfType<Player>();
        if (player != null) player.FromData(lastSave);  
    }

    // For a new run
    public void ClearSave() { hasSave = false; lastSave = null; }

}
