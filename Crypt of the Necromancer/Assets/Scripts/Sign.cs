using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [TextArea] public string text;
    [SerializeField] private UI signUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // only let player interact with/read sign
        if (!other.CompareTag("Player")) return;

        signUI.ShowSign(text);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // only let player interact with/read sign
        if (!other.CompareTag("Player")) return;

        signUI.HideSign();
    }
}
