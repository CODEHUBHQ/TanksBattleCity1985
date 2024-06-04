using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using TMPro;
using System.Linq;

public class BattleCityMapLoad : MonoBehaviour
{
    public static BattleCityMapLoad Instance { get; private set; }

    public Transform GeneratedWallContainer { get => generatedWallContainer; }
    public Transform GeneratedEnemyContainer { get => generatedEnemyContainer; }
    public Transform GeneratedBulletContainer { get => generatedBulletContainer; }
    public Transform PowerUp { get => powerUp; }
    public Transform LoadingScreen { get => loadingScreen; }

    public GameObject WallPrefab { get => wallPrefab; }
    public GameObject IronPrefab { get => ironPrefab; }

    public int Level { get => currentLevel; private set => currentLevel = value; }

    [Header("Containers")]
    [SerializeField] private Transform generatedWallContainer;
    [SerializeField] private Transform generatedEnemyContainer;
    [SerializeField] private Transform generatedBulletContainer;

    [SerializeField] private Transform spawnLocation;
    [SerializeField] private Transform powerUp;
    [SerializeField] private Transform loadingScreen;

    [SerializeField] private Animation loadingScreenAnimation;

    [SerializeField] private TMP_Text loadingScreenLevelText;

    [Header("Map Objects")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject ironPrefab;
    [SerializeField] private GameObject icePrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject bushPrefab;

    private PhotonView photonView;

    private int currentLevel;

    private bool isLoadingMap;

    private void Awake()
    {
        Instance = this;

        photonView = GetComponent<PhotonView>();
    }

    public void LoadMap(bool won, BattleCityEnemySpawning battleCityEnemySpawning)
    {
        if (won)
        {
            var nextLevel = ++currentLevel;

            LevelStatsUI.Instance.ShowPlayersStats(nextLevel - 1, () =>
            {
                var isCustomMap = bool.Parse(PlayerPrefs.GetString(StaticStrings.IS_CUSTOM_MAP, "false"));

                if (isCustomMap)
                {
                    LoadingManager.LoadScene(LoadingManager.Scene.MapEditorScene);
                }
                else
                {
                    if (currentLevel > GameManager.Instance.StagesSO.stageSOList.Count)
                    {
                        // TODO: the tank totals should change on the looping of the levels
                        currentLevel = 1;
                        nextLevel = 1;
                    }

                    StartCoroutine(LoadMapDelayed(nextLevel, battleCityEnemySpawning));
                }
            });
        }
    }

    public IEnumerator LoadMapDelayed(int level, BattleCityEnemySpawning battleCityEnemySpawning)
    {
        loadingScreenLevelText.text = $"Stage {level}";

        loadingScreen.gameObject.SetActive(true);
        loadingScreenAnimation.enabled = false;

        yield return new WaitForSeconds(1f);

        loadingScreenAnimation.enabled = true;

        loadingScreenAnimation.Rewind();
        loadingScreenAnimation.Play();
        loadingScreenAnimation.Sample();
        loadingScreenAnimation.Stop();

        LoadMap(level);

        loadingScreen.gameObject.SetActive(false);

        SetIsLoadingMap(false);
    }

    public void LoadMap(int level)
    {
        Level = level;
        currentLevel = level;

        // set game level number
        PlayerPrefs.SetString(StaticStrings.CURRENT_LEVEL, $"{level}");
        PlayerPrefs.Save();
        BattleCityEagle.Instance.SetGameLevel(level);

        // stop coroutines
        StopAllCoroutines();

        // Reset data
        DeleteChilds(generatedWallContainer);
        DeleteChilds(generatedEnemyContainer);
        DeleteChilds(generatedBulletContainer);

        if ((NetworkManager.Instance == null) || (NetworkManager.Instance != null && NetworkManager.Instance.GameMode != GameMode.Multiplayer))
        {
            // player reset
            var playerOne = GameManager.Instance.PlayerOne;

            playerOne.gameObject.GetComponent<BattleCityPlayerMovement>().ResetPosition();
            playerOne.gameObject.GetComponent<Animator>().SetBool(StaticStrings.HIT, false);
            playerOne.gameObject.GetComponent<BattleCityShooting>().SetShooting(false);
            playerOne.ResetLevelScore();
            playerOne.SetShield(6);

            if (NetworkManager.Instance != null)
            {
                if (NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer)
                {
                    var playerTwo = GameManager.Instance.PlayerTwo;

                    playerTwo.gameObject.GetComponent<BattleCityPlayerMovement>().ResetPosition();
                    playerTwo.gameObject.GetComponent<Animator>().SetBool(StaticStrings.HIT, false);
                    playerTwo.gameObject.GetComponent<BattleCityShooting>().SetShooting(false);
                    playerTwo.ResetLevelScore();
                    playerTwo.SetShield(6);
                }
            }
        }

        if (currentLevel == 1)
        {
            BattleCityEagle.Instance.ResetPlayersLives();
        }

        // Enemy spawning reset
        BattleCityEagle.Instance.ResetSpawnedEnemyList();

        spawnLocation.GetComponent<BattleCityEnemySpawning>().Reset();

        // Read map file
        StartCoroutine(LoadMapFromStreamingAssets($"{Application.streamingAssetsPath}/Maps/map{currentLevel}.txt"));

        // powerUp reset
        powerUp.GetComponent<BattleCityPowerUp>().Reset();

        // play a sound
        SoundManager.Instance.PlayLevelStartingSound();
    }

    public void LoadCustomMap(string customMap)
    {
        // set game level number
        BattleCityEagle.Instance.SetCustomGameLevel();

        // stop coroutines
        StopAllCoroutines();

        // Reset data
        DeleteChilds(generatedWallContainer);
        DeleteChilds(generatedEnemyContainer);
        DeleteChilds(generatedBulletContainer);

        // player reset
        var playerOne = GameManager.Instance.PlayerOne;

        playerOne.gameObject.GetComponent<BattleCityPlayerMovement>().ResetPosition();
        playerOne.gameObject.GetComponent<Animator>().SetBool(StaticStrings.HIT, false);
        playerOne.gameObject.GetComponent<BattleCityShooting>().SetShooting(false);
        playerOne.ResetLevelScore();
        playerOne.SetShield(6);

        if (NetworkManager.Instance != null)
        {
            if (NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer)
            {
                var playerTwo = GameManager.Instance.PlayerTwo;

                playerTwo.gameObject.GetComponent<BattleCityPlayerMovement>().ResetPosition();
                playerTwo.gameObject.GetComponent<Animator>().SetBool(StaticStrings.HIT, false);
                playerTwo.gameObject.GetComponent<BattleCityShooting>().SetShooting(false);
                playerTwo.ResetLevelScore();
                playerTwo.SetShield(6);
            }
        }

        // players lives reset
        BattleCityEagle.Instance.ResetPlayersLives();

        // Enemy spawning reset
        BattleCityEagle.Instance.ResetSpawnedEnemyList();

        spawnLocation.GetComponent<BattleCityEnemySpawning>().Reset();

        // Read map file
        GenerateMap(customMap.Split("\n"));

        // powerUp reset
        powerUp.GetComponent<BattleCityPowerUp>().Reset();

        // play a sound
        SoundManager.Instance.PlayLevelStartingSound();
    }

    private IEnumerator LoadMapFromStreamingAssets(string path)
    {
        if (path.Contains("://") || path.Contains(":///"))
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError($"ERROR: {webRequest.error}");
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"HTTP Error: {webRequest.error}");
                        break;
                    case UnityWebRequest.Result.Success:
                        var map = webRequest.downloadHandler.text.Split("\n");

                        GenerateMap(map);
                        break;
                }
            }
        }
        else
        {
            var map = System.IO.File.ReadAllLines($"{Application.streamingAssetsPath}/Maps/map{currentLevel}.txt");

            GenerateMap(map);
        }
    }

    private void DeleteChilds(Transform container)
    {
        Transform[] ts = container.GetComponentsInChildren<Transform>();

        foreach (Transform t in ts)
        {
            if (!t.gameObject.name.Contains("Generated"))
            {
                if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                {
                    PhotonNetwork.Destroy(t.gameObject);
                }
                else
                {
                    Destroy(t.gameObject);
                }
            }
        }
    }

    private void GenerateMap(string[] map)
    {
        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                GameObject t = null;

                if (map[i][j] == 'o')
                {
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        t = PhotonNetwork.InstantiateRoomObject(wallPrefab.name, new Vector3(j - 13, 13 - (i + 1), 0), wallPrefab.transform.rotation, 0);
                    }
                    else
                    {
                        t = Instantiate(wallPrefab, new Vector3(j - 13, 13 - (i + 1), 0), wallPrefab.transform.rotation);
                    }
                }
                else if (map[i][j] == 'Q')
                {
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        t = PhotonNetwork.InstantiateRoomObject(ironPrefab.name, new Vector3(j - 13, 13 - (i + 1), 0), ironPrefab.transform.rotation, 0);
                    }
                    else
                    {
                        t = Instantiate(ironPrefab, new Vector3(j - 13, 13 - (i + 1), 0), wallPrefab.transform.rotation);
                    }
                }
                else if (map[i][j] == 'b')
                {
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        t = PhotonNetwork.InstantiateRoomObject(bushPrefab.name, new Vector3(j - 13, 13 - (i + 1), 0), bushPrefab.transform.rotation, 0);
                    }
                    else
                    {
                        t = Instantiate(bushPrefab, new Vector3(j - 13, 13 - (i + 1), 0), wallPrefab.transform.rotation);
                    }
                }
                else if (map[i][j] == 'i')
                {
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        t = PhotonNetwork.InstantiateRoomObject(icePrefab.name, new Vector3(j - 13, 13 - (i + 1), 0), icePrefab.transform.rotation, 0);
                    }
                    else
                    {
                        t = Instantiate(icePrefab, new Vector3(j - 13, 13 - (i + 1), 0), wallPrefab.transform.rotation);
                    }
                }
                else if (map[i][j] == 'w')
                {
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        t = PhotonNetwork.InstantiateRoomObject(waterPrefab.name, new Vector3(j - 13, 13 - (i + 1), 0), waterPrefab.transform.rotation, 0);
                    }
                    else
                    {
                        t = Instantiate(waterPrefab, new Vector3(j - 13, 13 - (i + 1), 0), wallPrefab.transform.rotation);
                    }
                }

                if (map[i][j] != '.')
                {
                    t.transform.parent = generatedWallContainer;
                }
            }
        }

        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(ParentMapObjects), RpcTarget.Others);
        }
    }

    public void SetIsLoadingMap(bool isLoadingMap)
    {
        this.isLoadingMap = isLoadingMap;
    }

    public bool IsLoadingMap()
    {
        return isLoadingMap;
    }

    [PunRPC]
    public void ParentMapObjects()
    {
        // if not master then find map objects and parent them
        var mapObjects = FindObjectsByType<BattleCityMapObject>(FindObjectsSortMode.None);

        foreach (var mapObject in mapObjects)
        {
            mapObject.transform.parent = generatedWallContainer;
        }
    }
}
