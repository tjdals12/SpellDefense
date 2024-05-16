using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.OutGame.Battle {
    using LoginController = Controller.OutGame.LoginController;
    using IAccountModel = Model.OutGame.IAccountModel;
    using LoadingPopup = View.Common.LoadingPopup;

    public class LinkAccountConfirmPopup : MonoBehaviour
    {
        LoginController loginController;
        IAccountModel accountModel;
        LoadingPopup loadingPopup;

        [Header("Audio")]
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
        Button closeButton;
        [SerializeField]
        Button confirmButton;
        [SerializeField]
        Button cancelButton;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        ChangeAccountConfirmPopup changeAccountConfirmPopup;

        #region Unity Method
        void Awake() {
            this.accountModel = GameObject.FindObjectOfType<IAccountModel>();
            this.loginController = GameObject.FindObjectOfType<LoginController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.closeButton.onClick.AddListener(this.Cancel);
            this.confirmButton.onClick.AddListener(this.Confirm);
            this.cancelButton.onClick.AddListener(this.Cancel);
        }
        void OnEnable() {
            this.accountModel.OnRequestConfirmForChangeAccount += this.OnRequestConfirmForChangeAccount;
            this.accountModel.OnChangePlatform += OnChangePlatform;
        }
        void OnDisable() {
            this.accountModel.OnRequestConfirmForChangeAccount -= this.OnRequestConfirmForChangeAccount;
            this.accountModel.OnChangePlatform -= OnChangePlatform;
        }
        #endregion

        #region Event Listeners
        void OnRequestConfirmForChangeAccount() {
            this.Cancel();
            this.changeAccountConfirmPopup.Open();
        }
        void OnChangePlatform() {
            this.Cancel();
        }
        #endregion

        public void Open() {
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Confirm() {
            if (Application.platform == RuntimePlatform.Android) {
                this.loadingPopup.Open();
                this.loginController.LinkToGooglePlayAccount();
            } else {
                this.Cancel();
            }
        }

        void Cancel() {
            this.soundManager.Close();
            this.popup.SetActive(false);
        }
    }
}
