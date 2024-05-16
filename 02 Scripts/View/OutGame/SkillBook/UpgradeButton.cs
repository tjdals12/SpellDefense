using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.SkillBook {
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;

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
        GameObject cost;
        [SerializeField]
        TextMeshProUGUI requiredGoldText;
        [SerializeField]
        GameObject maxText;

        public void Initialize(InventorySkillBook inventorySkillBook, Action onClick) {
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.cost.SetActive(false);
                this.maxText.SetActive(true);
            } else {
                this.cost.SetActive(true);
                this.maxText.SetActive(false);
                this.requiredGoldText.text = currentUpgradeSpec.requiredGold.ToString();
            }
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke());
        }

        public void UpdateView(InventorySkillBook inventorySkillBook) {
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.cost.SetActive(false);
                this.maxText.SetActive(true);
            } else {
                this.cost.SetActive(true);
                this.maxText.SetActive(false);
                this.requiredGoldText.text = currentUpgradeSpec.requiredGold.ToString();
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
