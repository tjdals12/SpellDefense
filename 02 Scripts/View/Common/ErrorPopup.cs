using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.Common {
    using MailController = Controller.OutGame.MailController;
    using NoticeController = Controller.OutGame.NoticeController;
    using UserController = Controller.OutGame.UserController;
    using ShopController = Controller.OutGame.ShopController;

    public class ErrorPopup : MonoBehaviour
    {
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
        [SerializeField]
        GameObject window;
        [SerializeField]
        Button closeButton;

        #region Unity Method
        void Awake() {
            this.mailController = GameObject.FindObjectOfType<MailController>();
            this.noticeController = GameObject.FindObjectOfType<NoticeController>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.shopController = GameObject.FindObjectOfType<ShopController>();
            this.closeButton.onClick.AddListener(this.Close);
        }
        void OnEnable() {
            if (this.mailController != null) {
                this.mailController.OnError += this.OnError;
            }            
            if (this.userController != null) {
                this.userController.OnError += this.OnError;
            }            
            if (this.noticeController != null) {
                this.noticeController.OnError += this.OnError;
            }            
            if (this.shopController != null) {
                this.shopController.OnError += this.OnError;
            }            
        }
        void OnDisable() {
            if (this.mailController != null) {
                this.mailController.OnError -= this.OnError;
            }            
            if (this.userController != null) {
                this.userController.OnError -= this.OnError;
            }            
            if (this.noticeController != null) {
                this.noticeController.OnError -= this.OnError;
            }            
            if (this.shopController != null) {
                this.shopController.OnError -= this.OnError;
            }            
        }
        #endregion

        #region Event Listeners
        void OnError() {
            this.Open();
        }
        #endregion

        public void Open() {
            DOTween.Sequence()
                .OnStart(() => {
                    this.popup.SetActive(true);
                })
                .Join(this.backgroundImage.DOFade(0.6f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1f, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Close() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
    }
}
