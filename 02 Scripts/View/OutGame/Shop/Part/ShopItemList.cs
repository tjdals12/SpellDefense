using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace View.OutGame.Shop.Part {
    using UserModel = Model.OutGame.IUserModel;
    using ShopController = Controller.OutGame.ShopController;
    using ShopPart = Model.OutGame.IShopPart;
    using PartGrade = Repository.Part.PartGrade;
    using PartType = Repository.Part.PartType;
    using RewardPopup = Common.RewardPopup;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class ShopItemList : MonoBehaviour
    {
        UserModel userModel;
        ShopController shopController;

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
        ConfirmPurchasePopup confirmPurchasePopup;
        [SerializeField]
        RewardPopup rewardPopup;

        [Header("Prefab - Common")]
        [Space(4)]
        [SerializeField]
        GameObject commonWeaponPrefab;
        [SerializeField]
        GameObject commonArmorPrefab;
        [SerializeField]
        GameObject commonJewelryPrefab;

        [Header("Prefab - Uncommon")]
        [Space(4)]
        [SerializeField]
        GameObject uncommonWeaponPrefab;
        [SerializeField]
        GameObject uncommonArmorPrefab;
        [SerializeField]
        GameObject uncommonJewelryPrefab;

        [Header("Prefab - Rare")]
        [Space(4)]
        [SerializeField]
        GameObject rareWeaponPrefab;
        [SerializeField]
        GameObject rareArmorPrefab;
        [SerializeField]
        GameObject rareJewelryPrefab;

        [Header("Prefab - Epic")]
        [Space(4)]
        [SerializeField]
        GameObject epicWeaponPrefab;
        [SerializeField]
        GameObject epicArmorPrefab;
        [SerializeField]
        GameObject epicJewelryPrefab;


        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        Dictionary<int, Part> parts;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.shopController = GameObject.FindObjectOfType<ShopController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.ShowShop();
        }
        void OnEnable() {
            this.userModel.OnResetPartShop += this.OnResetPartShop;
            this.userModel.OnBuyPartInShop += this.OnBuyPartInShop;
        }
        void OnDisable() {
            this.userModel.OnResetPartShop -= this.OnResetPartShop;
            this.userModel.OnBuyPartInShop -= this.OnBuyPartInShop;
        }
        #endregion

        #region Event Listeners
        void OnResetPartShop() {
            this.ShowNewShop();
        }
        void OnBuyPartInShop(int index) {
            this.confirmPurchasePopup.Close();
            var partShop = this.userModel.user.partShop;
            ShopPart shopItem = Array.Find(partShop.shopItems, (shopItem) => shopItem.index == index);
            Part part = this.parts[shopItem.index];
            part.UpdateView(shopItem);
        }
        #endregion

        void ShowShop() {
            if (this.parts == null) {
                this.parts = new();
            } else {
                foreach (var part in parts.Values) {
                    Destroy(part.gameObject);
                }
                this.parts = new();
            }
            var partShop = this.userModel.user.partShop;
            foreach (var shopItem in partShop.shopItems) {
                this.CreatePart(shopItem);
            }
        }

        void ShowNewShop() {
            foreach (var part in parts.Values) {
                Destroy(part.gameObject);
            }
            this.parts = new();
            var partShop = this.userModel.user.partShop;
            foreach (var shopItem in partShop.shopItems) {
                this.CreatePart(shopItem);
            }
            Sequence sequence = DOTween.Sequence();
            foreach (var part in parts.Values) {
                sequence.Join(part.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack).SetDelay(0.1f));
            }
        }

        GameObject GetPrefab(PartType partType, PartGrade partGrade) {
            GameObject prefab = null;
            if (partType == PartType.Weapon) {
                switch (partGrade) {
                    case PartGrade.Common:
                        prefab = commonWeaponPrefab;
                        break;
                    case PartGrade.Uncommon:
                        prefab = uncommonWeaponPrefab;
                        break;
                    case PartGrade.Rare:
                        prefab = rareWeaponPrefab;
                        break;
                    case PartGrade.Epic:
                        prefab = epicWeaponPrefab;
                        break;
                    default:
                        prefab = commonWeaponPrefab;
                        break;
                }
            } else if (partType == PartType.Armor) {
                switch (partGrade) {
                    case PartGrade.Common:
                        prefab = commonArmorPrefab;
                        break;
                    case PartGrade.Uncommon:
                        prefab = uncommonArmorPrefab;
                        break;
                    case PartGrade.Rare:
                        prefab = rareArmorPrefab;
                        break;
                    case PartGrade.Epic:
                        prefab = epicArmorPrefab;
                        break;
                    default:
                        prefab = commonArmorPrefab;
                        break;
                }
            } else if (partType == PartType.Jewelry) {
                switch (partGrade) {
                    case PartGrade.Common:
                        prefab = commonJewelryPrefab;
                        break;
                    case PartGrade.Uncommon:
                        prefab = uncommonJewelryPrefab;
                        break;
                    case PartGrade.Rare:
                        prefab = rareJewelryPrefab;
                        break;
                    case PartGrade.Epic:
                        prefab = epicJewelryPrefab;
                        break;
                    default:
                        prefab = commonJewelryPrefab;
                        break;
                }
            }
            return prefab;
        }

        void CreatePart(ShopPart shopItem) {
            GameObject prefab = this.GetPrefab(shopItem.part.type, shopItem.part.grade);
            GameObject clone = Instantiate(prefab, this.shopItemListPanel.transform);
            Part part = clone.GetComponent<Part>();
            part.Initialize(
                shopItem,
                onClick: (shopPart) => {
                    this.soundManager.Click();
                    this.BuyShopItem(shopPart);
                }
            );
            this.parts[shopItem.index] = part;
        }

        void BuyShopItem(ShopPart shopItem) {
            bool isEnoughCost = this.userModel.user.IsEnoughCost(shopItem.cost);
            this.confirmPurchasePopup.Open(
                shopItem,
                isEnoughCost,
                onConfirm: () => {
                    this.loadingPopup.Open();
                    this.shopController.BuyPartInShop(shopItem.index);
                }
            );
        }

    }
}
