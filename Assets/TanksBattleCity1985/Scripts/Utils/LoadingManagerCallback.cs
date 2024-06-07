using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingManagerCallback : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;

    private bool isFirstUpdate = true;

    private float defaultLoadingTimer = 4f;

    private void Awake()
    {
        var isCustomMap = bool.Parse(PlayerPrefs.GetString(StaticStrings.IS_CUSTOM_MAP, "false"));

        if (LoadingManager.targetScene == LoadingManager.Scene.MenuScene)
        {
            levelText.text = $"Tanks: 1985";
        }
        else if (LoadingManager.targetScene == LoadingManager.Scene.LobbyScene)
        {
            levelText.text = $"Multiplayer Game";
        }
        else if (LoadingManager.targetScene == LoadingManager.Scene.MapEditorScene)
        {
            levelText.text = $"Map Editor";
        }
        else if (LoadingManager.targetScene == LoadingManager.Scene.SettingsScene)
        {
            levelText.text = $"Game Settings";
        }
        else if (isCustomMap)
        {
            levelText.text = $"Stage Custom";
        }
        else
        {
            var currentLevel = PlayerPrefs.GetString(StaticStrings.CURRENT_LEVEL, "1");

            levelText.text = $"Stage {currentLevel}";
        }
    }

    private void Update()
    {
        defaultLoadingTimer -= Time.deltaTime;

        if (defaultLoadingTimer <= 0f)
        {
            if (isFirstUpdate)
            {
                isFirstUpdate = false;

                LoadingManager.LoadingManagerCallback();
            }
        }
    }
}
