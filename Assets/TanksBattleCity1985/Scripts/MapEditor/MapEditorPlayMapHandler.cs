using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorPlayMapHandler : MonoBehaviour
{
    public static MapEditorPlayMapHandler Instance { get; private set; }

    public List<Button> PlayMapMenuOrderedButtons { get => playMapMenuOrderedButtons; }

    [SerializeField] private Transform playMapContainer;
    [SerializeField] private Transform playMapSelectGameModeContainer;
    [SerializeField] private Transform customMapsScrollviewContent;

    [SerializeField] private Button mapNameButtonTemplate;
    [SerializeField] private Button closeButton;

    [SerializeField] private TMP_Text selectedMapText;

    [SerializeField] private ComingSoonUI comingSoonUI;

    [Tooltip("The buttons of play map menu, should be ordered from top to bottom")]
    [SerializeField] private List<Button> playMapMenuOrderedButtons;

    private List<string> playMapMenuOrderedButtonsMethodNames = new List<string>();

    private string selectedMap;

    private void Awake()
    {
        Instance = this;

        mapNameButtonTemplate.gameObject.SetActive(false);

        playMapMenuOrderedButtonsMethodNames.Add(nameof(PlayerOneButtonOnClick));
        playMapMenuOrderedButtonsMethodNames.Add(nameof(PlayTwoPlayerButtonOnClick));
        playMapMenuOrderedButtonsMethodNames.Add(nameof(PlayerMultiplayerButtonOnClick));
        playMapMenuOrderedButtonsMethodNames.Add(nameof(BackButtonOnClick));

        for (int i = 0; i < playMapMenuOrderedButtons.Count; i++)
        {
            var index = i; // we use copy to solve closure issue

            playMapMenuOrderedButtons[i].onClick.AddListener(() =>
            {
                SendMessage(playMapMenuOrderedButtonsMethodNames[index], SendMessageOptions.DontRequireReceiver);
            });
        }
    }

    private void Start()
    {
        LoadCustomMaps();
    }

    private void PlayerOneButtonOnClick()
    {
        Debug.Log($"PlayerOneButtonOnClick");

        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.GameMode = GameMode.SinglePlayer;
        }

        PlayerPrefs.SetString(StaticStrings.CUSTOM_MAP, selectedMap);
        PlayerPrefs.SetString(StaticStrings.IS_CUSTOM_MAP, "true");
        PlayerPrefs.Save();

        LoadingManager.LoadScene(LoadingManager.Scene.GameScene);
    }

    private void PlayTwoPlayerButtonOnClick()
    {
        Debug.Log($"PlayTwoPlayerButtonOnClick");
        comingSoonUI.Show(ComingSoonOpts.Multiplayer);

        //if (NetworkManager.Instance == null)
        //{
        //    return;
        //}

        //NetworkManager.Instance.GameMode = GameMode.LocalMultiplayer;

        //PlayerPrefs.SetString(StaticStrings.CUSTOM_MAP, selectedMap);
        //PlayerPrefs.SetString(StaticStrings.IS_CUSTOM_MAP, "true");
        //PlayerPrefs.Save();

        //LoadingManager.LoadScene(LoadingManager.Scene.GameScene);
    }

    private void PlayerMultiplayerButtonOnClick()
    {
        Debug.Log($"PlayerMultiplayerButtonOnClick");

        comingSoonUI.Show(ComingSoonOpts.Multiplayer);
    }

    private void BackButtonOnClick()
    {
        Debug.Log($"BackButtonOnClick");
        playMapSelectGameModeContainer.gameObject.SetActive(false);
    }

    public void LoadCustomMaps()
    {
        foreach (Transform child in customMapsScrollviewContent)
        {
            if (child == mapNameButtonTemplate.transform) continue;

            Destroy(child.gameObject);
        }

        if (MapEditorHandler.Instance.GetCustomMaps().Count > 0)
        {
            for (int i = 0; i < MapEditorHandler.Instance.GetCustomMaps().Count; i++)
            {
                var customMap = MapEditorHandler.Instance.GetCustomMaps()[i];

                var mapNameButtonGameObject = Instantiate(mapNameButtonTemplate, customMapsScrollviewContent);

                mapNameButtonGameObject.GetComponentInChildren<TMP_Text>().text = $"map{i} - {BattleCityUtils.GetHashString(customMap)}";

                mapNameButtonGameObject.gameObject.SetActive(true);

                mapNameButtonGameObject.transform.Find("MapPreview").GetComponent<Image>().sprite = MapEditorHandler.Instance.GetCustomMapSpriteByIndex(i);

                if (i == 0)
                {
                    mapNameButtonGameObject.GetComponent<Button>().Select();
                }

                var index = i; // we use copy to solve closure issue

                mapNameButtonGameObject.onClick.AddListener(() =>
                {
                    PlayMapByIndex(index);
                });
            }
        }
        else
        {
            closeButton.Select();
        }
    }

    public void PlayMapByIndex(int index)
    {
        playMapContainer.gameObject.SetActive(false);
        playMapSelectGameModeContainer.gameObject.SetActive(true);

        playMapMenuOrderedButtons[0].Select();
        selectedMap = MapEditorHandler.Instance.GetCustomMaps()[index];
        selectedMapText.text = $"map{index} - {BattleCityUtils.GetHashString(MapEditorHandler.Instance.GetCustomMaps()[index])}";
    }

    public bool IsOpen()
    {
        return playMapContainer.gameObject.activeSelf;
    }
}
