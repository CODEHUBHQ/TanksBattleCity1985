using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityPlayer : MonoBehaviour, IPunObservable
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
                    BattleCityEagle.Instance.SetPlayerLives(localPlayerActorNumber, lives);
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

        BattleCityEagle.Instance.SetPlayerLives(localPlayerActorNumber, lives);
    }

    public bool IsPlayerFreezed()
    {
        return isFreezed;
    }

    public void FreezePlayer()
    {
        StopCoroutine(nameof(FreezePlayerDelayed));
        StartCoroutine(nameof(FreezePlayerDelayed));
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

    public void UpdatePlayerLevelScore(int score)
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(transform.position);
        //}
        //else if (stream.IsReading)
        //{
        //    transform.position = (Vector3)stream.ReceiveNext();
        //}
    }
}
