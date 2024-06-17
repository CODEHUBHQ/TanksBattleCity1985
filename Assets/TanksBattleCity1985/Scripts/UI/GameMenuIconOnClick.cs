using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuIconOnClick : MonoBehaviour
{
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void OnMenuButtonClicked()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.GameMode == GameMode.Multiplayer)
        {
            photonView.RPC(nameof(OnMenuButtonClickedPunRPC), RpcTarget.All);
        }
        else
        {
            if (!GameManager.Instance.IsGamePaused())
            {
                SoundManager.Instance.PlayPauseSound();
            }

            GameManager.Instance.ToggleGameIsPaused();
            GameMenuUI.Instance.ToggleGameMenu();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void OnMenuButtonClickedPunRPC()
    {
        if (!GameManager.Instance.IsGamePaused())
        {
            SoundManager.Instance.PlayPauseSound();
        }

        GameManager.Instance.ToggleGameIsPaused();
        GameMenuUI.Instance.ToggleGameMenu();
    }
}
