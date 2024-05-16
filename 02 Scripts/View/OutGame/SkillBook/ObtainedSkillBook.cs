using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.SkillBook {
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;

    public class ObtainedSkillBook : MonoBehaviour
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
        GameObject upgradeIcon;
        [SerializeField]
        GameObject equippedIcon;

        Color defaultColor = new Color32(0, 169, 255, 255);
        Color upgradeColor = new Color32(0, 235, 0, 255);
        Color maxColor = new Color32(255, 50, 0, 255);

        InventorySkillBook inventorySkillBook;

        public void Initialize(InventorySkillBook inventorySkillBook, Action<InventorySkillBook> onClick) {
            this.inventorySkillBook = inventorySkillBook;
            this.iconImage.sprite = inventorySkillBook.skillBook.image;
            this.levelText.text = inventorySkillBook.level.ToString();
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.levelImage.color = this.maxColor;
                this.amountImage.color = this.maxColor;
                this.amountText.text = "MAX";
                this.upgradeIcon.SetActive(false);
            } else {
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
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke(this.inventorySkillBook));
        }

        public void UpdateView(InventorySkillBook inventorySkillBook) {
            this.inventorySkillBook = inventorySkillBook;
            this.levelText.text = inventorySkillBook.level.ToString();
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.levelImage.color = this.maxColor;
                this.amountImage.color = this.maxColor;
                this.amountText.text = "MAX";
                this.upgradeIcon.SetActive(false);
            } else {
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
        }

        public void Equip() {
            this.equippedIcon.SetActive(true);
        }

        public void Unequip() {
            this.equippedIcon.SetActive(false);
        }
    }
}
