using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using TMPro;

public class ControllerSelectionUI : MonoBehaviour
{
    public static ControllerSelectionUI Instance { get; private set; }

    [SerializeField] private GameObject selectPlayerOneText;
    [SerializeField] private GameObject playerOneText;
    [SerializeField] private GameObject selectPlayerTwoText;
    [SerializeField] private GameObject playerTwoText;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject startGameText;
    [SerializeField] private GameMenuIconOnClick gameMenuIconOnClick;

    private List<InputDevice> playerInputDeviceList;

    private InputAction selectionInputAction;

    private void Awake()
    {
        Instance = this;

        playerInputDeviceList = new List<InputDevice>();

        selectPlayerOneText.SetActive(false);
        playerOneText.SetActive(false);
        selectPlayerTwoText.SetActive(false);
        playerTwoText.SetActive(false);
        loadingScreen.SetActive(false);
        gameMenuIconOnClick.Hide();
    }

    private void Start()
    {
        StartGame();

        //if (NetworkManager.Instance != null)
        //{
        //    switch (NetworkManager.Instance.GameMode)
        //    {
        //        case GameMode.SinglePlayer:
        //            selectPlayerOneText.SetActive(true);
        //            break;
        //        case GameMode.Multiplayer:
        //            selectPlayerOneText.SetActive(true);
        //            playerOneText.transform.Find("LetterOneIcon").gameObject.SetActive(false);
        //            playerTwoText.transform.Find("LetterTwoIcon").gameObject.SetActive(false);
        //            break;
        //        case GameMode.LocalMultiplayer:
        //            selectPlayerOneText.SetActive(true);
        //            selectPlayerTwoText.SetActive(true);
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //else
        //{
        //    selectPlayerOneText.SetActive(true);
        //}

        //selectionInputAction = new InputAction(binding: "/*/<button>");

        //selectionInputAction.performed += (action) =>
        //{
        //    var paths = action.control.path.Split("/");

        //    if (paths[^1] == "buttonSouth" || paths[^1] == "Gamepad" || paths[^1] == "space" || paths[^1] == "A")
        //    {
        //        if (NetworkManager.Instance != null)
        //        {
        //            if ((NetworkManager.Instance.GameMode == GameMode.SinglePlayer && playerInputDeviceList.Count == 0) ||
        //                (NetworkManager.Instance.GameMode == GameMode.Multiplayer && playerInputDeviceList.Count == 0))
        //            {
        //                AddPlayer(action.control.device);
        //            }

        //            if ((NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer && playerInputDeviceList.Count == 0) ||
        //                (NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer && playerInputDeviceList.Count == 1))
        //            {
        //                AddPlayer(action.control.device);
        //            }
        //        }
        //        else if (playerInputDeviceList.Count == 0)
        //        {
        //            AddPlayer(action.control.device);
        //        }
        //    }
        //};

        //selectionInputAction.Enable();
    }

    private void OnDisable()
    {
        //selectionInputAction.Disable();
    }

    private void AddPlayer(InputDevice device)
    {
        Debug.Log($"device: {device}");

        // Avoid running if the device is already paired to a player
        var deviceAlreadyPaired = false;

        foreach (var player in PlayerInput.all)
        {
            foreach (var playerDevice in player.devices)
            {
                if (device == playerDevice)
                {
                    deviceAlreadyPaired = true;
                    return;
                }
            }
        }

        if (deviceAlreadyPaired)
        {
            return;
        }

        // remove mouse
        if (device.displayName.Contains("Mouse"))
        {
            return;
        }

        // add playerIndex check
        if (playerInputDeviceList.Count > PlayerInputManager.instance.maxPlayerCount)
        {
            return;
        }

        playerInputDeviceList.Add(device);

        var playerSelectTurnIndex = playerInputDeviceList.IndexOf(device);

        var playerTextPrefix = $"-PLAYER";

        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            playerTextPrefix = $"PLAYER";
        }

        if (playerSelectTurnIndex == 0)
        {
            selectPlayerOneText.SetActive(false);
            playerOneText.SetActive(true);

#if UNITY_WEBGL
            var deviceName = device.displayName.ToLower().Contains("xbox") || device.displayName.ToLower().Contains("gamepad") ? "xbox" : "keyboard";

            playerOneText.GetComponent<TMP_Text>().text = $"{playerTextPrefix} / {deviceName}";
#else
            playerOneText.GetComponent<TMP_Text>().text = $"{playerTextPrefix} / {device.displayName}";
#endif
        }

        if (playerSelectTurnIndex == 1)
        {
            selectPlayerTwoText.SetActive(false);
            playerTwoText.SetActive(true);
#if UNITY_WEBGL
            var deviceName = device.displayName.ToLower().Contains("xbox") || device.displayName.ToLower().Contains("gamepad") ? "xbox" : "keyboard";

            playerTwoText.GetComponent<TMP_Text>().text = $"{playerTextPrefix} / {deviceName}";
#else
            playerTwoText.GetComponent<TMP_Text>().text = $"{playerTextPrefix} / {device.displayName}";
#endif
        }

        //GameManager.Instance.AddPlayerController(playerSelectTurnIndex, device);

        if (NetworkManager.Instance != null)
        {
            switch (NetworkManager.Instance.GameMode)
            {
                case GameMode.SinglePlayer:
                    startGameText.SetActive(true);
                    StartGame();
                    break;
                case GameMode.LocalMultiplayer:
                    if (playerSelectTurnIndex == 1)
                    {
                        startGameText.SetActive(true);
                    }
                    break;
                case GameMode.Multiplayer:
                    Debug.Log($"Multiplayer update player props");
                    Time.timeScale = 1f;
                    GameManager.Instance.UpdateLocalPlayerLoadedLevel();
                    break;
                default:
                    break;
            }
        }
        else
        {
            startGameText.SetActive(true);
            StartGame();
        }
    }

    public void StartGame()
    {
        //if (NetworkManager.Instance != null)
        //{
        //    if (NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        //    {
        //        return;
        //    }

        //    if (NetworkManager.Instance.GameMode == GameMode.SinglePlayer)
        //    {
        //        if (playerInputDeviceList.Count != 1)
        //        {
        //            return;
        //        }
        //    }

        //    if (NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer)
        //    {
        //        if (playerInputDeviceList.Count != 2)
        //        {
        //            return;
        //        }
        //    }
        //}
        //else
        //{
        //    if (playerInputDeviceList.Count != 1)
        //    {
        //        return;
        //    }
        //}

        startGameText.SetActive(false);
        gameObject.SetActive(false);
        loadingScreen.SetActive(true);
        gameMenuIconOnClick.Show();
        GameManager.Instance.StartGame();
    }

    public void HideControllerSelectionUI()
    {
        startGameText.SetActive(false);
        gameObject.SetActive(false);
        loadingScreen.SetActive(true);
    }

    public InputDevice GetPlayerInputDeviceByIndex(int playerIndex)
    {
        if (playerInputDeviceList.Count == 0 || playerIndex >= playerInputDeviceList.Count)
        {
            return null;
        }

        return playerInputDeviceList[playerIndex];
    }
}
