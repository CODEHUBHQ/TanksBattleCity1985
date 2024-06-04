using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private InterstitialAdsButton interstitialAdsOnePlayerButton;
    [SerializeField] private InterstitialAdsButton interstitialAdsMapEditorButton;

    [SerializeField] private string androidGameId;
    [SerializeField] private string iOSGameId;

    [SerializeField] private bool testMode = true;

    private string gameId;

    private void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
            gameId = iOSGameId;
#elif UNITY_ANDROID
        gameId = androidGameId;
#elif UNITY_EDITOR
            gameId = androidGameId; // Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }
    }


    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");

        interstitialAdsOnePlayerButton.LoadAd();
        interstitialAdsMapEditorButton.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}