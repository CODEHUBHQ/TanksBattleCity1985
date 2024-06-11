using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;

    [SerializeField] private GameObject movementButtons;
    [SerializeField] private GameObject moveUpButton;
    [SerializeField] private GameObject moveDownButton;
    [SerializeField] private GameObject moveRightButton;
    [SerializeField] private GameObject moveLeftButton;

    [SerializeField] private Image moveUpButtonIconImage;
    [SerializeField] private Image moveDownButtonIconImage;
    [SerializeField] private Image moveRightButtonIconImage;
    [SerializeField] private Image moveLeftButtonIconImage;

    private void Awake()
    {
        moveDownButtonIconImage.color = new Color32(89, 89, 89, 255);
    }

    private void Update()
    {
        if (!movementButtons.activeSelf) return;

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResult = new List<RaycastResult>();

        foreach (Touch touch in Input.touches)
        {
            pointer.position = touch.position;
            EventSystem.current.RaycastAll(pointer, raycastResult);

            if (raycastResult.Count == 0)
            {
                ResetAllButtonsUpIconImageColor();

                playerInputHandler.SetMovementVectorNormalized();

                return;
            }

            if (touch.phase.Equals(TouchPhase.Ended))
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

                    if (touch.phase.Equals(TouchPhase.Began))
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

                    if (touch.phase.Equals(TouchPhase.Began))
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

                    if (touch.phase.Equals(TouchPhase.Began))
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

                    if (touch.phase.Equals(TouchPhase.Began))
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
    }

    private void ResetAllButtonsUpIconImageColor()
    {
        moveUpButtonIconImage.color = new Color32(255, 255, 255, 255);
        moveDownButtonIconImage.color = new Color32(255, 255, 255, 255);
        moveRightButtonIconImage.color = new Color32(255, 255, 255, 255);
        moveLeftButtonIconImage.color = new Color32(255, 255, 255, 255);
    }
}
