using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] int healthRecovered = 2;
    public void OnTriggerEnter2D(Collider2D other)
    {
        // only let player interact with key
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();

        player.GainHealth(healthRecovered);

        Destroy(gameObject);
    }
}
