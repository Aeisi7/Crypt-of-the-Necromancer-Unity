using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [Header("Player Stats UI")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private TextMeshProUGUI chestKeyText;
    [SerializeField] private TextMeshProUGUI levelKeyText;
    [Header("Sign UI")]
    [SerializeField] private GameObject signImage;
    [SerializeField] private TextMeshProUGUI signText;

    Player player;

    private void Start()
    {
        // hide signImage on start
        signImage.SetActive(false);

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

    public void UpdateUI()
    {
        hpText.text = $"HP: {player.GetCurHealth()}/{player.GetMaxHealth()}";
        mpText.text = $"MP: {player.GetCurMana()}/{player.GetMaxMana()}";
        chestKeyText.text = $"Chest Keys: {player.GetChestKeyCount()}";
        levelKeyText.text = $"Level Key: {(player.GetLevelKey() ? "1" : "0")}";
    }

    /*------------ Functions For Signs  ------------*/
    public void ShowSign(string message)
    {
        signImage.SetActive(true);
        signText.text = message;
    }

    public void HideSign()
    {
        signImage.SetActive(false);
    }
    /*----------------------------------------------*/
}
