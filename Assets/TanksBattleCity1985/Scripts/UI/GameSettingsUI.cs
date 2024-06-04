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

    private List<string> gameSettingsOrderedButtonsMethodNames = new List<string>();

    private float gameSoundVolume;

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

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND, $"{gameSound}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_MOVE, $"{gameSoundMove}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_VOLUME, $"{gameSoundVolume}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_VIBRATE, $"{gameVibrate}");
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
}
