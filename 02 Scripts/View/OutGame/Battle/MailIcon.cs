using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.OutGame.Battle {
    using UserModel = Model.OutGame.IUserModel;

    public class MailIcon : MonoBehaviour
    {
        UserModel userModel;

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
        MailListPopup mailListPopup;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.CheckRedDot();
            this.button.onClick.AddListener(this.Open);
            this.userModel.OnReceiveMail += this.OnReceiveMail;
            this.userModel.OnReceiveMails += this.OnReceiveMails;
        }
        #endregion

        #region Event Listeners
        void OnReceiveMail(string mailId) {
            this.CheckRedDot();
        }
        void OnReceiveMails(List<string> mailIds) {
            this.CheckRedDot();
        }
        #endregion

        void CheckRedDot() {
            bool hasNewMail = false;
            foreach (var mail in this.userModel.user.mails) {
                if (mail.isExpired == false && mail.isReceived == false) {
                    hasNewMail = true;
                    break;
                }
            }
            this.redDot.gameObject.SetActive(hasNewMail);
        }

        void Open() {
            this.soundManager.Click();
            this.mailListPopup.Open();
        }
    }
}
