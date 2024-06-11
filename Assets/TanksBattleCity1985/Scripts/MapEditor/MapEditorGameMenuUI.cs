using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MapEditorGameMenuUI : MonoBehaviour
{
    public static MapEditorGameMenuUI Instance { get; private set; }

    public List<Button> MainMenuOrderedButtons { get => mainMenuOrderedButtons; }

    [SerializeField] private GameObject gameMenuContainer;
    [SerializeField] private GameObject loadMapContainer;
    [SerializeField] private GameObject playMapContainer;

    [Tooltip("The buttons of main menu, should be ordered from top to bottom")]
    [SerializeField] private List<Button> mainMenuOrderedButtons;

    [Header("CreateNewMap")]
    [SerializeField] private Transform newMapPanel;
    [SerializeField] private Transform notEnoughBalancePanel;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text newMapCostText;
    [SerializeField] private Button newMapYesButton;

    private List<string> mainMenuOrderedButtonsMethodNames = new List<string>();

    private int newMapCost = 100;

    private bool runOnce;

    private void Awake()
    {
        Instance = this;

        mainMenuOrderedButtonsMethodNames.Add(nameof(ResumeButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(NewMapButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(SaveMapButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(LoadMapButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(PlayMapButtonOnClick));
        mainMenuOrderedButtonsMethodNames.Add(nameof(MainMenuButtonOnClick));

        for (int i = 0; i < mainMenuOrderedButtons.Count; i++)
        {
            var index = i; // we use copy to solve closure issue

            mainMenuOrderedButtons[i].onClick.AddListener(() =>
            {
                SendMessage(mainMenuOrderedButtonsMethodNames[index], SendMessageOptions.DontRequireReceiver);
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !MapEditorLoadMapHandler.Instance.IsOpen() && !MapEditorPlayMapHandler.Instance.IsOpen())
        {
            gameMenuContainer.SetActive(!gameMenuContainer.activeSelf);
            mainMenuOrderedButtons[0].Select();
            MapEditorHandler.Instance.SetIsPaused(true);
        }
    }

    private void ResumeButtonOnClick()
    {
        gameMenuContainer.SetActive(false);
        MapEditorHandler.Instance.SetIsPaused(false);
    }

    private void NewMapButtonOnClick()
    {
        var playerBalance = PlayerPrefs.GetString(StaticStrings.PLAYER_BALANCE, "0");

        if (int.Parse(playerBalance) >= newMapCost)
        {
            newMapPanel.gameObject.SetActive(true);

            balanceText.text = $"Your Balance: {playerBalance} <sprite index=0>";
            newMapCostText.text = $"New Map Cost: {newMapCost}";

            newMapYesButton.onClick.AddListener(() =>
            {
                newMapPanel.gameObject.SetActive(false);
                gameMenuContainer.SetActive(false);

                var newPlayerBalance = int.Parse(playerBalance) - newMapCost;

                PlayerPrefs.SetString(StaticStrings.PLAYER_BALANCE, $"{newPlayerBalance}");
                PlayerPrefs.Save();

                CoinsManager.Instance.UpdateCoinsText();

                MapEditorHandler.Instance.SetIsPaused(false);
                MapEditorHandler.Instance.NewMap();

                mainMenuOrderedButtons[0].interactable = true;
                mainMenuOrderedButtons[0].transform.Find("ResumeText").GetComponent<TMP_Text>().color = new Color(255f, 255f, 255f, 1f);
            });
        }
        else
        {
            notEnoughBalancePanel.gameObject.SetActive(true);
        }
    }

    private void SaveMapButtonOnClick()
    {
        MapEditorHandler.Instance.SaveMapText();
    }

    private void LoadMapButtonOnClick()
    {
        gameMenuContainer.SetActive(false);
        loadMapContainer.SetActive(true);

        MapEditorHandler.Instance.SetIsPaused(true);
        MapEditorLoadMapHandler.Instance.LoadCustomMaps(() =>
        {
            mainMenuOrderedButtons[0].interactable = true;
            mainMenuOrderedButtons[0].transform.Find("ResumeText").GetComponent<TMP_Text>().color = new Color(255f, 255f, 255f, 1f);
        });
    }

    private void PlayMapButtonOnClick()
    {
        gameMenuContainer.SetActive(false);
        playMapContainer.SetActive(true);

        MapEditorHandler.Instance.SetIsPaused(true);
        MapEditorPlayMapHandler.Instance.LoadCustomMaps();
    }

    private void MainMenuButtonOnClick()
    {
        LoadingManager.LoadScene(LoadingManager.Scene.MenuScene);
    }

    public bool IsGameMenuOpen()
    {
        return gameMenuContainer.activeSelf;
    }

    public void Hide()
    {
        gameMenuContainer.SetActive(false);
        MapEditorHandler.Instance.SetIsPaused(false);
    }

    public void OpenMapEditorMenu()
    {
        gameMenuContainer.SetActive(!gameMenuContainer.activeSelf);
        mainMenuOrderedButtons[0].Select();
        MapEditorHandler.Instance.SetIsPaused(true);
    }
}
