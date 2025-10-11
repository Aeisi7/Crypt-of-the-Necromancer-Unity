using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{

    // player interaction 
    private void OnTriggerEnter2D(Collider2D other)
    {
        // only should interact with player
        if (!other.CompareTag("Player")) return;

        // check to see if player has a level key
        Player player = other.GetComponent<Player>();

        if (player.GetLevelKey())
        {
            // Save current stats
            if (GameManager.Instance) GameManager.Instance.SaveFrom(player);

            // TODO: Play success sound
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);   // loads next scene

        }
        else
        {
            // TODO: play fail sound
            return;
        }
    }
}
