using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityPowerUp : MonoBehaviour
{
    public static BattleCityPowerUp Instance { get; private set; }

    private Animator animator;

    private System.Random random;

    private int bonus = 1;
    private int freezeTime = 0;

    private void Awake()
    {
        Instance = this;

        animator = GetComponent<Animator>();

        random = new System.Random();
    }

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;

        animator.SetFloat(StaticStrings.BONUS, bonus);

        var ts = BattleCityMapLoad.Instance.GeneratedEnemyContainer.GetComponentsInChildren<Transform>();

        if (freezeTime > 0)
        {
            foreach (var t in ts)
            {
                if (!t.gameObject.name.Contains("Generated"))
                {
                    t.GetComponent<BattleCityEnemy>().SetIsFreezed(true);
                    t.GetComponent<Animator>().SetBool(StaticStrings.IS_MOVING, false);
                }
            }
        }
    }

    public void Reset()
    {
        transform.position = new Vector3(0, 100, 0);
        freezeTime = -100;
    }

    public void HidePowerUp()
    {
        SoundManager.Instance.PlayPowerUpTakenSound();

        transform.position = new Vector3(0, 100, 0);
    }

    public void ShowPowerUp(int bonus)
    {
        if (bonus > 0)
        {
            this.bonus = bonus;

            SoundManager.Instance.PlayPowerUpShowUpSound();

            var x = GetRandomCoords();
            var y = GetRandomCoords();

            transform.position = new Vector3(x, y, 0);
        }
    }

    public void DestroyAllTanks(BattleCityPlayer battleCityPlayer)
    {
        var ts = BattleCityMapLoad.Instance.GeneratedEnemyContainer.GetComponentsInChildren<Transform>();

        foreach (var t in ts)
        {
            if (!t.gameObject.name.Contains("Generated"))
            {
                if (t.TryGetComponent(out BattleCityEnemy battleCityEnemy))
                {
                    battleCityPlayer.UpdatePlayerLevelScore(battleCityEnemy.GetHitPTS());
                }

                t.GetComponent<Animator>().SetBool(StaticStrings.HIT, true);
            }
        }
    }

    public void FreezeTime()
    {
        if (freezeTime <= 0)
        {
            freezeTime = 15;

            StartCoroutine(FreezeEnumerator());
        }

        freezeTime = 15;
    }

    private IEnumerator FreezeEnumerator()
    {
        while (freezeTime > 0)
        {
            yield return new WaitForSeconds(1f);

            freezeTime--;
        }

        // blink enemy tanks to show warning that freeze will ends
        if (freezeTime <= 5)
        {
            float duration = 4f;
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                foreach (var t in BattleCityMapLoad.Instance.GeneratedEnemyContainer.GetComponentsInChildren<Transform>())
                {
                    if (!t.gameObject.name.Contains("Generated"))
                    {
                        t.GetComponent<SpriteRenderer>().enabled = !t.GetComponent<SpriteRenderer>().enabled;
                    }
                }

                yield return new WaitForSeconds(0.2f);
            }

            // make sure all tanks are visible
            foreach (var t in BattleCityMapLoad.Instance.GeneratedEnemyContainer.GetComponentsInChildren<Transform>())
            {
                if (!t.gameObject.name.Contains("Generated"))
                {
                    t.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }

        if (freezeTime <= 0)
        {
            var ts = BattleCityMapLoad.Instance.GeneratedEnemyContainer.GetComponentsInChildren<Transform>();

            foreach (var t in ts)
            {
                if (!t.gameObject.name.Contains("Generated"))
                {
                    t.GetComponent<BattleCityEnemy>().SetIsFreezed(false);
                    t.GetComponent<Animator>().SetBool(StaticStrings.IS_MOVING, true);
                }
            }
        }
    }

    private float GetRandomCoords()
    {
        return (random.Next(-120, 120) / 10f);
    }
}
