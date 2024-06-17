using Photon.Pun;
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
    [SerializeField] private GameObject shootButton;

    private Vector2 inputVector;

    private int playerInputIndex;

    private void Awake()
    {
        Instance = this;

        joystickStick = GameObject.Find("UICanvas").transform.Find("JoystickController").Find("Variable Joystick").GetComponent<Joystick>();
        joystickDpad = GameObject.Find("UICanvas").transform.Find("JoystickController").Find("Variable Joystick DPad").GetComponent<Joystick>();
        movementButtons = GameObject.Find("UICanvas").transform.Find("JoystickController").Find("MovementButtons").gameObject;
        shootButton = GameObject.Find("UICanvas").transform.Find("JoystickController").Find("ShootButton").gameObject;
    }

    private void Start()
    {
        shootButton.gameObject.SetActive(true);

        var joystickType = int.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_BUTTON_CONTROLLER, "0"));

        if (joystickType == 0)
        {
            joystickStick.gameObject.SetActive(true);
            joystickDpad.gameObject.SetActive(false);
            movementButtons.gameObject.SetActive(false);
        }
        else if (joystickType == 1)
        {
            joystickDpad.gameObject.SetActive(true);
            joystickStick.gameObject.SetActive(false);
            movementButtons.gameObject.SetActive(false);
        }
        else if (joystickType == 2)
        {
            movementButtons.gameObject.SetActive(true);
            joystickStick.gameObject.SetActive(false);
            joystickDpad.gameObject.SetActive(false);
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
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
}
