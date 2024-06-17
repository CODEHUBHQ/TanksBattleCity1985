using Photon.Pun;
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

    private PhotonView photonView;

    private void Awake()
    {
        Instance = this;

        photonView = GetComponent<PhotonView>();

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

    public void ResumeGame()
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
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            photonView.RPC(nameof(ResumeGamePunRPC), RpcTarget.All);
        }
        else
        {
            ResumeGame();
        }
    }

    public void MainMenuButtonOnClick()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            photonView.RPC(nameof(GoToMainMenuPunRPC), RpcTarget.All);
        }
        else
        {
            GoToMainMenu();
        }
    }

    public void ToggleGameMenu()
    {
        gameMenuPanel.SetActive(!gameMenuPanel.activeSelf);
        gameMenuIconOnClick.gameObject.SetActive(gameMenuPanel.activeSelf ? false : true);
    }

    [PunRPC]
    public void ResumeGamePunRPC()
    {
        ResumeGame();
    }

    [PunRPC]
    public void GoToMainMenuPunRPC()
    {
        GoToMainMenu();
    }
}
