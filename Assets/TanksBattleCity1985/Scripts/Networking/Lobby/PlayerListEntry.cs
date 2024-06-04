using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;

public class PlayerListEntry : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image playerImage;
    [SerializeField] private Image playerReadyImage;

    [SerializeField] private Button playerReadyButton;

    [SerializeField] private TMP_Text playerReadyButtonText;
    [SerializeField] private TMP_Text playerNameText;

    private int ownerId;
    private int defaultPlayerLives = 3;

    private bool isPlayerReady;

    private void Awake()
    {
        PlayerNumbering.OnPlayerNumberingChanged += PlayerNumbering_OnPlayerNumberingChanged;
    }

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            playerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            var initialProps = new ExitGames.Client.Photon.Hashtable()
            {
                { StaticStrings.IS_PLAYER_READY, isPlayerReady },
                { StaticStrings.PLAYER_LIVES, defaultPlayerLives },
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);

            playerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;

                SetPlayerReady(isPlayerReady);

                var props = new ExitGames.Client.Photon.Hashtable()
                {
                    { StaticStrings.IS_PLAYER_READY, isPlayerReady }
                };

                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient)
                {
                    LobbyHandler.Instance.LocalPlayerPropertiesUpdated();
                }
            });
        }
    }

    private void OnDestroy()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= PlayerNumbering_OnPlayerNumberingChanged;
    }

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        playerNameText.text = playerName;
    }

    private void PlayerNumbering_OnPlayerNumberingChanged()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == ownerId)
            {
                var plaeyrSprite = LobbyHandler.Instance.GetPlayerSprite(player.GetPlayerNumber());

                if (plaeyrSprite != null)
                {
                    playerImage.sprite = plaeyrSprite;
                }
            }
        }
    }

    public void SetPlayerReady(bool isPlayerReady)
    {
        playerReadyButtonText.text = isPlayerReady ? "Ready!" : "Ready?";
        playerReadyImage.enabled = isPlayerReady;
    }
}
