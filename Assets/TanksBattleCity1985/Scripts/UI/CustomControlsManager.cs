using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomControlsManager : MonoBehaviour
{
    public static CustomControlsManager Instance { get; private set; }

    [Header("Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button resetButton;

    [Header("Controls")]
    [SerializeField] private GameObject joystickStick;
    [SerializeField] private GameObject joystickDpad;
    [SerializeField] private GameObject dPadButtons;
    [SerializeField] private GameObject shootButton;
    [SerializeField] private GameObject scaleIndicator;

    [Header("Panels")]
    [SerializeField] private Transform customControlsPanel;
    [SerializeField] private Transform gameMenuPanel;

    [SerializeField] private Color32 notSelectedColor = new Color32(212, 214, 248, 255);
    [SerializeField] private Color32 selectedColor = new Color32(109, 114, 237, 255);

    private GameObject shootInstance;
    private GameObject joystickInstance;
    private GameObject scaleIndicatorInstance;
    private GameObject scaleIndicatorShootInstance;

    private float shootButtonPosX;
    private float shootButtonPosY;
    private float joystickStickPosX;
    private float joystickStickPosY;
    private float joystickDpadPosX;
    private float joystickDpadPosY;
    private float dPadButtonsPosX;
    private float dPadButtonsPosY;

    private int joystickType;

    private bool isActive;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        joystickType = int.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_BUTTON_CONTROLLER, "0"));

        UpdateControls();

        saveButton.onClick.AddListener(OnSaveButtonClicked);
        resetButton.onClick.AddListener(OnResetButtonClicked);
    }

    private void Update()
    {
        if (isActive)
        {
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
    }

    private void HandleMovement(PointerEventData pointer, List<RaycastResult> raycastResult, Vector2 pos)
    {
        pointer.position = pos;
        EventSystem.current.RaycastAll(pointer, raycastResult);

        if (raycastResult.Count == 0)
        {
            return;
        }

        if (BattleCityUtils.GetMouseButtonUp())
        {
            raycastResult.Clear();

            return;
        }

        foreach (RaycastResult result in raycastResult)
        {
            if (result.gameObject.Equals(scaleIndicatorShootInstance))
            {
                if (BattleCityUtils.GetMouseButton())
                {
                    scaleIndicatorShootInstance.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                    scaleIndicatorShootInstance.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y);

                    var size = scaleIndicatorShootInstance.GetComponent<RectTransform>().sizeDelta;
                    var anchoredPositionX = scaleIndicatorShootInstance.GetComponent<RectTransform>().anchoredPosition.x;
                    var anchoredPositionY = scaleIndicatorShootInstance.GetComponent<RectTransform>().anchoredPosition.y;

                    shootButtonPosX = anchoredPositionX > 0 ? (anchoredPositionX - size.x / 2) : (anchoredPositionX + size.x / 2);
                    shootButtonPosY = anchoredPositionY > 0 ? (anchoredPositionY - size.y / 2) : (anchoredPositionY + size.y / 2);
                }
            }

            if (result.gameObject.Equals(scaleIndicatorInstance))
            {
                if (BattleCityUtils.GetMouseButton())
                {
                    scaleIndicatorInstance.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                    scaleIndicatorInstance.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y);

                    if (joystickType == 0)
                    {
                        var size = scaleIndicatorInstance.GetComponent<RectTransform>().sizeDelta;
                        var anchoredPositionX = scaleIndicatorInstance.GetComponent<RectTransform>().anchoredPosition.x;
                        var anchoredPositionY = scaleIndicatorInstance.GetComponent<RectTransform>().anchoredPosition.y;

                        joystickStickPosX = anchoredPositionX > 0 ? (anchoredPositionX - size.x / 2) : (anchoredPositionX + size.x / 2);
                        joystickStickPosY = anchoredPositionY > 0 ? (anchoredPositionY - size.y / 2) : (anchoredPositionY + size.y / 2);
                    }
                    else if (joystickType == 1)
                    {
                        var size = scaleIndicatorInstance.GetComponent<RectTransform>().sizeDelta;
                        var anchoredPositionX = scaleIndicatorInstance.GetComponent<RectTransform>().anchoredPosition.x;
                        var anchoredPositionY = scaleIndicatorInstance.GetComponent<RectTransform>().anchoredPosition.y;

                        joystickDpadPosX = anchoredPositionX > 0 ? (anchoredPositionX - size.x / 2) : (anchoredPositionX + size.x / 2);
                        joystickDpadPosY = anchoredPositionY > 0 ? (anchoredPositionY - size.y / 2) : (anchoredPositionY + size.y / 2);
                    }
                    else if (joystickType == 2)
                    {
                        var size = scaleIndicatorInstance.GetComponent<RectTransform>().sizeDelta;
                        var anchoredPositionX = scaleIndicatorInstance.GetComponent<RectTransform>().anchoredPosition.x;
                        var anchoredPositionY = scaleIndicatorInstance.GetComponent<RectTransform>().anchoredPosition.y;

                        dPadButtonsPosX = anchoredPositionX > 0 ? (anchoredPositionX - size.x / 2) : (anchoredPositionX + size.x / 2);
                        dPadButtonsPosY = anchoredPositionY > 0 ? (anchoredPositionY - size.y / 2) : (anchoredPositionY + size.y / 2);
                    }
                }
            }
        }

        raycastResult.Clear();
    }

    private void CopyTransformValues(RectTransform from, RectTransform to)
    {
        to.anchorMin = from.anchorMin;
        to.anchorMax = from.anchorMax;
        to.sizeDelta = from.sizeDelta;
        to.pivot = from.pivot;
        to.anchoredPosition = from.anchoredPosition;
    }

    private void ResetTransformValues(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, 0);
    }

    private void UpdateControls()
    {
        shootButtonPosX = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_SHOOT_BUTTON_POSITION_X, "-100"));
        shootButtonPosY = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_SHOOT_BUTTON_POSITION_Y, "100"));

        shootButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(shootButtonPosX, shootButtonPosY);

        if (joystickType == 0)
        {
            joystickStickPosX = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_JOYSTICK_STICK_POSITION_X, "50"));
            joystickStickPosY = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_JOYSTICK_STICK_POSITION_Y, "200"));

            joystickStick.GetComponent<RectTransform>().anchoredPosition = new Vector2(joystickStickPosX, joystickStickPosY);
        }
        else if (joystickType == 1)
        {
            joystickDpadPosX = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_JOYSTICK_DPAD_POSITION_X, "50"));
            joystickDpadPosY = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_JOYSTICK_DPAD_POSITION_Y, "200"));

            joystickDpad.GetComponent<RectTransform>().anchoredPosition = new Vector2(joystickDpadPosX, joystickDpadPosY);
        }
        else if (joystickType == 2)
        {
            dPadButtonsPosX = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_DPAD_BUTTONS_POSITION_X, "150"));
            dPadButtonsPosY = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_DPAD_BUTTONS_POSITION_Y, "50"));

            dPadButtons.GetComponent<RectTransform>().anchoredPosition = new Vector2(dPadButtonsPosX, dPadButtonsPosY);
        }
    }

    private void CleanUp()
    {
        shootButton.SetActive(true);

        Destroy(shootInstance);
        Destroy(scaleIndicatorInstance);
        Destroy(scaleIndicatorShootInstance);

        if (joystickType == 0)
        {
            joystickStick.SetActive(true);

            Destroy(joystickInstance);
        }
        else if (joystickType == 1)
        {
            joystickDpad.SetActive(true);

            Destroy(joystickInstance);
        }
        else if (joystickType == 2)
        {
            dPadButtons.SetActive(true);

            Destroy(joystickInstance);
        }
    }

    public void Init()
    {
        joystickType = int.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_BUTTON_CONTROLLER, "0"));

        shootButtonPosX = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_SHOOT_BUTTON_POSITION_X, "-100"));
        shootButtonPosY = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_SHOOT_BUTTON_POSITION_Y, "100"));

        shootButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(shootButtonPosX, shootButtonPosY);

        shootInstance = Instantiate(shootButton, customControlsPanel);

        shootButton.SetActive(false);
        shootInstance.SetActive(true);

        CopyTransformValues(shootButton.GetComponent<RectTransform>(), shootInstance.GetComponent<RectTransform>());

        scaleIndicatorShootInstance = Instantiate(scaleIndicator, shootInstance.transform);
        scaleIndicatorShootInstance.transform.SetParent(customControlsPanel);
        shootInstance.transform.SetParent(scaleIndicatorShootInstance.transform);
        shootInstance.transform.SetAsFirstSibling();

        ResetTransformValues(shootInstance.GetComponent<RectTransform>());

        CopyTransformValues(shootButton.GetComponent<RectTransform>(), scaleIndicatorShootInstance.GetComponent<RectTransform>());

        if (joystickType == 0)
        {
            joystickStickPosX = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_JOYSTICK_STICK_POSITION_X, "50"));
            joystickStickPosY = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_JOYSTICK_STICK_POSITION_Y, "200"));

            joystickStick.GetComponent<RectTransform>().anchoredPosition = new Vector3(joystickStickPosX, joystickStickPosY);

            joystickInstance = Instantiate(joystickStick, customControlsPanel);

            joystickStick.SetActive(false);

            joystickInstance.GetComponent<VariableJoystick>().enabled = false;

            joystickInstance.SetActive(true);

            CopyTransformValues(joystickStick.GetComponent<RectTransform>(), joystickInstance.GetComponent<RectTransform>());

            scaleIndicatorInstance = Instantiate(scaleIndicator, joystickInstance.transform);
            scaleIndicatorInstance.transform.SetParent(customControlsPanel);
            joystickInstance.transform.SetParent(scaleIndicatorInstance.transform);
            joystickInstance.transform.SetAsFirstSibling();

            ResetTransformValues(joystickInstance.GetComponent<RectTransform>());

            CopyTransformValues(joystickStick.GetComponent<RectTransform>(), scaleIndicatorInstance.GetComponent<RectTransform>());
        }
        else if (joystickType == 1)
        {
            joystickDpadPosX = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_JOYSTICK_DPAD_POSITION_X, "50"));
            joystickDpadPosY = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_JOYSTICK_DPAD_POSITION_Y, "200"));

            joystickDpad.GetComponent<RectTransform>().anchoredPosition = new Vector3(joystickDpadPosX, joystickDpadPosY);

            joystickInstance = Instantiate(joystickDpad, customControlsPanel);

            joystickDpad.SetActive(false);

            joystickInstance.GetComponent<VariableJoystick>().enabled = false;

            joystickInstance.SetActive(true);

            CopyTransformValues(joystickDpad.GetComponent<RectTransform>(), joystickInstance.GetComponent<RectTransform>());

            scaleIndicatorInstance = Instantiate(scaleIndicator, joystickInstance.transform);
            scaleIndicatorInstance.transform.SetParent(customControlsPanel);
            joystickInstance.transform.SetParent(scaleIndicatorInstance.transform);
            joystickInstance.transform.SetAsFirstSibling();

            ResetTransformValues(joystickInstance.GetComponent<RectTransform>());

            CopyTransformValues(joystickDpad.GetComponent<RectTransform>(), scaleIndicatorInstance.GetComponent<RectTransform>());
        }
        else if (joystickType == 2)
        {
            dPadButtonsPosX = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_DPAD_BUTTONS_POSITION_X, "150"));
            dPadButtonsPosY = float.Parse(PlayerPrefs.GetString(StaticStrings.CONTROLS_DPAD_BUTTONS_POSITION_Y, "50"));

            dPadButtons.GetComponent<RectTransform>().anchoredPosition = new Vector3(dPadButtonsPosX, dPadButtonsPosY);

            joystickInstance = Instantiate(dPadButtons, customControlsPanel);

            dPadButtons.SetActive(false);
            joystickInstance.SetActive(true);

            CopyTransformValues(dPadButtons.GetComponent<RectTransform>(), joystickInstance.GetComponent<RectTransform>());

            scaleIndicatorInstance = Instantiate(scaleIndicator, joystickInstance.transform);
            scaleIndicatorInstance.transform.SetParent(customControlsPanel);
            joystickInstance.transform.SetParent(scaleIndicatorInstance.transform);
            joystickInstance.transform.SetAsFirstSibling();

            ResetTransformValues(joystickInstance.GetComponent<RectTransform>());

            CopyTransformValues(dPadButtons.GetComponent<RectTransform>(), scaleIndicatorInstance.GetComponent<RectTransform>());
        }

        isActive = true;
    }

    public void OnSaveButtonClicked()
    {
        if (joystickType == 0)
        {
            PlayerPrefs.SetString(StaticStrings.CONTROLS_JOYSTICK_STICK_POSITION_X, $"{joystickStickPosX}");
            PlayerPrefs.SetString(StaticStrings.CONTROLS_JOYSTICK_STICK_POSITION_Y, $"{joystickStickPosY}");
        }
        else if (joystickType == 1)
        {
            PlayerPrefs.SetString(StaticStrings.CONTROLS_JOYSTICK_DPAD_POSITION_X, $"{joystickDpadPosX}");
            PlayerPrefs.SetString(StaticStrings.CONTROLS_JOYSTICK_DPAD_POSITION_Y, $"{joystickDpadPosY}");
        }
        else if (joystickType == 2)
        {
            PlayerPrefs.SetString(StaticStrings.CONTROLS_DPAD_BUTTONS_POSITION_X, $"{dPadButtonsPosX}");
            PlayerPrefs.SetString(StaticStrings.CONTROLS_DPAD_BUTTONS_POSITION_Y, $"{dPadButtonsPosY}");
        }

        PlayerPrefs.SetString(StaticStrings.CONTROLS_SHOOT_BUTTON_POSITION_X, $"{shootButtonPosX}");
        PlayerPrefs.SetString(StaticStrings.CONTROLS_SHOOT_BUTTON_POSITION_Y, $"{shootButtonPosY}");

        PlayerPrefs.Save();

        isActive = false;
        customControlsPanel.gameObject.SetActive(false);
        gameMenuPanel.gameObject.SetActive(true);

        UpdateControls();

        CleanUp();
    }

    public void OnResetButtonClicked()
    {
        if (joystickType == 0)
        {
            PlayerPrefs.SetString(StaticStrings.CONTROLS_JOYSTICK_STICK_POSITION_X, $"50");
            PlayerPrefs.SetString(StaticStrings.CONTROLS_JOYSTICK_STICK_POSITION_Y, $"200");
        }
        else if (joystickType == 1)
        {
            PlayerPrefs.SetString(StaticStrings.CONTROLS_JOYSTICK_DPAD_POSITION_X, $"50");
            PlayerPrefs.SetString(StaticStrings.CONTROLS_JOYSTICK_DPAD_POSITION_Y, $"200");
        }
        else if (joystickType == 2)
        {
            PlayerPrefs.SetString(StaticStrings.CONTROLS_DPAD_BUTTONS_POSITION_X, $"150");
            PlayerPrefs.SetString(StaticStrings.CONTROLS_DPAD_BUTTONS_POSITION_Y, $"50");
        }

        PlayerPrefs.SetString(StaticStrings.CONTROLS_SHOOT_BUTTON_POSITION_X, $"-100");
        PlayerPrefs.SetString(StaticStrings.CONTROLS_SHOOT_BUTTON_POSITION_Y, $"100");

        PlayerPrefs.Save();

        isActive = false;
        customControlsPanel.gameObject.SetActive(false);
        gameMenuPanel.gameObject.SetActive(true);

        CleanUp();
    }
}
