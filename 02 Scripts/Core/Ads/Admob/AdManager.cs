using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

namespace Core.Ads.Admob {
    public class AdManager : MonoBehaviour
    {
        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<AdManager>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            RewardedAdView rewardedAdView = GameObject.FindObjectOfType<RewardedAdView>();
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            MobileAds.Initialize((initStatus) => {
                rewardedAdView.Initialize();
            });
        }
        #endregion
    }
}