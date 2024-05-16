using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace View.Common {
    using LocalizationRepository = Repository.Localization.IRepository;
    using MailController = Controller.OutGame.MailController;
    using NoticeController = Controller.OutGame.NoticeController;
    using UserController = Controller.OutGame.UserController;
    using ShopController = Controller.OutGame.ShopController;

    public class AlertPopup : MonoBehaviour
    {
        LocalizationRepository localizationRepository;
        MailController mailController;
        NoticeController noticeController;
        UserController userController;
        ShopController shopController;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject popup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        GameObject window;
        [SerializeField]
        TextMeshProUGUI contentText;
        [SerializeField]
        Button closeButton;

        #region Unity Method
        void Awake() {
            this.localizationRepository = GameObject.FindObjectOfType<LocalizationRepository>();
            this.mailController = GameObject.FindObjectOfType<MailController>();
            this.noticeController = GameObject.FindObjectOfType<NoticeController>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.shopController = GameObject.FindObjectOfType<ShopController>();
            this.closeButton.onClick.AddListener(this.Close);
        }
        void OnEnable() {
            if (this.mailController != null) {
                this.mailController.OnAlert += this.OnAlert;
            }
            if (this.noticeController != null) {
                this.noticeController.OnAlert += this.OnAlert;
            }
            if (this.userController != null) {
                this.userController.OnAlert += this.OnAlert;
            }
            if (this.shopController != null) {
                this.shopController.OnAlert += this.OnAlert;
            }
        }
        void OnDisable() {
            if (this.mailController != null) {
                this.mailController.OnAlert -= this.OnAlert;
            }
            if (this.noticeController != null) {
                this.noticeController.OnAlert -= this.OnAlert;
            }
            if (this.userController != null) {
                this.userController.OnAlert -= this.OnAlert;
            }
            if (this.shopController != null) {
                this.shopController.OnAlert -= this.OnAlert;
            }
        }
        #endregion

        #region Event Listeners
        void OnAlert(string message) {
            this.Open(message);
        }
        #endregion

        public void Open(string message) {
            string localizedMessage = this.localizationRepository.GetLocalizedText(message);
            this.contentText.text = localizedMessage;
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Close() {
            this.popup.SetActive(false);
        }
    }
}
