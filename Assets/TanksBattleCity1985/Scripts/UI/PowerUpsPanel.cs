using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PowerUpsPanel : MonoBehaviour
{
    [SerializeField] private Button levelPowerUpButton;
    [SerializeField] private Button destroyPowerUpButton;
    [SerializeField] private Button timerPowerUpButton;
    [SerializeField] private Button shieldPowerUpButton;
    [SerializeField] private Button healthPowerUpButton;
    [SerializeField] private Button shovelPowerUpButton;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        levelPowerUpButton.onClick.AddListener(() =>
        {
            if (HasBalance())
            {
                ShowPowerUpCoins(1);
            }
            else
            {
                ShowPowerUpAd(1);
            }
        });

        destroyPowerUpButton.onClick.AddListener(() =>
        {
            if (HasBalance())
            {
                ShowPowerUpCoins(2);
            }
            else
            {
                ShowPowerUpAd(2);
            }
        });

        timerPowerUpButton.onClick.AddListener(() =>
        {
            if (HasBalance())
            {
                ShowPowerUpCoins(3);
            }
            else
            {
                ShowPowerUpAd(3);
            }
        });

        shieldPowerUpButton.onClick.AddListener(() =>
        {
            if (HasBalance())
            {
                ShowPowerUpCoins(4);
            }
            else
            {
                ShowPowerUpAd(4);
            }
        });

        healthPowerUpButton.onClick.AddListener(() =>
        {
            if (HasBalance())
            {
                ShowPowerUpCoins(5);
            }
            else
            {
                ShowPowerUpAd(5);
            }
        });

        shovelPowerUpButton.onClick.AddListener(() =>
        {
            if (HasBalance())
            {
                ShowPowerUpCoins(6);
            }
            else
            {
                ShowPowerUpAd(6);
            }
        });
    }

    private bool HasBalance()
    {
        var playerBalance = PlayerPrefs.GetString(StaticStrings.PLAYER_BALANCE, "0");

        return int.Parse(playerBalance) >= CoinsManager.Instance.GetAdCoinsReward();
    }

    private void ShowPowerUpCoins(int powerUp)
    {
        var playerBalance = PlayerPrefs.GetString(StaticStrings.PLAYER_BALANCE, "0");

        var newPlayerBalance = int.Parse(playerBalance) - CoinsManager.Instance.GetAdCoinsReward();

        PlayerPrefs.SetString(StaticStrings.PLAYER_BALANCE, $"{newPlayerBalance}");
        PlayerPrefs.Save();

        CoinsManager.Instance.UpdateCoinsText();

        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            photonView.RPC(nameof(ShowPowerUpPunRPC), RpcTarget.All, powerUp);
        }
        else
        {
            BattleCityPowerUp.Instance.ShowPowerUp(powerUp);
        }
    }

    private void ShowPowerUpAd(int powerUp)
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            return;
        }

        GameManager.Instance.ToggleGameIsPaused();

        RewardedAds.Instance.LoadAd(() =>
        {
            RewardedAds.Instance.ShowAd(() =>
            {
                GameManager.Instance.ToggleGameIsPaused();

                BattleCityPowerUp.Instance.ShowPowerUp(powerUp);
            });
        });
    }

    [PunRPC]
    public void ShowPowerUpPunRPC(int powerUp)
    {
        BattleCityPowerUp.Instance.ShowPowerUp(powerUp);
    }
}
