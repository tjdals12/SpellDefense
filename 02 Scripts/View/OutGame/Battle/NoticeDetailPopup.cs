using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Battle {
    using Notice = Model.OutGame.INotice;

    public class NoticeDetailPopup : MonoBehaviour
    {
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
        Button topCloseButton;
        [SerializeField]
        Button bottomCloseButton;
        [SerializeField]
        TextMeshProUGUI titleText;
        [SerializeField]
        TextMeshProUGUI contentText;

        #region Unity Method
        void Awake() {
            this.topCloseButton.onClick.AddListener(this.Close);
            this.bottomCloseButton.onClick.AddListener(this.Close);
        }
        #endregion

        public void Open(Notice notice) {
            this.popup.SetActive(true);
            this.titleText.text = notice.title;
            this.contentText.text = notice.content;
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        public void Close() {
            this.soundManager.Close();
            this.popup.SetActive(false);
        }
    }
}
