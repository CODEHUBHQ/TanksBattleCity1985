using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance { get; private set; }

    public List<Button> MainMenuOrderedButtons { get => mainMenuOrderedButtons; }

    [SerializeField] private TMP_Text playerHighScoreText;
    [SerializeField] private ComingSoonUI comingSoonUI;
    [SerializeField] private Transform levelsPanel;

    [Tooltip("The buttons of main menu, should be ordered from top to bottom")]
    [SerializeField] private List<Button> mainMenuOrderedButtons;

    private List<string> mainMenuOrderedButtonsMethodNames = new List<string>();

    private void Awake()
    {
        Instance = this;

        mainMenuOrderedButtonsMethodNames.Add(nameof(PlayerOneButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(PlayerTwoButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(MultiplayerButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(SettingsButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(MapEditorButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(ShopButtonOnClick));

        for (int i = 0; i < mainMenuOrderedButtons.Count; i++)
        {
            var index = i; // we use copy to solve closure issue

            mainMenuOrderedButtons[i].onClick.AddListener(() =>
            {
                SendMessage(mainMenuOrderedButtonsMethodNames[index]);
            });
        }
    }

    private void Start()
    {
        var playerHighScore = PlayerPrefs.GetString(StaticStrings.PLAYER_HIGH_SCORE_PREF_KEY, $"00");

        playerHighScoreText.text = $"{playerHighScore}";
    }

    private void PlayerOneButtonOnClick()
    {
        Debug.Log("PlayerOneButtonOnClick");

        PhotonNetwork.OfflineMode = true;
        NetworkManager.Instance.GameMode = GameMode.SinglePlayer;

        PlayerPrefs.SetString(StaticStrings.CUSTOM_MAP, "");
        PlayerPrefs.SetString(StaticStrings.IS_CUSTOM_MAP, "false");
        //PlayerPrefs.SetString(StaticStrings.CURRENT_LEVEL, "1");
        PlayerPrefs.Save();

        //LoadingManager.LoadScene(LoadingManager.Scene.GameScene);

        levelsPanel.gameObject.SetActive(true);
    }

    private void PlayerTwoButtonOnClick()
    {
        Debug.Log("PlayerTwoButtonOnClick");

        PhotonNetwork.OfflineMode = false;
        NetworkManager.Instance.GameMode = GameMode.Multiplayer;

        PlayerPrefs.SetString(StaticStrings.CUSTOM_MAP, "");
        PlayerPrefs.SetString(StaticStrings.IS_CUSTOM_MAP, "false");
        PlayerPrefs.SetString(StaticStrings.CURRENT_LEVEL, "1");
        PlayerPrefs.Save();

        LoadingManager.LoadScene(LoadingManager.Scene.LobbyScene);
    }

    private void MultiplayerButtonOnClick()
    {
        Debug.Log("MultiplayerButtonOnClick");
        comingSoonUI.Show(ComingSoonOpts.Multiplayer);

        //PhotonNetwork.OfflineMode = false;
        //NetworkManager.Instance.GameMode = GameMode.Multiplayer;

        //LoadingManager.LoadScene(LoadingManager.Scene.LobbyScene);
    }

    private void SettingsButtonOnClick()
    {
        Debug.Log("SettingsButtonOnClick");

        LoadingManager.LoadScene(LoadingManager.Scene.SettingsScene);
    }

    private void MapEditorButtonOnClick()
    {
        Debug.Log("MapEditorButtonOnClick");

        InterstitialAds.Instance.LoadAd(() =>
        {
            InterstitialAds.Instance.ShowAd();
        });

        LoadingManager.LoadScene(LoadingManager.Scene.MapEditorScene);
    }

    private void ShopButtonOnClick()
    {
        Debug.Log("ShopButtonOnClick");
        comingSoonUI.Show(ComingSoonOpts.Shop);
    }
}
