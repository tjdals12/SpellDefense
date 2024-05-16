using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Newtonsoft.Json;

namespace Repository.Account {
    using Request;

    public class FirebaseRepository : IRepository
    {
        Entity accountEntity;

        DatabaseReference _accountsRef;
        DatabaseReference accountsRef {
            get {
                if (this._accountsRef == null) {
                    this._accountsRef = FirebaseDatabase.DefaultInstance.RootReference.Child("accounts");
                }
                return this._accountsRef;
            }
        }

        DatabaseReference _usersRef;
        DatabaseReference usersRef {
            get {
                if (this._usersRef == null) {
                    this._usersRef = FirebaseDatabase.DefaultInstance.RootReference.Child("users");
                }
                return this._usersRef;
            }
        }

        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<IRepository>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        #endregion

        public override async Task<Model.OutGame.Account> CreateAccount(CreateAccountRequest request)
        {
            DataSnapshot dataSnapshot = await this.accountsRef.Child(request.accountId).GetValueAsync();
            if (dataSnapshot.Exists) {
                DateTime now = DateTime.UtcNow;
                string rawJson = dataSnapshot.GetRawJsonValue();
                this.accountEntity = JsonConvert.DeserializeObject<Entity>(rawJson);
            } else {
                DateTime now = DateTime.UtcNow;
                this.accountEntity = new Entity(
                    platform: request.platform,
                    accountId: request.accountId,
                    createdAt: now
                );
                string rawJson = JsonConvert.SerializeObject(accountEntity);
                await this.accountsRef.Child(request.accountId).SetRawJsonValueAsync(rawJson);
            }
            return this.ToAccount(accountEntity);
        }

        public override async Task<Model.OutGame.Account> GetAccount(string accountId)
        {
            DataSnapshot dataSnapshot = await this.accountsRef.Child(accountId).GetValueAsync();
            string rawJson = dataSnapshot.GetRawJsonValue();
            if (rawJson == null) {
                return null;
            }
            DateTime now = DateTime.UtcNow;
            Entity accountEntity = JsonConvert.DeserializeObject<Entity>(rawJson);
            return this.ToAccount(accountEntity);
        }

        public override async Task<Model.OutGame.Account> ChangePlatform(ChangePlatformRequest request) {
            await this.accountsRef.Child(request.accountId).Child("platform").SetValueAsync((int)request.platform);
            this.accountEntity.platform = request.platform;
            return this.ToAccount(accountEntity);
        }

        Model.OutGame.Account ToAccount(Entity accountEntity) {
            Model.OutGame.Account account = new Model.OutGame.Account(
                platform: accountEntity.platform,
                accountId: accountEntity.accountId,
                createdAt: accountEntity.createdAt
            );
            return account;
        }
    }
}
