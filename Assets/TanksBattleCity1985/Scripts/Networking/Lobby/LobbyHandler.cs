using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviourPunCallbacks
{
    public static LobbyHandler Instance { get; private set; }
    [Header("Sprites")]
    [SerializeField] private List<Sprite> playersSprites;

    [Header("LoadingPanel")]
    [SerializeField] private Transform loadingScreen;

    [Header("SelectionPanel")]
    [SerializeField] private Transform selectionPanel;

    [Header("CreateRoomPanel")]
    [SerializeField] private Transform createRoomPanel;

    [SerializeField] private TMP_InputField roomNameInputField;

    [Header("QuickJoinRoomPanel")]
    [SerializeField] private Transform quickJoinRoomPanel;

    [Header("RoomListPanel")]
    [SerializeField] private Transform roomListPanel;
    [SerializeField] private Transform roomListContent;

    [SerializeField] private GameObject roomListEntryPrefab;

    [Header("InsideRoomPanel")]
    [SerializeField] private Transform insideRoomPanel;

    [SerializeField] private Button insideRoomStartGameButton;

    [SerializeField] private GameObject playerListEntryPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    private bool isConnectedToMaster;

    private void Awake()
    {
        Instance = this;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
        playerListEntries = new Dictionary<int, GameObject>();

        PhotonNetwork.LocalPlayer.NickName = $"Player{UnityEngine.Random.Range(100, 1000)}";
        PhotonNetwork.ConnectUsingSettings();

        loadingScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        PhotonNetwork.NetworkingClient.StateChanged += PhotonNetwork_StateChanged;
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.StateChanged -= PhotonNetwork_StateChanged;
    }

    private void PhotonNetwork_StateChanged(ClientState previousState, ClientState newState)
    {
        if (loadingScreen != null)
        {
            loadingScreen.GetComponentInChildren<TMP_Text>().text = $"{BattleCityUtils.SplitCamelCase($"{newState}")}";
        }

        if (newState == ClientState.ConnectedToMasterServer && !isConnectedToMaster)
        {
            isConnectedToMaster = true;

            var loadingScreenAnimation = loadingScreen.GetComponent<Animation>();

            loadingScreenAnimation.Rewind();
            loadingScreenAnimation.Play();
        }
    }

    public override void OnConnectedToMaster()
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        // whenever this joins a new lobby, clear any previous room lists
        cachedRoomList.Clear();

        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        // when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        cachedRoomList.Clear();

        ClearRoomListView();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        var roomName = $"Room{UnityEngine.Random.Range(100, 1000)}";
        var options = new RoomOptions { MaxPlayers = 2 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        SetActivePanel(insideRoomPanel.name);

        playerListEntries ??= new Dictionary<int, GameObject>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var entry = Instantiate(playerListEntryPrefab);

            entry.transform.SetParent(insideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;

            var playerListEntry = entry.GetComponent<PlayerListEntry>();

            playerListEntry.Initialize(player.ActorNumber, player.NickName);

            if (player.CustomProperties.TryGetValue(StaticStrings.IS_PLAYER_READY, out object isPlayerReady))
            {
                playerListEntry.SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(player.ActorNumber, entry);
        }

        insideRoomStartGameButton.gameObject.SetActive(CheckPlayersReady());

        var props = new ExitGames.Client.Photon.Hashtable()
        {
            { StaticStrings.IS_PLAYER_READY, false }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(selectionPanel.name);

        foreach (var entry in playerListEntries.Values)
        {
            Destroy(entry);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        var entry = Instantiate(playerListEntryPrefab);

        entry.transform.SetParent(insideRoomPanel.transform);
        entry.transform.localScale = Vector3.one;

        var playerListEntry = entry.GetComponent<PlayerListEntry>();

        playerListEntry.Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListEntries.Add(newPlayer.ActorNumber, entry);

        insideRoomStartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerListEntries.TryGetValue(otherPlayer.ActorNumber, out GameObject entry))
        {
            Destroy(entry);
        }

        playerListEntries.Remove(otherPlayer.ActorNumber);

        insideRoomStartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            insideRoomStartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        playerListEntries ??= new Dictionary<int, GameObject>();

        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out GameObject entry))
        {
            if (changedProps.TryGetValue(StaticStrings.IS_PLAYER_READY, out object isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        insideRoomStartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public void SelectionPanelCreateRoomButtonOnClick()
    {
        SetActivePanel(createRoomPanel.name);
    }

    public void SelectionPanelQuickJoinRoomButtonOnClick()
    {
        SetActivePanel(quickJoinRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public void SelectionPanelRoomListButtonOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(roomListPanel.name);
    }

    public void SelectionPanelMainMenuButtonOnClick()
    {
        PhotonNetwork.Disconnect();
    }

    public void CreateRoomPanelCancelButtonOnClick()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(selectionPanel.name);
    }

    public void CreateRoomPanelCreateRoomButtonOnClick()
    {
        var roomName = string.IsNullOrEmpty(roomNameInputField.text) ? "Room " + Random.Range(1000, 10000) : roomNameInputField.text;
        var maxPlayers = 2;

        var roomOptions = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }

    public void RoomListPanelBackButtonOnClick()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(selectionPanel.name);
    }

    public void InsideRoomPanelLeaveRoomButtonOnClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void InsideRoomPanelStartGameButtonOnClick()
    {
        Debug.Log($"Start Game");

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel($"{LoadingManager.Scene.GameScene}");
    }

    public void LocalPlayerPropertiesUpdated()
    {
        insideRoomStartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public Sprite GetPlayerSprite(int spriteChoice)
    {
        if (spriteChoice == 0 || spriteChoice == 1)
        {
            return playersSprites[spriteChoice];
        }

        return playersSprites[0];
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(StaticStrings.IS_PLAYER_READY, out object isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        return true;
    }

    private void SetActivePanel(string activePanel)
    {
        loadingScreen.gameObject.SetActive(activePanel.Equals(loadingScreen.gameObject.name));
        selectionPanel.gameObject.SetActive(activePanel.Equals(selectionPanel.gameObject.name));
        createRoomPanel.gameObject.SetActive(activePanel.Equals(createRoomPanel.gameObject.name));
        quickJoinRoomPanel.gameObject.SetActive(activePanel.Equals(quickJoinRoomPanel.gameObject.name));
        insideRoomPanel.gameObject.SetActive(activePanel.Equals(insideRoomPanel.gameObject.name));
        // UI should call OnRoomListButtonClicked() to activate this
        roomListPanel.gameObject.SetActive(activePanel.Equals(roomListPanel.gameObject.name));
    }

    private void ClearRoomListView()
    {
        foreach (var entry in roomListEntries.Values)
        {
            Destroy(entry);
        }

        roomListEntries.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (var info in cachedRoomList.Values)
        {
            var entry = Instantiate(roomListEntryPrefab);

            entry.transform.SetParent(roomListContent);
            entry.transform.localScale = Vector3.one;

            var roomListEntry = entry.GetComponent<RoomListEntry>();

            roomListEntry.Initialize(info.Name, info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, entry);
        }
    }
}
