using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BattleCityPlayer : MonoBehaviour
{
    public int Level { get => level; }
    public int Lives { get => lives; }
    public int LocalPlayerActorNumber { get => localPlayerActorNumber; }

    public float MaxSpeed { get => maxSpeed; }

    [SerializeField] private float maxSpeed = 0.1f;

    [SerializeField] private Animator shieldAnim;

    [SerializeField] private GameObject bulletWeak;
    [SerializeField] private GameObject bulletFast;
    [SerializeField] private GameObject bulletStrong;

    private PhotonView photonView;

    private Animator animator;

    private BattleCityShooting battleCityityShooting;

    private int level;
    private int lives;
    private int localPlayerActorNumber;
    private int shieldTime;

    private int easyTanksKilled;
    private int fastTanksKilled;
    private int mediumTanksKilled;
    private int strongTanksKilled;
    private int totalLevelScore;

    private bool isFreezed;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        battleCityityShooting = GetComponent<BattleCityShooting>();
    }

    private void Start()
    {
        level = 1;
        lives = 3;
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer && !photonView.IsMine) return;

        if (level == 1)
        {
            battleCityityShooting.SetBullet(bulletWeak.transform);
            battleCityityShooting.SetMaxBullets(1);
        }
        else if (level == 2)
        {
            battleCityityShooting.SetBullet(bulletFast.transform);
            battleCityityShooting.SetMaxBullets(1);
        }
        else if (level == 3)
        {
            battleCityityShooting.SetBullet(bulletFast.transform);
            battleCityityShooting.SetMaxBullets(2);
        }
        else if (level == 4)
        {
            battleCityityShooting.SetBullet(bulletStrong.transform);
            battleCityityShooting.SetMaxBullets(2);
        }

        animator.SetInteger(StaticStrings.LEVEL, level);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.GetComponent<Transform>();

        // TOOD: add script to identify a powerup
        if (other.name.Contains("PowerUp"))
        {
            var bonus = Mathf.Round(other.GetComponent<Animator>().GetFloat(StaticStrings.BONUS));

            switch (bonus)
            {
                case 1:
                    level++;
                    break;
                case 2:
                    other.GetComponent<BattleCityPowerUp>().DestroyAllTanks(this);
                    break;
                case 3:
                    other.GetComponent<BattleCityPowerUp>().FreezeTime();
                    break;
                case 4:
                    SetShield(15);
                    break;
                case 5:
                    lives++;
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        if (!photonView.IsMine) return;

                        var props = new ExitGames.Client.Photon.Hashtable()
                        {
                            { StaticStrings.PLAYER_LIVES, lives }
                        };

                        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    }
                    else
                    {
                        BattleCityEagle.Instance.SetPlayerLives(localPlayerActorNumber, lives);
                    }
                    break;
                case 6:
                    BattleCityEagle.Instance.RepairAndUpgradeEagleWallBaseToSteel();
                    break;
                default:
                    break;
            }

            other.GetComponent<BattleCityPowerUp>().HidePowerUp();
        }
    }

    public void SetShield(int time)
    {
        if (shieldTime <= 0)
        {
            shieldTime = time;
            StartCoroutine(ShieldEnumerator());
        }

        shieldTime = time;
        shieldAnim.SetBool(StaticStrings.IS_ON, true);
        animator.SetBool(StaticStrings.SHIELD, true);
    }

    private IEnumerator ShieldEnumerator()
    {
        while (shieldTime > 0)
        {
            yield return new WaitForSeconds(1);

            shieldTime--;
        }

        if (shieldTime <= 0)
        {
            shieldAnim.SetBool(StaticStrings.IS_ON, false);
            animator.SetBool(StaticStrings.SHIELD, false);
        }
    }

    public void SetLocalPlayerActorNumber(int localPlayerActorNumber)
    {
        this.localPlayerActorNumber = localPlayerActorNumber;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void SetLives(int lives)
    {
        this.lives = lives;

        BattleCityEagle.Instance.SetPlayerLives(localPlayerActorNumber, lives);
    }

    public void Hit()
    {
        lives--;

        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (!photonView.IsMine) return;

            var props = new ExitGames.Client.Photon.Hashtable()
                {
                    { StaticStrings.PLAYER_LIVES, lives }
                };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else
        {
            BattleCityEagle.Instance.SetPlayerLives(localPlayerActorNumber, lives);
        }
    }

    public bool IsPlayerFreezed()
    {
        return isFreezed;
    }

    public void FreezePlayer(BattleCityPlayer battleCityPlayer = null)
    {
        if (battleCityPlayer != null && NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            var photonViewID = battleCityPlayer.GetComponent<PhotonView>().ViewID;

            photonView.RPC(nameof(FreezePlayerPunRPC), RpcTarget.All, photonViewID);
        }
        else
        {
            StopCoroutine(nameof(FreezePlayerDelayed));
            StartCoroutine(nameof(FreezePlayerDelayed));
        }
    }

    public IEnumerator FreezePlayerDelayed()
    {
        isFreezed = true;

        float duration = 6f;
        float startTime = Time.time;

        var playerSprite = gameObject.GetComponent<SpriteRenderer>();


        while (Time.time < startTime + duration)
        {
            playerSprite.enabled = !playerSprite.enabled;

            yield return new WaitForSeconds(0.2f);
        }

        playerSprite.enabled = true;

        isFreezed = false;
    }

    public void ResetLevelScore()
    {
        easyTanksKilled = 0;
        fastTanksKilled = 0;
        mediumTanksKilled = 0;
        strongTanksKilled = 0;
    }

    public void UpdatePlayerLevelScore(int score, int playerIndex = 0)
    {
        totalLevelScore += score;

        if (PlayerPrefs.HasKey(StaticStrings.PLAYER_HIGH_SCORE_PREF_KEY))
        {
            var playerHighScore = PlayerPrefs.GetString(StaticStrings.PLAYER_HIGH_SCORE_PREF_KEY);

            if (totalLevelScore > int.Parse(playerHighScore))
            {
                PlayerPrefs.SetString(StaticStrings.PLAYER_HIGH_SCORE_PREF_KEY, $"{totalLevelScore}");

                PlayerPrefs.Save();
            }
        }
        else
        {
            PlayerPrefs.SetString(StaticStrings.PLAYER_HIGH_SCORE_PREF_KEY, $"{totalLevelScore}");

            PlayerPrefs.Save();
        }

        switch (score)
        {
            case 100:
                easyTanksKilled++;
                break;
            case 200:
                fastTanksKilled++;
                break;
            case 300:
                mediumTanksKilled++;
                break;
            case 400:
                strongTanksKilled++;
                break;
            default:
                break;
        }

        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (playerIndex == 1)
            {
                int[] playerOneStats = new int[5];

                playerOneStats[0] = easyTanksKilled;
                playerOneStats[1] = fastTanksKilled;
                playerOneStats[2] = mediumTanksKilled;
                playerOneStats[3] = strongTanksKilled;
                playerOneStats[4] = totalLevelScore;

                var props = new ExitGames.Client.Photon.Hashtable()
                {
                    { StaticStrings.PLAYER_ONE_SCORE, playerOneStats },
                };

                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
            else if (playerIndex == 2)
            {
                int[] playerTwoStats = new int[5];

                playerTwoStats[0] = easyTanksKilled;
                playerTwoStats[1] = fastTanksKilled;
                playerTwoStats[2] = mediumTanksKilled;
                playerTwoStats[3] = strongTanksKilled;
                playerTwoStats[4] = totalLevelScore;

                var props = new ExitGames.Client.Photon.Hashtable()
                {
                    { StaticStrings.PLAYER_TWO_SCORE, playerTwoStats },
                };

                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }
        else
        {
            GameManager.Instance.UpdatePlayerStats(easyTanksKilled, fastTanksKilled, mediumTanksKilled, strongTanksKilled, totalLevelScore);
        }
    }

    public int GetEasyTanksKilled()
    {
        return easyTanksKilled;
    }

    public int GetFastTanksKilled()
    {
        return fastTanksKilled;
    }

    public int GetMediumTanksKilled()
    {
        return mediumTanksKilled;
    }

    public int GetStrongTanksKilled()
    {
        return strongTanksKilled;
    }

    public int GetTotalLevelScore()
    {
        return totalLevelScore;
    }

    [PunRPC]
    public void FreezePlayerPunRPC(int photonViewID)
    {
        var go = PhotonView.Find(photonViewID);

        if (go != null)
        {
            var battleCityPlayer = go.gameObject.GetComponent<BattleCityPlayer>();

            battleCityPlayer.FreezePlayer();
        }
    }
}
