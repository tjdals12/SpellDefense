using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Account.Request {
    public class CreateAccountRequest {
        public Platform platform { get; private set; }
        public string accountId { get; private set; }
        public CreateAccountRequest(Platform platform, string accountId) {
            this.platform = platform;
            this.accountId = accountId;
        }
    }

    public class ChangePlatformRequest {
        public Platform platform { get; private set; }
        public string accountId { get; private set; }
        public ChangePlatformRequest(Platform platform, string accountId) {
            this.platform = platform;
            this.accountId = accountId;
        }
    }
}
