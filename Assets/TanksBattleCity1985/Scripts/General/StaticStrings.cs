using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticStrings
{
    // PlayerPrefs
    public const string PLAYER_NAME_PREF_KEY = "PlayerName";
    public const string PLAYER_HIGH_SCORE_PREF_KEY = "PlayerHighScore";
    public const string PLAYER_BALANCE = "PlayerBalance";
    public const string CURRENT_LEVEL = "CurrentLevel";
    public const string MAX_UNLOCKED_LEVEL = "MaxUnlockedLevel";
    public const string IS_CUSTOM_MAP = "IsCustomMap";
    public const string CUSTOM_MAP = "CustomMap";
    public const string GAME_SETTINGS_SOUND = "GameSound";
    public const string GAME_SETTINGS_SOUND_MOVE = "GameSoundMove";
    public const string GAME_SETTINGS_SOUND_VOLUME = "GameSoundVolume";
    public const string GAME_SETTINGS_VIBRATE = "GameVibrate";
    public const string GAME_SETTINGS_DIFFICULTY = "GameDifficulty";
    public const string GAME_SETTINGS_BUTTON_CONTROLLER = "GameButtonController";
    public const string CONTROLS_SHOOT_BUTTON_POSITION_X = "ControlsShootButtonPositionX";
    public const string CONTROLS_SHOOT_BUTTON_POSITION_Y = "ControlsShootButtonPositionY";
    public const string CONTROLS_JOYSTICK_STICK_POSITION_X = "ControlsJoystickStickPositionX";
    public const string CONTROLS_JOYSTICK_STICK_POSITION_Y = "ControlsJoystickStickPositionY";
    public const string CONTROLS_JOYSTICK_DPAD_POSITION_X = "ControlsJoystickDPadPositionX";
    public const string CONTROLS_JOYSTICK_DPAD_POSITION_Y = "ControlsJoystickDPadPositionY";
    public const string CONTROLS_DPAD_BUTTONS_POSITION_X = "ControlsDPadButtonsPositionX";
    public const string CONTROLS_DPAD_BUTTONS_POSITION_Y = "ControlsDPadButtonsPositionY";

    // photon
    public const string IS_PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    public const string PLAYER_INSTANTIATED = "PlayerInstantiated";
    public const string PLAYER_ONE_SCORE = "PlayerOneScore";
    public const string PLAYER_TWO_SCORE = "PlayerTwoScore";

    // Animator params
    public static readonly int INPUT_X = Animator.StringToHash("input_x");
    public static readonly int INPUT_Y = Animator.StringToHash("input_y");
    public static readonly int IS_MOVING = Animator.StringToHash("isMoving");
    public static readonly int HIT = Animator.StringToHash("hit");
    public static readonly int IS_ON = Animator.StringToHash("isOn");
    public static readonly int SHIELD = Animator.StringToHash("shield");
    public static readonly int BONUS = Animator.StringToHash("bonus");
    public static readonly int LIVES = Animator.StringToHash("lives");
    public static readonly int SPAWN = Animator.StringToHash("spawn");
    public static readonly int LEFT_NUMPAD = Animator.StringToHash("left_numpad");
    public static readonly int LEVEL = Animator.StringToHash("level");
    public static readonly int IS_DESTROYED = Animator.StringToHash("isDestroyed");
}
