using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Part {
    using InventoryPart = Model.OutGame.IInventoryPart;

    public class MaterialPart : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI levelText;
        [SerializeField]
        GameObject maskImage;
        [SerializeField]
        GameObject checkIcon;

        public void Initialize(InventoryPart inventoryPart, Action<InventoryPart> onClick) {
            this.iconImage.sprite = inventoryPart.part.image;
            this.levelText.text = $"Lv. {inventoryPart.level}";
            this.maskImage.SetActive(false);
            this.checkIcon.SetActive(false);
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke(inventoryPart));
        }

        public void Select() {
            this.maskImage.SetActive(true);
            this.checkIcon.SetActive(true);
        }

        public void Deselect() {
            this.maskImage.SetActive(false);
            this.checkIcon.SetActive(false);
        }
    }
}
