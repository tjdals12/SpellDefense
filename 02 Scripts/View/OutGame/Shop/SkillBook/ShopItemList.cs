using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Shop.SkillBook {
    using UserModel = Model.OutGame.IUserModel;
    using ShopController = Controller.OutGame.ShopController;
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;
    using ShopSkillBook = Model.OutGame.IShopSkillBook;
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;
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

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject commonSkillBookPrefab;
        [SerializeField]
        GameObject uncommonSkillBookPrefab;
        [SerializeField]
        GameObject rareSkillBookPrefab;
        [SerializeField]
        GameObject epicSkillBookPrefab;

        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        Dictionary<int, SkillBook> skillBooks;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.shopController = GameObject.FindObjectOfType<ShopController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.ShowShop();
        }
        void OnEnable() {
            this.userModel.OnResetSkillBookShop += this.OnResetSkillBookShop;
            this.userModel.OnEarnSkillBook += this.OnEarnSkillBook;
            this.userModel.OnBuySkillBookInShop += this.OnBuySkillBookInShop;
        }
        void OnDisable() {
            this.userModel.OnResetSkillBookShop -= this.OnResetSkillBookShop;
            this.userModel.OnEarnSkillBook -= this.OnEarnSkillBook;
            this.userModel.OnBuySkillBookInShop -= this.OnBuySkillBookInShop;
        }
        #endregion

        #region Event Listeners
        void OnResetSkillBookShop() {
            this.ShowNewShop();
        }
        void OnEarnSkillBook(int id) {
            InventorySkillBook inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == id);
            var skillBookShop = this.userModel.user.skillBookShop;
            ShopSkillBook[] shopItems = Array.FindAll(skillBookShop.shopItems, (shopItem) => shopItem.skillBook.id == id);
            foreach (var shopItem in shopItems) {
                SkillBook skillBook = this.skillBooks[shopItem.index];
                skillBook.UpdateView(shopItem, inventorySkillBook);
            }
        }
        void OnBuySkillBookInShop(int index) {
            this.confirmPurchasePopup.Close();
            var skillBookShop = this.userModel.user.skillBookShop;
            ShopSkillBook shopItem = Array.Find(skillBookShop.shopItems, (shopItem) => shopItem.index == index);
            SkillBook skillBook = this.skillBooks[shopItem.index];
            InventorySkillBook inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == shopItem.skillBook.id);
            skillBook.UpdateView(shopItem, inventorySkillBook);
        }
        #endregion

        void ShowShop() {
            if (this.skillBooks == null) {
                this.skillBooks = new();
            } else {
                foreach (var skillBook in skillBooks.Values) {
                    Destroy(skillBook.gameObject);
                }
                this.skillBooks = new();
            }
            var skillBookShop = this.userModel.user.skillBookShop;
            foreach (var shopItem in skillBookShop.shopItems) {
                this.CreateSkillBook(shopItem);
            }
        }

        void ShowNewShop() {
            foreach (var skillBook in skillBooks.Values) {
                Destroy(skillBook.gameObject);
            }
            this.skillBooks = new();
            var skillBookShop = this.userModel.user.skillBookShop;
            foreach (var shopItem in skillBookShop.shopItems) {
                this.CreateSkillBook(shopItem);
            }
            Sequence sequence = DOTween.Sequence();
            foreach (var skillBook in skillBooks.Values) {
                sequence.Join(skillBook.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack).SetDelay(0.1f));
            }
        }

        GameObject GetPrefab(SkillBookGrade skillBookGrade) {
            switch (skillBookGrade) {
                case SkillBookGrade.Common:
                    return this.commonSkillBookPrefab;
                case SkillBookGrade.Uncommon:
                    return this.uncommonSkillBookPrefab;
                case SkillBookGrade.Rare:
                    return this.rareSkillBookPrefab;
                case SkillBookGrade.Epic:
                    return this.epicSkillBookPrefab;
                default:
                    return this.commonSkillBookPrefab;
            }
        }

        void CreateSkillBook(ShopSkillBook shopItem) {
            InventorySkillBook inventorySkillBook =  Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == shopItem.skillBook.id);
            GameObject prefab = this.GetPrefab(inventorySkillBook.skillBook.grade);
            GameObject clone = Instantiate(prefab, this.shopItemListPanel.transform);
            SkillBook skillBook = clone.GetComponent<SkillBook>();
            skillBook.Initialize(
                shopItem,
                inventorySkillBook,
                onClick: (shopSkillBook) => {
                    this.soundManager.Click();
                    this.BuyShopItem(shopSkillBook);
                }
            );
            this.skillBooks[shopItem.index] = skillBook;
        }

        void BuyShopItem(ShopSkillBook shopItem) {
            bool isEnoughCost = this.userModel.user.IsEnoughCost(shopItem.cost);
            this.confirmPurchasePopup.Open(
                shopItem,
                isEnoughCost,
                onConfirm: () => {
                    this.loadingPopup.Open();
                    this.shopController.BuySkillBookInShop(shopItem.index);
                }
            );
        }
    }
}
