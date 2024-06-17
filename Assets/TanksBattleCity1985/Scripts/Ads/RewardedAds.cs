using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static RewardedAds Instance { get; private set; }

    [SerializeField] private string androidAdUnitId = "Rewarded_Android";
    [SerializeField] private string iOSAdUnitId = "Rewarded_iOS";

    private Action onAddLoaded;
    private Action onAddComplete;

    private string adUnitId; // This will remain null for unsupported platforms

    private void Awake()
    {
        Instance = this;
    }

    // Call this public method when you want to get an ad ready to show.
    public void LoadAd(Action onAddLoaded = null)
    {
#if !UNITY_STANDALONE
        this.onAddLoaded = onAddLoaded;

        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        adUnitId = iOSAdUnitId;
#elif UNITY_ANDROID
        adUnitId = androidAdUnitId;
#endif

        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + adUnitId);
        Advertisement.Load(adUnitId, this);
#endif
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
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

    // Implement a method to execute when the user clicks the button:
    public void ShowAd(Action onAddComplete = null)
    {
#if !UNITY_STANDALONE
        this.onAddComplete = onAddComplete;

        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + adUnitId);
        Advertisement.Show(adUnitId, this);
#endif
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Ad completed: " + adUnitId);
        Time.timeScale = 1f;

        if (adUnitId.Equals(this.adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            
            if (onAddComplete != null)
            {
                var tmpAction = onAddComplete;

                onAddComplete = null;

                tmpAction?.Invoke();
            }
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("Ad started: " + adUnitId);
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
    }
}