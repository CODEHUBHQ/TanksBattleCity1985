using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ComingSoonOpts
{
    Multiplayer,
    Shop,
}

public class ComingSoonUI : MonoBehaviour
{
    [SerializeField] private Transform comingSoonContainer;

    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button shopButton;

    private ComingSoonOpts comingSoonOpts;

    private void Awake()
    {
        comingSoonContainer.gameObject.SetActive(false);
    }

    public void CloseButtonOnClick()
    {
        comingSoonContainer.gameObject.SetActive(false);

        if (comingSoonOpts == ComingSoonOpts.Multiplayer && multiplayerButton != null)
        {
            multiplayerButton.Select();
        }
        else if (comingSoonOpts == ComingSoonOpts.Shop && shopButton != null)
        {
            shopButton.Select();
        }
    }

    public void Show(ComingSoonOpts comingSoonOpts)
    {
        this.comingSoonOpts = comingSoonOpts;

        comingSoonContainer.gameObject.SetActive(true);

        comingSoonContainer.gameObject.GetComponentInChildren<Button>().Select();
    }
}
