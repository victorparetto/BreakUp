using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityMonetization : MonoBehaviour, IUnityAdsListener
{
    string gameId = "4208815";
    string myPlacementId = "rewardedVideo";
    bool testMode = true;

    public ShowResult showResult;

    // Initialize the Ads listener and service:
    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    public bool AdvertisementIsReady()
    {
        if (Advertisement.IsReady(myPlacementId)) return true;

        return false;
    }

    public void ShowRewardedVideo()
    {
        // Check if UnityAds ready before calling Show method:
        if (Advertisement.IsReady(myPlacementId))
        {
            Advertisement.Show(myPlacementId);
        }
        else
        {
            Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
        }
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult sr)
    {
        // Define conditional logic for each ad completion status:
        if (sr == ShowResult.Finished)
        {
            Debug.LogWarning("Ad finished!");
            showResult = sr;
            // Reward the user for watching the ad to completion.
        }
        else if (sr == ShowResult.Skipped)
        {
            Debug.LogWarning("Ad skipped!");
            showResult = sr;
            // Do not reward the user for skipping the ad.
        }
        else if (sr == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
            showResult = sr;
        }

        Time.timeScale = 1;
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId)
        {
            // Optional actions to take when the placement becomes ready(For example, enable the rewarded ads button)
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    // When the object that subscribes to ad events is destroyed, remove the listener:
    public void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }
}
