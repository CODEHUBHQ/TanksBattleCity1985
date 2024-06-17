using Photon.Pun;
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

        //playerOneLevelScoreText.text = $"{GameManager.Instance.PlayerOne.GetTotalLevelScore()}";
        playerOneLevelScoreText.text = $"{GameManager.Instance.GetPlayerOneStats()[4]}";

        if (isMultiplayer)
        {
            // player two
            twoPlayerStatsBackground.SetActive(true);
            playerTwoStats.SetActive(true);

            //playerTwoLevelScoreText.text = $"{GameManager.Instance.PlayerOne.GetTotalLevelScore()}";
            playerTwoLevelScoreText.text = $"{GameManager.Instance.GetPlayerTwoStats()[4]}";
        }

        SoundManager.Instance.PlayScoreSound();

        if (GameManager.Instance.GetPlayerOneStats()[0] > 1 || GameManager.Instance.GetPlayerTwoStats()[0] > 1)
        {
            var easyTankPTS = 100;

            Coroutine playerOneEasyTanksStats = null;

            //playerOneEasyTanksStats = StartCoroutine(AnimatePlayerLevelStats(easyTankKillCountPlayerOneText, easyTankPTSCountPlayerOneText, GameManager.Instance.PlayerOne.GetEasyTanksKilled(), easyTankPTS));
            playerOneEasyTanksStats = StartCoroutine(AnimatePlayerLevelStats(easyTankKillCountPlayerOneText, easyTankPTSCountPlayerOneText, GameManager.Instance.GetPlayerOneStats()[0], easyTankPTS));

            Coroutine playerTwoEasyTanksStats = null;

            if (isMultiplayer)
            {
                //playerTwoEasyTanksStats = StartCoroutine(AnimatePlayerLevelStats(easyTankKillCountPlayerTwoText, easyTankPTSCountPlayerTwoText, GameManager.Instance.PlayerOne.GetEasyTanksKilled(), easyTankPTS));
                playerTwoEasyTanksStats = StartCoroutine(AnimatePlayerLevelStats(easyTankKillCountPlayerTwoText, easyTankPTSCountPlayerTwoText, GameManager.Instance.GetPlayerTwoStats()[0], easyTankPTS));
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
            //easyTankKillCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetEasyTanksKilled()}";
            //easyTankPTSCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetEasyTanksKilled() * 100}";
            easyTankKillCountPlayerOneText.text = $"{GameManager.Instance.GetPlayerOneStats()[0]}";
            easyTankPTSCountPlayerOneText.text = $"{GameManager.Instance.GetPlayerOneStats()[0] * 100}";

            if (isMultiplayer)
            {
                //easyTankKillCountPlayerTwoText.text = $"{GameManager.Instance.PlayerOne.GetEasyTanksKilled()}";
                //easyTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.PlayerOne.GetEasyTanksKilled() * 100}";
                easyTankKillCountPlayerTwoText.text = $"{GameManager.Instance.GetPlayerTwoStats()[0]}";
                easyTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.GetPlayerTwoStats()[0] * 100}";
            }
        }

        if (GameManager.Instance.GetPlayerOneStats()[1] > 1 || GameManager.Instance.GetPlayerTwoStats()[1] > 1)
        {
            var fastTankPTS = 200;

            Coroutine playerOneFastTanksStats = null;

            //playerOneFastTanksStats = StartCoroutine(AnimatePlayerLevelStats(fastTankKillCountPlayerOneText, fastTankPTSCountPlayerOneText, GameManager.Instance.PlayerOne.GetFastTanksKilled(), fastTankPTS));
            playerOneFastTanksStats = StartCoroutine(AnimatePlayerLevelStats(fastTankKillCountPlayerOneText, fastTankPTSCountPlayerOneText, GameManager.Instance.GetPlayerOneStats()[1], fastTankPTS));

            Coroutine playerTwoFastTanksStats = null;

            if (isMultiplayer)
            {
                //playerTwoFastTanksStats = StartCoroutine(AnimatePlayerLevelStats(fastTankKillCountPlayerTwoText, fastTankPTSCountPlayerTwoText, GameManager.Instance.PlayerOne.GetFastTanksKilled(), fastTankPTS));
                playerTwoFastTanksStats = StartCoroutine(AnimatePlayerLevelStats(fastTankKillCountPlayerTwoText, fastTankPTSCountPlayerTwoText, GameManager.Instance.GetPlayerTwoStats()[1], fastTankPTS));
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
            //fastTankKillCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetFastTanksKilled()}";
            //fastTankPTSCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetFastTanksKilled() * 200}";
            fastTankKillCountPlayerOneText.text = $"{GameManager.Instance.GetPlayerOneStats()[1]}";
            fastTankPTSCountPlayerOneText.text = $"{GameManager.Instance.GetPlayerOneStats()[1] * 200}";

            if (isMultiplayer)
            {
                //fastTankKillCountPlayerTwoText.text = $"{GameManager.Instance.PlayerOne.GetFastTanksKilled()}";
                //fastTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.PlayerOne.GetFastTanksKilled() * 200}";
                fastTankKillCountPlayerTwoText.text = $"{GameManager.Instance.GetPlayerTwoStats()[1]}";
                fastTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.GetPlayerTwoStats()[1] * 200}";
            }
        }

        if (GameManager.Instance.GetPlayerOneStats()[2] > 1 || GameManager.Instance.GetPlayerTwoStats()[2] > 1)
        {
            var mediumTankPTS = 300;

            Coroutine playerOneMediumTanksStats = null;

            //playerOneMediumTanksStats = StartCoroutine(AnimatePlayerLevelStats(mediumTankKillCountPlayerOneText, mediumTankPTSCountPlayerOneText, GameManager.Instance.PlayerOne.GetMediumTanksKilled(), mediumTankPTS));

            playerOneMediumTanksStats = StartCoroutine(AnimatePlayerLevelStats(mediumTankKillCountPlayerOneText, mediumTankPTSCountPlayerOneText, GameManager.Instance.GetPlayerOneStats()[2], mediumTankPTS));

            Coroutine playerTwoMediumTanksStats = null;

            if (isMultiplayer)
            {
                //playerTwoMediumTanksStats = StartCoroutine(AnimatePlayerLevelStats(mediumTankKillCountPlayerTwoText, mediumTankPTSCountPlayerTwoText, GameManager.Instance.PlayerOne.GetMediumTanksKilled(), mediumTankPTS));
                playerTwoMediumTanksStats = StartCoroutine(AnimatePlayerLevelStats(mediumTankKillCountPlayerTwoText, mediumTankPTSCountPlayerTwoText, GameManager.Instance.GetPlayerTwoStats()[2], mediumTankPTS));
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
            //mediumTankKillCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetMediumTanksKilled()}";
            //mediumTankPTSCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetMediumTanksKilled() * 300}";
            mediumTankKillCountPlayerOneText.text = $"{GameManager.Instance.GetPlayerOneStats()[2]}";
            mediumTankPTSCountPlayerOneText.text = $"{GameManager.Instance.GetPlayerOneStats()[2] * 300}";

            if (isMultiplayer)
            {
                //mediumTankKillCountPlayerTwoText.text = $"{GameManager.Instance.PlayerOne.GetMediumTanksKilled()}";
                //mediumTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.PlayerOne.GetMediumTanksKilled() * 300}";
                mediumTankKillCountPlayerTwoText.text = $"{GameManager.Instance.GetPlayerTwoStats()[2]}";
                mediumTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.GetPlayerTwoStats()[2] * 300}";
            }
        }


        if (GameManager.Instance.GetPlayerOneStats()[3] > 1 || GameManager.Instance.GetPlayerTwoStats()[3] > 1)
        {
            var strongTankPTS = 400;

            Coroutine playerOneStrongTanksStats = null;

            //playerOneStrongTanksStats = StartCoroutine(AnimatePlayerLevelStats(strongTankKillCountPlayerOneText, strongTankPTSCountPlayerOneText, GameManager.Instance.PlayerOne.GetStrongTanksKilled(), strongTankPTS));
            playerOneStrongTanksStats = StartCoroutine(AnimatePlayerLevelStats(strongTankKillCountPlayerOneText, strongTankPTSCountPlayerOneText, GameManager.Instance.GetPlayerOneStats()[3], strongTankPTS));

            Coroutine playerTwoStrongTanksStats = null;

            if (isMultiplayer)
            {
                //playerTwoStrongTanksStats = StartCoroutine(AnimatePlayerLevelStats(strongTankKillCountPlayerTwoText, strongTankPTSCountPlayerTwoText, GameManager.Instance.PlayerOne.GetStrongTanksKilled(), strongTankPTS));
                playerTwoStrongTanksStats = StartCoroutine(AnimatePlayerLevelStats(strongTankKillCountPlayerTwoText, strongTankPTSCountPlayerTwoText, GameManager.Instance.GetPlayerTwoStats()[3], strongTankPTS));
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
            //strongTankKillCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetStrongTanksKilled()}";
            //strongTankPTSCountPlayerOneText.text = $"{GameManager.Instance.PlayerOne.GetStrongTanksKilled() * 400}";
            strongTankKillCountPlayerOneText.text = $"{GameManager.Instance.GetPlayerOneStats()[3]}";
            strongTankPTSCountPlayerOneText.text = $"{GameManager.Instance.GetPlayerOneStats()[3] * 400}";

            if (isMultiplayer)
            {
                //strongTankKillCountPlayerTwoText.text = $"{GameManager.Instance.PlayerOne.GetStrongTanksKilled()}";
                //strongTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.PlayerOne.GetStrongTanksKilled() * 400}";
                strongTankKillCountPlayerTwoText.text = $"{GameManager.Instance.GetPlayerTwoStats()[3]}";
                strongTankPTSCountPlayerTwoText.text = $"{GameManager.Instance.GetPlayerTwoStats()[3] * 400}";
            }
        }

        var playerOneTotalTanksKilled = 0;

        //playerOneTotalTanksKilled = GameManager.Instance.PlayerOne.GetEasyTanksKilled() +
        //                            GameManager.Instance.PlayerOne.GetFastTanksKilled() +
        //                            GameManager.Instance.PlayerOne.GetMediumTanksKilled() +
        //                            GameManager.Instance.PlayerOne.GetStrongTanksKilled();
        playerOneTotalTanksKilled = GameManager.Instance.GetPlayerOneStats()[0] +
                                    GameManager.Instance.GetPlayerOneStats()[1] +
                                    GameManager.Instance.GetPlayerOneStats()[2] +
                                    GameManager.Instance.GetPlayerOneStats()[3];

        SoundManager.Instance.PlayScorePulseSound();

        playerOneTotalKillCountText.text = $"{playerOneTotalTanksKilled}";

        if (isMultiplayer)
        {
            var playerTwoTotalTanksKilled = 0;

            //playerTwoTotalTanksKilled = GameManager.Instance.PlayerOne.GetEasyTanksKilled() +
            //                            GameManager.Instance.PlayerOne.GetFastTanksKilled() +
            //                            GameManager.Instance.PlayerOne.GetMediumTanksKilled() +
            //                            GameManager.Instance.PlayerOne.GetStrongTanksKilled();
            playerTwoTotalTanksKilled = GameManager.Instance.GetPlayerTwoStats()[0] +
                                        GameManager.Instance.GetPlayerTwoStats()[1] +
                                        GameManager.Instance.GetPlayerTwoStats()[2] +
                                        GameManager.Instance.GetPlayerTwoStats()[3];

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
