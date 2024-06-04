using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEditorLoadMapHandler : MonoBehaviour
{
    public static MapEditorLoadMapHandler Instance { get; private set; }

    [SerializeField] private Transform loadMapContainer;
    [SerializeField] private Transform customMapsScrollviewContent;

    [SerializeField] private Button mapNameButtonTemplate;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;

        mapNameButtonTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        LoadCustomMaps();
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
                    LoadMapByIndex(index);
                });
            }
        }
        else
        {
            closeButton.Select();
        }
    }

    public void LoadMapByIndex(int index)
    {
        loadMapContainer.gameObject.SetActive(false);

        MapEditorHandler.Instance.LoadMapFromString(MapEditorHandler.Instance.GetCustomMaps()[index], index);

        MapEditorHandler.Instance.SetIsPaused(false);
    }

    public bool IsOpen()
    {
        return loadMapContainer.gameObject.activeSelf;
    }
}
