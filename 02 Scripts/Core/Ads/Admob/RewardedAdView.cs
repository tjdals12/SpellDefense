using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace Core.Ads.Admob {
    public class RewardedAdView : MonoBehaviour
    {
    #if UNITY_ANDROID
        string adUnitId = "unused";
    #elif UNITY_IPHONE
        string adUnitId = "unused";
    #else
        string adUnitId = "unused";
    #endif

        RewardedAd rewardedAd;

        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<RewardedAdView>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        #endregion

        public void Initialize() {
            this.LoadAd();
        }

        public void Show(Action onReward) {
            if (this.rewardedAd != null || this.rewardedAd.CanShowAd()) {
                this.rewardedAd.Show((Reward reward) => {
                    onReward?.Invoke();
                });
            }
        }

        void LoadAd() {
            if (this.rewardedAd != null) {
                this.rewardedAd.Destroy();
                this.rewardedAd = null;
            }
            Debug.Log("Loading the rewarded ad.");

            var adRequest = new AdRequest();

            RewardedAd.Load(this.adUnitId, adRequest, (RewardedAd ad, LoadAdError error) => {
                if (error != null || ad == null) {
                    Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                    return;
                }
                Debug.Log("Rewarded ad loaded with response" + ad.GetResponseInfo());
                this.RegisterHandlers(ad);
                this.rewardedAd = ad;
            });
        }

        void RegisterHandlers(RewardedAd ad) {
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () => {
                Debug.Log("Rewarded Ad full screen content closed.");
                // Reload the ad so that we can show another as soon as possible.
                this.LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) => {
                Debug.LogError("Rewarded ad failed to open full screen content with error : " + error);
                // Reload the ad so that we can show another as soon as possible.
                this.LoadAd();
            };
        }
    }
}
