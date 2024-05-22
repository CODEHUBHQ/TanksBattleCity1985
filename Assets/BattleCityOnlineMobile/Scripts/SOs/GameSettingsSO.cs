using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "CODEHUB/GameSettings")]
public class GameSettingsSO : ScriptableObject
{
    public string AppVersion { get => appVersion; set => appVersion = value; }

    public int[] MaxPing { get => maxPing; set => maxPing = value; }

    public int MaxPlayers { get => maxPlayers; set => maxPlayers = value; }

    public ServerRegionCode ServerRegionCode { get => serverRegionCode; set => serverRegionCode = value; }

    [Header("Photon")]
    [SerializeField] private string appVersion = "1.0.1";
    [SerializeField] private ServerRegionCode serverRegionCode = ServerRegionCode.eu;

    [Header("Room Options")]
    [SerializeField] private int[] maxPing = new int[] { 100, 200, 500, 1000 };
    [SerializeField] private int maxPlayers = 2;
}
