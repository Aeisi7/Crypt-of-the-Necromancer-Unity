using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [Header("TextMeshPro")]
    [SerializeField] public TextMeshProUGUI hpText;
    [SerializeField] public TextMeshProUGUI mpText;
    [SerializeField] public TextMeshProUGUI chestKeyText;
    [SerializeField] public TextMeshProUGUI levelKeyText;

    Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("[UI] Could not find Player object or Player component!");
            enabled = false;
            return;
        }

        UpdateUI(); // show initial values immediately
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        hpText.text = $"HP: {player.GetCurHealth()}/{player.GetMaxHealth()}";
        mpText.text = $"MP: {player.GetCurMana()}/{player.GetMaxMana()}";
        chestKeyText.text = $"Chest Keys: {player.GetChestKeyCount()}";
        levelKeyText.text = $"Level Key: {(player.GetLevelKey() ? "1" : "0")}";
    }
}
