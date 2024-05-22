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

    private void Awake()
    {
        Instance = this;

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
        GameManager.Instance.PlayerOne.GetComponent<BattleCityPlayer>().SetLevel(1);
        GameManager.Instance.PlayerOne.GetComponent<BattleCityPlayer>().SetLives(3);

        if (GameManager.Instance.PlayerTwo != null)
        {
            GameManager.Instance.PlayerTwo.GetComponent<BattleCityPlayer>().SetLevel(1);
            GameManager.Instance.PlayerTwo.GetComponent<BattleCityPlayer>().SetLives(3);
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
        playerOneLivesDigitNumber[0].sprite = numbers[3];
        playerOneLivesDigitNumber[1].gameObject.SetActive(false);
        playerTwoLivesDigitNumber[0].sprite = numbers[3];
        playerTwoLivesDigitNumber[1].gameObject.SetActive(false);
    }

    public void SetPlayerLives(int playerIndex, int lives)
    {
        if (playerIndex == 0)
        {
            var intLivesArray = BattleCityUtils.GetIntArray(lives);

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
            Destroy(sprite.gameObject);
        }

        var ts = BattleCityMapLoad.Instance.GeneratedWallContainer.GetComponentsInChildren<Transform>();
        var wallPrefab = BattleCityMapLoad.Instance.WallPrefab;

        foreach (var coords in eagleBaseWallCoords)
        {
            // remove temp brick wall and repair eagle base brick wall
            var wallTransform = ts.GetByNameAndCoords("Temp", coords.x, coords.y);

            if (wallTransform != null)
            {
                Destroy(wallTransform.gameObject);
            }

            var wallGameObject = Instantiate(wallPrefab, new Vector3(coords.x, coords.y, 0), wallPrefab.transform.rotation);

            wallGameObject.transform.parent = BattleCityMapLoad.Instance.GeneratedWallContainer;
        }
    }
}
