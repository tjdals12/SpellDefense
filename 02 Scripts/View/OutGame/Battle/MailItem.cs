using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Battle {
    using Mail = Model.OutGame.Mail;
    using LocalizationText = Common.LocalizationText;

    public class MailItem : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image itemIconImage;
        [SerializeField]
        TextMeshProUGUI itemAmountText;
        [SerializeField]
        TextMeshProUGUI mailTitleText;
        [SerializeField]
        LocalizationText remainingDaysText;
        [SerializeField]
        LocalizationText remainingHoursText;
        [SerializeField]
        LocalizationText remainingMinutesText;
        [SerializeField]
        LocalizationText receivedText;
        [SerializeField]
        LocalizationText expiredText;
        [SerializeField]
        GameObject mask;

        public void Initialize(Mail mail, Action<Mail> onClick) {
            this.itemIconImage.sprite = mail.reward.item.image;
            this.itemAmountText.text = mail.reward.amount.ToString();
            this.mailTitleText.text = mail.title;
            if (mail.isReceived) {
                this.remainingDaysText.gameObject.SetActive(false);
                this.remainingHoursText.gameObject.SetActive(false);
                this.remainingMinutesText.gameObject.SetActive(false);
                this.receivedText.gameObject.SetActive(true);
                this.expiredText.gameObject.SetActive(false);
                this.mask.SetActive(true);
            } else if (mail.isExpired) {
                this.remainingDaysText.gameObject.SetActive(false);
                this.remainingHoursText.gameObject.SetActive(false);
                this.remainingMinutesText.gameObject.SetActive(false);
                this.receivedText.gameObject.SetActive(false);
                this.expiredText.gameObject.SetActive(true);
                this.mask.SetActive(true);
            } else {
                int remainingSeconds = ((int)(mail.expiredAt - DateTime.UtcNow).TotalSeconds);
                this.remainingDaysText.gameObject.SetActive(false);
                this.remainingHoursText.gameObject.SetActive(false);
                this.remainingMinutesText.gameObject.SetActive(false);
                this.receivedText.gameObject.SetActive(false);
                this.expiredText.gameObject.SetActive(false);
                this.mask.SetActive(false);
                if (remainingSeconds > 86400) {
                    int days = remainingSeconds / 86400;
                    this.remainingDaysText.UpdateView(days);
                    this.remainingDaysText.gameObject.SetActive(true);
                } else if (remainingSeconds > 3600) {
                    int hours = remainingSeconds / 3600;
                    this.remainingHoursText.UpdateView(hours);
                    this.remainingHoursText.gameObject.SetActive(true);
                } else if (remainingSeconds > 60) {
                    int minutes = remainingSeconds / 60;
                    this.remainingMinutesText.UpdateView(minutes);
                    this.remainingMinutesText.gameObject.SetActive(true);
                }  else if (60 >= remainingSeconds && remainingSeconds > 0) {
                    this.remainingMinutesText.UpdateView(1);
                    this.remainingMinutesText.gameObject.SetActive(true);
                }
            }
            this.button.onClick.AddListener(() => {
                if (this.CanReceive(mail)) {
                    onClick?.Invoke(mail);
                }
            });
        }

        public void ToReceived() {
            this.button.enabled = false;
            this.remainingDaysText.gameObject.SetActive(false);
            this.remainingHoursText.gameObject.SetActive(false);
            this.remainingMinutesText.gameObject.SetActive(false);
            this.receivedText.gameObject.SetActive(true);
            this.expiredText.gameObject.SetActive(false);
            this.mask.SetActive(true);
        }

        bool CanReceive(Mail mail) {
            if (mail.isReceived || mail.isExpired) {
                return false;
            }
            int remainingSeconds = ((int)(mail.expiredAt - DateTime.UtcNow).TotalSeconds);
            if (30 > remainingSeconds) {
                return false;
            }
            return true;
        }
    }
}
