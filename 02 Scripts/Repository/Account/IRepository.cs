using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Account {
    using Request;

    public abstract class IRepository : MonoBehaviour
    {
        public abstract Task<Model.OutGame.Account> CreateAccount(CreateAccountRequest request);
        public abstract Task<Model.OutGame.Account> GetAccount(string accountId);
        public abstract Task<Model.OutGame.Account> ChangePlatform(ChangePlatformRequest request);
    }
}
