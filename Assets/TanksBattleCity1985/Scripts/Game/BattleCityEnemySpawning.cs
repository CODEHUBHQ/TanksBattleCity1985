using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleCityEnemySpawning : MonoBehaviour, IPunObservable
{
    public static BattleCityEnemySpawning Instance { get; private set; }

    [SerializeField] private GameObject easyTank;
    [SerializeField] private GameObject fastTank;
    [SerializeField] private GameObject mediumTank;
    [SerializeField] private GameObject strongTank;

    private PhotonView photonView;

    private Animator animator;

    private int[] tanks;
    private int next;

    private bool invokeOnce;

    private void Awake()
    {
        Instance = this;

        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        tanks = new int[20];

        //for (int i = 0; i < 20; i++)
        //{
        //    tanks[i] = new System.Random().Next(50) % 4 + 1;

        //    Debug.Log($"tanks {tanks[i]}");
        //}

        //Reset();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;

        int tankCount = BattleCityMapLoad.Instance.GeneratedEnemyContainer.GetComponentsInChildren<Transform>().Length;

        var isMultiplayer = false;

        if (NetworkManager.Instance != null)
        {
            isMultiplayer = NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer;
        }

        if (next < 20 && (tankCount < 5 && !isMultiplayer || tankCount < 7 && isMultiplayer))
        {
            animator.SetBool(StaticStrings.SPAWN, true);
        }
        else if (next >= 20 && tankCount <= 1)
        {
            // load next map
            if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
            {
                if (!invokeOnce)
                {
                    invokeOnce = true;

                    photonView.RPC(nameof(LoadNextMapPunRPC), RpcTarget.All);
                }
            }
            else
            {
                if (!BattleCityMapLoad.Instance.IsLoadingMap())
                {
                    BattleCityMapLoad.Instance.SetIsLoadingMap(true);

                    BattleCityMapLoad.Instance.LoadMap(true, this);
                }
            }
        }
    }

    public void Reset()
    {
        transform.position = new Vector3(-12, 12, 0);

        var isCustomMap = bool.Parse(PlayerPrefs.GetString(StaticStrings.IS_CUSTOM_MAP, "false"));

        if (isCustomMap)
        {
            for (int i = 0; i < 20; i++)
            {
                tanks[i] = new System.Random().Next(50) % 4 + 1;
            }
        }
        else
        {
            var stage = GameManager.Instance.StagesSO.stageSOList[BattleCityMapLoad.Instance.Level - 1];

            int index = 0;
            foreach (var stageData in stage.stageDataList)
            {
                if (stageData.TanksToSpawn == 0) continue;

                for (int i = 0; i < stageData.TanksToSpawn; i++)
                {
                    tanks[index] = (int)stageData.TankType;

                    index++;
                }
            }
        }

        next = 0;
    }

    // Called from animation event
    private void SpawnEnemy()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnEnemyOnline();
            }
        }
        else
        {
            SpawnEnemyOffline();
        }
    }

    private void SpawnEnemyOnline()
    {
        animator.SetBool(StaticStrings.SPAWN, false);

        GameObject t = null;

        if (tanks[next] == 1)
        {
            t = PhotonNetwork.Instantiate(easyTank.name, transform.position, easyTank.transform.rotation);
        }
        else if (tanks[next] == 2)
        {
            t = PhotonNetwork.Instantiate(fastTank.name, transform.position, fastTank.transform.rotation);
        }
        else if (tanks[next] == 3)
        {
            t = PhotonNetwork.Instantiate(mediumTank.name, transform.position, mediumTank.transform.rotation);
        }
        else if (tanks[next] == 4)
        {
            t = PhotonNetwork.Instantiate(strongTank.name, transform.position, strongTank.transform.rotation);

            t.GetComponent<BattleCityEnemy>().SetLives(5);
        }

        photonView.RPC(nameof(ParentSpawnedEnemies), RpcTarget.All);

        PushPosition();

        // every 4 enemies, one get bonus
        if ((next + 1) % 4 == 0)
        {
            t.GetComponent<BattleCityEnemy>().SetBonus(new System.Random().Next(50) % 6 + 1);
        }

        next++;

        photonView.RPC(nameof(EnemyWasSpawnedPunRPC), RpcTarget.All);
    }

    private void SpawnEnemyOffline()
    {
        animator.SetBool(StaticStrings.SPAWN, false);

        GameObject t = null;

        if (tanks[next] == 1)
        {
            t = Instantiate(easyTank, transform.position, easyTank.transform.rotation);
        }
        else if (tanks[next] == 2)
        {
            t = Instantiate(fastTank, transform.position, fastTank.transform.rotation);
        }
        else if (tanks[next] == 3)
        {
            t = Instantiate(mediumTank, transform.position, fastTank.transform.rotation);
        }
        else if (tanks[next] == 4)
        {
            t = Instantiate(strongTank, transform.position, fastTank.transform.rotation);

            t.GetComponent<BattleCityEnemy>().SetLives(5);
        }

        t.transform.parent = BattleCityMapLoad.Instance.GeneratedEnemyContainer;

        PushPosition();

        // every 4 enemies, one get bonus
        if ((next + 1) % 4 == 0)
        {
            t.GetComponent<BattleCityEnemy>().SetBonus(new System.Random().Next(50) % 6 + 1);
        }

        next++;

        BattleCityEagle.Instance.EnemyWasSpawned();
    }

    private void PushPosition()
    {
        transform.position += new Vector3(12, 0, 0);

        if (transform.position.x > 12)
        {
            transform.position = new Vector3(-12, 12, 0);
        }
    }

    public void SetInvokeOnce(bool invokeOnce)
    {
        this.invokeOnce = invokeOnce;
    }

    [PunRPC]
    public void ParentSpawnedEnemies()
    {
        // if not master then find spwaned enemies and parent them
        var spwanedEnemies = FindObjectsByType<BattleCityEnemy>(FindObjectsSortMode.None);

        foreach (var spwanedEnemy in spwanedEnemies)
        {
            spwanedEnemy.transform.parent = BattleCityMapLoad.Instance.GeneratedEnemyContainer;
        }
    }

    [PunRPC]
    public void EnemyWasSpawnedPunRPC()
    {
        BattleCityEagle.Instance.EnemyWasSpawned();
    }

    [PunRPC]
    public void LoadNextMapPunRPC()
    {
        // load next map
        if (!BattleCityMapLoad.Instance.IsLoadingMap())
        {
            BattleCityMapLoad.Instance.SetIsLoadingMap(true);

            BattleCityMapLoad.Instance.LoadMap(true, this);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tanks);
            stream.SendNext(next);
            stream.SendNext(transform.position);
        }
        else
        {
            tanks = (int[])stream.ReceiveNext();
            next = (int)stream.ReceiveNext();
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }
}
