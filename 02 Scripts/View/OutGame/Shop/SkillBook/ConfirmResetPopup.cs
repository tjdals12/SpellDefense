using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.OutGame.Shop.SkillBook {
    using ToggleButton = Common.ToggleButton;

    public class ConfirmResetPopup : MonoBehaviour
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
        ToggleButton confirmButton;
        [SerializeField]
        Button closeButton;

        Action onConfirm;

        #region Unity Method
        void Awake() {
            this.closeButton.onClick.AddListener(this.Close);
            this.confirmButton.Initialize(onClick: this.Confirm);
        }
        #endregion

        public void Open(bool canReset, Action onConfirm) {
            if (canReset) {
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
            this.soundManager.Close();
            this.popup.SetActive(false);
        }

        void Confirm() {
            this.soundManager.Click();
            this.onConfirm?.Invoke();
        }
    }
}
