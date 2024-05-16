using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using GooglePlayGames;

namespace View.Login {
    using Core.App;
    using LoginController = Controller.OutGame.LoginController;
    using UserModel = Model.OutGame.IUserModel;
    using LoadingView = View.Loading.LoadingView;
    using LoadingPopup = View.Common.LoadingPopup;
    using ErrorPopup = View.Common.ErrorPopup;

    public class LoginView : MonoBehaviour
    {
        [Header("App")]
        [Space(4)]
        [SerializeField]
        RemoteConfigManager remoteConfigManager;
        [SerializeField]
        AppVersionChecker appVersionChecker;
        [SerializeField]
        DataTablePatcher dataTablePatcher;

        [Header("GameObject")]
        [Space(4)]
        [SerializeField]
        GameObject controllers;
        [SerializeField]
        GameObject services;
        [SerializeField]
        GameObject repositories;
        [SerializeField]
        GameObject models;

        [Header("Controller")]
        [Space(4)]
        [SerializeField]
        LoginController loginController;

        [Header("Model")]
        [Space(4)]
        [SerializeField]
        UserModel userModel;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        ProgressBar progressBar;
        [SerializeField]
        TextMeshProUGUI versionText;
        [SerializeField]
        Button googlePlayLoginButton;
        [SerializeField]
        Button guestLoginButton;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        AppUpdatePopup appUpdatePopup;

        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        #region Unity Method
        void Awake() {
            this.guestLoginButton.gameObject.SetActive(false);
            this.googlePlayLoginButton.gameObject.SetActive(false);
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.versionText.text = $"v{Application.version}";
            PlayGamesPlatform.Activate();
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    StartCoroutine(this.Initialize());
                } else {
                    UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                    this.errorPopup.Open();
                }
            });
        }
        void OnEnable() {
            this.googlePlayLoginButton.onClick.AddListener(this.GooglePlayLogin);
            this.guestLoginButton.onClick.AddListener(this.GuestLogin);
            this.loginController.OnSuccess += this.OnSuccess;
            this.loginController.OnError += this.OnError;
        }
        void OnDisable() {
            this.googlePlayLoginButton.onClick.RemoveListener(this.GooglePlayLogin);
            this.guestLoginButton.onClick.RemoveListener(this.GuestLogin);
            this.loginController.OnSuccess -= this.OnSuccess;
            this.loginController.OnError -= this.OnError;
        }
        #endregion

        #region Event Listeners
        void OnSuccess() {
            LoadingView.LoadScene("OutGameScene");
        }
        void OnError() {
            this.progressBar.Close();
            this.loadingPopup.Close();
            this.guestLoginButton.enabled = true;
            this.googlePlayLoginButton.enabled = true;
            this.guestLoginButton.gameObject.SetActive(true);
            this.googlePlayLoginButton.gameObject.SetActive(true);
        }
        #endregion

        IEnumerator Initialize() {
            WaitForSeconds delay = new WaitForSeconds(0.5f);
            this.progressBar.Open();
            yield return null;

            this.progressBar.UpdateProgress(percent: 10, step: Step.LoadRemoteConfig);
            yield return delay;
            var remoteConfigInitializeTask = this.remoteConfigManager.Initialize();
            yield return new WaitUntil(() => remoteConfigInitializeTask.IsCompleted);

            AppSetting appSetting = this.remoteConfigManager.GetAppSetting();
            if (appSetting == null) throw new Exception("AppSetting is null.");
            VersionInfo versionInfo = appSetting.GetVersionInfo();

            this.progressBar.UpdateProgress(percent: 30, step: Step.CheckDataTableVersion);
            yield return delay;
            if (this.dataTablePatcher.CheckUpdate(versionInfo, out string dataTableVersion)) {
                yield return StartCoroutine(
                    this.dataTablePatcher.Patch(
                        version: dataTableVersion,
                        onStart: () => {
                            this.progressBar.UpdateProgress(percent: 40, step: Step.StartPatchDataTable);
                        },
                        onStepComplete: (currentCount, totalCount) => {
                            int percent = Mathf.FloorToInt(((currentCount * 1f) / (totalCount * 1f)) * 40);
                            this.progressBar.UpdateProgress(percent: 40 + percent, step: Step.StepCompleteDataTable, message: $"({currentCount}/{totalCount})");
                        },
                        onComplete: () => {
                            this.progressBar.UpdateProgress(percent: 80, step: Step.CompletePatchDataTable);
                        }
                    )
                );
                yield return delay;
            }

            this.repositories.SetActive(true);
            yield return null;
            this.models.SetActive(true);
            yield return null;
            this.services.SetActive(true);
            yield return null;
            this.controllers.SetActive(true);
            yield return null;

            this.progressBar.UpdateProgress(percent: 90, step: Step.CheckAppVersion);
            yield return delay;
            if (this.appVersionChecker.CheckUpdate(versionInfo)) {
                this.progressBar.Close();
                this.appUpdatePopup.Open();
                yield break;
            }
            yield return null;

            this.progressBar.UpdateProgress(percent: 100, step: Step.TryAutoLogin);
            yield return delay;
            this.TryAutoLogin();
            yield return null;
            this.progressBar.UpdateProgress(percent: 100);
        }

        void TryAutoLogin() {
            this.loadingPopup.Open();
            this.loginController.TryAutoLogin();
        }

        void GooglePlayLogin() {
            this.loadingPopup.Open();
            this.loginController.GooglePlayLogin();
        }

        void GuestLogin() {
            this.loadingPopup.Open();
            this.loginController.GuestLogin();
        }
    }
}
