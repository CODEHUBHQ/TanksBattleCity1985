using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuIconOnClick : MonoBehaviour
{
    public void OnMenuButtonClicked()
    {
        if (!GameManager.Instance.IsGamePaused())
        {
            SoundManager.Instance.PlayPauseSound();
        }

        GameManager.Instance.ToggleGameIsPaused();
        GameMenuUI.Instance.ToggleGameMenu();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
