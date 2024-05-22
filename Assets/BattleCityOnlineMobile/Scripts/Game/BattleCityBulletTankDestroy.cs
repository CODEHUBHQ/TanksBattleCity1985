using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityBulletTankDestroy : MonoBehaviour
{
    [SerializeField] private bool isFriendly;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Animator bulletAnim = gameObject.GetComponent<Animator>();

        Transform tank = collision.GetComponent<Transform>();
        Animator tankAnim = collision.GetComponent<Animator>();


        // Show power up if was red
        if (tank.name.Contains("Tank") && isFriendly && !bulletAnim.GetBool(StaticStrings.HIT) && !tankAnim.GetBool(StaticStrings.HIT))
        {
            BattleCityMapLoad.Instance.PowerUp.GetComponent<BattleCityPowerUp>().ShowPowerUp(tankAnim.GetInteger(StaticStrings.BONUS));

            tank.GetComponent<BattleCityEnemy>().SetBonus(0);

            tankAnim.SetInteger(StaticStrings.BONUS, 0);
        }

        if (collision.name.Contains("Player") && isFriendly && GetComponent<BattleCityBullet>() != null && GetComponent<BattleCityBullet>().GetShooterTank() != null && GetComponent<BattleCityBullet>().GetShooterTank().gameObject != collision.gameObject)
        {
            if (collision.TryGetComponent(out BattleCityPlayer battleCityPlayer))
            {
                battleCityPlayer.FreezePlayer();
            }

            Destroy(gameObject);

            SoundManager.Instance.PlayBulletIronHitSound();
        }

        // Destroy tank and bullet
        if ((tank.name.Contains("Tank") && isFriendly || tank.name.Contains("Player") &&
            !isFriendly) && !bulletAnim.GetBool(StaticStrings.HIT) && !tankAnim.GetBool(StaticStrings.HIT))
        {
            bulletAnim.SetBool(StaticStrings.HIT, true);

            if (!tank.name.Contains("Player") || !tankAnim.GetBool(StaticStrings.SHIELD))
            {
                // player
                if (tank.name.Contains("Player"))
                {
                    tank.GetComponent<BattleCityPlayer>().Hit();

                    tankAnim.SetBool(StaticStrings.HIT, true);

                    SoundManager.Instance.PlayTankDestroySound();
                }
                // not player
                else if (tankAnim.GetInteger(StaticStrings.LIVES) <= 1)
                {
                    var battleCityEnemy = tank.GetComponent<BattleCityEnemy>();

                    tankAnim.SetBool(StaticStrings.HIT, true);

                    if (GetComponent<BattleCityBullet>().GetShooterTank().TryGetComponent(out BattleCityPlayer battleCityPlayer))
                    {
                        battleCityPlayer.UpdatePlayerLevelScore(battleCityEnemy.GetHitPTS());
                    }

                    SoundManager.Instance.PlayTankDestroySound();
                }
                else
                {
                    if (tank.TryGetComponent(out BattleCityEnemy battleCityEnemy))
                    {
                        battleCityEnemy.SetLives(tankAnim.GetInteger(StaticStrings.LIVES) - 1);
                    }

                    SoundManager.Instance.PlayBulletIronHitSound();
                }
            }
        }
    }
}
