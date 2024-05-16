using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Battle {
    using AccountPlatform = Repository.Account.Platform;
    using IconSwitchButton = View.Common.IconSwitchButton;
    using ToastMessage = View.Common.ToastMessage;
    using IUserModel = Model.OutGame.IUserModel;
    using ISettingModel = Model.OutGame.ISettingModel;
    using IAccountModel = Model.OutGame.IAccountModel;
    using SettingController = Controller.OutGame.SettingController;

    public class SettingPopup : MonoBehaviour
    {
        IUserModel userModel;
        ISettingModel settingModel;
        IAccountModel accountModel;
        SettingController settingController;

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
        Button playerIdButton;
        [SerializeField]
        TextMeshProUGUI playerIdText;
        [SerializeField]
        IconSwitchButton BGMButton;
        [SerializeField]
        IconSwitchButton SFXButton;
        [SerializeField]
        Button languageButton;
        [SerializeField]
        TextMeshProUGUI languageText;
        [SerializeField]
        Button linkAccountButton;
        [SerializeField]
        TextMeshProUGUI linkAccountButtonText;
        [SerializeField]
        TextMeshProUGUI linkAccountVendorText;
        [SerializeField]
        Button closeButton;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        LanguagePopup languagePopup;
        [SerializeField]
        LinkAccountConfirmPopup linkAccountConfirmPopup;

        [Header("UI - Toast Message")]
        [Space(4)]
        [SerializeField]
        ToastMessage copiedToastMessage;
        [SerializeField]
        ToastMessage linkAccountToastMessage;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<IUserModel>();
            this.settingModel = GameObject.FindObjectOfType<ISettingModel>();
            this.accountModel = GameObject.FindObjectOfType<IAccountModel>();
            this.playerIdButton.onClick.AddListener(this.CopyPlayerIdToClipboard);
            this.playerIdText.text = this.userModel.user.id;
            this.settingController = GameObject.FindObjectOfType<SettingController>();
            this.BGMButton.Initialize(
                value: this.settingModel.currentSetting.BGM,
                onClick: () => {
                    this.settingController.ToggleBGM(!this.settingModel.currentSetting.BGM);
                }
            );
            this.SFXButton.Initialize(
                value: this.settingModel.currentSetting.SFX,
                onClick: () => {
                    this.settingController.ToggleSFX(!this.settingModel.currentSetting.SFX);
                }
            );
            this.languageButton.onClick.AddListener(this.OpenLanguagePopup);
            string currentLanguage = this.settingModel.currentLanguage;
            if (currentLanguage == "ko") {
                this.languageText.text = "한국어";
            } else {
                this.languageText.text = "English";
            }
            if (this.accountModel.account.platform == AccountPlatform.Guest) {
                this.linkAccountButton.onClick.RemoveAllListeners();
                this.linkAccountButton.onClick.AddListener(this.OpenLinkAccountConfirmPopup);
                this.linkAccountButton.enabled = true;
            } else {
                this.linkAccountButton.enabled = false;
                string vendor = this.linkAccountVendorText.text;
                this.linkAccountVendorText.gameObject.SetActive(false);
                this.linkAccountButtonText.text = vendor;
            }
            this.closeButton.onClick.AddListener(this.Close);
            if (Application.platform == RuntimePlatform.Android) {
                this.linkAccountVendorText.text = "Google Play";
            } else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
                this.linkAccountButton.enabled = true;
                this.linkAccountVendorText.text = "Game Center";
            } else {
                this.linkAccountButton.enabled = false;
                this.linkAccountVendorText.text = "Not Supported";
            }
        }
        void OnEnable() {
            this.settingModel.OnToggleBGM += this.OnToggleBGM;
            this.settingModel.OnToggleSFX += this.OnToggleSFX;
            this.accountModel.OnChangePlatform += this.OnChangePlatform;
        }
        void OnDisable() {
            this.settingModel.OnToggleBGM -= this.OnToggleBGM;
            this.settingModel.OnToggleSFX -= this.OnToggleSFX;
            this.accountModel.OnChangePlatform -= this.OnChangePlatform;
        }
        #endregion

        #region Event Listeners
        void OnToggleBGM() {
            if (this.settingModel.currentSetting.BGM) {
                this.soundManager.Click();
                this.BGMButton.Enable();
            } else {
                this.BGMButton.Disable();
            }
        }
        void OnToggleSFX() {
            if (this.settingModel.currentSetting.SFX) {
                this.soundManager.Click();
                this.SFXButton.Enable();
            } else {
                this.SFXButton.Disable();
            }
        }
        void OnChangePlatform() {
            if (this.accountModel.account.platform == AccountPlatform.Guest) {
                this.linkAccountButton.onClick.RemoveAllListeners();
                this.linkAccountButton.onClick.AddListener(this.OpenLinkAccountConfirmPopup);
                this.linkAccountButton.enabled = true;
            } else {
                this.linkAccountButton.enabled = false;
                string vendor = this.linkAccountVendorText.text;
                this.linkAccountVendorText.gameObject.SetActive(false);
                this.linkAccountButtonText.text = vendor;
                this.linkAccountToastMessage.Open();
            }
        }
        #endregion

        public void Open() {
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Close() {
            this.soundManager.Close();
            this.popup.SetActive(false);
        }

        void CopyPlayerIdToClipboard() {
            this.soundManager.Click();
            if (this.copiedToastMessage.isOpen == true) return;
            this.copiedToastMessage.Open();
            GUIUtility.systemCopyBuffer = this.playerIdText.text;
        }

        void OpenLanguagePopup() {
            this.soundManager.Click();
            this.languagePopup.Open();
        }

        void OpenLinkAccountConfirmPopup() {
            this.soundManager.Click();
            this.linkAccountConfirmPopup.Open();
        }
    }
}
