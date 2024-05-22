using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public event EventHandler OnShootAction;
    public event EventHandler OnPauseAction;

    public int PlayerInputIndex { get => playerInputIndex; }

    private Vector2 inputVector;

    private int playerInputIndex;

    public void Shoot_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!GameManager.Instance.IsGamePaused() && !GameManager.Instance.IsGameOver())
            {
                OnShootAction?.Invoke(this, EventArgs.Empty);
            }
        }

        //PlayerInputHandler.Instance.RumblePulse(playerInputIndex);
    }

    public void Pause_performed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPauseAction?.Invoke(this, EventArgs.Empty);

            if (GameManager.Instance.IsGamePaused() && ControllerSelectionUI.Instance.gameObject.activeSelf)
            {
                ControllerSelectionUI.Instance.StartGame();
            }
            else
            {
                if (!GameManager.Instance.IsGamePaused())
                {
                    SoundManager.Instance.PlayPauseSound();
                }

                GameManager.Instance.ToggleGameIsPaused();
                GameMenuUI.Instance.ToggleGameMenu();
            }
        }
    }

    public void SetMovementVectorNormalized(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>().normalized;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        return inputVector;
    }

    public void SetPlayerInputIndex(int playerInputIndex)
    {
        this.playerInputIndex = playerInputIndex;
    }

    public int GetPlayerInputIndex()
    {
        return playerInputIndex;
    }
}
