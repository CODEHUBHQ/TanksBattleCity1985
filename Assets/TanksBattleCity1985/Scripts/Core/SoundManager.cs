using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource shotSound;
    [SerializeField] private AudioSource levelStartingSound;
    [SerializeField] private AudioSource gameOverSound;
    [SerializeField] private AudioSource tankDestroySound;
    [SerializeField] private AudioSource bulletWallHitSound;
    [SerializeField] private AudioSource bulletIronHitSound;
    [SerializeField] private AudioSource eagleDestroySound;
    [SerializeField] private AudioSource pauseSound;
    [SerializeField] private AudioSource notMovingSound;
    [SerializeField] private AudioSource movingSound;
    [SerializeField] private AudioSource powerUpTakenSound;
    [SerializeField] private AudioSource powerUpShowUpSound;
    [SerializeField] private AudioSource scoreSound;
    [SerializeField] private AudioSource scorePulseSound;
    [SerializeField] private AudioSource highScoreSound;
    [SerializeField] private AudioSource bonusPTSSound;

    private float gameSoundVolume;

    private bool gameSound;
    private bool gameSoundMove;
    private bool gameVibrate;

    private void Awake()
    {
        Instance = this;

        gameSound = bool.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_SOUND, "true"));
        gameSoundMove = bool.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_SOUND_MOVE, "true"));
        gameSoundVolume = float.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_SOUND_VOLUME, "5"));
        gameVibrate = bool.Parse(PlayerPrefs.GetString(StaticStrings.GAME_SETTINGS_VIBRATE, "true"));

        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND, $"{gameSound}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_MOVE, $"{gameSoundMove}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_SOUND_VOLUME, $"{gameSoundVolume}");
        PlayerPrefs.SetString(StaticStrings.GAME_SETTINGS_VIBRATE, $"{gameVibrate}");
        PlayerPrefs.Save();

        shotSound.volume = gameSoundVolume / 10;
        levelStartingSound.volume = gameSoundVolume / 10;
        gameOverSound.volume = gameSoundVolume / 10;
        tankDestroySound.volume = gameSoundVolume / 10;
        bulletWallHitSound.volume = gameSoundVolume / 10;
        bulletIronHitSound.volume = gameSoundVolume / 10;
        eagleDestroySound.volume = gameSoundVolume / 10;
        pauseSound.volume = gameSoundVolume / 10;
        notMovingSound.volume = gameSoundVolume / 10;
        movingSound.volume = gameSoundVolume / 10;
        powerUpTakenSound.volume = gameSoundVolume / 10;
        powerUpShowUpSound.volume = gameSoundVolume / 10;
        scoreSound.volume = gameSoundVolume / 10;
        scorePulseSound.volume = gameSoundVolume / 10;
        highScoreSound.volume = gameSoundVolume / 10;
        bonusPTSSound.volume = gameSoundVolume / 10;
    }

    public void PlayShotSound()
    {
        if (!gameSound) return;

        shotSound.Play();
    }

    public void StopShotSound()
    {
        shotSound.Stop();
    }

    public void PlayLevelStartingSound()
    {
        if (!gameSound) return;

        levelStartingSound.Play();
    }

    public void StopLevelStartingSound()
    {
        levelStartingSound.Stop();
    }

    public void PlayGameOverSound()
    {
        if (!gameSound) return;

        gameOverSound.Play();
    }

    public void StopGameOverSound()
    {
        gameOverSound.Stop();
    }

    public void PlayTankDestroySound()
    {
        if (!gameSound) return;

        tankDestroySound.Play();
    }

    public void StopTankDestroySound()
    {
        tankDestroySound.Stop();
    }

    public void PlayBulletWallHitSound()
    {
        if (!gameSound) return;

        bulletWallHitSound.Play();
    }

    public void StopBulletWallHitSound()
    {
        bulletWallHitSound.Stop();
    }

    public void PlayBulletIronHitSound()
    {
        if (!gameSound) return;

        bulletIronHitSound.Play();
    }

    public void StopBulletIronHitSound()
    {
        bulletIronHitSound.Stop();
    }

    public void PlayEagleDestroySound()
    {
        if (!gameSound) return;

        eagleDestroySound.Play();
    }

    public void StopEagleDestroySound()
    {
        eagleDestroySound.Stop();
    }

    public void PlayPauseSound()
    {
        if (!gameSound) return;

        pauseSound.Play();
    }

    public void StopPauseSound()
    {
        pauseSound.Stop();
    }

    public void PlayNotMovingSound()
    {
        if (!gameSound || !gameSoundMove) return;

        notMovingSound.Play();
    }

    public void StopNotMovingSound()
    {
        notMovingSound.Stop();
    }

    public void PlayMovingSound()
    {
        if (!gameSound || !gameSoundMove) return;

        movingSound.Play();
    }

    public void StopMovingSound()
    {
        movingSound.Stop();
    }

    public void PlayPowerUpTakenSound()
    {
        if (!gameSound) return;

        powerUpTakenSound.Play();
    }

    public void StopPowerUpTakenSound()
    {
        powerUpTakenSound.Stop();
    }

    public void PlayPowerUpShowUpSound()
    {
        if (!gameSound) return;

        powerUpShowUpSound.Play();
    }

    public void StopPowerUpShowUpSound()
    {
        powerUpShowUpSound.Stop();
    }

    public void PlayScoreSound()
    {
        if (!gameSound) return;

        scoreSound.Play();
    }

    public void StopScoreSound()
    {
        scoreSound.Stop();
    }

    public void PlayScorePulseSound()
    {
        if (!gameSound) return;

        scorePulseSound.Play();
    }

    public void StopScorePulseSound()
    {
        scorePulseSound.Stop();
    }

    public void PlayHighScoreSound()
    {
        if (!gameSound) return;

        highScoreSound.Play();
    }

    public void StopHighScoreSound()
    {
        highScoreSound.Stop();
    }

    public void PlayBonusPTSSound()
    {
        if (!gameSound) return;

        bonusPTSSound.Play();
    }

    public void StopBonusPTSSound()
    {
        bonusPTSSound.Stop();
    }

    public bool IsNotMovingSoundPlaying()
    {
        return notMovingSound.isPlaying;
    }

    public bool IsMovingSoundPlaying()
    {
        return movingSound.isPlaying;
    }

    public bool IsVibrateEnabled()
    {
        return gameVibrate;
    }
}
