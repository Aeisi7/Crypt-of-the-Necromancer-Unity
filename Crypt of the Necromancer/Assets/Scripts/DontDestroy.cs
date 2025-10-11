using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance { get; private set; }

    // variables for defaults to reset player on game over
    private Player player;
    private int maxHealth, curHealth, chestKeys, projDamage, maxMana, curMana;
    private float mSpeed;
    private bool levelKey;

    private void Awake()
    {
        // prevents duplicates when switching scenes
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //gets default values for when scene
        player = GetComponent<Player>();
        if (player)
        {
            maxHealth = player.GetMaxHealth();
            curHealth = player.GetCurHealth();
            chestKeys = player.GetChestKeyCount();
            projDamage = player.GetProjDamage();
            maxMana = player.GetMaxMana();
            curMana = player.GetCurMana();
            mSpeed = player.GetSpeed();
            levelKey = player.GetLevelKey();
        }
    }

    /* - for placing the player on a an empty game object when new scene is loaded -*/
    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Look for an object named/ tagged "PlayerSpawn" or a component PlayerSpawn
        var marker = GameObject.FindWithTag("PlayerSpawn");
        if (marker != null) transform.position = marker.transform.position;
    }
    /*------------------------------------------------------------------------------------*/

    // getters for resetting in player class
    public int GetCurHealth() { return curHealth; }

    public int GetMaxHealth() { return maxHealth; }

    public int GetCurMana() { return curMana; }

    public int GetMaxMana() { return maxMana; }

    public int GetChestKeyCount() { return chestKeys; }

    public int GetProjDamage() { return projDamage; }

    public bool GetLevelKey() { return levelKey; }

    public float GetSpeed() { return mSpeed; }
}
