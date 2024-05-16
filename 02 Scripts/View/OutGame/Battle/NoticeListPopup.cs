using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.OutGame.Battle {
    using NoticeModel = Model.OutGame.INoticeModel;
    using Notice = Model.OutGame.INotice;
    using NoticeController = Controller.OutGame.NoticeController;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class NoticeListPopup : MonoBehaviour
    {
        NoticeModel noticeModel;
        NoticeController noticeController;
        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

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
        GameObject noticeListPanel;
        [SerializeField]
        Button closeButton;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        NoticeDetailPopup noticeDetailPopup;

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject noticeItemPrefab;

        Dictionary<int, NoticeItem> noticeItemDictionary;

        #region Unity Method
        void Awake() {
            this.noticeModel = GameObject.FindObjectOfType<NoticeModel>();
            this.noticeController = GameObject.FindObjectOfType<NoticeController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.closeButton.onClick.AddListener(this.Close);
            this.ShowNotices();
        }
        void OnEnable() {
            this.noticeModel.OnReadNotice += this.OnReadNotice;
        }
        void OnDisable() {
            this.noticeModel.OnReadNotice -= this.OnReadNotice;
        }
        #endregion

        #region Event Listeners
        void OnReadNotice(int noticeId) {
            if (this.noticeItemDictionary.ContainsKey(noticeId)) {
                this.noticeItemDictionary[noticeId].ToRead();
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

        void ShowNotices() {
            if (this.noticeItemDictionary != null) {
                foreach (var noticeItem in this.noticeItemDictionary.Values) {
                    Destroy(noticeItem.gameObject);
                }
            }
            this.noticeItemDictionary = new();
            foreach (var notice in this.noticeModel.notices) {
                GameObject clone = Instantiate(this.noticeItemPrefab, this.noticeListPanel.transform);
                NoticeItem noticeItem = clone.GetComponent<NoticeItem>();
                noticeItem.Initialize(notice: notice, onClick: this.ReadNotice);
                this.noticeItemDictionary[notice.id] = noticeItem;
            }
        }

        void ReadNotice(Notice notice) {
            this.soundManager.Click();
            this.loadingPopup.Open();
            this.noticeDetailPopup.Open(notice);
            this.noticeController.ReadNotice(notice.id);
        }
    }
}
