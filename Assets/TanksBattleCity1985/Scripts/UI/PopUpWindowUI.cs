using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpWindowUI : MonoBehaviour
{
    public static PopUpWindowUI Instance { get; private set; }

    [SerializeField] private Transform popupWindowContainer;

    [SerializeField] private TMP_Text messageText;

    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;

        closeButton.onClick.AddListener(CloseButtonOnClick);
    }

    public void ShowPopUpWindow(string message)
    {
        messageText.text = $"{message}";
        popupWindowContainer.gameObject.SetActive(true);
        MapEditorHandler.Instance.SetIsPaused(true);
    }

    public void HidePopUpWindow()
    {
        messageText.text = $"";
        popupWindowContainer.gameObject.SetActive(false);
        MapEditorHandler.Instance.SetIsPaused(false);
    }

    public void CloseButtonOnClick()
    {
        HidePopUpWindow();
    }
}
