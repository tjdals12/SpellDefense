using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace View.OutGame.Battle {
    using LoginController = Controller.OutGame.LoginController;
    using IAccountModel = Model.OutGame.IAccountModel;
    using LoadingPopup = View.Common.LoadingPopup;

    public class ChangeAccountConfirmPopup : MonoBehaviour
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

        #region Unity Method
        void Awake() {
            this.loginController = GameObject.FindObjectOfType<LoginController>();
            this.accountModel = GameObject.FindObjectOfType<IAccountModel>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.closeButton.onClick.AddListener(this.Cancel);
            this.confirmButton.onClick.AddListener(this.Confirm);
            this.cancelButton.onClick.AddListener(this.Cancel);
        }
        void OnEnable() {
            this.accountModel.OnChangeAccount += this.OnChangeAccount;
        }
        void OnDisable() {
            this.accountModel.OnChangeAccount -= this.OnChangeAccount;
        }
        #endregion

        #region Event Listeners
        void OnChangeAccount() {
            SceneManager.LoadScene("RestartScene");
        }
        #endregion

        public void Open() {
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.95f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Confirm() {
            if (Application.platform == RuntimePlatform.Android) {
                this.soundManager.Click();
                this.loadingPopup.Open();
                this.loginController.ChangeToGooglePlayAccount();
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