using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    public static GameMenuUI Instance { get; private set; }

    public List<Button> GameMenuOrderedButtons { get => gameMenuOrderedButtons; }

    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] private List<Button> gameMenuOrderedButtons;
    [SerializeField] private GameMenuIconOnClick gameMenuIconOnClick;

    private List<string> gameMenuOrderedButtonsMethodNames = new List<string>();

    private void Awake()
    {
        Instance = this;

        gameMenuOrderedButtonsMethodNames.Add(nameof(ResumeButtonOnClick));
        gameMenuOrderedButtonsMethodNames.Add(nameof(MainMenuButtonOnClick));

        for (int i = 0; i < gameMenuOrderedButtons.Count; i++)
        {
            var index = i; // we use copy to solve closure issue

            gameMenuOrderedButtons[i].onClick.AddListener(() =>
            {
                SendMessage(gameMenuOrderedButtonsMethodNames[index]);
            });
        }
    }

    private void ResumeGame()
    {
        StartCoroutine(nameof(ToggleGameIsPausedDelayed));

        ToggleGameMenu();
    }

    private IEnumerator ToggleGameIsPausedDelayed()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        GameManager.Instance.ToggleGameIsPaused();
    }

    private void GoToMainMenu()
    {
        GameManager.Instance.ToggleGameIsPaused();

        LoadingManager.LoadScene(LoadingManager.Scene.MenuScene);
    }

    public void ResumeButtonOnClick()
    {
        ResumeGame();
    }

    public void MainMenuButtonOnClick()
    {
        GoToMainMenu();
    }

    public void ToggleGameMenu()
    {
        gameMenuPanel.SetActive(!gameMenuPanel.activeSelf);
        gameMenuIconOnClick.gameObject.SetActive(gameMenuPanel.activeSelf ? false : true);
    }
}
