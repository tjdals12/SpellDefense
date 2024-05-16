using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.Common {
    using LoginController = Controller.OutGame.LoginController;
    using MailController = Controller.OutGame.MailController;
    using NoticeController = Controller.OutGame.NoticeController;
    using UserController = Controller.OutGame.UserController;
    using ShopController = Controller.OutGame.ShopController;

    public class LoadingPopup : MonoBehaviour
    {
        LoginController loginController;
        MailController mailController;
        NoticeController noticeController;
        UserController userController;
        ShopController shopController;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject popup;
        [SerializeField]
        Image backgroundImage;

        #region Unity Method
        void Awake() {
            this.loginController = GameObject.FindObjectOfType<LoginController>();
            this.mailController = GameObject.FindObjectOfType<MailController>();
            this.noticeController = GameObject.FindObjectOfType<NoticeController>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.shopController = GameObject.FindObjectOfType<ShopController>();
        }
        void OnEnable() {
            if (this.loginController != null) {
                this.loginController.OnSuccess += this.OnSuccess;
                this.loginController.OnAlert += this.OnAlert;
                this.loginController.OnError += this.OnError;
            }
            if (this.mailController != null) {
                this.mailController.OnSuccess += this.OnSuccess;
                this.mailController.OnAlert += this.OnAlert;
                this.mailController.OnError += this.OnError;
            }
            if (this.noticeController != null) {
                this.noticeController.OnSuccess += this.OnSuccess;
                this.noticeController.OnAlert += this.OnAlert;
                this.noticeController.OnError += this.OnError;
            }
            if (this.userController != null) {
                this.userController.OnSuccess += this.OnSuccess;
                this.userController.OnAlert += this.OnAlert;
                this.userController.OnError += this.OnError;
            }
            if (this.shopController != null) {
                this.shopController.OnSuccess += this.OnSuccess;
                this.shopController.OnAlert += this.OnAlert;
                this.shopController.OnError += this.OnError;
            }
        }
        void OnDisable() {
            if (this.mailController != null) {
                this.mailController.OnSuccess -= this.OnSuccess;
                this.mailController.OnAlert -= this.OnAlert;
                this.mailController.OnError -= this.OnError;
            }
            if (this.noticeController != null) {
                this.noticeController.OnSuccess -= this.OnSuccess;
                this.noticeController.OnAlert -= this.OnAlert;
                this.noticeController.OnError -= this.OnError;
            }
            if (this.userController != null) {
                this.userController.OnSuccess -= this.OnSuccess;
                this.userController.OnAlert -= this.OnAlert;
                this.userController.OnError -= this.OnError;
            }
            if (this.shopController != null) {
                this.shopController.OnSuccess -= this.OnSuccess;
                this.shopController.OnAlert -= this.OnAlert;
                this.shopController.OnError -= this.OnError;
            }
        }
        #endregion

        #region Event Listeners
        void OnSuccess() {
            this.Close();
        }
        void OnAlert(string message) {
            this.Close();
        }
        void OnError() {
            this.Close();
        }
        #endregion

        public void Open() {
            this.popup.SetActive(true);
        }

        public void Close() {
            this.popup.SetActive(false);
        }
    }
}
