using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsPanel : MonoBehaviour
{
    [SerializeField] private Button levelPowerUpButton;
    [SerializeField] private Button destroyPowerUpButton;
    [SerializeField] private Button timerPowerUpButton;
    [SerializeField] private Button shieldPowerUpButton;
    [SerializeField] private Button healthPowerUpButton;
    [SerializeField] private Button shovelPowerUpButton;

    private void Awake()
    {
        levelPowerUpButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleGameIsPaused();

            RewardedAds.Instance.LoadAd(() =>
            {
                RewardedAds.Instance.ShowAd(() =>
                {
                    GameManager.Instance.ToggleGameIsPaused();

                    BattleCityPowerUp.Instance.ShowPowerUp(1);
                });
            });
        });

        destroyPowerUpButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleGameIsPaused();

            RewardedAds.Instance.LoadAd(() =>
            {
                RewardedAds.Instance.ShowAd(() =>
                {
                    GameManager.Instance.ToggleGameIsPaused();

                    BattleCityPowerUp.Instance.ShowPowerUp(2);
                });
            });
        });

        timerPowerUpButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleGameIsPaused();

            RewardedAds.Instance.LoadAd(() =>
            {
                RewardedAds.Instance.ShowAd(() =>
                {
                    GameManager.Instance.ToggleGameIsPaused();

                    BattleCityPowerUp.Instance.ShowPowerUp(3);
                });
            });
        });

        shieldPowerUpButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleGameIsPaused();

            RewardedAds.Instance.LoadAd(() =>
            {
                RewardedAds.Instance.ShowAd(() =>
                {
                    GameManager.Instance.ToggleGameIsPaused();

                    BattleCityPowerUp.Instance.ShowPowerUp(4);
                });
            });
        });

        healthPowerUpButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleGameIsPaused();

            RewardedAds.Instance.LoadAd(() =>
            {
                RewardedAds.Instance.ShowAd(() =>
                {
                    GameManager.Instance.ToggleGameIsPaused();

                    BattleCityPowerUp.Instance.ShowPowerUp(5);
                });
            });
        });

        shovelPowerUpButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ToggleGameIsPaused();

            RewardedAds.Instance.LoadAd(() =>
            {
                RewardedAds.Instance.ShowAd(() =>
                {
                    GameManager.Instance.ToggleGameIsPaused();

                    BattleCityPowerUp.Instance.ShowPowerUp(6);
                });
            });
        });
    }
}
