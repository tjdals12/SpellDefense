using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.SkillBook {
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;

    public class SkillBook : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
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

        Color defaultColor = new Color32(0, 169, 255, 255);
        Color upgradeColor = new Color32(0, 235, 0, 255);
        Color maxColor = new Color32(255, 50, 0, 255);

        public void Initialize(InventorySkillBook inventorySkillBook) {
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
        }

        public void UpdateView(InventorySkillBook inventorySkillBook) {
            this.levelText.text = inventorySkillBook.level.ToString();
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.levelImage.color = this.maxColor;
                this.amountImage.color = this.maxColor;
                this.amountText.text = "MAX";
                this.upgradeIcon.SetActive(false);
            } else {
                this.amountText.text = $"{inventorySkillBook.amount} / {currentUpgradeSpec.requiredAmount}";
                float fillAmount = 0;
                if (inventorySkillBook.amount >= currentUpgradeSpec.requiredAmount) {
                    this.amountImage.color = this.upgradeColor;
                    this.upgradeIcon.SetActive(true);
                    fillAmount = 1;
                } else {
                    this.amountImage.color = this.defaultColor;
                    this.upgradeIcon.SetActive(false);
                    fillAmount = (float)inventorySkillBook.amount / currentUpgradeSpec.requiredAmount;
                }
                this.amountImage.DOFillAmount(fillAmount, 0.3f).From(0);
            }
        }
    }
}
