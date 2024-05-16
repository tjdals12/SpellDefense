using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Battle {
    using SettingController = Controller.OutGame.SettingController;
    using SettingModel = Model.OutGame.SettingModel;

    public class LanguageChangeConfirmPopup : MonoBehaviour
    {
        SettingController settingController;
        SettingModel settingModel;

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
        TextMeshProUGUI titleText;
        [SerializeField]
        TextMeshProUGUI contentText;
        [SerializeField]
        Button confirmButton;
        [SerializeField]
        TextMeshProUGUI confirmText;
        [SerializeField]
        Button cancelButton;
        [SerializeField]
        TextMeshProUGUI cancelText;

        #region Unity Method
        void Awake() {
            this.settingController = GameObject.FindObjectOfType<SettingController>();
            this.settingModel = GameObject.FindObjectOfType<SettingModel>();
            this.cancelButton.onClick.AddListener(this.Cancel);
        }
        void OnEnable() {
            this.settingModel.OnChangeLanguage += this.OnChangeLanguage;
        }
        void OnDisable() {
            this.settingModel.OnChangeLanguage -= this.OnChangeLanguage;
        }
        #endregion

        #region Event Listeners
        void OnChangeLanguage() {
            SceneManager.LoadScene("RestartScene");
        }
        #endregion

        public void Open(string language) {
            if (language == "ko") {
                this.titleText.text = "언어 변경";
                this.contentText.text = "언어를 변경하시겠습니까?\n(확인 시 게임을 재시작합니다.)";
                this.confirmText.text = "확인";
                this.cancelText.text = "취소";
            } else {
                this.titleText.text = "Change Language";
                this.contentText.text = "Do you want to change the language?\n(The game will restart upon confirmation.)";
                this.confirmText.text = "Confirm";
                this.cancelText.text = "Cancel";
            }
            this.confirmButton.onClick.RemoveAllListeners();
            this.confirmButton.onClick.AddListener(() => this.Confirm(language));

            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.95f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Confirm(string language) {
            this.soundManager.Click();
            this.settingController.ChangeLanguage(language);
        }

        void Cancel() {
            this.soundManager.Close();
            this.popup.SetActive(false);
        }
    }
}
