using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Shop.SkillBook {
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;
    using ShopSkillBook = Model.OutGame.IShopSkillBook;

    public class SkillBook : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        Image levelImage;
        [SerializeField]
        TextMeshProUGUI levelText;
        [SerializeField]
        Image amountImage;
        [SerializeField]
        TextMeshProUGUI amountText;
        [SerializeField]
        GameObject unobtainedText;
        [SerializeField]
        TextMeshProUGUI salesAmountText;
        [SerializeField]
        Image costIconImage;
        [SerializeField]
        TextMeshProUGUI costAmountText;
        [SerializeField]
        GameObject upgradeIcon;
        [SerializeField]
        GameObject mask;
        [SerializeField]
        GameObject checkIcon;

        Color defaultColor = new Color32(0, 169, 255, 255);
        Color upgradeColor = new Color32(0, 235, 0, 255);
        Color maxColor = new Color32(255, 50, 0, 255);

        public void Initialize(ShopSkillBook shopSkillBook, InventorySkillBook inventorySkillBook, Action<ShopSkillBook> onClick) {
            this.iconImage.sprite = inventorySkillBook.skillBook.image;
            if (inventorySkillBook.isObtained) {
                this.amountImage.gameObject.SetActive(true);
                this.amountText.gameObject.SetActive(true);
                this.unobtainedText.SetActive(false);
                this.levelText.text = inventorySkillBook.level.ToString();
                var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
                if (currentUpgradeSpec == null) {
                    this.levelImage.color = this.maxColor;
                    this.amountImage.color = this.maxColor;
                    this.amountImage.fillAmount = 1;
                    this.amountText.text = "MAX";
                    this.upgradeIcon.SetActive(false);
                } else {
                    this.levelImage.color = this.defaultColor;
                    this.amountText.text = $"{inventorySkillBook.amount} / {currentUpgradeSpec.requiredAmount}";
                    if (inventorySkillBook.amount >= currentUpgradeSpec.requiredAmount) {
                        this.amountImage.color = this.upgradeColor;
                        this.amountImage.fillAmount = 1;
                        this.upgradeIcon.SetActive(true);
                    } else {
                        this.amountImage.color = this.defaultColor;
                        this.amountImage.fillAmount = (float)inventorySkillBook.amount / currentUpgradeSpec.requiredAmount;
                        this.upgradeIcon.SetActive(false);
                    }
                }
            } else {
                this.levelImage.color = this.defaultColor;
                this.levelText.text = "1";
                this.amountImage.gameObject.SetActive(false);
                this.amountText.gameObject.SetActive(false);
                this.unobtainedText.gameObject.SetActive(true);
                this.upgradeIcon.SetActive(false);
            }
            this.salesAmountText.text = $"X {shopSkillBook.item.amount}";
            this.costIconImage.sprite = shopSkillBook.cost.item.image;
            this.costAmountText.text = shopSkillBook.cost.amount.ToString();
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke(shopSkillBook));
            if (shopSkillBook.remainBuyCount == 0) {
                this.button.enabled = false;
                this.Disable();
            } else {
                this.button.enabled = true;
                this.Enable();
            }
        }

        public void UpdateView(ShopSkillBook shopSkillBook, InventorySkillBook inventorySkillBook) {
            this.iconImage.sprite = inventorySkillBook.skillBook.image;
            if (inventorySkillBook.isObtained) {
                this.amountImage.gameObject.SetActive(true);
                this.amountText.gameObject.SetActive(true);
                this.unobtainedText.SetActive(false);
                this.levelText.text = inventorySkillBook.level.ToString();
                var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
                if (currentUpgradeSpec == null) {
                    this.levelImage.color = this.maxColor;
                    this.amountImage.color = this.maxColor;
                    this.amountImage.fillAmount = 1;
                    this.amountText.text = "MAX";
                    this.upgradeIcon.SetActive(false);
                } else {
                    this.levelImage.color = this.defaultColor;
                    this.amountText.text = $"{inventorySkillBook.amount} / {currentUpgradeSpec.requiredAmount}";
                    if (inventorySkillBook.amount >= currentUpgradeSpec.requiredAmount) {
                        this.amountImage.color = this.upgradeColor;
                        this.amountImage.fillAmount = 1;
                        this.upgradeIcon.SetActive(true);
                    } else {
                        this.amountImage.color = this.defaultColor;
                        this.amountImage.fillAmount = (float)inventorySkillBook.amount / currentUpgradeSpec.requiredAmount;
                        this.upgradeIcon.SetActive(false);
                    }
                }
            } else {
                this.levelImage.color = this.defaultColor;
                this.levelText.text = "1";
                this.amountImage.gameObject.SetActive(false);
                this.amountText.gameObject.SetActive(false);
                this.unobtainedText.gameObject.SetActive(true);
                this.upgradeIcon.SetActive(false);
            }
            this.salesAmountText.text = $"X {shopSkillBook.item.amount}";
            this.costIconImage.sprite = shopSkillBook.cost.item.image;
            this.costAmountText.text = shopSkillBook.cost.amount.ToString();
            if (shopSkillBook.remainBuyCount == 0) {
                this.button.enabled = false;
                this.Disable();
            } else {
                this.button.enabled = true;
                this.Enable();
            }
        }

        void Enable() {
            this.mask.SetActive(false);
            this.checkIcon.SetActive(false);
        }

        void Disable() {
            this.mask.SetActive(true);
            this.checkIcon.SetActive(true);
        }
    }
}