using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    public GameMode GameMode { get => gameMode; set => gameMode = value; }

    [Header("Game Data")]
    [SerializeField] private GameSettingsSO gameSettingsSO;

    private GameMode gameMode;

    private void Awake()
    {
        Instance = this;

#if UNITY_STANDALONE
        Application.targetFrameRate = 59;
        QualitySettings.vSyncCount = 0;
#else
        Application.targetFrameRate = 60;
#endif

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 1;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LoadingManager.LoadScene(LoadingManager.Scene.MenuScene);
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }
}
