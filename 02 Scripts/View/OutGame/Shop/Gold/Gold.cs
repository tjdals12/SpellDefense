using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Shop.Gold {
    using ShopGold = Model.OutGame.IShopGold;

    public class Gold : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI salesAmountText;
        [SerializeField]
        Image costIconImage;
        [SerializeField]
        TextMeshProUGUI costAmountText;
        [SerializeField]
        GameObject freeText;
        [SerializeField]
        GameObject mask;
        [SerializeField]
        GameObject checkIcon;

        [Header("Sprite")]
        [Space(4)]
        [SerializeField]
        Sprite iconSprite_1000;
        [SerializeField]
        Sprite iconSprite_3000;
        [SerializeField]
        Sprite iconSprite_5000;

        public void Initialize(ShopGold shopGold, Action<ShopGold> onClick) {
            this.SetItem(shopGold);
            this.SetCost(shopGold);
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke(shopGold));
            if (shopGold.remainBuyCount == 0) {
                this.Disable();
            } else {
                this.Enable();
            }
        }

        public void UpdateView(ShopGold shopGold) {
            this.SetCost(shopGold);
            if (shopGold.remainBuyCount == 0) {
                this.Disable();
            } else {
                this.Enable();
            }
        }

        void SetItem(ShopGold shopGold) {
            if (shopGold.gold.amount >= 5000) {
                this.iconImage.sprite = this.iconSprite_5000;
            } else if (shopGold.gold.amount >= 3000) {
                this.iconImage.sprite = this.iconSprite_3000;
            } else {
                this.iconImage.sprite = this.iconSprite_1000;
            }
            this.salesAmountText.text = shopGold.gold.amount.ToString();
        }

        void SetCost(ShopGold shopGold) {
            this.costIconImage.sprite = shopGold.cost.item.image;
            switch (shopGold.cost.item.id) {
                // Free
                case 0:
                    this.costIconImage.gameObject.SetActive(false);
                    this.costAmountText.gameObject.SetActive(false);
                    this.freeText.SetActive(true);
                    break;
                // Ads
                case 4:
                    this.costIconImage.gameObject.SetActive(true);
                    this.costAmountText.gameObject.SetActive(true);
                    this.costAmountText.text = $"{shopGold.remainBuyCount} / {shopGold.maxBuyCount}";
                    this.freeText.SetActive(false);
                    break;
                default:
                    this.costIconImage.gameObject.SetActive(true);
                    this.costAmountText.gameObject.SetActive(true);
                    this.costAmountText.text = shopGold.cost.amount.ToString();
                    this.freeText.SetActive(false);
                    break;
            }
        }

        void Enable() {
            this.button.enabled = true;
            this.mask.SetActive(false);
            this.checkIcon.SetActive(false);
        }

        void Disable() {
            this.button.enabled = false;
            this.mask.SetActive(true);
            this.checkIcon.SetActive(true);
        }
    }
}
