using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityEnemy : MonoBehaviour
{
    public int Bonus { get => bonus; }
    public int Lives { get => lives; }

    [SerializeField] private Sprite hitPTSSprite;

    [SerializeField] private float maxSpeed = 0.10f;

    [SerializeField] private int hitPTS;

    private Animator animator;

    private float inputX = 0;
    private float inputY = -1;

    private int bonus = 0;
    private int lives = 1;

    private bool isMoving;
    private bool changingPos;
    private bool isFreezed;

    private System.Random random = new System.Random();

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        animator.SetFloat(StaticStrings.INPUT_X, inputX);
        animator.SetFloat(StaticStrings.INPUT_Y, inputY);
        animator.SetInteger(StaticStrings.BONUS, bonus);
        animator.SetInteger(StaticStrings.LIVES, lives);
        
        if (!isFreezed)
        {
            animator.SetBool(StaticStrings.IS_MOVING, isMoving);
        }
    }

    private void LateUpdate()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        if (!isFreezed && !animator.GetBool(StaticStrings.HIT))
        {
            // AI
            if (!changingPos)
            {
                StartCoroutine(ChangePosition());
            }

            // Movement
            if (inputX != 0 || inputY != 0)
            {
                // Move Object
                isMoving = true;
                transform.position += new Vector3(maxSpeed * inputX, maxSpeed * inputY, 0);

                // Align to cells
                if (inputX == 0)
                {
                    transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, 0);
                }

                if (inputY == 0)
                {
                    transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), 0);
                }
            }
            else
            {
                isMoving = false;
            }

            animator.SetFloat(StaticStrings.INPUT_X, inputX);
            animator.SetFloat(StaticStrings.INPUT_Y, inputY);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        if (transform.position.y < -11.5f && inputY < 0 || transform.position.y > 11.5f && inputY > 0)
        {
            inputX = random.Next(50) % 3 - 1;

            if (inputX == 0)
            {
                inputY = -inputY;
            }
            else
            {
                inputY = 0;
            }
        }
        else if (transform.position.x < -11.5f && inputX < 0 || transform.position.x > 11.5f && inputX > 0)
        {
            inputY = random.Next(50) % 3 - 1;

            if (inputY == 0)
            {
                inputX = -inputX;
            }
            else
            {
                inputX = 0;
            }
        }

        animator.SetFloat(StaticStrings.INPUT_X, inputX);
        animator.SetFloat(StaticStrings.INPUT_Y, inputY);
    }

    private IEnumerator ChangePosition()
    {
        changingPos = true;

        yield return new WaitForSeconds(3f);

        if (!isFreezed)
        {
            SetRandomValues();
        }

        changingPos = false;
    }

    private void SetRandomValues()
    {
        inputX = random.Next(50) % 3 - 1;
        inputY = random.Next(50) % 3 - 1;

        if ((inputX == 0 && inputY == 0) || (inputY != 0 && inputX != 0))
        {
            SetRandomValues();
        }
    }

    public void SetBonus(int bonus)
    {
        this.bonus = bonus;
    }

    public void SetLives(int lives)
    {
        this.lives = lives;
    }

    public void SetIsFreezed(bool isFreezed)
    {
        this.isFreezed = isFreezed;
    }

    public bool IsFreezed()
    {
        return isFreezed;
    }

    public Sprite GetHitPTSSprite()
    {
        return hitPTSSprite;
    }

    public int GetHitPTS()
    {
        return hitPTS;
    }
}
