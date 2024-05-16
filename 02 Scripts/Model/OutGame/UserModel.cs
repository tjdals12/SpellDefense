using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Model.OutGame {
    using ItemType = Repository.Item.ItemType;
    using PartType = Repository.Part.PartType;
    using Common;

    public abstract class IUserModel : MonoBehaviour {
        protected User _user;
        public IUser user { get => this._user; }
        protected SummaryStats _summaryStats;
        public ISummaryStats summaryStats { get => this._summaryStats; }

        public Action OnInitialize;
        public Action<string> OnReceiveMail;
        public Action<RewardItem> OnReceiveReward;
        public Action<List<string>> OnReceiveMails;
        public Action<List<RewardItem>> OnReceiveRewards;
        public Action OnRefreshMails;
        public Action OnChangeEnergy;
        public Action OnChangeGold;
        public Action OnChangeKey;
        public Action<int> OnEarnSkillBook;
        public Action<int> OnUpgradeSkillBook;
        public Action<int> OnEquipSkillBook;
        public Action<int> OnChangeDeck;
        public Action<int> OnBuySkillBookInShop;
        public Action OnResetSkillBookShop;
        public Action<string> OnEarnPart;
        public Action<string, string[]> OnUpgradePart;
        public Action<string> OnEquipPart;
        public Action<int> OnBuyPartInShop;
        public Action OnResetPartShop;
        public Action<int> OnBuyGoldInShop;
        public Action OnResetGoldShop;
        public Action<int> OnBuyEnergyInShop;
        public Action OnResetEnergyShop;
        public Action<Chest, int, List<RewardItem>> OnOpenChest;
        public Action OnGameStart;
    }

    public class UserModel : IUserModel
    {
        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<IUserModel>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        #endregion

        public void Initialize(User user) {
            this._user = user;

            Stats defaultStats = new Stats(attackPower: 30, attackSpeed: 1f, criticalDamage: 120, criticalRate: 0);
            IInventoryPart equippedArmor = this._user.GetEquippedArmor();
            Stats armorStats = equippedArmor.GetStats();
            IInventoryPart equippedWeapon = this._user.GetEquippedWeapon();
            Stats weaponStats = equippedWeapon.GetStats();
            IInventoryPart equippedJewelry = this._user.GetEquippedJewelry();
            Stats jewelryStats = equippedJewelry.GetStats();
            this._summaryStats = new SummaryStats(defaultStats, armorStats, weaponStats, jewelryStats);

            this.OnInitialize?.Invoke();
        }

        public void ReceiveMail(Mail mail) {
            this._user.ReceiveMail(mail);
            this.OnReceiveMail?.Invoke(mail.id);
            this.OnReceiveReward?.Invoke(mail.reward);
        }

        public void ReceiveMails(List<Mail> mails) {
            foreach (var mail in mails) {
                this._user.ReceiveMail(mail);
            }
            List<string> mailIds = mails.ConvertAll((e) => e.id);
            this.OnReceiveMails?.Invoke(mailIds);
            List<RewardItem> rewardItems = mails.ConvertAll((e) => e.reward);
            this.OnReceiveRewards?.Invoke(rewardItems);
        }

        public void RefreshMails(List<Mail> mails) {
            this._user.RefreshMails(mails);
            this.OnRefreshMails?.Invoke();
        }

        public void EarnCurrency(RewardItem rewardItem) {
            switch (rewardItem.item.id) {
                // Gold
                case 1:
                    this._user.AddGold(rewardItem.amount);
                    this.OnChangeGold?.Invoke();
                    break;
                // Energy
                case 3:
                    this._user.AddEnergy(rewardItem.amount);
                    this.OnChangeEnergy?.Invoke();
                    break;
            }
        }

        public void UseCurrency(Cost cost, int count = 1) {
            switch (cost.item.id) {
                // Gold
                case 1:
                    this._user.MinusGold(cost.amount * count);
                    this.OnChangeGold?.Invoke();
                    break;
                // Energy
                case 3:
                    this._user.MinusEnergy(cost.amount * count);
                    this.OnChangeEnergy?.Invoke();
                    break;
            }
        }

        public void EarnKey(RewardItem rewardItem) {
            switch (rewardItem.item.id) {
                // Silver Key
                case 10:
                    this._user.AddKey(rewardItem.amount);
                    this.OnChangeKey?.Invoke();
                    break;
            }
        }

        public void UseKey(Cost cost, int count = 1) {
            switch (cost.item.id) {
                // Silver Key
                case 10:
                    this._user.MinusKey(cost.amount * count);
                    this.OnChangeKey?.Invoke();
                    break;
            }
        }

        public void EarnSkillBook(InventorySkillBook inventorySkillBook) {
            this._user.UpsertSkillBook(inventorySkillBook);
            this.OnEarnSkillBook?.Invoke(inventorySkillBook.skillBook.id);
        }

        public void UpgradeSkillBook(int skillBookId) {
            this._user.UpgradeSkillBook(skillBookId);
            this.OnChangeGold?.Invoke();
            this.OnUpgradeSkillBook?.Invoke(skillBookId);
        }

        public void EquipSkillBook(int index, int skillBookId) {
            this._user.EquipSkillBook(index, skillBookId);
            this.OnEquipSkillBook?.Invoke(index);
        }

        public void ChangeDeck(int index) {
            this._user.ChangeDeck(index);
            this.OnChangeDeck?.Invoke(index);
        }

        public void BuySkillBookInShop(int index) {
            this._user.BuySkillBookInShop(index);
            this.OnBuySkillBookInShop?.Invoke(index);

            IShopSkillBook shopSkillBook = this._user.skillBookShop.shopItems[index];
            Cost cost = shopSkillBook.cost;
            if (cost.item.type == ItemType.Currency) {
                this.UseCurrency(cost);
            } else if (cost.item.type == ItemType.Key) {
                this.UseKey(cost);
            }

            RewardItem rewardItem = shopSkillBook.item;
            this.OnReceiveReward(rewardItem);
        }

        public void ResetSkillBookShop(SkillBookShop skillBookShop) {
            this._user.ResetSkillBookShop(skillBookShop);
            this.OnResetSkillBookShop?.Invoke();
        }

        public void ResetSkillBookShopByAds(ShopSkillBook[] shopItems) {
            this._user.ResetSkillBookShopByAds(shopItems);
            this.OnResetSkillBookShop?.Invoke();
        }

        public void EarnPart(InventoryPart inventoryPart) {
            this._user.UpsertPart(inventoryPart);
            this.OnEarnPart?.Invoke(inventoryPart.id);
        }

        public void UpgradePart(string id, string[] materialIds) {
            this._user.UpgradePart(id, materialIds);
            if (this._user.IsEquippedPart(id)) {
                InventoryPart inventoryPart = this._user.inventoryParts.Find((e) => e.id == id);
                Stats stats = inventoryPart.GetStats();
                switch (inventoryPart.part.type) {
                    case PartType.Armor:
                        this._summaryStats.ChangeArmorStat(stats);
                        break;
                    case PartType.Weapon:
                        this._summaryStats.ChangeWeaponStats(stats);
                        break;
                    case PartType.Jewelry:
                        this._summaryStats.ChangeJewelryStats(stats);
                        break;
                }
            }
            this.OnChangeGold?.Invoke();
            this.OnUpgradePart?.Invoke(id, materialIds);
        }

        public void EquipPart(string id) {
            this._user.EquipPart(id);
            InventoryPart inventoryPart = this._user.inventoryParts.Find((e) => e.id == id);
            Stats stats = inventoryPart.GetStats();
            switch (inventoryPart.part.type) {
                case PartType.Armor:
                    this._summaryStats.ChangeArmorStat(stats);
                    break;
                case PartType.Weapon:
                    this._summaryStats.ChangeWeaponStats(stats);
                    break;
                case PartType.Jewelry:
                    this._summaryStats.ChangeJewelryStats(stats);
                    break;
            }
            this.OnEquipPart?.Invoke(id);
        }

        public void BuyPartInShop(int index) {
            this._user.BuyPartInShop(index);
            this.OnBuyPartInShop?.Invoke(index);

            IShopPart shopPart = this._user.partShop.shopItems[index];
            Cost cost = shopPart.cost;
            if (cost.item.type == ItemType.Currency) {
                this.UseCurrency(cost);
            } else if (cost.item.type == ItemType.Key) {
                this.UseKey(cost);
            }

            RewardItem rewardItem = shopPart.item;
            this.OnReceiveReward?.Invoke(rewardItem);
        }

        public void ResetPartShop(PartShop partShop) {
            this._user.ResetPartShop(partShop);
            this.OnResetPartShop?.Invoke();
        }

        public void ResetPartShopByAds(ShopPart[] shopItems) {
            this._user.ResetPartShopByAds(shopItems);
            this.OnResetPartShop?.Invoke();
        }

        public void BuyGoldInShop(int index) {
            this._user.BuyGoldInShop(index);
            this.OnBuyGoldInShop?.Invoke(index);

            IShopGold shopGold = this._user.goldShop.shopItems[index];
            Cost cost = shopGold.cost;
            if (cost.item.type == ItemType.Currency) {
                this.UseCurrency(cost);
            } else if (cost.item.type == ItemType.Key) {
                this.UseKey(cost);
            }

            RewardItem rewardItem = shopGold.gold;
            this.OnReceiveReward?.Invoke(rewardItem);
        }

        public void ResetGoldShop(GoldShop goldShop) {
            this._user.ResetGoldShop(goldShop);
            this.OnResetGoldShop?.Invoke();
        }

        public void ResetGoldShopByAds(ShopGold[] shopItems) {
            this._user.ResetGoldShopByAds(shopItems);
            this.OnResetGoldShop?.Invoke();
        }

        public void BuyEnergyInShop(int index) {
            this._user.BuyEnergyInShop(index);
            this.OnBuyEnergyInShop?.Invoke(index);

            IShopEnergy shopEnergy = this._user.energyShop.shopItems[index];
            Cost cost = shopEnergy.cost;
            if (cost.item.type == ItemType.Currency) {
                this.UseCurrency(cost);
            } else if (cost.item.type == ItemType.Key) {
                this.UseKey(cost);
            }

            RewardItem rewardItem = shopEnergy.energy;
            this.OnReceiveReward?.Invoke(rewardItem);
        }

        public void ResetEnergyShop(EnergyShop energyShop) {
            this._user.ResetEnergyShop(energyShop);
            this.OnResetEnergyShop?.Invoke();
        }

        public void ResetEnergyShopByAds(ShopEnergy[] shopItems) {
            this._user.ResetEnergyShopByAds(shopItems);
            this.OnResetEnergyShop?.Invoke();
        }

        public void OpenChest(Chest chest, int count, List<RewardItem> rewardItems) {
            Cost cost = chest.cost;
            switch (cost.item.type) {
                case ItemType.Currency:
                    this.UseCurrency(cost, count);
                    break;
                case ItemType.Key:
                    this.UseKey(cost, count);
                    break;
            }
            this.OnOpenChest?.Invoke(chest, count, rewardItems);
        }

        public void GameStart(Energy energy) {
            this._user.UpdateEnergy(energy);
            this.OnChangeEnergy?.Invoke();
            this.OnGameStart?.Invoke();
        }
    }
}
