using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleCityEagle : MonoBehaviour
{
    public static BattleCityEagle Instance { get; private set; }

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject enemiesListGameObject;

    [SerializeField] private GameObject playerOneIconGameObject;
    [SerializeField] private List<SpriteRenderer> playerOneLivesDigitNumber;

    [SerializeField] private GameObject playerTwoIconGameObject;
    [SerializeField] private List<SpriteRenderer> playerTwoLivesDigitNumber;

    [SerializeField] private List<SpriteRenderer> levelDigitNumber;
    [SerializeField] private SpriteRenderer levelCustom;

    [SerializeField] private List<Sprite> numbers;

    [SerializeField] private List<Vector2> eagleBaseWallCoords;

    private Animator animator;

    private List<GameObject> enemiesList;

    private PhotonView photonView;

    private void Awake()
    {
        Instance = this;

        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();

        enemiesList = new List<GameObject>();

        if (NetworkManager.Instance != null)
        {
            if (NetworkManager.Instance.GameMode == GameMode.SinglePlayer)
            {
                playerOneIconGameObject.SetActive(true);
                playerTwoIconGameObject.SetActive(false);
            }

            if (NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer || NetworkManager.Instance.GameMode == GameMode.Multiplayer)
            {
                playerOneIconGameObject.SetActive(true);
                playerTwoIconGameObject.SetActive(true);
            }
        }
        else
        {
            playerTwoIconGameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.GetComponent<Transform>();
        var otherAnimator = other.GetComponent<Animator>();

        if (other.name.Contains("Bullet") && !otherAnimator.GetBool(StaticStrings.HIT) && !animator.GetBool(StaticStrings.IS_DESTROYED))
        {
            otherAnimator.SetBool(StaticStrings.HIT, true);

            animator.SetBool(StaticStrings.IS_DESTROYED, true);

            SoundManager.Instance.PlayEagleDestroySound();

            GameManager.Instance.SetIsGameOver(true);

            this.DoAfter(1, () => SoundManager.Instance.PlayGameOverSound());
        }
    }

    public void FinishGame()
    {
        if (NetworkManager.Instance == null || (NetworkManager.Instance != null && NetworkManager.Instance.GameMode != GameMode.Multiplayer))
        {
            GameManager.Instance.PlayerOne.GetComponent<BattleCityPlayer>().SetLevel(1);
            GameManager.Instance.PlayerOne.GetComponent<BattleCityPlayer>().SetLives(3);
        }

        gameOverUI.SetActive(true);

        animator.SetBool(StaticStrings.IS_DESTROYED, false);
    }

    public void ResetSpawnedEnemyList()
    {
        var ts = enemiesListGameObject.GetComponentsInChildren<Transform>(true);

        enemiesList = new List<GameObject>();

        foreach (var t in ts)
        {
            t.gameObject.SetActive(true);

            enemiesList.Add(t.gameObject);
        }
    }

    public void EnemyWasSpawned()
    {
        var enemyIcon = enemiesList.Last();

        enemyIcon.SetActive(false);

        enemiesList.RemoveAt(enemiesList.Count - 1);
    }

    public void ResetPlayersLives()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(ResetPlayersLivesPunRPC), RpcTarget.All);
            }
        }
        else
        {
            playerOneLivesDigitNumber[0].sprite = numbers[3];
            playerOneLivesDigitNumber[1].gameObject.SetActive(false);
            playerTwoLivesDigitNumber[0].sprite = numbers[3];
            playerTwoLivesDigitNumber[1].gameObject.SetActive(false);
        }
    }

    public void SetPlayerLives(int playerIndex, int lives)
    {
        if (playerIndex == 0)
        {
            var intLivesArray = BattleCityUtils.GetIntArray(lives);

            if (intLivesArray.Length == 0)
            {
                playerOneLivesDigitNumber[0].gameObject.SetActive(true);
                playerOneLivesDigitNumber[0].sprite = numbers[0];
            }

            for (int i = 0; i < intLivesArray.Length; i++)
            {
                var intLives = intLivesArray[i];

                playerOneLivesDigitNumber[i].gameObject.SetActive(true);
                playerOneLivesDigitNumber[i].sprite = numbers[intLives];
            }
        }

        if (playerIndex == 1)
        {
            var intLivesArray = BattleCityUtils.GetIntArray(lives);

            if (intLivesArray.Length == 0)
            {
                playerTwoLivesDigitNumber[0].gameObject.SetActive(true);
                playerTwoLivesDigitNumber[0].sprite = numbers[0];
            }

            for (int i = 0; i < intLivesArray.Length; i++)
            {
                var intLives = intLivesArray[i];

                playerTwoLivesDigitNumber[i].gameObject.SetActive(true);
                playerTwoLivesDigitNumber[i].sprite = numbers[intLives];
            }
        }
    }

    public void SetGameLevel(int level)
    {
        var intLevelArray = BattleCityUtils.GetIntArray(level);

        for (int i = 0; i < intLevelArray.Length; i++)
        {
            var intLevel = intLevelArray[i];

            levelDigitNumber[i].gameObject.SetActive(true);

            levelDigitNumber[i].sprite = numbers[intLevel];
        }
    }

    public void SetCustomGameLevel()
    {
        levelCustom.gameObject.SetActive(true);
    }

    public void RepairAndUpgradeEagleWallBaseToSteel()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            photonView.RPC(nameof(RepairAndUpgradeEagleWallBaseToSteelPunRPC), RpcTarget.MasterClient);
        }
        else
        {
            var ts = BattleCityMapLoad.Instance.GeneratedWallContainer.GetComponentsInChildren<Transform>();
            var baseSteelSprites = new List<SpriteRenderer>();
            var wallPrefab = BattleCityMapLoad.Instance.WallPrefab;
            var ironPrefab = BattleCityMapLoad.Instance.IronPrefab;

            foreach (var coords in eagleBaseWallCoords)
            {
                var ironGameObject = Instantiate(ironPrefab, new Vector3(coords.x, coords.y, 0), wallPrefab.transform.rotation);

                ironGameObject.transform.parent = BattleCityMapLoad.Instance.GeneratedWallContainer;

                var ironSpriteRenderer = ironGameObject.GetComponent<SpriteRenderer>();

                ironSpriteRenderer.sortingOrder = 5;

                baseSteelSprites.Add(ironSpriteRenderer);

                // add temp brick wall and remove old one
                var wallTransform = ts.GetByNameAndCoords("Wall", coords.x, coords.y);

                if (wallTransform != null)
                {
                    Destroy(wallTransform.gameObject);
                }

                var wallGameObject = Instantiate(wallPrefab, new Vector3(coords.x, coords.y, 0), wallPrefab.transform.rotation);

                wallGameObject.name = $"Temp";

                wallGameObject.transform.parent = BattleCityMapLoad.Instance.GeneratedWallContainer;
            }

            StopCoroutine(nameof(DestroyEagleBaseSteelDelayed));
            StartCoroutine(nameof(DestroyEagleBaseSteelDelayed), baseSteelSprites);
        }
    }

    public IEnumerator DestroyEagleBaseSteelDelayed(List<SpriteRenderer> sprites)
    {
        yield return new WaitForSeconds(15f);

        float duration = 5f;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            foreach (var sprite in sprites)
            {
                sprite.enabled = !sprite.enabled;
            }

            yield return new WaitForSeconds(0.2f);
        }

        foreach (var sprite in sprites)
        {
            if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(sprite.gameObject);
                }
            }
            else
            {
                Destroy(sprite.gameObject);
            }
        }

        var ts = BattleCityMapLoad.Instance.GeneratedWallContainer.GetComponentsInChildren<Transform>();
        var wallPrefab = BattleCityMapLoad.Instance.WallPrefab;

        foreach (var coords in eagleBaseWallCoords)
        {
            // remove temp brick wall and repair eagle base brick wall
            var wallTransform = ts.GetByNameAndCoords("Temp", coords.x, coords.y);

            if (wallTransform != null)
            {
                if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(wallTransform.gameObject);
                    }
                }
                else
                {
                    Destroy(wallTransform.gameObject);
                }
            }

            if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    var wallGameObjectInstance = PhotonNetwork.Instantiate(wallPrefab.name, new Vector3(coords.x, coords.y, 0), wallPrefab.transform.rotation);
                    var wallGameObjectPhotonViewID = wallGameObjectInstance.GetComponent<PhotonView>().ViewID;

                    photonView.RPC(nameof(ParentWallPunRPC), RpcTarget.All, wallGameObjectPhotonViewID);
                }
            }
            else
            {
                var wallGameObject = Instantiate(wallPrefab, new Vector3(coords.x, coords.y, 0), wallPrefab.transform.rotation);

                wallGameObject.transform.parent = BattleCityMapLoad.Instance.GeneratedWallContainer;
            }
        }
    }

    [PunRPC]
    public void ResetPlayersLivesPunRPC()
    {
        playerOneLivesDigitNumber[0].sprite = numbers[3];
        playerOneLivesDigitNumber[1].gameObject.SetActive(false);
        playerTwoLivesDigitNumber[0].sprite = numbers[3];
        playerTwoLivesDigitNumber[1].gameObject.SetActive(false);
    }

    [PunRPC]
    public void RepairAndUpgradeEagleWallBaseToSteelPunRPC()
    {
        var ts = BattleCityMapLoad.Instance.GeneratedWallContainer.GetComponentsInChildren<Transform>();
        var baseSteelSprites = new List<SpriteRenderer>();
        var wallPrefab = BattleCityMapLoad.Instance.WallPrefab;
        var ironPrefab = BattleCityMapLoad.Instance.IronPrefab;

        var ironGameObjectsArray = new int[eagleBaseWallCoords.Count];
        var wallGameObjectsArray = new int[eagleBaseWallCoords.Count];

        var index = 0;

        foreach (var coords in eagleBaseWallCoords)
        {
            var ironGameObject = PhotonNetwork.Instantiate(ironPrefab.name, new Vector3(coords.x, coords.y, 0), wallPrefab.transform.rotation);

            var ironGameObjectPhotonViewID = ironGameObject.GetComponent<PhotonView>().ViewID;

            ironGameObjectsArray[index] = ironGameObjectPhotonViewID;

            var ironSpriteRenderer = ironGameObject.GetComponent<SpriteRenderer>();

            ironSpriteRenderer.sortingOrder = 5;

            baseSteelSprites.Add(ironSpriteRenderer);

            // add temp brick wall and remove old one
            var wallTransform = ts.GetByNameAndCoords("Wall", coords.x, coords.y);

            if (wallTransform != null)
            {
                PhotonNetwork.Destroy(wallTransform.gameObject);
            }

            var wallGameObject = PhotonNetwork.Instantiate(wallPrefab.name, new Vector3(coords.x, coords.y, 0), wallPrefab.transform.rotation);

            wallGameObject.name = $"Temp";

            var wallGameObjectPhotonViewID = wallGameObject.GetComponent<PhotonView>().ViewID;

            wallGameObjectsArray[index] = wallGameObjectPhotonViewID;

            index++;
        }

        photonView.RPC(nameof(ParentWallArrayPunRPC), RpcTarget.All, ironGameObjectsArray, wallGameObjectsArray);

        photonView.RPC(nameof(DestroyEagleBaseSteelDelayedPunRPC), RpcTarget.All, ironGameObjectsArray);
    }

    [PunRPC]
    public void ParentWallPunRPC(int photonViewID)
    {
        var go = PhotonView.Find(photonViewID);

        if (go != null)
        {
            go.transform.parent = BattleCityMapLoad.Instance.GeneratedWallContainer;
        }
    }


    [PunRPC]
    public void ParentWallArrayPunRPC(int[] ironGameObjectsArray, int[] wallGameObjectsArray)
    {
        foreach (var ironPhotonViewID in ironGameObjectsArray)
        {
            var ironGameObject = PhotonView.Find(ironPhotonViewID);

            if (ironGameObject != null)
            {
                var ironSpriteRenderer = ironGameObject.GetComponent<SpriteRenderer>();

                ironSpriteRenderer.sortingOrder = 5;

                ironGameObject.transform.parent = BattleCityMapLoad.Instance.GeneratedWallContainer;
            }
        }

        foreach (var wallPhotonViewID in wallGameObjectsArray)
        {
            var wallGameObject = PhotonView.Find(wallPhotonViewID);

            if (wallGameObject != null)
            {
                wallGameObject.transform.parent = BattleCityMapLoad.Instance.GeneratedWallContainer;
            }
        }
    }

    [PunRPC]
    public void DestroyEagleBaseSteelDelayedPunRPC(int[] ironGameObjectsArray)
    {
        var baseSteelSprites = new List<SpriteRenderer>();

        foreach (var ironGameObjectPhotonViewID in ironGameObjectsArray)
        {
            var ironGameObject = PhotonView.Find(ironGameObjectPhotonViewID);

            if (ironGameObject != null)
            {
                var ironSpriteRenderer = ironGameObject.GetComponent<SpriteRenderer>();

                baseSteelSprites.Add(ironSpriteRenderer);
            }
        }

        StopCoroutine(nameof(DestroyEagleBaseSteelDelayed));
        StartCoroutine(nameof(DestroyEagleBaseSteelDelayed), baseSteelSprites);
    }
}
