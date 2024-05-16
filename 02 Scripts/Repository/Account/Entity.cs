using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Account {
    public enum Platform {
        Auto = -1,
        Guest = 0,
        GooglePlay
    }

    public class Entity
    {
        public Platform platform;
        public string accountId;
        public DateTime createdAt;
        public Entity(Platform platform, string accountId, DateTime createdAt) {
            this.platform = platform;
            this.accountId = accountId;
            this.createdAt = createdAt;
        }
    }
}
