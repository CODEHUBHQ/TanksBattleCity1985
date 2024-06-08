using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static InterstitialAds Instance { get; private set; }

    [SerializeField] private string androidAdUnitId = "Interstitial_Android";
    [SerializeField] private string iOsAdUnitId = "Interstitial_iOS";

    private Action onAddLoaded;

    private string adUnitId;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    // Load content to the Ad Unit:
    public void LoadAd(Action onAddLoaded = null)
    {
        this.onAddLoaded = onAddLoaded;

        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        adUnitId = iOsAdUnitId;
#elif UNITY_ANDROID
        adUnitId = androidAdUnitId;
#endif

        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + adUnitId);
        Advertisement.Load(adUnitId, this);
    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + adUnitId);
        Advertisement.Show(adUnitId, this);
    }

    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(this.adUnitId))
        {
            if (onAddLoaded != null)
            {
                var tmpAction = onAddLoaded;

                onAddLoaded = null;

                tmpAction?.Invoke();
            }
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("Ad started: " + adUnitId);
        Time.timeScale = 0f;
    }
    
    public void OnUnityAdsShowClick(string adUnitId)
    {
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Ad completed: " + adUnitId);
        Time.timeScale = 1f;
    }
}