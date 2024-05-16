using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.OutGame {
    using ValidationException = Core.CustomException.ValidationException;
    using AuthService = Service.OutGame.AuthService;
    using Platform = Repository.Account.Platform;

    public class LoginController : MonoBehaviour
    {
        AuthService authService;

        public Action OnInitialize;
        public Action OnSuccess;
        public Action<string> OnAlert;
        public Action OnError;

        #region Unity Method
        void Awake() {
            this.authService = GameObject.FindObjectOfType<AuthService>();
            StartCoroutine(WaitForResolveDependencies());
        }
        #endregion

        IEnumerator WaitForResolveDependencies() {
            while (this.authService.isLoaded == false) {
                yield return null;
            }
            this.OnInitialize?.Invoke();
        }

        public async void TryAutoLogin() {
            try {
                await this.authService.Login(Platform.Auto);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void GooglePlayLogin() {
            try {
                await this.authService.Login(Platform.GooglePlay);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void GuestLogin() {
            try {
                await this.authService.Login(Platform.Guest);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void LinkToGooglePlayAccount() {
            try {
                await this.authService.Link(Platform.GooglePlay);
                this.OnSuccess?.Invoke();
            } catch(ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void ChangeToGooglePlayAccount() {
            try {
                await this.authService.ChangeAccount(Platform.GooglePlay);
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
