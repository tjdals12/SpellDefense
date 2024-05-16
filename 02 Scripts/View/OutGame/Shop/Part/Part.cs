using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Shop.Part {
    using ShopPart = Model.OutGame.IShopPart;

    public class Part : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI salesAmountText;
        [SerializeField]
        Image costIconImage;
        [SerializeField]
        TextMeshProUGUI costAmountText;
        [SerializeField]
        GameObject mask;
        [SerializeField]
        GameObject checkIcon;

        public void Initialize(ShopPart shopPart, Action<ShopPart> onClick) {
            this.iconImage.sprite = shopPart.part.image;
            this.salesAmountText.text = $"X {shopPart.item.amount}";
            this.costIconImage.sprite = shopPart.cost.item.image;
            this.costAmountText.text = shopPart.cost.amount.ToString();
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke(shopPart));
            if (shopPart.remainBuyCount == 0) {
                this.button.enabled = false;
                this.Disable();
            } else {
                this.button.enabled = true;
                this.Enable();
            }
        }

        public void UpdateView(ShopPart shopPart) {
            if (shopPart.remainBuyCount == 0) {
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
