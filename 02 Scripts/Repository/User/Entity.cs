using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Repository.User {
    public class Energy {
        public int amount;
        public DateTime lastChargeTime;
        public Energy(int amount, DateTime lastChargeTime) {
            this.amount = amount;
            this.lastChargeTime = lastChargeTime;
        }
        public void Charge(int amount) {
            this.amount += amount;
            if (this.amount >= 30) {
                this.lastChargeTime = DateTime.UtcNow;
            }
        }
        public void Use(int amount) {
            DateTime now = DateTime.UtcNow;
            if (this.amount >= 30) {
                this.lastChargeTime = now;
            } else {
                TimeSpan timeSpan = now - this.lastChargeTime;
                now = now.AddSeconds(timeSpan.TotalSeconds * -1f);
                this.lastChargeTime = now;
            }
            this.amount -= amount;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "amount", this.amount },
                { "lastChargeTime", this.lastChargeTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ffffffZ") }
            };
        }
    }

    public class Decks {
        public int current;
        public int[][] list;
        public Decks(int current, int[][] list) {
            this.current = current;
            this.list = list;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "current", this.current },
                { "list", this.list }
            };
        }
    }

    public class Reward {
        public string id;
        public int itemId;
        public int itemAmount;
        [JsonConstructor]
        public Reward(string id, int itemId, int itemAmount) {
            this.id = id;
            this.itemId = itemId;
            this.itemAmount = itemAmount;
        }
        public Reward(int itemId, int itemAmount) {
            this.itemId = itemId;
            this.itemAmount = itemAmount;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "id", this.id },
                { "itemId", this.itemId },
                { "itemAmount", this.itemAmount }
            };
        }
    }

    public class SystemMail {
        public string id;
        public Dictionary<string, string> titles;
        public DateTime startedAt;
        public DateTime endedAt;
        public int storeDays;
        public Reward reward;
        public SystemMail(string id, Dictionary<string, string> titles, DateTime startedAt, DateTime endedAt, int storeDays, Reward reward)
        {
            this.id = id;
            this.titles = titles;
            this.startedAt = startedAt;
            this.endedAt = endedAt;
            this.storeDays = storeDays;
            this.reward = reward;
        }
        public bool CanReceive() {
            DateTime now = DateTime.UtcNow;
            return (now - this.startedAt).TotalSeconds >= 0 && (this.endedAt - now).TotalSeconds >= 0;
        }
    }

    public class Mail {
        public string id;
        public Dictionary<string, string> titles;
        public DateTime createdAt;
        public DateTime expiredAt;
        public bool isExpired {
            get => 0 >= (this.expiredAt - DateTime.UtcNow).TotalSeconds;
        }
        public DateTime receivedAt;
        public bool isReceived;
        public Reward reward;
        public Mail(string id, Dictionary<string, string> titles, DateTime createdAt, DateTime expiredAt, DateTime receivedAt, bool isReceived, Reward reward) {
            this.id = id;
            this.titles = titles;
            this.createdAt = createdAt;
            this.expiredAt = expiredAt;
            this.receivedAt = receivedAt;
            this.isReceived = isReceived;
            this.reward = reward;
        }
        public void ToReceived() {
            this.receivedAt = DateTime.UtcNow;
            this.isReceived = true;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "id", this.id },
                { "titles", this.titles },
                { "createdAt", this.createdAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ffffffZ") },
                { "expiredAt", this.expiredAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ffffffZ") },
                { "isExpired", this.isExpired },
                { "receivedAt", this.receivedAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ffffffZ") },
                { "isReceived", this.isReceived },
                { "reward", this.reward.ToDictionary() },
            };
        }
    }

    public class SkillBook {
        public int id;
        public int level;
        public int amount;
        public SkillBook(int id, int level, int amount) {
            this.id = id;
            this.level = level;
            this.amount = amount;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "id", this.id },
                { "level", this.level },
                { "amount", this.amount }
            };
        }
    }

    public class EquippedParts {
        public string weapon;
        public string armor;
        public string jewelry;
        public EquippedParts(string weapon, string armor, string jewelry) {
            this.weapon = weapon;
            this.armor = armor;
            this.jewelry = jewelry;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "weapon", this.weapon },
                { "armor", this.armor },
                { "jewelry", this.jewelry }
            };
        }
    }

    public class Part {
        public string id;
        public int partId;
        public int level;
        public int exp;
        public Part(string id, int partId, int level, int exp) {
            this.id = id;
            this.partId = partId;
            this.level = level;
            this.exp = exp;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "id", this.id },
                { "partId", this.partId },
                { "level", this.level },
                { "exp", this.exp }
            };
        }
    }

    public class Cost {
        public int itemId;
        public int itemAmount;
        public Cost(int itemId, int itemAmount) {
            this.itemId = itemId;
            this.itemAmount = itemAmount;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "itemId", this.itemId },
                { "itemAmount", this.itemAmount }
            };
        }
    }

    public class ShopItem {
        public Reward reward;
        public Cost cost;
        public int maxBuyCount;
        public int remainBuyCount;
        public ShopItem(Reward reward, Cost cost, int maxBuyCount, int remainBuyCount) {
            this.reward = reward;
            this.cost = cost;
            this.maxBuyCount = maxBuyCount;
            this.remainBuyCount = remainBuyCount;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "reward", this.reward.ToDictionary() },
                { "cost", this.cost.ToDictionary() },
                { "maxBuyCount", this.maxBuyCount },
                { "remainBuyCount", this.remainBuyCount }
            };
        }
    }

    public class SkillBookShop {
        public ShopItem[] items;
        public int maxResetCount;
        public int remainResetCount;
        public DateTime lastResetTime;
        public SkillBookShop(ShopItem[] items, int maxResetCount, int remainResetCount, DateTime lastResetTime) {
            this.items = items;
            this.maxResetCount = maxResetCount;
            this.remainResetCount = remainResetCount;
            this.lastResetTime = lastResetTime;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "items", this.items },
                { "maxResetCount", this.maxResetCount },
                { "remainResetCount", this.remainResetCount },
                { "lastResetTime", this.lastResetTime }
            };
        }
    }

    public class PartShop {
        public ShopItem[] items;
        public int maxResetCount;
        public int remainResetCount;
        public DateTime lastResetTime;
        public PartShop(ShopItem[] items, int maxResetCount, int remainResetCount, DateTime lastResetTime) {
            this.items = items;
            this.maxResetCount = maxResetCount;
            this.remainResetCount = remainResetCount;
            this.lastResetTime = lastResetTime;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "items", this.items },
                { "maxResetCount", this.maxResetCount },
                { "remainResetCount", this.remainResetCount },
                { "lastResetTime", this.lastResetTime }
            };
        }
    }

    public class GoldShop {
        public ShopItem[] items;
        public int maxResetCount;
        public int remainResetCount;
        public DateTime lastResetTime;
        public GoldShop(ShopItem[] items, int maxResetCount, int remainResetCount, DateTime lastResetTime) {
            this.items = items;
            this.maxResetCount = maxResetCount;
            this.remainResetCount = remainResetCount;
            this.lastResetTime = lastResetTime;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "items", this.items },
                { "maxResetCount", this.maxResetCount },
                { "remainResetCount", this.remainResetCount },
                { "lastResetTime", this.lastResetTime }
            };
        }
    }

    public class EnergyShop {
        public ShopItem[] items;
        public int maxResetCount;
        public int remainResetCount;
        public DateTime lastResetTime;
        public EnergyShop(ShopItem[] items, int maxResetCount, int remainResetCount, DateTime lastResetTime) {
            this.items = items;
            this.maxResetCount = maxResetCount;
            this.remainResetCount = remainResetCount;
            this.lastResetTime = lastResetTime;
        }
        public Dictionary<string, object> ToDictionary() {
            return new Dictionary<string, object>() {
                { "items", this.items },
                { "maxResetCount", this.maxResetCount },
                { "remainResetCount", this.remainResetCount },
                { "lastResetTime", this.lastResetTime }
            };
        }
    }

    public class Entity
    {
        public string id;
        public string nickname;
        public Energy energy;
        public int gold;
        public int key;
        public Decks decks;
        public List<Mail> mails;
        public List<SkillBook> skillBooks;
        public EquippedParts equippedParts;
        public Dictionary<string, Part> parts;
        public SkillBookShop skillBookShop;
        public PartShop partShop;
        public GoldShop goldShop;
        public EnergyShop energyShop;
        public Entity(
            string id,
            string nickname,
            Energy energy,
            int gold,
            int key,
            Decks decks,
            List<Mail> mails,
            List<SkillBook> skillBooks,
            EquippedParts equippedParts,
            Dictionary<string, Part> parts,
            SkillBookShop skillBookShop,
            PartShop partShop,
            GoldShop goldShop,
            EnergyShop energyShop
        ) {
            this.id = id;
            this.nickname = nickname;
            this.energy = energy;
            this.gold = gold;
            this.key = key;
            this.decks = decks;
            this.mails = mails == null ? new List<Mail>() : mails;
            this.skillBooks = skillBooks;
            this.equippedParts = equippedParts;
            this.parts = parts;
            this.skillBookShop = skillBookShop;
            this.partShop = partShop;
            this.goldShop = goldShop;
            this.energyShop = energyShop;
        }
    }
}
