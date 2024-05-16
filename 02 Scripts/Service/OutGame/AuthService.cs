using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

namespace Service.OutGame {
    using ValidationException = Core.CustomException.ValidationException;
    using Platform = Repository.Account.Platform;
    using IAccountRepository = Repository.Account.IRepository;
    using IUserRepository = Repository.User.IRepository;
    using INoticeRepository = Repository.Notice.IRepository;
    using ISettingRepository = Repository.Setting.IRepository;
    using ILocalizationRepository = Repository.Localization.IRepository;
    using Repository.Account.Request;
    using Model.OutGame;

    public class AuthService : MonoBehaviour
    {
        public bool isLoaded { get; private set; } = false;

        IAccountRepository accountRepository;
        IUserRepository userRepository;
        INoticeRepository noticeRepository;
        ISettingRepository settingRepository;
        ILocalizationRepository localizationRepository;
        AccountModel accountModel;
        UserModel userModel;
        NoticeModel noticeModel;
        SettingModel settingModel;
        FirebaseAuth _auth;
        FirebaseAuth auth {
            get {
                if (this._auth == null) {
                    this._auth = FirebaseAuth.DefaultInstance;
                }
                return this._auth;
            }
        }

        #region Unity Method
        void Awake() {
            this.accountRepository = GameObject.FindObjectOfType<IAccountRepository>();
            this.accountModel = GameObject.FindObjectOfType<AccountModel>();
            this.userRepository = GameObject.FindObjectOfType<IUserRepository>();
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.noticeRepository = GameObject.FindObjectOfType<INoticeRepository>();
            this.noticeModel = GameObject.FindObjectOfType<NoticeModel>();
            this.settingRepository = GameObject.FindObjectOfType<ISettingRepository>();
            this.settingModel = GameObject.FindObjectOfType<SettingModel>();
            this.localizationRepository = GameObject.FindObjectOfType<ILocalizationRepository>();
        }
        #endregion

        public async Task Login(Platform platform) {
            string accountId = null;
            switch (platform) {
                case Platform.Auto:
                    accountId = this.auth.CurrentUser == null ? null : this.auth.CurrentUser.UserId;
                    break;
                case Platform.Guest:
                    accountId = await this.GuestLogin();
                    break;
                case Platform.GooglePlay:
                    accountId = await this.GooglePlayLogin();
                    break;
            }
            if (accountId == null) throw new Exception("accountId is null");
            Account account = await this.accountRepository.GetAccount(accountId);
            if (account == null) {
                if (platform == Platform.Auto) throw new Exception("account is null");
                CreateAccountRequest request = new CreateAccountRequest(platform, accountId);
                account = await this.accountRepository.CreateAccount(request);
            }
            this.accountModel.Initialize(account);
            User user = await this.userRepository.GetUser(account.accountId);
            this.userModel.Initialize(user);
            Notice[] notices = await this.noticeRepository.FindAll();
            this.noticeModel.Initialize(notices);
            CurrentSetting currentSetting = this.settingRepository.GetCurrentSetting();
            string currentLanguage = this.localizationRepository.GetCurrentLanguage();
            this.settingModel.Initialize(currentSetting, currentLanguage);
        }

        async Task<Firebase.Auth.Credential> GetGooglePlayCredential() {
            var taskCompletionSource1 = new TaskCompletionSource<bool>();
            Action<bool> onAuthenticate = taskCompletionSource1.SetResult;
            var task1 = taskCompletionSource1.Task;
            Social.localUser.Authenticate(onAuthenticate);
            bool success = await task1;
            if (success == false) return null; 
            var taskCompletionSource2 = new TaskCompletionSource<string>();
            Action<string> onRequestAuthCode = taskCompletionSource2.SetResult;
            var task2 = taskCompletionSource2.Task;
            PlayGamesPlatform.Instance.RequestServerSideAccess(
                forceRefreshToken: false,
                callback: onRequestAuthCode
            );
            string authCode = await task2;
            Firebase.Auth.Credential credential = Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
            return credential;
        }

        async Task<string> GooglePlayLogin() {
            Firebase.Auth.Credential credential = await this.GetGooglePlayCredential();
            AuthResult authResult = await this.auth.SignInAndRetrieveDataWithCredentialAsync(credential);
            return authResult.User.UserId;
        }

        async Task<string> GuestLogin() {
            AuthResult authResult = await this.auth.SignInAnonymouslyAsync();
            return authResult.User.UserId;
        }

        public async Task Link(Platform platform) {
            switch (platform) {
                case Platform.GooglePlay:
                    await this.LinkToGooglePlayAccount();
                    break;
                default:
                    throw new ValidationException();
            }
        }

        async Task LinkToGooglePlayAccount() {
            Firebase.Auth.Credential credential = await this.GetGooglePlayCredential();
            var task = this.auth.CurrentUser.LinkWithCredentialAsync(credential);
            AuthResult authResult = await task;
            if (task.IsCanceled) {
                throw new Exception();
            } else if (task.IsFaulted) {
                this.accountModel.RequestConfirmForChangeAccount();
            } else {
                ChangePlatformRequest request = new ChangePlatformRequest(Platform.GooglePlay, authResult.User.UserId);
                Account account = await this.accountRepository.ChangePlatform(request);
                this.accountModel.ChangePlatform(account);
            }
        }

        public async Task ChangeAccount(Platform platform) {
            switch (platform) {
                case Platform.GooglePlay:
                    await this.ChangeToGooglePlayAccount();
                    break;
            }
        }

        async Task ChangeToGooglePlayAccount() {
            Firebase.Auth.Credential credential = await this.GetGooglePlayCredential();
            var task = this.auth.SignInAndRetrieveDataWithCredentialAsync(credential);
            if (task.IsCanceled || task.IsFaulted) throw new Exception();
            this.accountModel.ChangeAccount();
        }
    }
}
