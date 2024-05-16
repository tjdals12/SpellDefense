using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.OutGame.Battle {
    using SettingModel = Model.OutGame.SettingModel;
    using ToggleButton = Common.ToggleButton;

    public class LanguagePopup : MonoBehaviour
    {
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
        Button closeButton;

        [Header("UI - Language")]
        [Space(4)]
        [SerializeField]
        ToggleButton koreanButton;
        [SerializeField]
        ToggleButton englishButton;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        LanguageChangeConfirmPopup languageChangeConfirmPopup;

        #region Unity Method
        void Awake() {
            this.settingModel = GameObject.FindObjectOfType<SettingModel>();
            this.closeButton.onClick.AddListener(this.Close);
            this.koreanButton.Initialize(onClick: () => {
                this.soundManager.Click();
                this.languageChangeConfirmPopup.Open("ko");
            });
            this.englishButton.Initialize(onClick: () => {
                this.soundManager.Click();
                this.languageChangeConfirmPopup.Open("en");
            });
            string currentLanguage = this.settingModel.currentLanguage;
            if (currentLanguage == "ko") {
                this.koreanButton.Enable();
                this.englishButton.Disable(withButton: false);
            } else {
                this.koreanButton.Disable(withButton: false);
                this.englishButton.Enable();
            }
        }
        #endregion

        public void Open() {
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Close() {
            this.soundManager.Close();
            this.popup.SetActive(false);
        }
    }
}
