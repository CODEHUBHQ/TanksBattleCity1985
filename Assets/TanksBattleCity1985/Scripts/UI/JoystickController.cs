using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickController : MonoBehaviour
{
    public static JoystickController Instance { get; private set; }

    [SerializeField] private GameObject movementButtons;
    [SerializeField] private GameObject moveUpButton;
    [SerializeField] private GameObject moveDownButton;
    [SerializeField] private GameObject moveRightButton;
    [SerializeField] private GameObject moveLeftButton;

    [SerializeField] private Image moveUpButtonIconImage;
    [SerializeField] private Image moveDownButtonIconImage;
    [SerializeField] private Image moveRightButtonIconImage;
    [SerializeField] private Image moveLeftButtonIconImage;

    private PlayerInputHandler playerInputHandler;

    private Button shootButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        if (playerInputHandler == null)
        {
            playerInputHandler = FindObjectOfType<PlayerInputHandler>();

            shootButton = GameObject.Find("UICanvas").transform.Find("JoystickController").Find("ShootButton").GetComponent<Button>();

            shootButton.GetComponent<ButtonLongPress>().OnLongPressDown.AddListener(() =>
            {
                playerInputHandler.Shoot();
            });
        }

        if (playerInputHandler == null) return;

        if (!movementButtons.activeSelf) return;

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResult = new List<RaycastResult>();

        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            foreach (UnityEngine.InputSystem.EnhancedTouch.Touch touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
            {
                HandleMovement(pointer, raycastResult, touch.screenPosition);
            }
        }
        else
        {
            HandleMovement(pointer, raycastResult, BattleCityUtils.GetMousePosition());
        }
    }

    private void HandleMovement(PointerEventData pointer, List<RaycastResult> raycastResult, Vector2 pos)
    {
        pointer.position = pos;
        EventSystem.current.RaycastAll(pointer, raycastResult);

        if (raycastResult.Count == 0)
        {
            ResetAllButtonsUpIconImageColor();

            playerInputHandler.SetMovementVectorNormalized();

            return;
        }

        if (BattleCityUtils.GetMouseButtonUp())
        {
            raycastResult.Clear();

            ResetAllButtonsUpIconImageColor();

            playerInputHandler.SetMovementVectorNormalized();

            return;
        }

        foreach (RaycastResult result in raycastResult)
        {
            if (result.gameObject.Equals(moveUpButton))
            {
                playerInputHandler.MoveUp();

                if (BattleCityUtils.GetMouseButtonDown())
                {
                    moveUpButtonIconImage.color = new Color32(89, 89, 89, 255);
                    moveDownButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveRightButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveLeftButtonIconImage.color = new Color32(255, 255, 255, 255);
                }
            }

            if (result.gameObject.Equals(moveDownButton))
            {
                playerInputHandler.MoveDown();

                if (BattleCityUtils.GetMouseButtonDown())
                {
                    moveUpButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveDownButtonIconImage.color = new Color32(89, 89, 89, 255);
                    moveRightButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveLeftButtonIconImage.color = new Color32(255, 255, 255, 255);
                }
            }

            if (result.gameObject.Equals(moveRightButton))
            {
                playerInputHandler.MoveRight();

                if (BattleCityUtils.GetMouseButtonDown())
                {
                    moveUpButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveDownButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveRightButtonIconImage.color = new Color32(89, 89, 89, 255);
                    moveLeftButtonIconImage.color = new Color32(255, 255, 255, 255);
                }
            }

            if (result.gameObject.Equals(moveLeftButton))
            {
                playerInputHandler.MoveLeft();

                if (BattleCityUtils.GetMouseButtonDown())
                {
                    moveUpButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveDownButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveRightButtonIconImage.color = new Color32(255, 255, 255, 255);
                    moveLeftButtonIconImage.color = new Color32(89, 89, 89, 255);
                }
            }
        }

        raycastResult.Clear();
    }

    private void ResetAllButtonsUpIconImageColor()
    {
        moveUpButtonIconImage.color = new Color32(255, 255, 255, 255);
        moveDownButtonIconImage.color = new Color32(255, 255, 255, 255);
        moveRightButtonIconImage.color = new Color32(255, 255, 255, 255);
        moveLeftButtonIconImage.color = new Color32(255, 255, 255, 255);
    }

    public void DisableJoystickController()
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.SetActive(false);
        }
    }
}
