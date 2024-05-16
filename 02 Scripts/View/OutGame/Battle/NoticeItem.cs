using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Battle {
    using Notice = Model.OutGame.INotice;
    using NoticeType = Repository.Notice.NoticeType;

    public class NoticeItem : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI titleText;
        [SerializeField]
        TextMeshProUGUI dateText;
        [SerializeField]
        GameObject redDot;

        [Header("Sprite")]
        [Space(4)]
        [SerializeField]
        Sprite announcementIconSprite;
        [SerializeField]
        Sprite updateIconSprite;

        public void Initialize(Notice notice, Action<Notice> onClick) {
            switch (notice.type) {
                case NoticeType.Announcement:
                    this.iconImage.sprite = this.announcementIconSprite;
                    break;
                case NoticeType.Update:
                    this.iconImage.sprite = this.updateIconSprite;
                    break;
            }
            this.titleText.text = notice.title;
            this.dateText.text = notice.createdAt.ToString("yyyy-MM-dd");
            if (notice.isRead) {
                this.redDot.SetActive(false);
            } else {
                this.redDot.SetActive(true);
            }
            this.button.onClick.AddListener(() => {
                onClick?.Invoke(notice);
            });
        }

        public void ToRead() {
            this.redDot.gameObject.SetActive(false);
        }
    }
}
