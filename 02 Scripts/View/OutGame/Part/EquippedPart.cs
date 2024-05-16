using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Part {
    using InventoryPart = Model.OutGame.IInventoryPart;

    public class EquippedPart : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI levelText;

        public void Initialize(InventoryPart inventoryPart, Action<InventoryPart> onClick) {
            this.iconImage.sprite = inventoryPart.part.image;
            this.levelText.text = $"Lv. {inventoryPart.level}";
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke(inventoryPart));
        }

        public void UpdateView(InventoryPart inventoryPart) {
            this.levelText.text = $"Lv. {inventoryPart.level}";
        }
    }
}
