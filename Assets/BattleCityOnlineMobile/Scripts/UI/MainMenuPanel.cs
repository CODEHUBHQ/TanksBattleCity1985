using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuPanel : MonoBehaviour
{
    public static MainMenuPanel Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OnMainMenuEnteredAnimationStarted()
    {
        FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include).gameObject.SetActive(false);
    }

    public void OnMainMenuEnteredAnimationEnded()
    {
        FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include).gameObject.SetActive(true);
    }
}
