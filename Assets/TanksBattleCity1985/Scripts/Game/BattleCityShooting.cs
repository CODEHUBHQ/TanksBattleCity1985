using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityShooting : MonoBehaviour
{
    [SerializeField] private GameObject bullet;

    [SerializeField] private bool isNPC;

    private PhotonView photonView;

    private Animator animator;

    private BattleCityPlayer battleCityPlayer;
    private BattleCityEnemy battleCityEnemy;

    [SerializeField] private int alreadyShot = 0;
    private int maxBulletsAtOneTime = 1;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        animator = GetComponent<Animator>();

        if (TryGetComponent(out BattleCityPlayer battleCityPlayer))
        {
            this.battleCityPlayer = battleCityPlayer;
        }
        else
        {
            battleCityEnemy = GetComponent<BattleCityEnemy>();
        }
    }

    private void Start()
    {
        if (PlayerInputHandler.Instance != null && battleCityPlayer != null)
        {
            PlayerInputHandler.Instance.OnShootAction += InputHandler_OnShootAction;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;

        if (battleCityEnemy != null && !battleCityEnemy.IsFreezed() && CanShoot() && !animator.GetBool(StaticStrings.HIT))
        {
            if (isNPC)
            {
                alreadyShot++;

                StartCoroutine(DelayShootingFor(0.2f));
            }
        }
    }

    private void OnDestroy()
    {
        if (PlayerInputHandler.Instance != null && battleCityPlayer != null)
        {
            PlayerInputHandler.Instance.OnShootAction += InputHandler_OnShootAction;
        }
    }

    private IEnumerator DelayShootingFor(float time)
    {
        yield return new WaitForSeconds(time);

        if (!animator.GetBool(StaticStrings.HIT))
        {
            LaunchBullet();
        }
    }

    private void InputHandler_OnShootAction(object sender, System.EventArgs e)
    {
        if (CanShoot() && !animator.GetBool(StaticStrings.HIT))
        {
            if ((!isNPC && battleCityPlayer.LocalPlayerActorNumber == 0) || (!isNPC && battleCityPlayer.LocalPlayerActorNumber == 1))
            {
                alreadyShot++;

                LaunchBullet();
            }
        }
    }

    private void LaunchBullet()
    {
        var x = animator.GetFloat(StaticStrings.INPUT_X);
        var y = animator.GetFloat(StaticStrings.INPUT_Y);

        // Calculate rotation angle
        var r = 0f;

        if (x == 0 && y == 1)
        {
            r = 270f;
        }

        if (x == 1 && y == 0)
        {
            r = 180f;
        }

        if (x == 0 && y == -1)
        {
            r = 90f;
        }

        if (x == -1 && y == 0)
        {
            r = 0f;
        }

        // Creates new bullet
        //var pos = transform.position + new Vector3(x, y, 0);
        var pos = transform.position;
        GameObject newBullet;

        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            photonView.RPC(nameof(LaunchBulletByMasterRPC), RpcTarget.MasterClient, bullet.name, pos, transform.rotation, r, x, y);
            //newBullet = PhotonNetwork.Instantiate(bullet.name, pos, transform.rotation, 0);
        }
        else
        {
            newBullet = Instantiate(bullet, pos, transform.rotation);
            newBullet.transform.parent = BattleCityMapLoad.Instance.GeneratedBulletContainer;
            newBullet.transform.eulerAngles += new Vector3(0, 0, r);

            // Passes variables x and y
            var newBulletAnimator = newBullet.GetComponent<Animator>();

            newBulletAnimator.SetFloat(StaticStrings.INPUT_X, x);
            newBulletAnimator.SetFloat(StaticStrings.INPUT_Y, y);

            if (newBulletAnimator.TryGetComponent(out BattleCityBullet battleCityBullet))
            {
                battleCityBullet.SetShooterTank(transform);
            }
        }

        // play a sound
        if (!isNPC)
        {
            SoundManager.Instance.PlayShotSound();
        }
    }

    private bool CanShoot()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            return maxBulletsAtOneTime > alreadyShot && photonView.IsMine;
        }
        else
        {
            return maxBulletsAtOneTime > alreadyShot;
        }
    }

    public void SetShooting(bool shouldAddBullet)
    {
        if (shouldAddBullet)
        {
            alreadyShot++;
        }
        else
        {
            alreadyShot--;
        }

        if (alreadyShot < 0)
        {
            alreadyShot = 0;
        }

        if (alreadyShot > maxBulletsAtOneTime)
        {
            alreadyShot = maxBulletsAtOneTime;
        }
    }

    public void SetBullet(Transform bullet)
    {
        this.bullet = bullet.gameObject;
    }

    public void SetMaxBullets(int max)
    {
        maxBulletsAtOneTime = max;
    }

    public void Destroy()
    {
        if (isNPC)
        {
            battleCityEnemy.GetComponent<Animator>().enabled = false;
            battleCityEnemy.GetComponent<SpriteRenderer>().sprite = battleCityEnemy.GetHitPTSSprite();
            battleCityEnemy.GetComponent<BoxCollider2D>().enabled = false;

            this.DoAfter(1f, () =>
            {
                if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            });
        }
        else if (!isNPC)
        {
            transform.GetComponent<BoxCollider2D>().enabled = false;
            transform.position = new Vector3(120, 20, 0);

            if (transform.TryGetComponent(out BattleCityPlayer battleCityPlayer))
            {
                battleCityPlayer.SetIsHit(false);

                if (SoundManager.Instance.IsVibrateEnabled())
                {
                    PlayerInputHandler.Instance.RumblePulse(battleCityPlayer.LocalPlayerActorNumber);
                }

                battleCityPlayer.SetLevel(1);

                var lives = battleCityPlayer.Lives;

                if (lives <= 0)
                {
                    transform.GetComponent<BattleCityPlayer>().SetLives(0);

                    var battleCityPlayers = FindObjectsByType<BattleCityPlayer>(FindObjectsSortMode.None);

                    var allLives = 0;

                    foreach (var bcp in battleCityPlayers)
                    {
                        if (bcp.Lives > 0)
                        {
                            allLives = bcp.Lives;
                        }
                    }

                    if (allLives == 0)
                    {
                        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                        {
                            photonView.RPC(nameof(GameOverPunRPC), RpcTarget.All);
                        }
                        else
                        {
                            GameManager.Instance.SetIsGameOver(true);

                            StartCoroutine(FinishGameAfter(3f));
                        }
                    }
                }
                else
                {
                    this.DoAfter(1.5f, () =>
                    {
                        if (transform.TryGetComponent(out BattleCityPlayerMovement battleCityPlayerMovement))
                        {
                            battleCityPlayerMovement.ResetPosition();
                        }

                        transform.GetComponent<BoxCollider2D>().enabled = true;
                        transform.GetComponent<Animator>().SetBool(StaticStrings.HIT, false);

                        alreadyShot = 0;

                        battleCityPlayer.SetShield(6);
                    });
                }
            }
        }
    }

    private IEnumerator FinishGameAfter(float time)
    {
        yield return new WaitForSeconds(time / 3);

        SoundManager.Instance.PlayGameOverSound();

        yield return new WaitForSeconds(time / 3 * 2);

        animator.SetBool(StaticStrings.HIT, false);

        BattleCityEagle.Instance.FinishGame();
    }

    [PunRPC]
    public void GameOverPunRPC()
    {
        GameManager.Instance.SetIsGameOver(true);

        StartCoroutine(FinishGameAfter(3f));
    }

    [PunRPC]
    public void LaunchBulletByMasterRPC(string bulletName, Vector3 pos, Quaternion rotation, float r, float x, float y)
    {
        var newBullet = PhotonNetwork.Instantiate(bulletName, pos, rotation, 0);
        var newBulletPhotonViewID = newBullet.GetComponent<PhotonView>().ViewID;

        photonView.RPC(nameof(ParentSpawnedBullets), RpcTarget.All, newBulletPhotonViewID);

        newBullet.transform.eulerAngles += new Vector3(0, 0, r);

        // Passes variables x and y
        var newBulletAnimator = newBullet.GetComponent<Animator>();

        newBulletAnimator.SetFloat(StaticStrings.INPUT_X, x);
        newBulletAnimator.SetFloat(StaticStrings.INPUT_Y, y);

        if (newBulletAnimator.TryGetComponent(out BattleCityBullet battleCityBullet))
        {
            battleCityBullet.SetShooterTank(transform);
        }
    }

    [PunRPC]
    public void ParentSpawnedBullets(int newBulletPhotonViewID)
    {
        var newBulletGameObject = PhotonView.Find(newBulletPhotonViewID);

        if (newBulletGameObject != null)
        {
            newBulletGameObject.transform.parent = BattleCityMapLoad.Instance.GeneratedBulletContainer;
        }
    }

    [PunRPC]
    public void SetShootingPunRPC(int photonViewID)
    {
        var go = PhotonView.Find(photonViewID);

        if (go != null)
        {
            if (go.gameObject.TryGetComponent(out BattleCityShooting battleCityShooting))
            {
                battleCityShooting.SetShooting(false);
            }
        }
    }
}
