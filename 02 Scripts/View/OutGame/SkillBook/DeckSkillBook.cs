using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.SkillBook {
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;

    public class DeckSkillBook : MonoBehaviour
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
        GameObject upgradeIcon;

        Color defaultColor = new Color32(0, 169, 255, 255);
        Color maxColor = new Color32(255, 50, 0, 255);

        InventorySkillBook inventorySkillBook;

        public void Initialize(InventorySkillBook inventorySkillBook, Action<InventorySkillBook> onClick) {
            this.inventorySkillBook = inventorySkillBook;
            this.iconImage.sprite = inventorySkillBook.skillBook.image;
            this.levelText.text = inventorySkillBook.level.ToString();
            var upgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (upgradeSpec != null) {
                this.levelImage.color = this.defaultColor;
                if (inventorySkillBook.amount >= upgradeSpec.requiredAmount) {
                    this.upgradeIcon.SetActive(true);
                } else {
                    this.upgradeIcon.SetActive(false);
                }
            } else {
                this.levelImage.color = this.maxColor;
                this.upgradeIcon.SetActive(false);
            }
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke(this.inventorySkillBook));
        }

        public void UpdateView(InventorySkillBook inventorySkillBook) {
            this.inventorySkillBook = inventorySkillBook;
            this.levelText.text = inventorySkillBook.level.ToString();
            var upgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (upgradeSpec != null) {
                this.levelImage.color = this.defaultColor;
                if (inventorySkillBook.amount >= upgradeSpec.requiredAmount) {
                    this.upgradeIcon.SetActive(true);
                } else {
                    this.upgradeIcon.SetActive(false);
                }
            } else {
                this.levelImage.color = this.maxColor;
                this.upgradeIcon.SetActive(false);
            }
        }
    }
}
