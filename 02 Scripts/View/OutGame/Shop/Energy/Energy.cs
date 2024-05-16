using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Shop.Energy {
    using ShopEnergy = Model.OutGame.IShopEnergy;

    public class Energy : MonoBehaviour
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

        public void Initialize(ShopEnergy shopEnergy, Action<ShopEnergy> onClick) {
            this.SetItem(shopEnergy);
            this.SetCost(shopEnergy);
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke(shopEnergy));
            if (shopEnergy.remainBuyCount == 0) {
                this.Disable();
            } else {
                this.Enable();
            }
        }

        public void UpdateView(ShopEnergy shopEnergy) {
            this.SetCost(shopEnergy);
            if (shopEnergy.remainBuyCount == 0) {
                this.Disable();
            } else {
                this.Enable();
            }
        }

        void SetItem(ShopEnergy shopEnergy) {
            this.iconImage.sprite = shopEnergy.energy.item.image;
            this.salesAmountText.text = $"X {shopEnergy.energy.amount}";
        }

        void SetCost(ShopEnergy shopEnergy) {
            this.costIconImage.sprite = shopEnergy.cost.item.image;
            switch (shopEnergy.cost.item.id) {
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
                    this.costAmountText.text = $"{shopEnergy.remainBuyCount} / {shopEnergy.maxBuyCount}";
                    this.freeText.SetActive(false);
                    break;
                default:
                    this.costIconImage.gameObject.SetActive(true);
                    this.costAmountText.gameObject.SetActive(true);
                    this.costAmountText.text = shopEnergy.cost.amount.ToString();
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
