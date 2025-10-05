using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestKey : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        // only let player interact with key
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();

        player.GrabChestKey();

        Destroy(gameObject);
    }
}
