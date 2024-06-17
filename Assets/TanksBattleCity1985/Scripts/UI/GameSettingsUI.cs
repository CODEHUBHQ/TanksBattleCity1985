using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettingsUI : MonoBehaviour
{
    public static GameSettingsUI Instance { get; private set; }

    public List<Button> GameSettingsOrderedButtons { get => gameSettingsOrderedButtons; }

    [SerializeField] private List<Button> gameSettingsOrderedButtons;
    [SerializeField] private TMP_Text gameSoundVolumeText;
    [SerializeField] private Image buttonMovement;
    [SerializeField] private Image buttonJoystick;
    [SerializeField] private Image buttonDpad;

    private List<string> gameSettingsOrderedButtonsMethodNames = new List<string>();

    private float gameSoundVolume;

    private int joystickType;

    private bool gameSound;
    private bool gameSoundMove;
    private bool gameVibrate;

    private void Awake()
    {
        Instance = this;

        gameSettingsOrderedButtonsMethodNames.Add(nameof(SoundButtonOnClick));
        gameSettingsOrderedButtonsMethodNames.Add(nameof(SoundUpButtonOnClick));
        gameSettingsOrderedButtonsMethodNames.Add(nameof(SoundDownButtonOnClick));
        gameSettingsOrderedButtonsMethodNames.Add(nameof(VibrateButtonOnClick));
        gameSettingsOrderedButtonsMethodNames.Add(nameof(SoundMoveButtonOnClick));
        gameSettingsOrderedButtonsMethodNames.Add(nameof(MainMenuButtonOnClick));

        for (int i = 0; i < gameSettingsOrderedButtons.Count; i++)
        {
            var index = i; // we use copy to solve closure issue

            gameSettingsOrderedButtons[i].onClick.AddListener(() =>
            {
                SendMessage(gameSettingsOrderedButtonsMethodNames[index]);
            });
        }

        LoadGameSettings();
    }

    private void LoadGameSettings()
    {
        gameSound = bool.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_SOUND, "true"));
        gameSoundMove = bool.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_SOUND_MOVE, "true"));
        gameSoundVolume = float.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_SOUND_VOLUME, "5"));
        gameVibrate = bool.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_VIBRATE, "true"));

        try
        {
            joystickType = int.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_BUTTON_CONTROLLER, "0"));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LoadGameSettings: {ex}");
            PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_BUTTON_CONTROLLER, $"0");
            PlayerPrefs.Save();
        }

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND, $"{gameSound}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_MOVE, $"{gameSoundMove}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_VOLUME, $"{gameSoundVolume}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_VIBRATE, $"{gameVibrate}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_BUTTON_CONTROLLER, $"{joystickType}");
        PlayerPrefs.Save();

        UpdateGameSettingsUI();
    }

    private void UpdateGameSettingsUI()
    {
        foreach (var button in gameSettingsOrderedButtons)
        {
            if (button.gameObject.name == "SoundButton")
            {
                if (gameSound)
                {
                    button.transform.Find("Selected").GetComponent<Image>().enabled = true;
                }
                else
                {
                    button.transform.Find("Selected").GetComponent<Image>().enabled = false;
                }
            }
            else if (button.gameObject.name == "VibrateButton")
            {
                if (gameVibrate)
                {
                    button.transform.Find("Selected").GetComponent<Image>().enabled = true;
                }
                else
                {
                    button.transform.Find("Selected").GetComponent<Image>().enabled = false;
                }
            }
            else if (button.gameObject.name == "SoundMoveButton")
            {
                if (gameSoundMove)
                {
                    button.transform.Find("Selected").GetComponent<Image>().enabled = true;
                }
                else
                {
                    button.transform.Find("Selected").GetComponent<Image>().enabled = false;
                }
            }
        }

        gameSoundVolumeText.text = $"{gameSoundVolume}";

        Debug.Log($"UpdateGameSettingsUI joystickType {joystickType}");

        if (joystickType == 0)
        {
            buttonJoystick.color = new Color(255f, 255f, 255f, 1f);
            buttonMovement.color = new Color(255f, 255f, 255f, 0.1f);
            buttonDpad.color = new Color(255f, 255f, 255f, 0.1f);
        }
        else if (joystickType == 1)
        {
            buttonDpad.color = new Color(255f, 255f, 255f, 1f);
            buttonJoystick.color = new Color(255f, 255f, 255f, 0.1f);
            buttonMovement.color = new Color(255f, 255f, 255f, 0.1f);
        }
        else if (joystickType == 2)
        {
            buttonMovement.color = new Color(255f, 255f, 255f, 1f);
            buttonJoystick.color = new Color(255f, 255f, 255f, 0.1f);
            buttonDpad.color = new Color(255f, 255f, 255f, 0.1f);
        }
    }

    private void SoundButtonOnClick()
    {
        Debug.Log($"SoundButtonOnClick");

        gameSound = !gameSound;

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND, $"{gameSound}");
        PlayerPrefs.Save();

        UpdateGameSettingsUI();
    }

    private void SoundUpButtonOnClick()
    {
        Debug.Log($"SoundUpButtonOnClick");

        gameSoundVolume = Mathf.Clamp(gameSoundVolume + 1, 1, 9);

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_VOLUME, $"{gameSoundVolume}");
        PlayerPrefs.Save();

        UpdateGameSettingsUI();
    }

    private void SoundDownButtonOnClick()
    {
        Debug.Log($"SoundDownButtonOnClick");

        gameSoundVolume = Mathf.Clamp(gameSoundVolume - 1, 1, 9);

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_VOLUME, $"{gameSoundVolume}");
        PlayerPrefs.Save();

        UpdateGameSettingsUI();
    }

    private void VibrateButtonOnClick()
    {
        Debug.Log($"VibrateButtonOnClick");

        gameVibrate = !gameVibrate;

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_VIBRATE, $"{gameVibrate}");
        PlayerPrefs.Save();

        UpdateGameSettingsUI();
    }

    private void SoundMoveButtonOnClick()
    {
        Debug.Log($"SoundMoveButtonOnClick");

        gameSoundMove = !gameSoundMove;

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_MOVE, $"{gameSoundMove}");
        PlayerPrefs.Save();

        UpdateGameSettingsUI();
    }

    private void MainMenuButtonOnClick()
    {
        Debug.Log($"MainMenuButtonOnClick");

        LoadingManager.LoadScene(LoadingManager.Scene.MenuScene);
    }

    public void OnControllerButtonClicked(int joystickType)
    {
        Debug.Log($"OnControllerButtonClicked");

        this.joystickType = joystickType;

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_BUTTON_CONTROLLER, $"{joystickType}");
        PlayerPrefs.Save();

        UpdateGameSettingsUI();
    }
}
