using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Shop.Part {
    using UserModel = Model.OutGame.IUserModel;
    using ShopController = Controller.OutGame.ShopController;
    using RewardedAdView = Core.Ads.Admob.RewardedAdView;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class ResetButton : MonoBehaviour
    {
        UserModel userModel;
        ShopController shopController;
        RewardedAdView rewardedAdView;

        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        GameObject enableImage;
        [SerializeField]
        GameObject disableImage;
        [SerializeField]
        TextMeshProUGUI remainCountText;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        ConfirmResetPopup confirmResetPopup;

        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.shopController = GameObject.FindObjectOfType<ShopController>();
            this.rewardedAdView = GameObject.FindObjectOfType<RewardedAdView>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.button.onClick.AddListener(this.ResetShop);
            this.UpdateView();
        }
        void OnEnable() {
            this.userModel.OnResetPartShop += this.OnResetPartShop;
        }
        void OnDisable() {
            this.userModel.OnResetPartShop -= this.OnResetPartShop;
        }
        #endregion

        #region Event Listeners
        void OnResetPartShop() {
            this.confirmResetPopup.Close();
            this.UpdateView();
        }
        #endregion

        void UpdateView() {
            var partShop = this.userModel.user.partShop;
            this.remainCountText.text = $"({partShop.remainResetCount} / {partShop.maxResetCount})";
            if (partShop.remainResetCount == 0) {
                this.enableImage.SetActive(false);
                this.disableImage.SetActive(true);
                this.button.enabled = false;
            } else {
                this.enableImage.SetActive(true);
                this.disableImage.SetActive(false);
                this.button.enabled = true;
            }
        }

        void ResetShop() {
            this.soundManager.Click();
            var partShop = this.userModel.user.partShop;
            bool canReset = partShop.remainResetCount > 0;
            this.confirmResetPopup.Open(
                canReset,
                onConfirm: this.WatchAds
            );
        }

        void WatchAds() {
            this.rewardedAdView.Show(onReward: () => {
                this.loadingPopup.Open();
                this.shopController.ResetPartShopByAds();
            });
        }
    }
}
