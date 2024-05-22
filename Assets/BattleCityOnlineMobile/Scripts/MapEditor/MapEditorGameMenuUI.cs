using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorGameMenuUI : MonoBehaviour
{
    public static MapEditorGameMenuUI Instance { get; private set; }

    public List<Button> MainMenuOrderedButtons { get => mainMenuOrderedButtons; }

    [SerializeField] private GameObject gameMenuContainer;
    [SerializeField] private GameObject loadMapContainer;
    [SerializeField] private GameObject playMapContainer;

    [Tooltip("The buttons of main menu, should be ordered from top to bottom")]
    [SerializeField] private List<Button> mainMenuOrderedButtons;

    private List<string> mainMenuOrderedButtonsMethodNames = new List<string>();

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
        gameMenuContainer.SetActive(false);
        MapEditorHandler.Instance.SetIsPaused(false);
        MapEditorHandler.Instance.NewMap();
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
        MapEditorLoadMapHandler.Instance.LoadCustomMaps();
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
