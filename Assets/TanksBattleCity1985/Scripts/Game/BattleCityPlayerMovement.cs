using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleCityPlayerMovement : MonoBehaviour
{
    private BattleCityPlayer battleCityPlayer;

    private Animator animator;

    private PhotonView photonView;

    private float axisX;
    private float axisY;
    private float inputX = 0;
    private float inputY = 1;
    private float lookX = 0;
    private float lookY = 1;

    private bool isMoving;

    private void Awake()
    {
        battleCityPlayer = GetComponent<BattleCityPlayer>();
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (BattleCityMapLoad.Instance.IsLoadingMap()) return;
        if (battleCityPlayer.IsPlayerFreezed()) return;
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer && !photonView.IsMine) return;

        CalculateAxis();

        animator.SetBool(StaticStrings.IS_MOVING, isMoving);
        animator.SetFloat(StaticStrings.INPUT_X, inputX);
        animator.SetFloat(StaticStrings.INPUT_Y, inputY);

        if (animator.GetBool(StaticStrings.HIT))
        {
            animator.SetBool(StaticStrings.IS_MOVING, false);
        }
    }

    private void LateUpdate()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGamePaused()) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (BattleCityMapLoad.Instance.IsLoadingMap()) return;
        if (battleCityPlayer.IsPlayerFreezed()) return;
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer && !photonView.IsMine) return;

        // Do everything only then if not hit
        if (!animator.GetBool(StaticStrings.HIT))
        {
            ChangeInputFromMultipleKeyPresses();
            ActualyChangingCoordinatesAccordingToInput();
            SetLookingDirection();
            ApplyMovementSound();
        }
    }

    private void CalculateAxis()
    {
        if (PlayerInputHandler.Instance != null)
        {
            var inputVectorNormalized = PlayerInputHandler.Instance.GetMovementVectorNormalized();

            axisX = inputVectorNormalized.x;
            axisY = inputVectorNormalized.y;
        }
    }

    private void ChangeInputFromMultipleKeyPresses()
    {
        // Movement changing when pressing keys for both directions
        if (axisX != 0 && axisY != 0)
        {
            if (inputX == 0)
            {
                inputX = axisX;
                inputY = 0;
            }

            if (inputY == 0)
            {
                inputY = axisY;
                inputX = 0;
            }
        }
        else if (axisX != 0 || axisY != 0)
        {
            // If at least one key pressed
            inputX = axisX;
            inputY = axisY;
        }
    }

    private void ActualyChangingCoordinatesAccordingToInput()
    {
        // Movement when pressing a key
        if (axisX != 0 || axisY != 0)
        {
            // Move object
            isMoving = true;
            transform.position += new Vector3(battleCityPlayer.MaxSpeed * inputX, battleCityPlayer.MaxSpeed * inputY, 0);

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
    }

    private void SetLookingDirection()
    {
        lookX = inputX;
        lookY = inputY;
    }

    private void ApplyMovementSound()
    {
        if (battleCityPlayer.LocalPlayerActorNumber == 0 || battleCityPlayer.LocalPlayerActorNumber == 1)
        {
            // Sounds moving and not moving
            if (IsSomethingPressed() && !SoundManager.Instance.IsMovingSoundPlaying())
            {
                SoundManager.Instance.StopNotMovingSound();
                SoundManager.Instance.PlayMovingSound();
            }
            else if (!IsSomethingPressed() && !SoundManager.Instance.IsNotMovingSoundPlaying())
            {
                SoundManager.Instance.StopMovingSound();
                SoundManager.Instance.PlayNotMovingSound();
            }
        }
    }

    private bool IsSomethingPressed()
    {
        if (PlayerInputHandler.Instance == null)
        {
            return false;
        }

        var inputVectorNormalized = PlayerInputHandler.Instance.GetMovementVectorNormalized();

        return inputVectorNormalized.x != 0 || inputVectorNormalized.y != 0;
    }

    public void ResetPosition()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                transform.position = new Vector3(-4, -12, 0);
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                transform.position = new Vector3(4, -12, 0);
            }
        }
        else
        {
            if (battleCityPlayer.LocalPlayerActorNumber == 0)
            {
                transform.position = new Vector3(-4, -12, 0);
            }
            else if (battleCityPlayer.LocalPlayerActorNumber == 1)
            {
                transform.position = new Vector3(4, -12, 0);
            }
        }
    }
}
