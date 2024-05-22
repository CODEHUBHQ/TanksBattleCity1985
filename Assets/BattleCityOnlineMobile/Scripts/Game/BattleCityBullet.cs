using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityBullet : MonoBehaviour
{
    [SerializeField] private float speed;

    private PhotonView photonView;

    private Animator animator;

    private Transform shooterTank;

    private float inputX;
    private float inputY;

    private bool isFriendly;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        inputX = animator.GetFloat(StaticStrings.INPUT_X);
        inputY = animator.GetFloat(StaticStrings.INPUT_Y);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer && !photonView.IsMine) return;

        var hit = animator.GetBool(StaticStrings.HIT);

        if (!hit)
        {
            transform.position += new Vector3(speed * inputX, speed * inputY, 0);
        }
    }

    private void OnDestroy()
    {
        if (shooterTank != null)
        {
            shooterTank.NotNull((t) =>
            {
                if (t.gameObject.TryGetComponent(out BattleCityShooting battleCityShooting))
                {
                    battleCityShooting.SetShooting(false);
                }
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.GetComponent<Transform>();

        if (other.name.Contains("Bullet"))
        {
            if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
            {
                photonView.RPC(nameof(DestroyOnTriggerEnter2DRPC), RpcTarget.MasterClient, collision);
            }
            else
            {
                Destroy(gameObject);
                Destroy(other.gameObject);
            }
        }
    }

    // called by animation event
    public void DestroyAfterAnimationFinishes()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            photonView.RPC(nameof(DestroyAfterAnimationFinishesRPC), RpcTarget.MasterClient);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetShooterTank(Transform shooterTank)
    {
        this.shooterTank = shooterTank;
    }

    public Transform GetShooterTank()
    {
        return shooterTank;
    }

    public void PlayBrickHitSound()
    {
        if (isFriendly)
        {
            SoundManager.Instance.PlayBulletWallHitSound();
        }
    }

    public void PlayIronHitSound()
    {
        if (isFriendly)
        {
            SoundManager.Instance.PlayBulletIronHitSound();
        }
    }

    [PunRPC]
    public void DestroyAfterAnimationFinishesRPC()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void DestroyOnTriggerEnter2DRPC(Collider2D collision)
    {
        Debug.Log($"DestroyOnTriggerEnter2DRPC collision: {collision}");

        var other = collision.GetComponent<Transform>();

        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.Destroy(other.gameObject);
    }
}
