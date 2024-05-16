using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Part {
    using InventoryPart = Model.OutGame.IInventoryPart;

    public class Part : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI levelText;
        [SerializeField]
        Image expBarImage;
        [SerializeField]
        TextMeshProUGUI expText;

        InventoryPart inventoryPart;

        public void Initialize(InventoryPart inventoryPart) {
            this.inventoryPart = inventoryPart;
            this.iconImage.sprite = inventoryPart.part.image;
            this.levelText.text = $"Lv. {inventoryPart.level}";
            var upgradeSpec = inventoryPart.GetCurrentUpgradeSpec();
            var prevUpgradeSpec = Array.Find(inventoryPart.part.upgradeSpecs, (e) => e.level == upgradeSpec.level - 1);
            int percent = Mathf.RoundToInt((inventoryPart.exp - prevUpgradeSpec.requiredExp) / (upgradeSpec.requiredExp - prevUpgradeSpec.requiredExp));
            if (percent == 1) {
                this.expBarImage.transform.localScale = Vector3.one;
            } else if (percent == 0) {
                this.expBarImage.transform.localScale = Vector3.zero;
            } else {
                this.expBarImage.transform.localScale = new Vector3(percent, 1, 1);
            }
            this.expText.text = $"{percent * 100}%";
        }

        public void UpdateView(int earnExp) {
            if (earnExp == 0) {
                var upgradeSpec = this.inventoryPart.GetCurrentUpgradeSpec();
                var prevUpgradeSpec = Array.Find(this.inventoryPart.part.upgradeSpecs, (e) => e.level == upgradeSpec.level - 1);
                this.levelText.text = $"Lv. {this.inventoryPart.level}";
                float percent = (float)((inventoryPart.exp - prevUpgradeSpec.requiredExp) / (upgradeSpec.requiredExp - prevUpgradeSpec.requiredExp));
                this.expBarImage.transform.localScale = new Vector3(percent, 1, 1);
                this.expText.text = $"{Mathf.RoundToInt(percent * 100)}%";
            } else {
                int totalExp = this.inventoryPart.exp + earnExp;
                var upgradeSpec = Array.Find(this.inventoryPart.part.upgradeSpecs, (e) => e.requiredExp >= totalExp);
                var prevUpgradeSpec = Array.Find(this.inventoryPart.part.upgradeSpecs, (e) => e.level == upgradeSpec.level - 1);
                int upLevel = prevUpgradeSpec.level - this.inventoryPart.level;
                float percent = (float)(totalExp - prevUpgradeSpec.requiredExp) / (upgradeSpec.requiredExp - prevUpgradeSpec.requiredExp);
                if (upLevel == 0) {
                    if (percent == 1) {
                        this.levelText.text = $"Lv. {this.inventoryPart.level} <size=26><color=green>(+1)</size></color>";
                    } else {
                        this.levelText.text = $"Lv. {this.inventoryPart.level}";
                    }
                } else {
                    if (percent == 1) {
                        this.levelText.text = $"Lv. {this.inventoryPart.level} <size=26><color=green>(+{upLevel + 1})</size></color>";
                    } else {
                        this.levelText.text = $"Lv. {this.inventoryPart.level} <size=26><color=green>(+{upLevel})</size></color>";
                    }
                }
                this.expBarImage.transform.localScale = new Vector3(percent, 1, 1);
                this.expText.text = $"{Mathf.RoundToInt(percent * 100)}%";
            }
        }

        public void UpdateView(InventoryPart inventoryPart) {
            this.Initialize(inventoryPart);
        }
    }
}
