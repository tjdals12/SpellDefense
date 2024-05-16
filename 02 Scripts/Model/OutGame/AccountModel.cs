using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.OutGame {
    public abstract class IAccountModel : MonoBehaviour
    {
        protected Account _account;
        public IAccount account { get => this._account; }

        public Action OnInitialize;
        public Action OnRequestConfirmForChangeAccount;
        public Action OnChangeAccount;
        public Action OnChangePlatform;
    }

    public class AccountModel : IAccountModel {
        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<IAccountModel>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        #endregion

        public void Initialize(Account account) {
            this._account = account;
            this.OnInitialize?.Invoke();
        }

        public void RequestConfirmForChangeAccount() {
            this.OnRequestConfirmForChangeAccount?.Invoke();
        }

        public void ChangeAccount() {
            this.OnChangeAccount?.Invoke();
        }

        public void ChangePlatform(Account account) {
            this._account = account;
            this.OnChangePlatform?.Invoke();
        }
    }
}
