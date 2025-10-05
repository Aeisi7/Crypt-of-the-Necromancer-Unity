using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool open = false;  // is chest open?

    // Chest Visuals
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer srOpenCLosed;
    [SerializeField] private Sprite closedChest, openChest;     // set in unity 

    public enum ChestRewards { levelKey, increaseHealth, IncreaseDamage, IncreaseMovementSpeed }

    [Header("Reward")]
    [SerializeField] private ChestRewards reward = ChestRewards.levelKey;   // change in unity depending on 
    [SerializeField] private int healthBoost = 4;
    [SerializeField] private int damageBoost = 1;
    [SerializeField] private float speedBoost = 0.5f;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        // Shouldn't interact with an already opened chest
        if (open) return;

        // only let player interact with chest
        if (!other.CompareTag("Player")) return;

        // Only should interact with player, and only open if player has a chest key
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); ;
        if (player == null) return;   // is there an active player object in game
        
        // Checks if player has chest key
        if (!player.CanOpenChest())
        {
            //TODO: play sound effect
            return;
        }

        // Give player reward
        switch (reward)
        {
            case ChestRewards.levelKey:
                player.GrabLevelKey();
                break;

            case ChestRewards.increaseHealth:
                player.IncreaseHealth(healthBoost);
                break;

            case ChestRewards.IncreaseDamage:
                player.IncreaseDamage(damageBoost);
                break;

            case ChestRewards.IncreaseMovementSpeed:
                player.IncreaseMoveSpeed(speedBoost);
                break;

            default:
                break;
        }

        // open chest and set logic accordingly
        open = true;
        srOpenCLosed.sprite = openChest;

        //// set collider is trigger toggle to false
        //Collider2D collider = GetComponent<Collider2D>();
        //collider.isTrigger = false;

    }

}
