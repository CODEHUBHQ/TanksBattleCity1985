using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Reflection;

public class LevelsPanel : MonoBehaviour
{
    [SerializeField] private Transform levelsContent;
    [SerializeField] private Transform levelPrefab;
    [SerializeField] private Sprite[] levelsIcons;

    [Header("UnLockLevelPanel")]
    [SerializeField] private Transform unLockLevelPanel;

    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text openLevelText;
    [SerializeField] private TMP_Text totalCoinsText;

    [SerializeField] private Button unLockLevelYesButton;
    [SerializeField] private Button unLockLevelNoButton;

    private int unLockLevelCost = 100;


    private void OnEnable()
    {
        unLockLevelPanel.gameObject.SetActive(false);

        LoadLevels();
    }

    private void OnDisable()
    {
        CoinsManager.Instance.UpdateCoinsText();
    }

    public void LoadLevels()
    {
        var currentLevel = PlayerPrefs.GetString(StaticStrings.CURRENT_LEVEL, "1");

        for (int i = 0; i < 35; i++)
        {
            var levelInstance = Instantiate(levelPrefab, levelsContent);

            levelInstance.Find("Icon").GetComponent<Image>().sprite = levelsIcons[i];
            levelInstance.Find("LevelNumberBG").Find("LevelText").GetComponent<TMP_Text>().text = $"{i + 1}";

            if (int.Parse(currentLevel) + 1 == i + 1)
            {
                levelInstance.Find("LockedIcon").gameObject.SetActive(true);
                levelInstance.Find("LockedIcon").GetComponent<Image>().color = Color.yellow;
                levelInstance.GetComponent<Button>().interactable = true;

                var index = i; // we use copy to solve closure issue

                levelInstance.GetComponent<Button>().onClick.AddListener(() => OnLevelUnLockClick(index + 1));

                continue;
            }

            if (int.Parse(currentLevel) >= i + 1)
            {
                levelInstance.Find("LockedIcon").gameObject.SetActive(false);
                levelInstance.GetComponent<Button>().interactable = true;

                var index = i; // we use copy to solve closure issue

                levelInstance.GetComponent<Button>().onClick.AddListener(() => OnLevelClick(index + 1));
            }
            else
            {
                levelInstance.Find("LockedIcon").gameObject.SetActive(true);
                levelInstance.Find("Icon").GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.2f);

                levelInstance.GetComponent<Button>().interactable = false;
            }
        }
    }

    private void OnLevelClick(int index)
    {
        PlayerPrefs.SetString(StaticStrings.CURRENT_LEVEL, $"{index}");
        PlayerPrefs.Save();

        LoadingManager.LoadScene(LoadingManager.Scene.GameScene);
    }

    private void OnLevelUnLockClick(int index)
    {
        unLockLevelPanel.gameObject.SetActive(true);

        var playerBalance = PlayerPrefs.GetString(StaticStrings.PLAYER_BALANCE, "0");

        balanceText.text = $"Your Balance: {playerBalance} <sprite index=0>";
        openLevelText.text = $"Open Level: {index}";
        totalCoinsText.text = $"Total Coins: {index + unLockLevelCost}";

        if (int.Parse(playerBalance) < index + unLockLevelCost)
        {
            unLockLevelYesButton.interactable = false;
        }
        else
        {
            unLockLevelYesButton.interactable = true;
        }

        unLockLevelYesButton.onClick.AddListener(() =>
        {
            var newPlayerBalance = int.Parse(playerBalance) - (index + unLockLevelCost);

            PlayerPrefs.SetString(StaticStrings.PLAYER_BALANCE, $"{newPlayerBalance}");
            PlayerPrefs.SetString(StaticStrings.CURRENT_LEVEL, $"{index}");
            PlayerPrefs.Save();

            CoinsManager.Instance.UpdateCoinsText();

            unLockLevelYesButton.interactable = false;

            LoadingManager.LoadScene(LoadingManager.Scene.GameScene);
        });

        unLockLevelNoButton.onClick.AddListener(() =>
        {
            unLockLevelPanel.gameObject.SetActive(false);
        });
    }
}
