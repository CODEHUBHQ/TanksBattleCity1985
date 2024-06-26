using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityBullet : MonoBehaviour, IPunObservable
{
    [SerializeField] private float speed;

    private PhotonView photonView;

    private Animator animator;

    [SerializeField] private Transform shooterTank;

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
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        var pv = t.GetComponent<PhotonView>();
                        var photonViewID = pv.ViewID;

                        if (pv.IsOwnerActive && PhotonNetwork.IsConnected)
                        {
                            pv.RPC("SetShootingPunRPC", RpcTarget.All, photonViewID);
                        }
                    }
                    else
                    {
                        battleCityShooting.SetShooting(false);
                    }
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
                var otherPhotonViewID = other.GetComponent<PhotonView>().ViewID;
                var goPhotonViewID = GetComponent<PhotonView>().ViewID;

                photonView.RPC(nameof(DestroyOnTriggerEnter2DRPC), RpcTarget.MasterClient, otherPhotonViewID, goPhotonViewID);
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
            //var photonViewID = photonView.ViewID;

            //photonView.RPC(nameof(DestroyAfterAnimationFinishesRPC), RpcTarget.MasterClient, photonViewID);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
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
    public void DestroyAfterAnimationFinishesRPC(int photonViewID)
    {
        var go = PhotonView.Find(photonViewID);

        if (go != null)
        {
            PhotonNetwork.Destroy(go.gameObject);
        }
    }

    [PunRPC]
    public void DestroyOnTriggerEnter2DRPC(int otherPhotonViewID, int goPhotonViewID)
    {
        var otherGameObject = PhotonView.Find(otherPhotonViewID);
        var go = PhotonView.Find(goPhotonViewID);

        if (go != null)
        {
            PhotonNetwork.Destroy(go.gameObject);
        }

        if (otherGameObject != null)
        {
            PhotonNetwork.Destroy(otherGameObject.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(transform.position);
        //    stream.SendNext(transform.rotation);
        //}
        //else if (stream.IsReading)
        //{
        //    transform.position = (Vector3)stream.ReceiveNext();
        //    transform.rotation = (Quaternion)stream.ReceiveNext();
        //}
    }
}
