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

    private void Awake()
    {
        Instance = this;

        storePanel.gameObject.SetActive(false);

        storeButton.onClick.AddListener(OnStoreButtonClicked);
        watchAdsButton.onClick.AddListener(OnWatchAdsButtonClicked);

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
            RewardedAds.Instance.ShowAd();
        });
    }

    public void UpdateCoinsText()
    {
        var playerBalance = PlayerPrefs.GetString(StaticStrings.PLAYER_BALANCE, "0");

        coinsText.text = $"{playerBalance}";
    }
}
