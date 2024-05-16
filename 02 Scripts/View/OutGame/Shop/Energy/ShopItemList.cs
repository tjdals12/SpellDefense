using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.OutGame.Shop.Energy {
    using UserModel = Model.OutGame.IUserModel;
    using ShopController = Controller.OutGame.ShopController;
    using ShopEnergy = Model.OutGame.IShopEnergy;
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
        GameObject energyPrefab;
        
        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        Dictionary<int, Energy> energies;

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
            this.userModel.OnResetEnergyShop += this.OnResetEnergyShop;
            this.userModel.OnBuyEnergyInShop += this.OnBuyEnergyInShop;
        }
        void OnDisable() {
            this.userModel.OnResetEnergyShop -= this.OnResetEnergyShop;
            this.userModel.OnBuyEnergyInShop -= this.OnBuyEnergyInShop;
        }
        #endregion

        #region Event Listeners
        void OnResetEnergyShop() {
            this.ShowShop();
        }
        void OnBuyEnergyInShop(int index) {
            var energyShop = this.userModel.user.energyShop;
            ShopEnergy shopItem = Array.Find(energyShop.shopItems, (shopItem) => shopItem.index == index);
            Energy energy = this.energies[shopItem.index];
            energy.UpdateView(shopItem);
        }
        #endregion

        void ShowShop() {
            if (this.energies == null) {
                this.energies = new();
            } else {
                foreach (var energy in energies.Values) {
                    Destroy(energy.gameObject);
                }
                this.energies = new();
            }
            var energyShop = this.userModel.user.energyShop;
            foreach (var shopItem in energyShop.shopItems) {
                this.CreateEnergy(shopItem);
            }
        }

        void CreateEnergy(ShopEnergy shopItem) {
            GameObject clone = Instantiate(this.energyPrefab, this.shopItemListPanel.transform);
            Energy energy = clone.GetComponent<Energy>();
            energy.Initialize(
                shopItem,
                onClick: (shopEnergy) => {
                    this.soundManager.Click();
                    this.BuyShopItem(shopEnergy);
                }
            );
            this.energies[shopItem.index] = energy;
        }

        void BuyShopItem(ShopEnergy shopItem) {
            if (shopItem.cost.item.id == 4) {
                this.rewardedAdView.Show(onReward: () => {
                    this.loadingPopup.Open();
                    this.shopController.BuyEnergyInShop(shopItem.index);
                });
            } else {
                this.loadingPopup.Open();
                this.shopController.BuyEnergyInShop(shopItem.index);
            }
        }
    }
}
