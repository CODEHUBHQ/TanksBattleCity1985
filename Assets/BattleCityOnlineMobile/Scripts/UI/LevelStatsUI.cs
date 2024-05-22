using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelStatsUI : MonoBehaviour
{
    public static LevelStatsUI Instance { get; private set; }

    [SerializeField] private GameObject onePlayerStatsBackground;
    [SerializeField] private GameObject playerOneStats;
    [SerializeField] private GameObject twoPlayerStatsBackground;
    [SerializeField] private GameObject playerTwoStats;
    [SerializeField] private GameObject highScoreText;
    [SerializeField] private GameObject stageText;

    [Header("PlayerOneStats")]
    [SerializeField] private TMP_Text playerOneLevelScoreText;
    [SerializeField] private TMP_Text easyTankKillCountPlayerOneText;
    [SerializeField] private TMP_Text easyTankPTSCountPlayerOneText;
    [SerializeField] private TMP_Text fastTankKillCountPlayerOneText;
    [SerializeField] private TMP_Text fastTankPTSCountPlayerOneText;
    [SerializeField] private TMP_Text mediumTankKillCountPlayerOneText;
    [SerializeField] private TMP_Text mediumTankPTSCountPlayerOneText;
    [SerializeField] private TMP_Text strongTankKillCountPlayerOneText;
    [SerializeField] private TMP_Text strongTankPTSCountPlayerOneText;
    [SerializeField] private TMP_Text playerOneTotalKillCountText;

    [Header("PlayerTwoStats")]
    [SerializeField] private TMP_Text playerTwoLevelScoreText;
    [SerializeField] private TMP_Text easyTankKillCountPlayerTwoText;
    [SerializeField] private TMP_Text easyTankPTSCountPlayerTwoText;
    [SerializeField] private TMP_Text fastTankKillCountPlayerTwoText;
    [SerializeField] private TMP_Text fastTankPTSCountPlayerTwoText;
    [SerializeField] private TMP_Text mediumTankKillCountPlayerTwoText;
    [SerializeField] private TMP_Text mediumTankPTSCountPlayerTwoText;
    [SerializeField] private TMP_Text strongTankKillCountPlayerTwoText;
    [SerializeField] private TMP_Text strongTankPTSCountPlayerTwoText;
    [SerializeField] private TMP_Text playerTwoTotalKillCountText;

    private Action cb;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPlayersStats(int level, Action cb)
    {
        this.cb = cb;

        if (NetworkManager.Instance != null)
        {
            if (NetworkManager.Instance.GameMode == GameMode.SinglePlayer)
            {
                StartCoroutine(ShowPlayersStats(level));
            }
            else if (NetworkManager.Instance.GameMode == GameMode.LocalMultiplayer || NetworkManager.Instance.GameMode == GameMode.Multiplayer)
            {
                StartCoroutine(ShowPlayersStats(level, true));
            }
        }
        else
        {
            StartCoroutine(ShowPlayersStats(level));
        }
    }

    private IEnumerator ShowPlayersStats(int level, bool isMultiplayer = false)
    {
        highScoreText.SetActive(true);
        stageText.SetActive(true);

        var playerHighScore = PlayerPrefs.GetString(StaticStrings.PLAYER_HIGH_SCORE_PREF_KEY);

        highScoreText.GetComponent<TMP_Text>().text = $"{playerHighScore}";
        stageText.GetComponent<TMP_Text>().text = $"{level}";

        // player one
        onePlayerStatsBackground.SetActive(true);
        playerOneStats.SetActive(true);

        playerOneLevelScoreText.text = $"{GameManager.Instance.PlayerOne.GetTotalLevelScore()}";

        if (isMultiplayer)
        {
            // player two
            twoPlayerStatsBackground.SetActive(true);
            playerTwoStats.SetActive(true);

            playerTwoLevelScoreText.text = $"{GameManager.Instance.PlayerTwo.GetTotalLevelScore()}";
        }

        SoundManager.Instance.PlayScoreSound();

        if (GameManager.Instance.PlayerOne.GetEasyTanksKilled() > 1)
        {
            var easyTankPTS = 100;

            var playerOneEasyTanksStats = StartCoroutine(AnimatePlayerLevelStats(easyTankKillCountPlayerOneText, easyTankPTSCountPlayerOneText, GameManager.Instance.PlayerOne.GetEasyTanksKilled(), easyTankPTS));

            Coroutine playerTwoEasyTanksStats = null;

            if (isMultiplayer)
            {
                playerTwoEasyTanksStats = StartCoroutine(AnimatePlayerLevelStats(easyTankKillCountPlayerTwoText, easyTankPTSCountPlayerTwoText, GameManager.Instance.PlayerTwo.GetEasyTanksKilled(), easyTankPTS));
            }

            if (isMultiplayer)
            {
                yield return playerOneEasyTanksStats;
                yield return playerTwoEasyTanksStats;
            }
            else
            {
                yield return playerOneEasyTanksStats;
            }
        }
        else
        {
            easyTankKillCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetEasyTanksKilled()}";
            easyTankPTSCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetEasyTanksKilled() * 100}";

            if (isMultiplayer)
            {
                easyTankKillCountPlayerTwoText.text = $"{GameManager.Instance.PlayerTwo.GetEasyTanksKilled()}";
                easyTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.PlayerTwo.GetEasyTanksKilled() * 100}";
            }
        }

        if (GameManager.Instance.PlayerOne.GetFastTanksKilled() > 1)
        {
            var fastTankPTS = 200;

            var playerOneFastTanksStats = StartCoroutine(AnimatePlayerLevelStats(fastTankKillCountPlayerOneText, fastTankPTSCountPlayerOneText, GameManager.Instance.PlayerOne.GetFastTanksKilled(), fastTankPTS));

            Coroutine playerTwoFastTanksStats = null;

            if (isMultiplayer)
            {
                StartCoroutine(AnimatePlayerLevelStats(fastTankKillCountPlayerTwoText, fastTankPTSCountPlayerTwoText, GameManager.Instance.PlayerTwo.GetFastTanksKilled(), fastTankPTS));
            }

            if (isMultiplayer)
            {
                yield return playerOneFastTanksStats;
                yield return playerTwoFastTanksStats;
            }
            else
            {
                yield return playerOneFastTanksStats;
            }
        }
        else
        {
            fastTankKillCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetFastTanksKilled()}";
            fastTankPTSCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetFastTanksKilled() * 200}";

            if (isMultiplayer)
            {
                fastTankKillCountPlayerTwoText.text = $"{GameManager.Instance.PlayerTwo.GetFastTanksKilled()}";
                fastTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.PlayerTwo.GetFastTanksKilled() * 200}";
            }
        }

        if (GameManager.Instance.PlayerOne.GetMediumTanksKilled() > 1)
        {
            var mediumTankPTS = 300;

            var playerOneMediumTanksStats = StartCoroutine(AnimatePlayerLevelStats(mediumTankKillCountPlayerOneText, mediumTankPTSCountPlayerOneText, GameManager.Instance.PlayerOne.GetMediumTanksKilled(), mediumTankPTS));

            Coroutine playerTwoMediumTanksStats = null;

            if (isMultiplayer)
            {
                StartCoroutine(AnimatePlayerLevelStats(mediumTankKillCountPlayerTwoText, mediumTankPTSCountPlayerTwoText, GameManager.Instance.PlayerTwo.GetMediumTanksKilled(), mediumTankPTS));
            }

            if (isMultiplayer)
            {
                yield return playerOneMediumTanksStats;
                yield return playerTwoMediumTanksStats;
            }
            else
            {
                yield return playerOneMediumTanksStats;
            }
        }
        else
        {
            mediumTankKillCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetMediumTanksKilled()}";
            mediumTankPTSCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetMediumTanksKilled() * 300}";

            if (isMultiplayer)
            {
                mediumTankKillCountPlayerTwoText.text = $"{GameManager.Instance.PlayerTwo.GetMediumTanksKilled()}";
                mediumTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.PlayerTwo.GetMediumTanksKilled() * 300}";
            }
        }


        if (GameManager.Instance.PlayerOne.GetStrongTanksKilled() > 1)
        {
            var strongTankPTS = 400;

            var playerOneStrongTanksStats = StartCoroutine(AnimatePlayerLevelStats(strongTankKillCountPlayerOneText, strongTankPTSCountPlayerOneText, GameManager.Instance.PlayerOne.GetStrongTanksKilled(), strongTankPTS));

            Coroutine playerTwoStrongTanksStats = null;

            if (isMultiplayer)
            {
                StartCoroutine(AnimatePlayerLevelStats(strongTankKillCountPlayerTwoText, strongTankPTSCountPlayerTwoText, GameManager.Instance.PlayerTwo.GetStrongTanksKilled(), strongTankPTS));
            }

            if (isMultiplayer)
            {
                yield return playerOneStrongTanksStats;
                yield return playerTwoStrongTanksStats;
            }
            else
            {
                yield return playerOneStrongTanksStats;
            }
        }
        else
        {
            strongTankKillCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetStrongTanksKilled()}";
            strongTankPTSCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetStrongTanksKilled() * 400}";

            if (isMultiplayer)
            {
                strongTankKillCountPlayerTwoText.text = $"{GameManager.Instance.PlayerTwo.GetStrongTanksKilled()}";
                strongTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.PlayerTwo.GetStrongTanksKilled() * 400}";
            }
        }

        var playerOneTotalTanksKilled = GameManager.Instance.PlayerOne.GetEasyTanksKilled() +
                                        GameManager.Instance.PlayerOne.GetFastTanksKilled() +
                                        GameManager.Instance.PlayerOne.GetMediumTanksKilled() +
                                        GameManager.Instance.PlayerOne.GetStrongTanksKilled();

        SoundManager.Instance.PlayScorePulseSound();

        playerOneTotalKillCountText.text = $"{playerOneTotalTanksKilled}";

        if (isMultiplayer)
        {
            var playerTwoTotalTanksKilled = GameManager.Instance.PlayerTwo.GetEasyTanksKilled() +
                                        GameManager.Instance.PlayerTwo.GetFastTanksKilled() +
                                        GameManager.Instance.PlayerTwo.GetMediumTanksKilled() +
                                        GameManager.Instance.PlayerTwo.GetStrongTanksKilled();

            playerTwoTotalKillCountText.text = $"{playerTwoTotalTanksKilled}";
        }

        yield return new WaitForSeconds(3f);

        highScoreText.SetActive(false);
        stageText.SetActive(false);

        // player one
        onePlayerStatsBackground.SetActive(false);
        playerOneStats.SetActive(false);

        playerOneLevelScoreText.text = $"";
        easyTankKillCountPlayerOneText.text = $"";
        easyTankPTSCountPlayerOneText.text = $"";
        fastTankKillCountPlayerOneText.text = $"";
        fastTankPTSCountPlayerOneText.text = $"";
        mediumTankKillCountPlayerOneText.text = $"";
        mediumTankPTSCountPlayerOneText.text = $"";
        strongTankKillCountPlayerOneText.text = $"";
        strongTankPTSCountPlayerOneText.text = $"";
        playerOneTotalKillCountText.text = $"";

        if (isMultiplayer)
        {
            // player two
            twoPlayerStatsBackground.SetActive(false);
            playerTwoStats.SetActive(false);

            playerTwoLevelScoreText.text = $"";
            easyTankKillCountPlayerTwoText.text = $"";
            easyTankPTSCountPlayerTwoText.text = $"";
            fastTankKillCountPlayerTwoText.text = $"";
            fastTankPTSCountPlayerTwoText.text = $"";
            mediumTankKillCountPlayerTwoText.text = $"";
            mediumTankPTSCountPlayerTwoText.text = $"";
            strongTankKillCountPlayerTwoText.text = $"";
            strongTankPTSCountPlayerTwoText.text = $"";
            playerTwoTotalKillCountText.text = $"";
        }

        cb?.Invoke();
    }

    private IEnumerator AnimatePlayerLevelStats(TMP_Text killsText, TMP_Text ptsText, int playerKills, int tankPTS)
    {
        var kills = 0;
        var pts = 0;

        while (kills < playerKills)
        {
            killsText.text = $"{kills + 1}";
            ptsText.text = $"{pts + tankPTS}";

            kills++;
            pts += tankPTS;

            yield return new WaitForSeconds(0.28f);
        }
    }
}
