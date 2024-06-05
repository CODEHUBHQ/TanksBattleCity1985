using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticStrings
{
    // PlayerPrefs
    public const string PLAYER_NAME_PREF_KEY = "PlayerName";
    public const string PLAYER_HIGH_SCORE_PREF_KEY = "PlayerHighScore";
    public const string CURRENT_LEVEL = "CurrentLevel";
    public const string IS_CUSTOM_MAP = "IsCustomMap";
    public const string CUSTOM_MAP = "CustomMap";
    public const string GAME_SETTINGS_SOUND = "GameSound";
    public const string GAME_SETTINGS_SOUND_MOVE = "GameSoundMove";
    public const string GAME_SETTINGS_SOUND_VOLUME = "GameSoundVolume";
    public const string GAME_SETTINGS_VIBRATE = "GameVibrate";
    public const string GAME_SETTINGS_DIFFICULTY = "GameDifficulty";

    // photon
    public const string IS_PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    public const string PLAYER_INSTANTIATED = "PlayerInstantiated";

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