using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityBulletWallDestroy : MonoBehaviour
{
    private Transform bulletTransform;
    private Transform wallTransform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var bulletAnimator = gameObject.GetComponent<Animator>();

        bulletTransform = gameObject.GetComponent<Transform>();
        wallTransform = collision.GetComponent<Transform>();

        if (collision.gameObject.name.Contains("Zone"))
        {
            bulletAnimator.SetBool(StaticStrings.HIT, true);

            SoundManager.Instance.PlayBulletIronHitSound();
        }

        if ((wallTransform.name.Contains("Wall") || wallTransform.name.Contains("Iron")) && !bulletAnimator.GetBool(StaticStrings.HIT))
        {
            bulletAnimator.SetBool(StaticStrings.HIT, true);

            DestroyWallsAccordingToCoordinates(Mathf.Round(bulletTransform.position.x), Mathf.Round(bulletTransform.position.y), wallTransform);

            if (wallTransform.name.Contains("Wall"))
            {
                SoundManager.Instance.PlayBulletWallHitSound();
            }

            if (wallTransform.name.Contains("Iron"))
            {
                SoundManager.Instance.PlayBulletIronHitSound();
            }
        }
    }

    private void DestroyWallsAccordingToCoordinates(float x, float y, Transform wallTransform)
    {
        var ts = BattleCityMapLoad.Instance.GeneratedWallContainer.GetComponentsInChildren<Transform>();
        var bulletAnimator = gameObject.GetComponent<Animator>();

        var inputX = bulletAnimator.GetFloat(StaticStrings.INPUT_X);
        var inputY = bulletAnimator.GetFloat(StaticStrings.INPUT_Y);

        // Horizontal shot
        if (inputY == 0)
        {
            if (inputX == -1)
            {
                x -= 1;
            }

            Transform wallPart1 = null;

            wallPart1 = ts.GetByNameAndCoords("Wall", x, y);

            if (wallPart1 == null)
            {
                wallPart1 = ts.GetByNameAndCoords("Wall", wallTransform.position.x, wallTransform.position.y);
            }

            Transform wallPart2 = null;

            wallPart2 = ts.GetByNameAndCoords("Wall", x, y - 1);

            if (wallPart2 == null)
            {
                wallPart2 = ts.GetByNameAndCoords("Wall", wallTransform.position.x, wallTransform.position.y - 1);
            }

            //PartiallyDestroy(ts.GetByNameAndCoords("Wall", x, y), bulletAnimator);
            //PartiallyDestroy(ts.GetByNameAndCoords("Wall", x, y - 1), bulletAnimator);
            PartiallyDestroy(wallPart1, bulletAnimator);
            PartiallyDestroy(wallPart2, bulletAnimator);
        }

        // Vertical shot
        if (inputX == 0)
        {
            if (inputY == -1)
            {
                y -= 1;
            }

            Transform wallPart1 = null;

            wallPart1 = ts.GetByNameAndCoords("Wall", x, y);

            if (wallPart1 == null)
            {
                wallPart1 = ts.GetByNameAndCoords("Wall", wallTransform.position.x, wallTransform.position.y);
            }

            Transform wallPart2 = null;

            wallPart2 = ts.GetByNameAndCoords("Wall", x - 1, y);

            if (wallPart2 == null)
            {
                wallPart2 = ts.GetByNameAndCoords("Wall", wallTransform.position.x - 1, wallTransform.position.y);
            }

            //PartiallyDestroy(ts.GetByNameAndCoords("Wall", x, y), bulletAnimator);
            //PartiallyDestroy(ts.GetByNameAndCoords("Wall", x - 1, y), bulletAnimator);
            PartiallyDestroy(wallPart1, bulletAnimator);
            PartiallyDestroy(wallPart2, bulletAnimator);
        }
    }

    public static void PartiallyDestroy(Transform obj, Animator animator)
    {
        var inputX = animator.GetFloat(StaticStrings.INPUT_X);
        var inputY = animator.GetFloat(StaticStrings.INPUT_Y);

        obj.NotNull((t) =>
        {
            var wallAnimator = t.GetComponent<Animator>();
            var curr = wallAnimator.GetFloat(StaticStrings.LEFT_NUMPAD);
            var boxCollider2D = t.GetComponent<BoxCollider2D>();

            // The tyniest piece of wall left
            if (curr.IsIn(1, 3, 7, 9))
            {
                if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(t.gameObject);
                    }
                }
                else
                {
                    Destroy(t.gameObject);
                }
            }
            // Vertical shot
            else if (inputX == 0)
            {
                // re-calculate collider size
                if (inputY == -1)
                {
                    boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y - 0.25f);
                    boxCollider2D.size = new Vector2(boxCollider2D.size.x, boxCollider2D.size.y - 0.5f);
                }
                else
                {
                    boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y + 0.25f);
                    boxCollider2D.size = new Vector2(boxCollider2D.size.x, boxCollider2D.size.y - 0.5f);
                }

                if (curr.IsIn(2, 8))
                {
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.Destroy(t.gameObject);
                        }
                    }
                    else
                    {
                        Destroy(t.gameObject);
                    }
                }
                else if (curr.IsIn(4, 5, 6))
                {
                    wallAnimator.SetFloat(StaticStrings.LEFT_NUMPAD, curr + inputY * 3);
                }
            }
            // Horizontal shot
            else if (inputY == 0)
            {
                // re-calculate collider size
                if (inputX == -1)
                {
                    boxCollider2D.offset = new Vector2(boxCollider2D.offset.x - 0.25f, boxCollider2D.offset.y);
                    boxCollider2D.size = new Vector2(boxCollider2D.size.x - 0.5f, boxCollider2D.size.y);
                }
                else
                {
                    boxCollider2D.offset = new Vector2(boxCollider2D.offset.x + 0.25f, boxCollider2D.offset.y);
                    boxCollider2D.size = new Vector2(boxCollider2D.size.x - 0.5f, boxCollider2D.size.y);
                }

                if (curr.IsIn(4, 6))
                {
                    if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.Destroy(t.gameObject);
                        }
                    }
                    else
                    {
                        Destroy(t.gameObject);
                    }
                }
                else if (curr.IsIn(2, 5, 8))
                {
                    wallAnimator.SetFloat(StaticStrings.LEFT_NUMPAD, curr + inputX);
                }
            }
        });
    }
}
