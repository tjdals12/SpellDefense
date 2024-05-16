using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.OutGame {
    using PartType = Repository.Part.PartType;
    using NoticeType = Repository.Notice.NoticeType;
    using Platform = Repository.Account.Platform;
    using Common;

    public abstract class IEnergy {
        public int amount { get; protected set; }
        public DateTime lastChargeTime { get; protected set; }
        public IEnergy(int amount, DateTime lastChargeTime) {
            this.amount = amount;
            this.lastChargeTime = lastChargeTime;
        }
    }
    public class Energy : IEnergy
    {
        public Energy(int amount, DateTime lastChargeTime) : base(amount, lastChargeTime)
        {
        }
        public void Charge(int amount) {
            this.amount += amount;
            if (this.amount >= 30) {
                this.lastChargeTime = DateTime.UtcNow;
            }
        }
        public void Use(int amount) {
            this.amount -= amount;
            if (30 > this.amount) {
                this.lastChargeTime = DateTime.UtcNow;
            }
        }
    }



    public abstract class IInventorySkillBook {
        public SkillBook skillBook { get; protected set; }
        public int level { get; protected set; }
        public int amount { get; protected set; }
        public bool isObtained { get; protected set; }
        public IInventorySkillBook(SkillBook skillBook, int level, int amount, bool isObtained) {
            this.skillBook = skillBook;
            this.level = level;
            this.amount = amount;
            this.isObtained = isObtained;
        }
        public SkillBookUpgradeSpec GetCurrentUpgradeSpec() {
            SkillBookUpgradeSpec upgradeSpec = Array.Find(this.skillBook.upgradeSpecs, (e) => e.level == this.level + 1);
            return upgradeSpec;
        }
    }

    public class InventorySkillBook : IInventorySkillBook
    {
        public InventorySkillBook(SkillBook skillBook, int level, int amount, bool isObtained) : base(skillBook, level, amount, isObtained)
        {
        }
        public void AddAmount(int amount) {
            this.amount += amount;
        }
        public void MinusAmount(int amount) {
            this.amount -= amount;
        }
        public void IncreaseLevel() {
            this.level += 1;
        }
        public void ToObtained() {
            this.isObtained = true;
        }
    }

    public abstract class IDeck {
        public bool isUse { get; protected set; }
        public int[] skillBooks { get; protected set; }
        public IDeck(bool isUse, int[] skillBooks) {
            this.isUse = isUse;
            this.skillBooks = skillBooks;
        }
    }

    public class Deck : IDeck
    {
        public Deck(bool isUse, int[] skillBooks) : base(isUse, skillBooks)
        {
        }
        public void Use() {
            this.isUse = true;
        }
        public void Unuse() {
            this.isUse = false;
        }
        public void ChangeSkillBook(int index, int skillBookId) {
            this.skillBooks[index] = skillBookId;
        }
    }


    public abstract class IInventoryPart {
        public string id { get; protected set; }
        public Part part { get; protected set; }
        public int level { get; protected set; }
        public int exp { get; protected set; }
        public IInventoryPart(string id, Part part, int level, int exp) {
            this.id = id;
            this.part = part;
            this.level = level;
            this.exp = exp;
        }
        public PartUpgradeSpec GetCurrentUpgradeSpec() {
            PartUpgradeSpec upgradeSpec = Array.Find(this.part.upgradeSpecs, (e) => e.level == this.level + 1);
            return upgradeSpec;
        }
        public Stats GetStats() {
            PartSpec spec = this.part.spec;
            int attackPower = spec.attackPower + (spec.attackPowerPerLevel * (this.level - 1));
            float attackSpeed = spec.attackSpeed + (spec.attackSpeedPerLevel * (this.level - 1));
            int criticalDamage = spec.criticalDamage + (spec.criticalDamagePerLevel * (this.level - 1));
            int criticalRate = spec.criticalRate + (spec.criticalRatePerLevel * (this.level - 1));
            return new Stats(attackPower, attackSpeed, criticalDamage, criticalRate);
        }
    }

    public class InventoryPart : IInventoryPart
    {
        public InventoryPart(string id, Part part, int level, int exp) : base(id, part, level, exp)
        {
        }
        public void Upgrade(int level, int exp) {
            this.level = level;
            this.exp = exp;
        }
    }

    public abstract class IEquippedParts {
        public string weapon { get; protected set; }
        public string armor { get; protected set; }
        public string jewelry { get; protected set; }
        public IEquippedParts(string weapon, string armor, string jewelry) {
            this.weapon = weapon;
            this.armor = armor;
            this.jewelry = jewelry;
        }
    }

    public class EquippedParts : IEquippedParts
    {
        public EquippedParts(string weapon, string armor, string jewelry) : base(weapon, armor, jewelry)
        {
        }
        public void Change(InventoryPart inventoryPart) {
            switch (inventoryPart.part.type) {
                case PartType.Armor:
                    this.armor = inventoryPart.id;
                    break;
                case PartType.Weapon:
                    this.weapon = inventoryPart.id;
                    break;
                case PartType.Jewelry:
                    this.jewelry = inventoryPart.id;
                    break;
            }
        }
    }

    public class RewardSkillBook : RewardItem
    {
        public InventorySkillBook inventorySkillBook { get; private set; }
        public RewardSkillBook(Item item, int amount, InventorySkillBook inventorySkillBook) : base(item, amount)
        {
            this.inventorySkillBook = inventorySkillBook;
        }
    }

    public class RewardPart : RewardItem {
        public InventoryPart inventoryPart { get; private set; }
        public RewardPart(Item item, int amount, InventoryPart inventoryPart) : base(item, amount)
        {
            this.inventoryPart = inventoryPart;
        }
    }

    public class ReceivedReward {
        public RewardItem rewardItem { get; private set; }
        public RewardSkillBook rewardSkillBook { get; private set; }
        public RewardPart rewardPart { get; private set; }
        public ReceivedReward(RewardItem rewardItem) {
            this.rewardItem = rewardItem;
        }
        public ReceivedReward(RewardSkillBook rewardSkillBook) {
            this.rewardSkillBook = rewardSkillBook;
        }
        public ReceivedReward(RewardPart rewardPart) {
            this.rewardPart = rewardPart;
        }
    }

    public abstract class IShopSkillBook
    {
        public SkillBook skillBook { get; protected set; }
        public RewardItem item { get; protected set; }
        public int index { get; protected set; }
        public Cost cost { get; protected set; }
        public int maxBuyCount { get; protected set; }
        public int remainBuyCount { get; protected set; }
        public IShopSkillBook(SkillBook skillBook, RewardItem item, int index, Cost cost, int maxBuyCount, int remainBuyCount) {
            this.skillBook = skillBook;
            this.item = item;
            this.index = index;
            this.cost = cost;
            this.maxBuyCount = maxBuyCount;
            this.remainBuyCount = remainBuyCount;
        }
    }

    public class ShopSkillBook : IShopSkillBook
    {
        public ShopSkillBook(SkillBook skillBook, RewardItem item, int index, Cost cost, int maxBuyCount, int remainBuyCount) : base(skillBook, item, index, cost, maxBuyCount, remainBuyCount)
        {
        }
        public void MinusRemainBuyCount() {
            if (this.remainBuyCount == 0) return;
            this.remainBuyCount -= 1;
        }
    }

    public abstract class ISkillBookShop
    {
        protected ShopSkillBook[] _shopItems;
        public IShopSkillBook[] shopItems { get => this._shopItems; }
        public int maxResetCount { get; protected set; }
        public int remainResetCount { get; protected set; }
        public DateTime lastResetTime { get; protected set; }
        public ISkillBookShop(ShopSkillBook[] shopItems, int maxResetCount, int remainResetCount, DateTime lastResetTime)
        {
            this._shopItems = shopItems;
            this.maxResetCount = maxResetCount;
            this.remainResetCount = remainResetCount;
            this.lastResetTime = lastResetTime;
        }
    }

    public class SkillBookShop : ISkillBookShop
    {
        public SkillBookShop(ShopSkillBook[] shopItems, int maxResetCount, int remainResetCount, DateTime lastResetTime) : base(shopItems, maxResetCount, remainResetCount, lastResetTime)
        {
        }
        public void Buy(int index) {
            this._shopItems[index].MinusRemainBuyCount();
        }
        public void ChangeShopItems(ShopSkillBook[] shopItems) {
            this._shopItems = shopItems;
        }
        public void MinusRemainResetCount() {
            this.remainResetCount -= 1;
        }
    }

    public abstract class IShopPart
    {
        public Part part { get; protected set; }
        public RewardItem item { get; protected set; }
        public int index { get; protected set; }
        public Cost cost { get; protected set; }
        public int maxBuyCount { get; protected set; }
        public int remainBuyCount { get; protected set; }
        public IShopPart(Part part, RewardItem item, int index, Cost cost, int maxBuyCount, int remainBuyCount) {
            this.part = part;
            this.item = item;
            this.index = index;
            this.cost = cost;
            this.maxBuyCount = maxBuyCount;
            this.remainBuyCount = remainBuyCount;
        }
    }

    public class ShopPart : IShopPart
    {
        public ShopPart(Part part, RewardItem item, int index, Cost cost, int maxBuyCount, int remainBuyCount) : base(part, item, index, cost, maxBuyCount, remainBuyCount)
        {
        }
        public void MinusRemainBuyCount() {
            if (this.remainBuyCount == 0) return;
            this.remainBuyCount -= 1;
        }
    }

    public abstract class IPartShop {
        protected ShopPart[] _shopItems;
        public IShopPart[] shopItems { get => this._shopItems; }
        public int maxResetCount { get; protected set; }
        public int remainResetCount { get; protected set; }
        public DateTime lastResetTime { get; protected set; }
        public IPartShop(ShopPart[] shopItems, int maxResetCount, int remainResetCount, DateTime lastResetTime) {
            this._shopItems = shopItems;
            this.maxResetCount = maxResetCount;
            this.remainResetCount = remainResetCount;
            this.lastResetTime = lastResetTime;
        }
    }

    public class PartShop : IPartShop
    {
        public PartShop(ShopPart[] shopItems, int maxResetCount, int remainResetCount, DateTime lastResetTime) : base(shopItems, maxResetCount, remainResetCount, lastResetTime)
        {
        }
        public void Buy(int index) {
            this._shopItems[index].MinusRemainBuyCount();
        }
        public void ChangeShopItems(ShopPart[] shopItems) {
            this._shopItems = shopItems;
        }
        public void MinusRemainResetCount() {
            this.remainResetCount -= 1;
        }
    }

    public abstract class IShopGold
    {
        public RewardItem gold { get; protected set; }
        public int index { get; protected set; }
        public Cost cost { get; protected set; }
        public int maxBuyCount { get; protected set; }
        public int remainBuyCount { get; protected set; }
        public IShopGold(RewardItem gold, int index, Cost cost, int maxBuyCount, int remainBuyCount) {
            this.gold = gold;
            this.index = index;
            this.cost = cost;
            this.maxBuyCount = maxBuyCount;
            this.remainBuyCount = remainBuyCount;
        }
    }

    public class ShopGold : IShopGold
    {
        public ShopGold(RewardItem gold, int index, Cost cost, int maxBuyCount, int remainBuyCount) : base(gold, index, cost, maxBuyCount, remainBuyCount)
        {
        }
        public void MinusRemainBuyCount() {
            if (this.remainBuyCount == 0) return;
            this.remainBuyCount -= 1;
        }
    }

    public abstract class IGoldShop {
        protected ShopGold[] _shopItems;
        public IShopGold[] shopItems { get => this._shopItems; }
        public int maxResetCount { get; protected set; }
        public int remainResetCount { get; protected set; }
        public DateTime lastResetTime { get; protected set; }
        public IGoldShop(ShopGold[] shopItems, int maxResetCount, int remainResetCount, DateTime lastResetTime)
        {
            this._shopItems = shopItems;
            this.maxResetCount = maxResetCount;
            this.remainResetCount = remainResetCount;
            this.lastResetTime = lastResetTime;
        }
    }

    public class GoldShop : IGoldShop
    {
        public GoldShop(ShopGold[] shopItems, int maxResetCount, int remainResetCount, DateTime lastResetTime) : base(shopItems, maxResetCount, remainResetCount, lastResetTime)
        {
        }
        public void Buy(int index) {
            this._shopItems[index].MinusRemainBuyCount();
        }
        public void ChangeShopItems(ShopGold[] shopItems) {
            this._shopItems = shopItems;
        }
        public void MinusRemainResetCount() {
            this.remainResetCount -= 1;
        }
    }

    public abstract class IShopEnergy {
        public RewardItem energy { get; protected set; }
        public int index { get; protected set; }
        public Cost cost { get; protected set; }
        public int maxBuyCount { get; protected set; }
        public int remainBuyCount { get; protected set; }
        public IShopEnergy(RewardItem energy, int index, Cost cost, int maxBuyCount, int remainBuyCount) {
            this.energy = energy;
            this.index = index;
            this.cost = cost;
            this.maxBuyCount = maxBuyCount;
            this.remainBuyCount = remainBuyCount;
        }
    }

    public class ShopEnergy : IShopEnergy
    {
        public ShopEnergy(RewardItem energy, int index, Cost cost, int maxBuyCount, int remainBuyCount) : base(energy, index, cost, maxBuyCount, remainBuyCount)
        {
        }
        public void MinusRemainBuyCount() {
            if (this.remainBuyCount == 0) return;
            this.remainBuyCount -= 1;
        }
    }

    public abstract class IEnergyShop {
        protected ShopEnergy[] _shopItems;
        public IShopEnergy[] shopItems { get => this._shopItems; }
        public int maxResetCount { get; protected set; }
        public int remainResetCount { get; protected set; }
        public DateTime lastResetTime { get; protected set; }
        public IEnergyShop(ShopEnergy[] shopItems, int maxResetCount, int remainResetCount, DateTime lastResetTime)
        {
            this._shopItems = shopItems;
            this.maxResetCount = maxResetCount;
            this.remainResetCount = remainResetCount;
            this.lastResetTime = lastResetTime;
        }
    }

    public class EnergyShop : IEnergyShop
    {
        public EnergyShop(ShopEnergy[] shopItems, int maxResetCount, int remainResetCount, DateTime lastResetTime) : base(shopItems, maxResetCount, remainResetCount, lastResetTime)
        {
        }
        public void Buy(int index) {
            this._shopItems[index].MinusRemainBuyCount();
        }
        public void ChangeShopItems(ShopEnergy[] shopItems) {
            this._shopItems = shopItems;
        }
        public void MinusRemainResetCount() {
            this.remainResetCount -= 1;
        }
    }

    public abstract class IMail {
        public string id { get; protected set; }
        public string title { get; protected set; }
        public DateTime createdAt { get; protected set; }
        public DateTime expiredAt { get; protected set; }
        public bool isExpired { get; protected set; }
        public DateTime receivedAt { get; protected set; }
        public bool isReceived { get; protected set; }
        public RewardItem reward { get; protected set; }
        public IMail(string id, string title, DateTime createdAt, DateTime expiredAt, bool isExpired, DateTime receivedAt, bool isReceived, RewardItem reward) {
            this.id = id;
            this.title = title;
            this.createdAt = createdAt;
            this.expiredAt = expiredAt;
            this.isExpired = isExpired;
            this.receivedAt = receivedAt;
            this.isReceived = isReceived;
            this.reward = reward;
        }
    }

    public class Mail : IMail
    {
        public Mail(string id, string title, DateTime createdAt, DateTime expiredAt, bool isExpired, DateTime receivedAt, bool isReceived, RewardItem reward) : base(id, title, createdAt, expiredAt, isExpired, receivedAt, isReceived, reward)
        {
        }
        public void ToReceived() {
            if (this.isExpired || this.isReceived) return;
            this.receivedAt = DateTime.UtcNow;
            this.isReceived = true;
        }
    }

    public abstract class IUser {
        public string id { get; protected set; }
        public string nickname { get; protected set; }
    
        protected Energy _energy;
        public IEnergy energy { get => this._energy; }

        public int gold { get; protected set; }
        public int key { get; protected set; }

        protected List<Mail> _mails;
        public List<Mail> mails { get => this._mails; }

        protected Deck[] _decks;
        public IDeck[] decks { get => this._decks; }

        protected InventorySkillBook[] _inventorySkillBooks;
        public IInventorySkillBook[] inventorySkillBooks { get => this._inventorySkillBooks; }

        protected EquippedParts _equippedParts;
        public IEquippedParts equippedParts { get => this._equippedParts; }

        protected List<InventoryPart> _inventoryParts;
        public List<InventoryPart> inventoryParts { get => this._inventoryParts; }

        protected SkillBookShop _skillBookShop;
        public ISkillBookShop skillBookShop { get => this._skillBookShop; }

        protected PartShop _partShop;
        public IPartShop partShop { get => this._partShop; }

        protected GoldShop _goldShop;
        public IGoldShop goldShop { get => this._goldShop; }

        protected EnergyShop _energyShop;
        public IEnergyShop energyShop { get => this._energyShop; }

        public IUser(
            string id,
            string nickname,
            Energy energy,
            int gold,
            int key,
            List<Mail> mails,
            Deck[] decks,
            InventorySkillBook[] inventorySkillBooks,
            EquippedParts equippedParts,
            List<InventoryPart> inventoryParts,
            SkillBookShop skillBookShop,
            PartShop partShop,
            GoldShop goldShop,
            EnergyShop energyShop
        ) {
            this.id = id;
            this.nickname = nickname;
            this._energy = energy;
            this.gold = gold;
            this.key = key;
            this._mails = mails;
            this._decks = decks;
            this._inventorySkillBooks = inventorySkillBooks;
            this._equippedParts = equippedParts;
            this._inventoryParts = inventoryParts;
            this._skillBookShop = skillBookShop;
            this._partShop = partShop;
            this._goldShop = goldShop;
            this._energyShop = energyShop;
        }

        public bool IsEnoughCost(Cost cost) {
            switch (cost.item.id) {
                // Free
                case 0:
                    return true;
                // Gold
                case 1: 
                    return this.gold >= cost.amount;
                // Energy
                case 3:
                    return this.energy.amount >= cost.amount;
                // Silver Key
                case 10:
                    return this.key >= cost.amount;
                default:
                    return false;
            }
        }

        public bool IsEquippedSkillBook(int skillBookId) {
            var deck = Array.Find(this.decks, (deck) => deck.isUse);
            return Array.Exists(deck.skillBooks, (e) => e == skillBookId);
        }

        public IInventorySkillBook GetSkillBook(int skillBookId) {
            return Array.Find(this.inventorySkillBooks, (e) => e.skillBook.id == skillBookId);
        }

        public bool IsEquippedPart(string id) {
            if (
                this.equippedParts.armor == id ||
                this.equippedParts.weapon == id ||
                this.equippedParts.jewelry == id
            ) {
                return true;
            } else {
                return false;
            }
        }

        public IInventoryPart GetEquippedWeapon() {
            IInventoryPart inventoryPart = this.GetPart(this.equippedParts.weapon);
            return inventoryPart;
        }

        public IInventoryPart GetEquippedArmor() {
            IInventoryPart inventoryPart = this.GetPart(this.equippedParts.armor);
            return inventoryPart;
        }

        public IInventoryPart GetEquippedJewelry() {
            IInventoryPart inventoryPart = this.GetPart(this.equippedParts.jewelry);
            return inventoryPart;
        }

        public IInventoryPart GetPart(string id) {
            return this.inventoryParts.Find((e) => e.id == id);
        }
    }

    public abstract class IAccount {
        public Platform platform { get; protected set; }
        public string accountId { get; protected set; }
        public DateTime createdAt { get; protected set; }
        public IAccount(Platform platform, string accountId, DateTime createdAt) {
            this.platform = platform;
            this.accountId = accountId;
            this.createdAt = createdAt;
        }
    }

    public class Account : IAccount
    {
        public Account(Platform platform, string accountId, DateTime createdAt) : base(platform, accountId, createdAt)
        {
        }
    }

    public class User : IUser
    {
        public User(string id, string nickname, Energy energy, int gold, int key, List<Mail> mails, Deck[] decks, InventorySkillBook[] inventorySkillBooks, EquippedParts equippedParts, List<InventoryPart> inventoryParts, SkillBookShop skillBookShop, PartShop partShop, GoldShop goldShop, EnergyShop energyShop) : base(id, nickname, energy, gold, key, mails, decks, inventorySkillBooks, equippedParts, inventoryParts, skillBookShop, partShop, goldShop, energyShop)
        {
        }

        public void AddEnergy(int amount) {
            this._energy.Charge(amount);
        }

        public void MinusEnergy(int amount) {
            this._energy.Use(amount);
        }

        public void UpdateEnergy(Energy energy) {
            this._energy = energy;
        }

        public void AddGold(int amount) {
            this.gold += amount;
        }

        public void MinusGold(int amount) {
            this.gold -= amount;
        }

        public void AddKey(int amount) {
            this.key += amount;
        }

        public void MinusKey(int amount) {
            this.key -= amount;
        }

        public void UpsertSkillBook(InventorySkillBook inventorySkillBook) {
            int index = Array.FindIndex(this._inventorySkillBooks, (e) => e.skillBook.id == inventorySkillBook.skillBook.id);
            if (this._inventorySkillBooks[index].isObtained == false) {
                this._inventorySkillBooks[index].ToObtained();
            }
            this._inventorySkillBooks[index] = inventorySkillBook;
        }

        public void UpgradeSkillBook(int skillBookId) {
            InventorySkillBook inventorySkillBook = Array.Find(this._inventorySkillBooks, (e) => e.skillBook.id == skillBookId);
            if (inventorySkillBook == null) return;
            SkillBookUpgradeSpec upgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (upgradeSpec == null) return;
            if (upgradeSpec.requiredAmount > inventorySkillBook.amount) return;
            if (upgradeSpec.requiredGold > this.gold) return;
            inventorySkillBook.IncreaseLevel();
            inventorySkillBook.MinusAmount(upgradeSpec.requiredAmount);
            this.MinusGold(upgradeSpec.requiredGold);
        }

        public void EquipSkillBook(int index, int skillBookId) {
            InventorySkillBook inventorySkillBook = Array.Find(this._inventorySkillBooks, (e) => e.skillBook.id == skillBookId);
            if (inventorySkillBook == null || inventorySkillBook.isObtained == false) return;
            Deck currentDeck = Array.Find(this._decks, (e) => e.isUse == true);
            if (currentDeck == null) return;
            currentDeck.ChangeSkillBook(index, skillBookId);
        }

        public void ChangeDeck(int index) {
            for (int i = 0; i < this._decks.Length; i++) {
                Deck deck = this._decks[i];
                if (i == index) {
                    deck.Use();
                } else {
                    deck.Unuse();
                }
            }
        }

        public void BuySkillBookInShop(int index) {
            this._skillBookShop.Buy(index);
        }

        public void ResetSkillBookShop(SkillBookShop skillBookShop) {
            this._skillBookShop = skillBookShop;
        }

        public void ResetSkillBookShopByAds(ShopSkillBook[] shopItems) {
            this._skillBookShop.ChangeShopItems(shopItems);
            this._skillBookShop.MinusRemainResetCount();
        }

        public void UpsertPart(InventoryPart inventoryPart) {
            int index = this._inventoryParts.FindIndex((e) => e.id == inventoryPart.id);
            if (index == -1) {
                this._inventoryParts.Add(inventoryPart);
            }
        }

        public void UpgradePart(string id, string[] materialIds) {
            InventoryPart inventoryPart = this._inventoryParts.Find((e) => e.id == id);
            if (inventoryPart == null) return;
            int earnedExp = 0; 
            foreach (string materialId in materialIds) {
                InventoryPart materialInventoryPart = this._inventoryParts.Find((e) => e.id == materialId);
                if (materialInventoryPart == null) continue;
                earnedExp += materialInventoryPart.exp;
            }
            int newExp = inventoryPart.exp + earnedExp;
            var upgradeSpecs = Array.FindAll(inventoryPart.part.upgradeSpecs, (e) => e.level > inventoryPart.level && newExp >= e.requiredExp);
            int requiredGold = 0;
            int newLevel = 0;
            foreach (var upgradeSpec in upgradeSpecs) {
                requiredGold += upgradeSpec.requiredGold;
                newLevel = Mathf.Max(newLevel, upgradeSpec.level);
            }
            if (requiredGold > this.gold) return;
            this.gold -= requiredGold;
            inventoryPart.Upgrade(newLevel, newExp);
            foreach (string materialId in materialIds) {
                int index = this._inventoryParts.FindIndex((e) => e.id == materialId);
                if (index == -1) continue;
                this._inventoryParts.RemoveAt(index);
            }
        }

        public void EquipPart(string id) {
            InventoryPart inventoryPart = this._inventoryParts.Find((e) => e.id == id);
            if (inventoryPart == null) return;
            this._equippedParts.Change(inventoryPart);
        }

        public void BuyPartInShop(int index) {
            this._partShop.Buy(index);
        }

        public void ResetPartShop(PartShop partShop) {
            this._partShop = partShop;
        }

        public void ResetPartShopByAds(ShopPart[] shopItems) {
            this._partShop.ChangeShopItems(shopItems);
            this._partShop.MinusRemainResetCount();
        }

        public void BuyGoldInShop(int index) {
            this._goldShop.Buy(index);
        }

        public void ResetGoldShop(GoldShop goldShop) {
            this._goldShop = goldShop;
        }

        public void ResetGoldShopByAds(ShopGold[] shopItems) {
            this._goldShop.ChangeShopItems(shopItems);
            this._goldShop.MinusRemainResetCount();
        }

        public void BuyEnergyInShop(int index) {
            this._energyShop.Buy(index);
        }

        public void ResetEnergyShop(EnergyShop energyShop) {
            this._energyShop = energyShop;
        }

        public void ResetEnergyShopByAds(ShopEnergy[] shopItems) {
            this._energyShop.ChangeShopItems(shopItems);
            this._energyShop.MinusRemainResetCount();
        }

        public void ReceiveMail(Mail mail) {
            Mail found = this._mails.Find((e) => e.id == mail.id);
            found.ToReceived();
        }

        public void RefreshMails(List<Mail> mails) {
            this._mails = mails;
        }
    }

    public class Chest {
        public int id { get; private set; }
        public string name { get; private set; }
        public Sprite image { get; private set; }
        public Cost cost { get; private set; }
        public ChestItem[] chestItems { get; private set; }
        public Chest(int id, string name, Sprite image, Cost cost, ChestItem[] chestItems) {
            this.id = id;
            this.name = name;
            this.image = image;
            this.cost = cost;
            this.chestItems = chestItems;
        }
    }

    public class ChestItem {
        public int chestId { get; private set; }
        public string name { get; private set; }
        public Sprite image { get; private set; }
        public Item[] items { get; private set; }
        public int amount { get; private set; }
        public float probability { get; private set; }
        public ChestItem(int chestId, string name, Sprite image, Item[] items, int amount, float probability) {
            this.chestId = chestId;
            this.name = name;
            this.image = image;
            this.items = items;
            this.amount = amount;
            this.probability = probability;
        }
    }
    
    public abstract class INotice {
        public int id { get; protected set; }
        public NoticeType type { get; protected set; }
        public string title { get; protected set; }
        public string content { get; protected set; }
        public DateTime createdAt { get; protected set; }
        public bool isRead { get; protected set; }
        public INotice(int id, NoticeType type, string title, string content, DateTime createdAt, bool isRead) {
            this.id = id;
            this.type = type;
            this.title = title;
            this.content = content;
            this.createdAt = createdAt;
            this.isRead = isRead;
        }
    }

    public class Notice : INotice
    {
        public Notice(int id, NoticeType type, string title, string content, DateTime createdAt, bool isRead) : base(id, type, title, content, createdAt, isRead)
        {
        }
        public void ToRead() {
            this.isRead = true;
        }
    }

    public abstract class ICurrentSetting {
        public bool BGM { get; protected set; }
        public bool SFX { get; protected set; }
        public ICurrentSetting(bool BGM, bool SFX) {
            this.BGM = BGM;
            this.SFX = SFX;
        }
    }

    public class CurrentSetting : ICurrentSetting
    {
        public CurrentSetting(bool BGM, bool SFX) : base(BGM, SFX)
        {
        }
        public void ToggleBGM(bool isOn) {
            this.BGM = isOn;
        }
        public void ToggleVFX(bool isOn) {
            this.SFX = isOn;
        }
    }

    public abstract class ISummaryStats {
        public Stats defaultStats { get; protected set; }
        public Stats armorStats { get; protected set; }
        public Stats weaponStats { get; protected set; }
        public Stats jewelryStats { get; protected set; }
        public int attackPower {
            get {
                return this.defaultStats.attackPower + this.armorStats.attackPower + this.weaponStats.attackPower + this.jewelryStats.attackPower;
            }
        }
        public float attackSpeed {
            get {
                return this.defaultStats.attackSpeed + this.armorStats.attackSpeed + this.weaponStats.attackSpeed + this.jewelryStats.attackSpeed;
            }
        }
        public int criticalDamage {
            get {
                return this.defaultStats.criticalDamage + this.armorStats.criticalDamage + this.weaponStats.criticalDamage + this.jewelryStats.criticalDamage;
            }
        }
        public int criticalRate {
            get {
                return this.defaultStats.criticalRate + this.armorStats.criticalRate + this.weaponStats.criticalRate + this.jewelryStats.criticalRate;
            }
        }
        public ISummaryStats(Stats defaultStats, Stats armorStats, Stats weaponStats, Stats jewelryStats) {
            this.defaultStats = defaultStats;
            this.armorStats = armorStats;
            this.weaponStats = weaponStats;
            this.jewelryStats = jewelryStats;
        }
    }

    public class SummaryStats : ISummaryStats
    {
        public SummaryStats(Stats defaultStats, Stats armorStats, Stats weaponStats, Stats jewelryStats) : base(defaultStats, armorStats, weaponStats, jewelryStats)
        {
        }
        public void ChangeArmorStat(Stats armorStats) {
            this.armorStats = armorStats;
        }
        public void ChangeWeaponStats(Stats weaponStats) {
            this.weaponStats = weaponStats;
        }
        public void ChangeJewelryStats(Stats jewelryStats) {
            this.jewelryStats = jewelryStats;
        }
    }
}
