using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance { get; private set; }

    [SerializeField] private Button storeButton;
    [SerializeField] private Button watchAdsButton;
    [SerializeField] private TMP_Text coinsText;

    [SerializeField] private Transform storePanel;

    private int adCoinsReward = 20;

    private void Awake()
    {
        Instance = this;

        if (storePanel != null)
        {
            storePanel.gameObject.SetActive(false);
        }

        if (storeButton != null)
        {
            storeButton.onClick.AddListener(OnStoreButtonClicked);
        }

        if (watchAdsButton != null)
        {
            watchAdsButton.onClick.AddListener(OnWatchAdsButtonClicked);
        }

        UpdateCoinsText();
    }

    private void OnStoreButtonClicked()
    {
        storePanel.gameObject.SetActive(true);
    }

    private void OnWatchAdsButtonClicked()
    {
        storePanel.gameObject.SetActive(false);

        RewardedAds.Instance.LoadAd(() =>
        {
            RewardedAds.Instance.ShowAd(() =>
            {
                var playerBalance = PlayerPrefs.GetString(StaticStrings.PLAYER_BALANCE, "0");
                var newPlayerBalance = int.Parse(playerBalance) + adCoinsReward;

                PlayerPrefs.SetString(StaticStrings.PLAYER_BALANCE, $"{newPlayerBalance}");
                PlayerPrefs.Save();

                UpdateCoinsText();
            });
        });
    }

    public void UpdateCoinsText()
    {
        var playerBalance = PlayerPrefs.GetString(StaticStrings.PLAYER_BALANCE, "0");

        coinsText.text = $"{playerBalance}";
    }

    public int GetAdCoinsReward()
    {
        return adCoinsReward;
    }
}
