using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }

    public event EventHandler OnShootAction;

    [SerializeField] private Joystick joystickStick;
    [SerializeField] private Joystick joystickDpad;
    [SerializeField] private GameObject movementButtons;

    private Vector2 inputVector;

    private int playerInputIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var gameButtonController = bool.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_BUTTON_CONTROLLER, "true"));

        if (gameButtonController)
        {
            joystickStick.gameObject.SetActive(true);
            joystickDpad.gameObject.SetActive(false);
        }
        else
        {
            joystickStick.gameObject.SetActive(false);
            joystickDpad.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (joystickStick.gameObject.activeSelf)
        {
            inputVector = joystickStick.Direction;
        }

        if (joystickDpad.gameObject.activeSelf)
        {
            inputVector = joystickDpad.Direction;
        }
    }

    public void Shoot()
    {
        if (!GameManager.Instance.IsGamePaused() && !GameManager.Instance.IsGameOver())
        {
            OnShootAction?.Invoke(this, EventArgs.Empty);
        }
    }

    public void MoveUp()
    {
        inputVector = new Vector2(0, 1);
    }

    public void MoveDown()
    {
        inputVector = new Vector2(0, -1);
    }

    public void MoveLeft()
    {
        inputVector = new Vector2(-1, 0);
    }

    public void MoveRight()
    {
        inputVector = new Vector2(1, 0);
    }

    public void SetMovementVectorNormalized()
    {
        inputVector = new Vector2(0, 0);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        return inputVector.normalized;
    }

    public void SetPlayerInputIndex(int playerInputIndex)
    {
        this.playerInputIndex = playerInputIndex;
    }

    public int GetPlayerInputIndex()
    {
        return playerInputIndex;
    }

    public void RumblePulse(int playerInputIndex, float lowFrequency = 0.25f, float highFrequency = 0.5f, float duration = 0.5f)
    {
        Handheld.Vibrate();
    }
}
