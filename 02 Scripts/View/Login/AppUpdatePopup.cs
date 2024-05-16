using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.Login {
    public class AppUpdatePopup : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject popup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        GameObject window;
        [SerializeField]
        Button updateButton;

        #region Unity Method
        void Awake() {
            this.updateButton.onClick.AddListener(this.RequestAppUpdate);
        }
        #endregion

        public void Open() {
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.95f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void RequestAppUpdate() {
            if (Application.platform == RuntimePlatform.Android) {
                Application.OpenURL("market://details?id=com.lseongmin.spellwar");
            } else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
                Application.OpenURL("itms-apps://itunes.apple.com/app/id");
            } else {
                Application.OpenURL("https://play.google.com/store/apps/details?id=com.lseongmin.spellwar");
            }
        }
    }
}