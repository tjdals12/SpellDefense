using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.OutGame.Battle {
    using UserModel = Model.OutGame.IUserModel;
    using Mail = Model.OutGame.Mail;
    using RewardItem = Model.Common.RewardItem;
    using MailController = Controller.OutGame.MailController;
    using ToggleButton = Common.ToggleButton;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;
    using RewardPopup = Common.RewardPopup;

    public class MailListPopup : MonoBehaviour
    {
        UserModel userModel;
        MailController mailController;
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
        GameObject mailListPanel;
        [SerializeField]
        Button refreshButton;
        [SerializeField]
        Image refreshButtonMask;
        [SerializeField]
        ToggleButton receiveAllButton;
        [SerializeField]
        Button closeButton;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        RewardPopup rewardPopup;

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject mailItemPrefab;

        Dictionary<string, MailItem> mailItemsDictionary;

        Coroutine refreshDelayCoroutine;
        float refreshDelaySeconds = 300;
        float remainingRefreshDelaySeconds = 0;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.mailController = GameObject.FindObjectOfType<MailController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.refreshButton.onClick.AddListener(this.RefreshMails);
            this.receiveAllButton.Initialize(onClick: this.ReceiveAllMail);
            this.closeButton.onClick.AddListener(this.Close);
            this.ShowMails();
        }
        void OnEnable() {
            this.userModel.OnReceiveMail += this.OnReceiveMail;
            this.userModel.OnReceiveReward += this.OnReceiveReward;
            this.userModel.OnReceiveMails += this.OnReceiveMails;
            this.userModel.OnReceiveRewards += this.OnReceiveRewards;
            this.userModel.OnRefreshMails += this.OnRefreshMails;
        }
        void OnDisable() {
            this.userModel.OnReceiveMail -= this.OnReceiveMail;
            this.userModel.OnReceiveReward -= this.OnReceiveReward;
            this.userModel.OnReceiveMails -= this.OnReceiveMails;
            this.userModel.OnReceiveRewards -= this.OnReceiveRewards;
            this.userModel.OnRefreshMails -= this.OnRefreshMails;
        }
        #endregion

        #region Event Listeners
        void OnReceiveMail(string mailId) {
            if (this.mailItemsDictionary.ContainsKey(mailId)) {
                this.mailItemsDictionary[mailId].ToReceived();
            }
            this.CheckHasReceivableMail();
        }
        void OnReceiveReward(RewardItem rewardItem) {
            this.rewardPopup.Open(rewardItem);
        }
        void OnReceiveMails(List<string> mailIds) {
            foreach (string mailId in mailIds) {
                this.OnReceiveMail(mailId);
            }
        }
        void OnReceiveRewards(List<RewardItem> rewardItems) {
            if (rewardItems.Count > 0) {
                this.rewardPopup.Open(rewardItems);
            }
        }
        void OnRefreshMails() {
            this.ShowMails();
        }
        #endregion

        public void Open() {
            if (this.refreshButton.enabled) {
                this.refreshButtonMask.gameObject.SetActive(false);
            } else {
                this.refreshButtonMask.gameObject.SetActive(true);
                this.refreshButtonMask.fillAmount = this.remainingRefreshDelaySeconds / this.refreshDelaySeconds;
            }
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        public void Close() {
            this.soundManager.Close();
            this.popup.SetActive(false);
        }

        void ShowMails() {
            if (this.mailItemsDictionary != null) {
                foreach (var mailItem in this.mailItemsDictionary.Values) {
                    Destroy(mailItem.gameObject);
                }
            }
            this.mailItemsDictionary = new();
            foreach (var mail in this.userModel.user.mails) {
                GameObject clone = Instantiate(this.mailItemPrefab, this.mailListPanel.transform);
                MailItem mailItem = clone.GetComponent<MailItem>();
                mailItem.Initialize(mail: mail, onClick: this.ReceiveMail);
                this.mailItemsDictionary[mail.id] = mailItem;
            }
            this.CheckHasReceivableMail();
        }

        void CheckHasReceivableMail() {
            bool hasReceivableMail = false;
            foreach (var mail in this.userModel.user.mails) {
                if (mail.isReceived == false) {
                    hasReceivableMail = true;
                    break;
                }
            }
            if (hasReceivableMail) {
                this.receiveAllButton.Enable();
            } else {
                this.receiveAllButton.Disable();
            }
        }

        void ReceiveMail(Mail mail) {
            this.soundManager.Click();
            this.loadingPopup.Open();
            this.mailController.ReceiveMail(mail.id);
        }

        void ReceiveAllMail() {
            this.soundManager.Click();
            this.receiveAllButton.enabled = false;
            this.loadingPopup.Open();
            this.mailController.ReceiveAllMail();
        }

        void RefreshMails() {
            if (this.refreshDelayCoroutine != null) {
                StopCoroutine(this.refreshDelayCoroutine);
            }
            this.soundManager.Click();
            this.refreshDelayCoroutine = StartCoroutine(this.StartRefreshDelay());
            this.loadingPopup.Open();
            this.mailController.RefreshMails();
        }

        IEnumerator StartRefreshDelay() {
            this.refreshButton.enabled = false;
            this.refreshButtonMask.gameObject.SetActive(true);
            this.remainingRefreshDelaySeconds = this.refreshDelaySeconds;
            WaitForSeconds delay = new WaitForSeconds(1f);
            while (this.remainingRefreshDelaySeconds > 0) {
                yield return delay;
                this.remainingRefreshDelaySeconds--;
            }
            this.refreshButton.enabled = true;
        }
    }
}
