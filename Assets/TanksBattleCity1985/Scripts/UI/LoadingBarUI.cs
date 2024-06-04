using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarUI : MonoBehaviour
{
    private Image loadingBar;

    private void Awake()
    {
        loadingBar = GetComponent<Image>();
    }

    private void Update()
    {
        loadingBar.fillAmount = LoadingManager.GetLoadingProgress();
    }
}
