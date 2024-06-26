using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class BattleCityBulletIronDestroy : MonoBehaviour
{
    private Animator bulletAnimator;

    private Transform bulletTransform;
    private Transform ironWall;

    private Transform[] ts;

    private void Awake()
    {
        bulletAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bulletTransform = gameObject.GetComponent<Transform>();
        ironWall = collision.GetComponent<Transform>();

        if (collision.gameObject.name.Contains("Zone"))
        {
            bulletAnimator.SetBool(StaticStrings.HIT, true);

            SoundManager.Instance.PlayBulletIronHitSound();
        }

        if ((ironWall.name.Contains("Iron") || ironWall.name.Contains("Wall")) && !bulletAnimator.GetBool(StaticStrings.HIT))
        {
            bulletAnimator.SetBool(StaticStrings.HIT, true);

            DestroyWallsAccordingToCoordinates(Mathf.Round(bulletTransform.position.x), Mathf.Round(bulletTransform.position.y));
        }

        PlaySound();
    }

    private void PlaySound()
    {
        if (ironWall.name.Contains("Iron"))
        {
            SoundManager.Instance.PlayBulletIronHitSound();
        }

        if (ironWall.name.Contains("Wall"))
        {
            SoundManager.Instance.PlayBulletWallHitSound();
        }
    }

    private void DestroyWallsAccordingToCoordinates(float x, float y)
    {
        ts = BattleCityMapLoad.Instance.GeneratedWallContainer.GetComponentsInChildren<Transform>();

        var inputX = bulletAnimator.GetFloat(StaticStrings.INPUT_X);
        var inputY = bulletAnimator.GetFloat(StaticStrings.INPUT_Y);

        // Horizontal shot
        if (inputY == 0)
        {
            if (inputX == -1)
            {
                x -= 1;
            }

            // If Iron destroys instantly
            ts.GetByNameAndCoords("Iron", x, y).NotNull((t) =>
            {
                Destroy(t.gameObject);
            });

            ts.GetByNameAndCoords("Iron", x, y - 1).NotNull((t) =>
            {
                Destroy(t.gameObject);
            });

            // Walls destroys doubled
            PartiallyDestroy(ts.GetByNameAndCoords("Wall", x, y), bulletAnimator);
            PartiallyDestroy(ts.GetByNameAndCoords("Wall", x, y - 1), bulletAnimator);
        }

        // Vertical shot
        if (inputX == 0)
        {
            if (inputY == -1)
            {
                y -= 1;
            }

            // If Iron destroys instantly
            ts.GetByNameAndCoords("Iron", x, y).NotNull((t) =>
            {
                Destroy(t.gameObject);
            });

            ts.GetByNameAndCoords("Iron", x - 1, y).NotNull((t) =>
            {
                Destroy(t.gameObject);
            });

            // Walls destroys doubled
            PartiallyDestroy(ts.GetByNameAndCoords("Wall", x, y), bulletAnimator);
            PartiallyDestroy(ts.GetByNameAndCoords("Wall", x - 1, y), bulletAnimator);
        }
    }

    private void PartiallyDestroy(Transform obj, Animator bulletAnimator)
    {
        float inputX = bulletAnimator.GetFloat(StaticStrings.INPUT_X);
        float inputY = bulletAnimator.GetFloat(StaticStrings.INPUT_Y);

        obj.NotNull((t) =>
        {
            Animator wallAnimator = t.GetComponent<Animator>();

            float curr = wallAnimator.GetFloat(StaticStrings.LEFT_NUMPAD);

            // Strong shot will allways destroy a piece (and maybe two)
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

            // Horizontal shot
            if (inputX == 1 && curr.IsIn(3, 6, 9))
            {
                obj.position += new Vector3(1, 0, 0);

                BattleCityBulletWallDestroy.PartiallyDestroy(ts.GetByNameAndCoords("Wall", obj.position.x, obj.position.y), bulletAnimator);
            }

            if (inputX == -1 && curr.IsIn(1, 4, 7))
            {
                obj.position += new Vector3(-1, 0, 0);

                BattleCityBulletWallDestroy.PartiallyDestroy(ts.GetByNameAndCoords("Wall", obj.position.x, obj.position.y), bulletAnimator);
            }

            // Vertical shot
            if (inputY == 1 && curr.IsIn(7, 8, 9))
            {
                obj.position += new Vector3(0, 1, 0);

                BattleCityBulletWallDestroy.PartiallyDestroy(ts.GetByNameAndCoords("Wall", obj.position.x, obj.position.y), bulletAnimator);
            }

            if (inputY == -1 && curr.IsIn(1, 2, 3))
            {
                obj.position += new Vector3(0, -1, 0);

                BattleCityBulletWallDestroy.PartiallyDestroy(ts.GetByNameAndCoords("Wall", obj.position.x, obj.position.y), bulletAnimator);
            }
        });
    }
}
