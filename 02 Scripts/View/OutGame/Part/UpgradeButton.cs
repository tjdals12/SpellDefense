using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Part {
    using InventoryPart = Model.OutGame.IInventoryPart;

    public class UpgradeButton : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        GameObject enableImage;
        [SerializeField]
        GameObject disableImage;
        [SerializeField]
        GameObject goldIcon;
        [SerializeField]
        TextMeshProUGUI requiredGoldText;

        InventoryPart inventoryPart;

        public void Initialize(InventoryPart inventoryPart, int currentGold, Action onClick) {
            this.inventoryPart = inventoryPart;
            var currentUpgradeSpec = inventoryPart.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.goldIcon.SetActive(false);
                this.requiredGoldText.text = "MAX";
                this.requiredGoldText.color = new Color32(255, 50, 0, 255);
            } else {
                this.goldIcon.SetActive(true);
                this.requiredGoldText.text = currentUpgradeSpec.requiredGold.ToString();
                this.requiredGoldText.color = new Color32(255, 255, 255, 255);
                if (inventoryPart.exp >= currentUpgradeSpec.requiredExp && currentGold >= currentUpgradeSpec.requiredGold) {
                    this.Enable();
                } else {
                    this.Disable();
                }
            }
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke());
        }

        public void UpdateView(int earnExp, int currentGold) {
            int totalExp = this.inventoryPart.exp + earnExp;
            var upgradeSpecs = Array.FindAll(this.inventoryPart.part.upgradeSpecs, (e) => e.level > this.inventoryPart.level && totalExp >= e.requiredExp);
            if (upgradeSpecs.Length > 0) {
                int totalRequiredGold = 0;
                foreach (var upgradeSpec in upgradeSpecs) {
                    totalRequiredGold += upgradeSpec.requiredGold;
                }
                this.requiredGoldText.text = totalRequiredGold.ToString();
                if (currentGold >= totalRequiredGold) {
                    this.Enable();
                } else {
                    this.Disable();
                }
            } else {
                var upgradeSpec = this.inventoryPart.GetCurrentUpgradeSpec();
                this.requiredGoldText.text = upgradeSpec.requiredGold.ToString();
                this.Disable();
            }
        }

        public void UpdateView(InventoryPart inventoryPart) {
            var currentUpgradeSpec = inventoryPart.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.goldIcon.SetActive(false);
                this.requiredGoldText.text = "MAX";
                this.requiredGoldText.color = new Color32(255, 50, 0, 255);
            } else {
                this.goldIcon.SetActive(true);
                this.requiredGoldText.text = currentUpgradeSpec.requiredGold.ToString();
                this.requiredGoldText.color = new Color32(255, 255, 255, 255);
                if (inventoryPart.exp >= currentUpgradeSpec.requiredExp) {
                    this.Enable();
                } else {
                    this.Disable();
                }
            }
        }

        public void Enable() {
            this.enableImage.SetActive(true);
            this.disableImage.SetActive(false);
            this.button.enabled = true;
        }

        public void Disable() {
            this.enableImage.SetActive(false);
            this.disableImage.SetActive(true);
            this.button.enabled = false;
        }
    }
}
