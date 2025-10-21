using System;
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

    // make sure all levels are ordered correctly (start -> levels -> end)
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

        if (IsPlayableLevel(scene.name)) // won't interact with start or end screens
        {
            lastSave.chestKeys = 0; // don't bring old keys to new levels
        }

        // find the new scene's Player and apply stats
        var player = FindObjectOfType<Player>();
        if (player != null) player.FromData(lastSave);  
    }

    bool IsPlayableLevel(string sceneName)
    {
        sceneName = sceneName.ToLowerInvariant();
        return sceneName.ToLower().StartsWith("level") || sceneName.ToLower().Contains("tutorial");
    }

    // For a new run
    public void ClearSave() { hasSave = false; lastSave = null; }

}
