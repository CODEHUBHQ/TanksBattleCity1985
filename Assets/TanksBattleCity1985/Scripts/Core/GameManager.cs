using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager Instance { get; private set; }

    public GameSettingsSO GameSettingsSO { get => gameSettingsSO; }
    public StagesSO StagesSO { get => stagesSO; }

    public BattleCityPlayer PlayerOne { get => playerOne; }
    public BattleCityPlayer PlayerTwo { get => playerTwo; }

    [SerializeField] private GameSettingsSO gameSettingsSO;

    [SerializeField] private BattleCityMapLoad mapLoad;

    [SerializeField] private GameObject playerOnePrefab;
    [SerializeField] private GameObject playerTwoPrefab;

    [SerializeField] private Transform playerOneSpawnPoint;
    [SerializeField] private Transform playerTwoSpawnPoint;

    [SerializeField] private StagesSO stagesSO;

    [SerializeField] private TMP_Text waitingForAllPlayersText;

    [SerializeField] private GameObject playerInputManagerPrefab;

    private BattleCityPlayer playerOne;
    private BattleCityPlayer playerTwo;

    private bool isGamePaused;
    private bool isGameOver;

    private int[] playerOneStats = new int[5];
    private int[] playerTwoStats = new int[5];

    private void Awake()
    {
        Instance = this;

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        GetComponent<CountdownTimer>().enabled = false;
        //if (NetworkManager.Instance == null || NetworkManager.Instance.GameMode != GameMode.Multiplayer)
        //{
        //    GetComponent<CountdownTimer>().enabled = false;
        //}
    }

    private void Start()
    {
        ToggleGameIsPaused();
    }

    //public override void OnEnable()
    //{
    //    base.OnEnable();

    //    CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();

    //    CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
    //}

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(StaticStrings.PLAYER_INSTANTIATED))
        {
            var battleCityPlayers = FindObjectsByType<BattleCityPlayer>(FindObjectsSortMode.None);

            foreach (var battleCityPlayer in battleCityPlayers)
            {
                if (playerOne.gameObject.name == battleCityPlayer.gameObject.name) continue;

                battleCityPlayer.gameObject.name = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? "PlayerTwo" : "PlayerOne";
                battleCityPlayer.transform.SetParent(PhotonNetwork.LocalPlayer.ActorNumber == 1 ? playerTwoSpawnPoint : playerOneSpawnPoint);
            }

            return;
        }

        if (changedProps.ContainsKey(StaticStrings.PLAYER_LIVES))
        {
            BattleCityEagle.Instance.SetPlayerLives(targetPlayer.ActorNumber - 1, (int)changedProps[StaticStrings.PLAYER_LIVES]);

            return;
        }

        if (changedProps.ContainsKey(StaticStrings.PLAYER_LOADED_LEVEL))
        {
            BattleCityEagle.Instance.SetGameLevel((int)changedProps[StaticStrings.PLAYER_LOADED_LEVEL]);

            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
        //var startTimeIsSet = CountdownTimer.TryGetStartTime(out int startTimestamp);

        //if (changedProps.ContainsKey(StaticStrings.PLAYER_LOADED_LEVEL))
        //{
        //    if (CheckAllPlayerLoadedLevel())
        //    {
        //        if (!startTimeIsSet)
        //        {
        //            CountdownTimer.SetStartTime();
        //        }
        //    }
        //    else
        //    {
        //        if (waitingForAllPlayersText != null)
        //        {
        //            // not all players loaded yet. wait:
        //            Debug.Log("setting text waiting for players!");
        //            waitingForAllPlayersText.text = "Waiting for other players...";
        //        }
        //    }
        //}
    }

    //private bool CheckAllPlayerLoadedLevel()
    //{
    //    foreach (var player in PhotonNetwork.PlayerList)
    //    {
    //        if (player.CustomProperties.TryGetValue(StaticStrings.PLAYER_LOADED_LEVEL, out object playerLoadedLevel))
    //        {
    //            if ((bool)playerLoadedLevel)
    //            {
    //                continue;
    //            }
    //        }

    //        return false;
    //    }

    //    return true;
    //}

    //private void OnCountdownTimerIsExpired()
    //{
    //    ControllerSelectionUI.Instance.HideControllerSelectionUI();

    //    StartGame();
    //}

    public void UpdateLocalPlayerLoadedLevel()
    {
        var props = new ExitGames.Client.Photon.Hashtable
        {
            { StaticStrings.PLAYER_LOADED_LEVEL, true }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void StartGame()
    {
        ToggleGameIsPaused();

        var isCustomMap = bool.Parse(PlayerPrefs.GetString(StaticStrings.IS_CUSTOM_MAP, "false"));

        var currentLevel = "-1";

        if (!isCustomMap)
        {
            //PlayerPrefs.SetString(StaticStrings.CURRENT_LEVEL, "1");
            //PlayerPrefs.Save();

            currentLevel = PlayerPrefs.GetString(StaticStrings.CURRENT_LEVEL, "1");
        }

        Instantiate(playerInputManagerPrefab);

        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(InstantiatePlayers), RpcTarget.All);
            }
        }
        else
        {
            var playerOneGameObject = Instantiate(playerOnePrefab, playerOneSpawnPoint);

            if (playerOneGameObject.TryGetComponent(out BattleCityPlayer battleCityPlayerOne))
            {
                battleCityPlayerOne.SetLocalPlayerActorNumber(0);

                battleCityPlayerOne.gameObject.name = $"PlayerOne";

                playerOne = battleCityPlayerOne;
            }

            if (NetworkManager.Instance != null)
            {
                if (NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer)
                {
                    var playerTwoGameObject = Instantiate(playerTwoPrefab, playerTwoSpawnPoint);

                    if (playerTwoGameObject.TryGetComponent(out BattleCityPlayer battleCityPlayerTwo))
                    {
                        battleCityPlayerTwo.SetLocalPlayerActorNumber(1);

                        battleCityPlayerTwo.gameObject.name = $"PlayerTwo";

                        playerTwo = battleCityPlayerTwo;
                    }
                }
            }
        }

        if (isCustomMap)
        {
            var customMap = PlayerPrefs.GetString(StaticStrings.CUSTOM_MAP, "");

            if (string.IsNullOrEmpty(customMap))
            {
                LoadingManager.LoadScene(LoadingManager.Scene.MenuScene);
                return;
            }

            mapLoad.LoadCustomMap(customMap);
        }
        else
        {
            if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    //mapLoad.LoadMap(int.Parse(currentLevel));
                    photonView.RPC(nameof(LoadMapRPC), RpcTarget.MasterClient, int.Parse(currentLevel));
                }
            }
            else
            {
                mapLoad.LoadMap(int.Parse(currentLevel));
            }
        }

        mapLoad.LoadingScreen.gameObject.SetActive(false);
    }

    public void ToggleGameIsPaused()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;
        }

        if (!isGamePaused)
        {
            Time.timeScale = 1f;
        }
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public void SetIsGameOver(bool isGameOver)
    {
        this.isGameOver = isGameOver;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public int[] GetPlayerOneStats()
    {
        return playerOneStats;
    }

    public int[] GetPlayerTwoStats()
    {
        return playerTwoStats;
    }

    public void ResetPlayersStats()
    {
        Array.Clear(playerOneStats, 0, playerOneStats.Length);
        Array.Clear(playerTwoStats, 0, playerTwoStats.Length);
    }

    public void UpdatePlayerStats(int playerIndex, int index, int value)
    {
        if (playerIndex == 1)
        {
            playerOneStats[index] = value;
        }
        else if (playerIndex == 2)
        {
            playerTwoStats[index] = value;
        }
    }

    [PunRPC]
    public void InstantiatePlayers()
    {
        var prefabName = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? playerOnePrefab.name : playerTwoPrefab.name;
        var prefabGameObjectName = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? "PlayerOne" : "PlayerTwo";
        var playerSpawnPoint = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? playerOneSpawnPoint : playerTwoSpawnPoint;
        var playerOneGameObject = PhotonNetwork.Instantiate(prefabName, playerSpawnPoint.position, Quaternion.identity, 0);

        if (playerOneGameObject.TryGetComponent(out BattleCityPlayer battleCityPlayerOne))
        {
            battleCityPlayerOne.SetLocalPlayerActorNumber(PhotonNetwork.LocalPlayer.ActorNumber == 1 ? 0 : 1);

            battleCityPlayerOne.gameObject.name = prefabGameObjectName;
            battleCityPlayerOne.transform.SetParent(playerSpawnPoint);

            playerOne = battleCityPlayerOne;

            playerOne.gameObject.GetComponent<BattleCityPlayerMovement>().ResetPosition();
            playerOne.gameObject.GetComponent<Animator>().SetBool(StaticStrings.HIT, false);
            playerOne.gameObject.GetComponent<BattleCityShooting>().SetShooting(false);
            playerOne.ResetLevelScore();
            playerOne.SetShield(6);
        }

        var props = new ExitGames.Client.Photon.Hashtable
        {
            { StaticStrings.PLAYER_INSTANTIATED, true }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    [PunRPC]
    public void LoadMapRPC(int currentLevel)
    {
        mapLoad.LoadMap(currentLevel);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerOneStats);
            stream.SendNext(playerTwoStats);
            stream.SendNext(isGameOver);
        }
        else
        {
            playerOneStats = (int[])stream.ReceiveNext();
            playerTwoStats = (int[])stream.ReceiveNext();
            isGameOver = (bool)stream.ReceiveNext();
        }
    }
}
