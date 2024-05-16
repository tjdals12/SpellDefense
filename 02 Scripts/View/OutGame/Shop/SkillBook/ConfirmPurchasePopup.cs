using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Shop.SkillBook {
    using ShopSkillBook = Model.OutGame.IShopSkillBook;
    using ToggleButton = Common.ToggleButton;

    public class ConfirmPurchasePopup : MonoBehaviour
    {
        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject popup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        GameObject window;
        [SerializeField]
        TextMeshProUGUI nameText;
        [SerializeField]
        TextMeshProUGUI gradeText;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI amountText;
        [SerializeField]
        Image costIconImage;
        [SerializeField]
        TextMeshProUGUI costAmountText;
        [SerializeField]
        ToggleButton confirmButton;
        [SerializeField]
        Button closeButton;

        Action onConfirm;

        #region Unity Method
        void Awake() {
            this.closeButton.onClick.AddListener(() => {
                this.soundManager.Close();
                this.Close();
            });
            this.confirmButton.Initialize(onClick: this.Confirm);
        }
        #endregion

        public void Open(ShopSkillBook shopSkillBook, bool isEnoughCost, Action onConfirm) {
            this.nameText.text = shopSkillBook.skillBook.name;
            this.gradeText.text = shopSkillBook.skillBook.gradeName;
            this.iconImage.sprite = shopSkillBook.skillBook.image;
            this.amountText.text = $"X {shopSkillBook.item.amount}";
            this.costIconImage.sprite = shopSkillBook.cost.item.image;
            this.costAmountText.text = shopSkillBook.cost.amount.ToString();
            if (isEnoughCost) {
                this.confirmButton.Enable();
                this.onConfirm = onConfirm;
            } else {
                this.confirmButton.Disable();
                this.onConfirm = null;
            }
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        public void Close() {
            this.popup.SetActive(false);
        }

        void Confirm() {
            this.soundManager.Click();
            this.onConfirm?.Invoke();
        }
    }
}