using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.OutGame {
    using ValidationException = Core.CustomException.ValidationException;
    using UserService = Service.OutGame.UserService;

    public class MailController : MonoBehaviour
    {
        UserService userService;

        public event Action OnSuccess;
        public event Action<string> OnAlert;
        public event Action OnError;

        #region Unity Method
        void Awake() {
            this.userService = GameObject.FindObjectOfType<UserService>();
        }
        #endregion

        public async void ReceiveMail(string mailId) {
            try {
                await this.userService.ReceiveMail(mailId);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void ReceiveAllMail() {
            try {
                await this.userService.ReceiveAllMail();
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void RefreshMails() {
            try {
                await this.userService.RefreshMails();
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }
    }
}
