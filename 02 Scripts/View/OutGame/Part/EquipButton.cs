using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.OutGame.Part {
    using InventoryPart = Model.OutGame.IInventoryPart;

    public class EquipButton : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        GameObject enableImage;
        [SerializeField]
        GameObject disableImage;

        public void Initialize(InventoryPart inventoryPart, Action onClick) {
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke());
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
