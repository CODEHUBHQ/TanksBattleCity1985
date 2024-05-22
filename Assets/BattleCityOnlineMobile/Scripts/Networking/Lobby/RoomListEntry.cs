using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RoomListEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text roomPlayersText;

    [SerializeField] private Button joinRoomButton;

    private string roomName;

    private void Start()
    {
        joinRoomButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(roomName);
        });
    }

    public void Initialize(string roomName, int currentPlayers, int maxPlayers)
    {
        this.roomName = roomName;

        roomNameText.text = roomName;
        roomPlayersText.text = $"{currentPlayers} / {maxPlayers}";
    }
}
