using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.OutGame.Battle {
    using NoticeModel = Model.OutGame.INoticeModel;

    public class NoticeIcon : MonoBehaviour
    {
        NoticeModel noticeModel;

        [Header("Audio")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        GameObject redDot;
        [SerializeField]
        NoticeListPopup noticeListPopup;

        #region Unity Method
        void Awake() {
            this.noticeModel = GameObject.FindObjectOfType<NoticeModel>();
            this.CheckRedDot();
            this.button.onClick.AddListener(this.Open);
        }
        void OnEnable()  {
            this.noticeModel.OnReadNotice += this.OnReadNotice;
        }
        void OnDisable() {
            this.noticeModel.OnReadNotice -= this.OnReadNotice;
        }
        #endregion

        #region Event Listeners
        void OnReadNotice(int noticeId) {
            this.CheckRedDot();
        }
        #endregion

        void CheckRedDot() {
            bool hasNewNotice = false;
            foreach (var notice in this.noticeModel.notices) {
                if (notice.isRead == false) {
                    hasNewNotice = true;
                    break;
                }
            }
            this.redDot.gameObject.SetActive(hasNewNotice);
        }

        void Open() {
            this.soundManager.Click();
            this.noticeListPopup.Open();
        }
    }
}
