using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [TextArea] public string text;
    [SerializeField] private UI signUI;

    private Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // only let player interact with/read sign
        if (!other.CompareTag("Player")) return;

        player = other.GetComponent<Player>();
        // Set player to be invincible while reading (avoids adding a pause system)
        player.SetInvincible(true);

        signUI.ShowSign(text);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // only let player interact with/read sign
        if (!other.CompareTag("Player")) return;

        signUI.HideSign();

        player = other.GetComponent<Player>();
        // make sure player is no longer invincible
        player.SetInvincible(false);
    }
}
