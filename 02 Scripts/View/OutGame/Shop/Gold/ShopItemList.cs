using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.OutGame.Shop.Gold {
    using UserModel = Model.OutGame.IUserModel;
    using ShopController = Controller.OutGame.ShopController;
    using ShopGold = Model.OutGame.IShopGold;
    using RewardedAdView = Core.Ads.Admob.RewardedAdView;
    using RewardPopup = Common.RewardPopup;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class ShopItemList : MonoBehaviour
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
        GameObject shopItemListPanel;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        RewardPopup rewardPopup;

        [Header("Prefab - Common")]
        [Space(4)]
        [SerializeField]
        GameObject goldPrefab;
        
        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        Dictionary<int, Gold> golds;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.shopController = GameObject.FindObjectOfType<ShopController>();
            this.rewardedAdView = GameObject.FindObjectOfType<RewardedAdView>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.ShowShop();
        }
        void OnEnable() {
            this.userModel.OnResetGoldShop += this.OnResetGoldShop;
            this.userModel.OnBuyGoldInShop += this.OnBuyGoldInShop;
        }
        void OnDisable() {
            this.userModel.OnResetGoldShop -= this.OnResetGoldShop;
            this.userModel.OnBuyGoldInShop -= this.OnBuyGoldInShop;
        }
        #endregion

        #region Event Listeners
        void OnResetGoldShop() {
            this.ShowShop();
        }
        void OnBuyGoldInShop(int index) {
            var goldShop = this.userModel.user.goldShop;
            ShopGold shopItem = Array.Find(goldShop.shopItems, (shopItem) => shopItem.index == index);
            Gold gold = this.golds[shopItem.index];
            gold.UpdateView(shopItem);
        }
        #endregion

        void ShowShop() {
            if (this.golds == null) {
                this.golds = new();
            } else {
                foreach (var gold in golds.Values) {
                    Destroy(gold.gameObject);
                }
                this.golds = new();
            }
            var goldShop = this.userModel.user.goldShop;
            foreach (var shopItem in goldShop.shopItems) {
                this.CreateGold(shopItem);
            }
        }

        void CreateGold(ShopGold shopItem) {
            GameObject clone = Instantiate(this.goldPrefab, this.shopItemListPanel.transform);
            Gold gold = clone.GetComponent<Gold>();
            gold.Initialize(
                shopItem,
                onClick: (shopGold) => {
                    this.soundManager.Click();
                    this.BuyShopItem(shopGold);
                }
            );
            this.golds[shopItem.index] = gold;
        }

        void BuyShopItem(ShopGold shopItem) {
            if (shopItem.cost.item.id == 4) {
                this.rewardedAdView.Show(onReward: () => {
                    this.loadingPopup.Open();
                    this.shopController.BuyGoldInShop(shopItem.index);
                });
            } else {
                this.loadingPopup.Open();
                this.shopController.BuyGoldInShop(shopItem.index);
            }
        }
    }
}
