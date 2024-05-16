using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Newtonsoft.Json;

namespace Repository.User {
    using Request;
    using Response;
    using ValidationException = Core.CustomException.ValidationException;
    using ILocalizationRepository = Repository.Localization.IRepository;
    using IItemRepository = Repository.Item.IRepository;
    using ItemType = Repository.Item.ItemType;
    using ISkillBookRepository = Repository.SkillBook.IRepository;
    using IPartRepository = Repository.Part.IRepository;
    using PartType = Repository.Part.PartType;
    using ISkillBookShopRepository = Repository.SkillBookShop.IRepository;
    using IPartShopRepository = Repository.PartShop.IRepository;
    using IGoldShopRepository = Repository.GoldShop.IRepository;
    using IEnergyShopRepository = Repository.EnergyShop.IRepository;
    using IChestRepository = Repository.Chest.IRepository;
    using IWaveRepository = Repository.Wave.IRepository;

    public class FirebaseRepository : IRepository
    {
        ILocalizationRepository localizationRepository;
        IItemRepository itemRepository;
        ISkillBookRepository skillBookRepository;
        IPartRepository partRepository;
        ISkillBookShopRepository skillBookShopRepository;
        IPartShopRepository partShopRepository;
        IGoldShopRepository goldShopRepository;
        IEnergyShopRepository energyShopRepository;
        IChestRepository chestRepository;
        IWaveRepository waveRepository;
        DatabaseReference _usersRef;
        DatabaseReference usersRef {
            get {
                if (this._usersRef == null) {
                    this._usersRef = FirebaseDatabase.DefaultInstance.RootReference.Child("users");
                }
                return this._usersRef;
            }
        }
        DatabaseReference _systemMailsRef;
        DatabaseReference systemMailRef {
            get {
                if (this._systemMailsRef == null) {
                    this._systemMailsRef = FirebaseDatabase.DefaultInstance.RootReference.Child("systemMails");
                }
                return this._systemMailsRef;
            }
        }
        DatabaseReference _systemMailsLogRef;
        DatabaseReference systemMailsLogRef {
            get {
                if (this._systemMailsLogRef == null) {
                    this._systemMailsLogRef = FirebaseDatabase.DefaultInstance.RootReference.Child("systemMailsLog");
                }
                return this._systemMailsLogRef;
            }
        }

        Entity userEntity;


        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<IRepository>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        void Start() {
            this.localizationRepository = GameObject.FindObjectOfType<ILocalizationRepository>();
            this.itemRepository = GameObject.FindObjectOfType<IItemRepository>();
            this.skillBookRepository = GameObject.FindObjectOfType<ISkillBookRepository>();
            this.partRepository = GameObject.FindObjectOfType<IPartRepository>();
            this.skillBookShopRepository = GameObject.FindObjectOfType<ISkillBookShopRepository>();
            this.partShopRepository = GameObject.FindObjectOfType<IPartShopRepository>();
            this.goldShopRepository = GameObject.FindObjectOfType<IGoldShopRepository>();
            this.energyShopRepository = GameObject.FindObjectOfType<IEnergyShopRepository>();
            this.chestRepository = GameObject.FindObjectOfType<IChestRepository>();
            this.waveRepository = GameObject.FindObjectOfType<IWaveRepository>();
        }
        #endregion

        public override async Task<Model.OutGame.User> GetUser(string userId)
        {
            DataSnapshot dataSnapshot = await this.usersRef.Child(userId).GetValueAsync();
            string rawJson = dataSnapshot.GetRawJsonValue();
            if (rawJson == null) {
                this.userEntity = await this.CreateEntity(userId);
            } else {
                this.userEntity = JsonConvert.DeserializeObject<Entity>(rawJson);
                await this.TryResetSkillBookShop();
                await this.TryResetPartShop();
                await this.TryResetGoldShop();
                await this.TryResetEnergyShop();
                await this.ChargeEnergy();
            }
            await this.RefreshMails();
            return this.ToUser(this.userEntity);
        }

        public override async Task<GameStartResponse> GameStart(GameStartRequest request) {
            if (this.userEntity == null) throw new Exception();
            await this.ChargeEnergy();
            Cost cost = new Cost(itemId: 3, itemAmount: 5);
            bool isEnoughCost = this.CheckIsEnoughCost(cost);
            if (isEnoughCost == false) throw new ValidationException();
            Dictionary<string, object> result = this.PayCost(cost);
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            Model.OutGame.Energy energy = this.ToEnergy(this.userEntity.energy);
            GameStartResponse response = new GameStartResponse(energy);
            return response;
        }

        public override Task<Model.InGame.User> GetInGameUser() {
            if (this.userEntity == null) throw new Exception();
            return Task.FromResult(this.ToInGameUser(this.userEntity));
        }

        Model.InGame.User ToInGameUser(Entity userEntity) {
            EquippedParts equippedParts = userEntity.equippedParts;
            int[] deck = userEntity.decks.list[userEntity.decks.current];
            Model.InGame.User user = new Model.InGame.User(
                equippedParts: this.ToInGameEquippedParts(equippedParts),
                equippedSkillBooks: this.ToInGameEquippedSkillBooks(deck)
            );
            return user;
        }
        Model.InGame.EquippedParts ToInGameEquippedParts(EquippedParts equippedParts) {
            return new Model.InGame.EquippedParts(
                armor: this.ToInGameEquippedPart(equippedParts.armor),
                weapon: this.ToInGameEquippedPart(equippedParts.weapon),
                jewelry: this.ToInGameEquippedPart(equippedParts.jewelry)
            );
        }
        Model.InGame.EquippedPart ToInGameEquippedPart(string id) {
            Part userPart = this.userEntity.parts[id];
            Model.Common.Part part = this.partRepository.FindById(userPart.partId);
            return new Model.InGame.EquippedPart(part, userPart.level);
        }
        Model.InGame.EquippedSkillBook[] ToInGameEquippedSkillBooks(int[] deck) {
            List<Model.InGame.EquippedSkillBook> equippedSkillBooks = new();
            foreach (int skillBookId in deck) {
                Model.InGame.EquippedSkillBook equippedSkillBook = this.ToInGameEquippedSkillBook(skillBookId);
                equippedSkillBooks.Add(equippedSkillBook);
            }
            return equippedSkillBooks.ToArray();
        }
        Model.InGame.EquippedSkillBook ToInGameEquippedSkillBook(int skillBookId) {
            SkillBook userSkillBook = this.userEntity.skillBooks.Find((e) => e.id == skillBookId);
            Model.Common.SkillBook skillBook = this.skillBookRepository.FindById(skillBookId);
            Model.InGame.SkillBookAdditionalUpgradeSpec[] skillBookUpgradeSpecs = new Model.InGame.SkillBookAdditionalUpgradeSpec[] {
                new Model.InGame.SkillBookAdditionalUpgradeSpec(level: 1, requiredSp: 100),
                new Model.InGame.SkillBookAdditionalUpgradeSpec(level: 2, requiredSp: 300),
                new Model.InGame.SkillBookAdditionalUpgradeSpec(level: 3, requiredSp: 500),
                new Model.InGame.SkillBookAdditionalUpgradeSpec(level: 4, requiredSp: 800),
                new Model.InGame.SkillBookAdditionalUpgradeSpec(level: 5, requiredSp: 1200),
            };
            return new Model.InGame.EquippedSkillBook(
                skillBook: skillBook,
                level: userSkillBook.level,
                upgradeSpecs: skillBookUpgradeSpecs
            );
        }

        Dictionary<int, int> GetClearRewards(int clearWave) {
            Dictionary<int, int> summaryReward = new();
            int currentWave = 1;
            if (clearWave > 0) {
                while (true) {
                    var wave = this.waveRepository.FindByWave(currentWave);
                    if (wave.endWave > clearWave) {
                        foreach (var clearReward in wave.clearRewards) {
                            int amount = clearReward.amount * (clearWave - (wave.startWave - 1));
                            if (summaryReward.ContainsKey(clearReward.item.id)) {
                                summaryReward[clearReward.item.id] += amount;
                            } else {
                                summaryReward[clearReward.item.id] = amount;
                            }
                        }
                        currentWave = clearWave;
                        break;
                    }
                    foreach (var clearReward in wave.clearRewards) {
                        int amount = clearReward.amount * (wave.endWave - (wave.startWave - 1));
                        if (summaryReward.ContainsKey(clearReward.item.id)) {
                            summaryReward[clearReward.item.id] += amount;
                        } else {
                            summaryReward[clearReward.item.id] = amount;
                        }
                    }
                    currentWave = wave.endWave + 1;
                }
            }
            return summaryReward;
        }

        public override async Task<GameOverResponse> GameOver(GameOverRequest request) {
            Dictionary<int, int> summaryReward = this.GetClearRewards(request.wave);
            List<Reward> rewards = new();
            List<Model.OutGame.ReceivedReward> receivedRewards = new();
            foreach (var kv in summaryReward) {
                Reward reward = new Reward(kv.Key, kv.Value);
                rewards.Add(reward);
                Model.OutGame.ReceivedReward receivedReward = this.ToReceivedReward(reward);
                receivedRewards.Add(receivedReward);
            }
            Dictionary<string, object> result = this.ReceiveRewards(rewards);
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            GameOverResponse response = new GameOverResponse(receivedRewards);
            return response;
        }

        public override async Task<WatchAdsResponse> WatchAds(WatchAdsRequest request) {
            Dictionary<int, int> summaryReward = this.GetClearRewards(request.wave);
            List<Reward> rewards = new();
            List<Model.OutGame.ReceivedReward> receivedRewards = new();
            foreach (var kv in summaryReward) {
                Reward reward = new Reward(kv.Key, kv.Value);
                rewards.Add(reward);
                Model.OutGame.ReceivedReward receivedReward = this.ToReceivedReward(reward);
                receivedRewards.Add(receivedReward);
            }
            Dictionary<string, object> result = this.ReceiveRewards(rewards);
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            WatchAdsResponse response = new WatchAdsResponse(receivedRewards);
            return response;
        }

        public override async Task<ReceiveMailResponse> ReceiveMail(ReceiveMailRequest request) {
            int index = this.userEntity.mails.FindIndex((e) => e.id == request.mailId);
            if (index == -1) throw new ValidationException();
            Mail mail = this.userEntity.mails[index];
            Reward reward = mail.reward;
            Dictionary<string, object> result = this.ReceiveReward(reward);
            mail.ToReceived();
            result.Add($"mails/{index}", mail.ToDictionary());
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            string currentLanguage = this.localizationRepository.GetCurrentLanguage();
            Model.OutGame.Mail receivedMail = this.ToMail(mail, currentLanguage);
            Model.OutGame.ReceivedReward receivedReward = this.ToReceivedReward(reward);
            ReceiveMailResponse response = new ReceiveMailResponse(receivedMail, receivedReward);
            return response;
        }

        public override async Task<ReceiveAllMailResponse> ReceiveAllMail(ReceiveAllMailRequest request) {
            Dictionary<int, Mail> mails = new();
            List<Model.OutGame.Mail> receivedMails = new();
            List<Reward> rewards = new();
            List<Model.OutGame.ReceivedReward> receivedRewards = new();
            string currentLanguage = this.localizationRepository.GetCurrentLanguage();
            for (int i = 0; i < this.userEntity.mails.Count; i++) {
                var mail = this.userEntity.mails[i];
                if (mail.isExpired || mail.isReceived) continue;
                mail.ToReceived();
                mails.Add(i, mail);
                receivedMails.Add(this.ToMail(mail, currentLanguage));
                var reward = mail.reward;
                rewards.Add(reward);
            }
            Dictionary<string, object> result = this.ReceiveRewards(rewards);
            foreach (var kv in mails) {
                result.Add($"mails/{kv.Key}", kv.Value.ToDictionary());
            }
            foreach (var reward in rewards) {
                receivedRewards.Add(this.ToReceivedReward(reward));
            }
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            ReceiveAllMailResponse response = new ReceiveAllMailResponse(receivedMails, receivedRewards);
            return response;
        }

        public override async Task<OpenSilverChestResponse> OpenSilverChest(OpenSilverChestRequest request) {
            Model.OutGame.Chest chest = this.chestRepository.FindById(100);
            if (chest == null) throw new ValidationException();
            Cost cost = new Cost(chest.cost.item.id, chest.cost.amount * request.count);
            bool isEnoughCost = this.CheckIsEnoughCost(cost);
            if (isEnoughCost == false) throw new ValidationException();
            List<Reward> rewards = this.OpenChest(chest, openCount: request.count);
            Dictionary<string, object> payCostResult = this.PayCost(cost);
            Dictionary<string, object> result = this.ReceiveRewards(rewards);
            foreach (var kv in payCostResult) {
                result.Add(kv.Key, kv.Value);
            }
            List<Model.OutGame.ReceivedReward> receivedRewards = rewards.ConvertAll((e) => this.ToReceivedReward(e));
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            OpenSilverChestResponse response = new OpenSilverChestResponse(
                chest: chest,
                count: request.count,
                receivedRewards: receivedRewards
            );
            return response;
        }

        public override async Task<BuySkillBookInShopResponse> BuySkillBookInShop(BuySkillBookInShopRequest request) {
            SkillBookShop skillBookShop = this.userEntity.skillBookShop;
            if (request.index >= skillBookShop.items.Length) throw new ValidationException();
            ShopItem shopItem = skillBookShop.items[request.index];
            if (shopItem == null) throw new ValidationException();
            if (shopItem.remainBuyCount == 0) throw new ValidationException();
            shopItem.remainBuyCount -= 1;
            bool isEnoughCost = this.CheckIsEnoughCost(shopItem.cost);
            if (isEnoughCost == false) throw new ValidationException();
            Dictionary<string, object> payCostResult = this.PayCost(shopItem.cost);
            Reward reward = shopItem.reward;
            Dictionary<string, object> receiveRewardResult = this.ReceiveReward(reward);
            Dictionary<string, object> result = new Dictionary<string, object>() {
                { $"skillBookShop/items/{request.index}", shopItem.ToDictionary() }
            };
            foreach (var kv in payCostResult) {
                result.Add(kv.Key, kv.Value);
            }
            foreach (var kv in receiveRewardResult) {
                result.Add(kv.Key, kv.Value);
            }
            Model.OutGame.ShopSkillBook shopSkillBook = this.ToShopSkillBook(request.index, shopItem);
            Model.OutGame.ReceivedReward receivedReward = this.ToReceivedReward(reward);
            BuySkillBookInShopResponse response = new BuySkillBookInShopResponse(
                shopSkillBook,
                receivedReward
            );
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            return response;
        }

        public override async Task<Model.OutGame.SkillBookShop> ResetSkillBookShop(ResetSkillBookShopRequest request) {
            bool canReset = this.CanResetSkillBookShop();
            if (canReset == false) throw new ValidationException();
            await this.RefreshSkillBookShopItems();
            return this.ToSkillBookShop(this.userEntity.skillBookShop);
        }

        public override async Task<Model.OutGame.ShopSkillBook[]> ResetSkillBookShopByAds(ResetSkillBookShopByAdsRequest request) {
            SkillBookShop skillBookShop = this.userEntity.skillBookShop;
            if (skillBookShop.remainResetCount == 0) throw new ValidationException();
            ShopItem[] shopItems = this.GetNewShopSkillBooks();
            SkillBookShop newSkillBookShop = new SkillBookShop(
                items: shopItems,
                maxResetCount: skillBookShop.maxResetCount,
                remainResetCount: skillBookShop.remainResetCount - 1,
                lastResetTime: skillBookShop.lastResetTime
            ); 
            this.userEntity.skillBookShop = newSkillBookShop;
            string rawJson = JsonConvert.SerializeObject(newSkillBookShop);
            await this.usersRef.Child(this.userEntity.id).Child("skillBookShop").SetRawJsonValueAsync(rawJson);
            return this.ToShopSkillBooks(shopItems);
        }

        public override async Task<BuyPartInShopResponse> BuyPartInShop(BuyPartInShopRequest request) {
            PartShop partShop = this.userEntity.partShop;
            if (request.index >= partShop.items.Length) throw new ValidationException();
            ShopItem shopItem = partShop.items[request.index];
            if (shopItem == null) throw new ValidationException();
            if (shopItem.remainBuyCount == 0) throw new ValidationException();
            shopItem.remainBuyCount -= 1;
            bool isEnoughCost = this.CheckIsEnoughCost(shopItem.cost);
            if (isEnoughCost == false) throw new ValidationException();
            Dictionary<string, object> payCostResult = this.PayCost(shopItem.cost);
            Reward reward = shopItem.reward;
            Dictionary<string, object> receiveRewardResult = this.ReceiveReward(reward);
            Dictionary<string, object> result = new Dictionary<string, object>() {
                { $"partShop/items/{request.index}", shopItem.ToDictionary() }
            };
            foreach (var kv in payCostResult) {
                result.Add(kv.Key, kv.Value);
            }
            foreach (var kv in receiveRewardResult) {
                result.Add(kv.Key, kv.Value);
            }
            Model.OutGame.ShopPart shopPart = this.ToShopPart(request.index, shopItem);
            Model.OutGame.ReceivedReward receivedReward = this.ToReceivedReward(reward);
            BuyPartInShopResponse response = new BuyPartInShopResponse(
                shopPart,
                receivedReward
            );
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            return response;
        }

        public override async Task<Model.OutGame.PartShop> ResetPartShop(ResetPartShopRequest request) {
            bool canReset = this.CanResetPartShop();
            if (canReset == false) throw new ValidationException();
            await this.RefreshPartShopItems();
            return this.ToPartShop(this.userEntity.partShop);
        }

        public override async Task<Model.OutGame.ShopPart[]> ResetPartShopByAds(ResetPartShopByAdsRequest request) {
            PartShop partShop = this.userEntity.partShop;
            if (partShop.remainResetCount == 0) throw new ValidationException();
            ShopItem[] shopItems = this.GetNewShopParts();
            PartShop newPartShop = new PartShop(
                items: shopItems,
                maxResetCount: partShop.maxResetCount,
                remainResetCount: partShop.remainResetCount - 1,
                lastResetTime: partShop.lastResetTime
            );
            this.userEntity.partShop = newPartShop;
            string rawJson = JsonConvert.SerializeObject(newPartShop);
            await this.usersRef.Child(this.userEntity.id).Child("partShop").SetRawJsonValueAsync(rawJson);
            return this.ToShopParts(shopItems);
        }
        
        public override async Task<BuyGoldInShopResponse> BuyGoldInShop(BuyGoldInShopRequest request) {
            GoldShop goldShop = this.userEntity.goldShop;
            if (request.index >= goldShop.items.Length) throw new ValidationException();
            ShopItem shopItem = goldShop.items[request.index];
            if (shopItem == null) throw new ValidationException();
            if (shopItem.remainBuyCount == 0) throw new ValidationException();
            shopItem.remainBuyCount -= 1;
            bool isEnoughCost = this.CheckIsEnoughCost(shopItem.cost);
            if (isEnoughCost == false) throw new ValidationException();
            Dictionary<string, object> payCostResult = this.PayCost(shopItem.cost);
            Reward reward = shopItem.reward;
            Dictionary<string, object> receiveRewardResult = this.ReceiveReward(reward);
            Dictionary<string, object> result = new Dictionary<string, object>() {
                { $"goldShop/items/{request.index}", shopItem.ToDictionary() }
            };
            foreach (var kv in payCostResult) {
                result.Add(kv.Key, kv.Value);
            }
            foreach (var kv in receiveRewardResult) {
                result.Add(kv.Key, kv.Value);
            }
            Model.OutGame.ShopGold shopGold = this.ToShopGold(request.index, shopItem);
            Model.OutGame.ReceivedReward receivedReward = this.ToReceivedReward(reward);
            BuyGoldInShopResponse response = new BuyGoldInShopResponse(
                shopGold,
                receivedReward
            );
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            return response;
        }

        public override async Task<Model.OutGame.GoldShop> ResetGoldShop(ResetGoldShopRequest request) {
            bool canReset = this.CanResetGoldShop();
            if (canReset == false) throw new ValidationException();
            await this.RefreshGoldShopItems();
            return this.ToGoldShop(this.userEntity.goldShop);
        }

        public override async Task<Model.OutGame.ShopGold[]> ResetGoldShopByAds(ResetGoldShopByAdsRequest request) {
            GoldShop goldShop = this.userEntity.goldShop;
            if (goldShop.remainResetCount == 0) throw new ValidationException();
            ShopItem[] shopItems = this.GetNewShopGolds();
            GoldShop newGoldShop = new GoldShop(
                items: shopItems,
                maxResetCount: goldShop.maxResetCount,
                remainResetCount: goldShop.remainResetCount - 1,
                lastResetTime: goldShop.lastResetTime
            );
            this.userEntity.goldShop = newGoldShop;
            string rawJson = JsonConvert.SerializeObject(newGoldShop);
            await this.usersRef.Child(this.userEntity.id).Child("goldShop").SetRawJsonValueAsync(rawJson);
            return this.ToShopGolds(shopItems);
        }

        public override async Task<BuyEnergyInShopResponse> BuyEnergyInShop(BuyEnergyInShopRequest request) {
            EnergyShop energyShop = this.userEntity.energyShop;
            if (request.index >= energyShop.items.Length) throw new ValidationException();
            ShopItem shopItem = energyShop.items[request.index];
            if (shopItem == null) throw new ValidationException();
            if (shopItem.remainBuyCount == 0) throw new ValidationException();
            shopItem.remainBuyCount -= 1;
            bool isEnoughCost = this.CheckIsEnoughCost(shopItem.cost);
            if (isEnoughCost == false) throw new ValidationException();
            Dictionary<string, object> payCostResult = this.PayCost(shopItem.cost);
            Reward reward = shopItem.reward;
            Dictionary<string, object> receiveRewardResult = this.ReceiveReward(reward);
            Dictionary<string, object> result = new Dictionary<string, object>() {
                { $"energyShop/items/{request.index}", shopItem.ToDictionary() }
            };
            foreach (var kv in payCostResult) {
                result.Add(kv.Key, kv.Value);
            }
            foreach (var kv in receiveRewardResult) {
                result.Add(kv.Key, kv.Value);
            }
            Model.OutGame.ShopEnergy shopEnergy = this.ToShopEnergy(request.index, shopItem);
            Model.OutGame.ReceivedReward receivedReward = this.ToReceivedReward(reward);
            BuyEnergyInShopResponse response = new BuyEnergyInShopResponse(
                shopEnergy,
                receivedReward
            );
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
            return response;
        }

        public override async Task<Model.OutGame.EnergyShop> ResetEnergyShop(ResetEnergyShopRequest request) {
            bool canReset = this.CanResetEnergyShop();
            if (canReset == false) throw new ValidationException();
            await this.RefreshEnergyShopItems();
            return this.ToEnergyShop(this.userEntity.energyShop);
        }

        public override async Task<Model.OutGame.ShopEnergy[]> ResetEnergyShopByAds(ResetEnergyShopByAdsRequest request) {
            EnergyShop energyShop = this.userEntity.energyShop;
            if (energyShop.remainResetCount == 0) throw new ValidationException();
            ShopItem[] shopItems = this.GetNewShopEnergies();
            EnergyShop newEnergyShop = new EnergyShop(
                items: shopItems,
                maxResetCount: energyShop.maxResetCount,
                remainResetCount: energyShop.remainResetCount - 1,
                lastResetTime: energyShop.lastResetTime
            );
            this.userEntity.energyShop = newEnergyShop;
            string rawJson = JsonConvert.SerializeObject(newEnergyShop);
            await this.usersRef.Child(this.userEntity.id).Child("energyShop").SetRawJsonValueAsync(rawJson);
            return this.ToShopEnergies(shopItems);
        }

        bool CheckIsEnoughCost(Cost cost) {
            if (cost.itemId == 0 || cost.itemId == 4) { // Free or Ads
                return true;
            } else if (cost.itemId == 1) { // Gold
                return this.userEntity.gold >= cost.itemAmount;
            } else if (cost.itemId == 3) { // Energy
                return this.userEntity.energy.amount >= cost.itemAmount;
            } else if (cost.itemId == 10) { // Silver Key
                return this.userEntity.key >= cost.itemAmount;
            } else {
                return false;
            }
        }

        Dictionary<string, object> PayCost(Cost cost) {
            Dictionary<string, object> result = new();
            switch (cost.itemId) {
                // Gold
                case 1:
                    this.userEntity.gold -= cost.itemAmount;
                    result.Add("gold", this.userEntity.gold);
                    break;
                // Energy
                case 3:
                    this.userEntity.energy.Use(cost.itemAmount);
                    result.Add("energy", this.userEntity.energy.ToDictionary());
                    break;
                // Silver Key
                case 10:
                    this.userEntity.key -= cost.itemAmount;
                    result.Add("key", this.userEntity.key);
                    break;
            }
            return result;
        }

        public override async Task EquipPart(EquipPartRequest request) {
            bool isExists = this.userEntity.parts.TryGetValue(request.id, out Part userPart);
            if (isExists == false) throw new ValidationException();
            Model.Common.Part part = this.partRepository.FindById(userPart.partId);
            if (part == null) throw new ValidationException();
            EquippedParts equippedParts = this.userEntity.equippedParts;
            Dictionary<string, object> result = new();
            switch (part.type) {
                case PartType.Armor:
                    equippedParts.armor = userPart.id;
                    result.Add("equippedParts/armor", userPart.id);
                    break;
                case PartType.Weapon:
                    equippedParts.weapon = userPart.id;
                    result.Add("equippedParts/weapon", userPart.id);
                    break;
                case PartType.Jewelry:
                    equippedParts.jewelry = userPart.id;
                    result.Add("equippedParts/jewelry", userPart.id);
                    break;
            }
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
        }

        public override async Task UpgradePart(UpgradePartRequest request) {
            bool isExists = this.userEntity.parts.TryGetValue(request.id, out Part userPart);
            if (isExists == false) throw new ValidationException();
            Model.Common.Part part = this.partRepository.FindById(userPart.partId);
            if (part == null) throw new ValidationException();
            int exp = userPart.exp;
            foreach (string materialId in request.materialIds) {
                bool isExistsMaterial = this.userEntity.parts.TryGetValue(materialId, out Part userMaterialPart);
                if (userMaterialPart == null) continue;
                exp += userMaterialPart.exp;
            }
            int level = userPart.level;
            int gold = 0;
            while (true) {
                var upgradeSpec = Array.Find(part.upgradeSpecs, (e) => exp >= e.requiredExp && e.level == level + 1);
                if (level == userPart.level && upgradeSpec == null) throw new ValidationException();
                if (upgradeSpec == null) break;
                level += 1;
                gold += upgradeSpec.requiredGold;
            }
            if (gold > userEntity.gold) throw new ValidationException();
            this.userEntity.gold -= gold;
            userPart.exp = exp;
            userPart.level = level;
            Dictionary<string, object> result = new Dictionary<string, object>() {
                { "gold", this.userEntity.gold },
                { $"parts/{request.id}", userPart.ToDictionary() }
            };
            foreach (string materialId in request.materialIds) {
                if (this.userEntity.parts.ContainsKey(materialId) == false) continue;
                this.userEntity.parts.Remove(materialId);
                result.Add($"parts/{materialId}", null);
            }
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
        }

        public override async Task ChangeDeck(ChangeDeckRequest request) {
            Decks userDecks = this.userEntity.decks;
            if (request.index >= userDecks.list.Length) throw new ValidationException();
            userDecks.current = request.index;
            Dictionary<string, object> result = new Dictionary<string, object>() {
                { "decks/current", request.index }
            };
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
        }

        public override async Task EquipSkillBook(EquipSkillBookRequest request) {
            bool isExists = this.userEntity.skillBooks.Exists((e) => e.id == request.skillBookId);
            if (isExists == false) throw new ValidationException();

            Decks userDecks = this.userEntity.decks;
            int current = userDecks.current;
            int[] list = userDecks.list[current];
            if (request.index >= list.Length) throw new ValidationException();

            bool isEquip = Array.Exists(list, (e) => e == request.skillBookId);
            if (isEquip) throw new ValidationException();

            list[request.index] = request.skillBookId;
            Dictionary<string, object> result = new Dictionary<string, object>() {
                { $"decks/list/{current}", list }
            };
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
        }

        public override async Task UpgradeSkillBook(UpgradeSkillBookRequest request) {
            int index = this.userEntity.skillBooks.FindIndex((e) => e.id == request.skillBookId);
            if (index == -1) throw new ValidationException();
            SkillBook userSkillBook = this.userEntity.skillBooks[index];
            Model.Common.SkillBook skillBook = this.skillBookRepository.FindById(userSkillBook.id);
            if (skillBook == null) throw new ValidationException();
            var skillBookUpgradeSpec = Array.Find(skillBook.upgradeSpecs, (e) => e.level == userSkillBook.level + 1);
            if (skillBookUpgradeSpec.requiredGold > this.userEntity.gold) throw new ValidationException();
            if (skillBookUpgradeSpec.requiredAmount > userSkillBook.amount) throw new ValidationException();
            this.userEntity.gold -= skillBookUpgradeSpec.requiredGold;
            userSkillBook.amount -= skillBookUpgradeSpec.requiredAmount;
            userSkillBook.level += 1;
            Dictionary<string, object> result = new Dictionary<string, object>() {
                { "gold", this.userEntity.gold },
                { $"skillBooks/{index}", userSkillBook.ToDictionary() }
            };
            await this.usersRef.Child(this.userEntity.id).UpdateChildrenAsync(result);
        }

        public override async Task<List<Model.OutGame.Mail>> RefreshMails(RefreshMailsRequest request) {
            await this.RefreshMails();
            string currentLanguage = this.localizationRepository.GetCurrentLanguage();
            return this.ToMails(this.userEntity.mails, currentLanguage);
        }

        Nullable<KeyValuePair<string, object>> EarnReward(Reward reward) {
            Model.Common.Item item = this.itemRepository.FindById(reward.itemId);
            if (item.type == ItemType.Currency) {
                switch (item.id) {
                    // Gold
                    case 1:
                        this.userEntity.gold += reward.itemAmount;
                        return new KeyValuePair<string, object>("gold", this.userEntity.gold);
                    // Energy
                    case 3:
                        this.userEntity.energy.Charge(reward.itemAmount);
                        return new KeyValuePair<string, object>("energy", this.userEntity.energy.ToDictionary());
                }
            } else if (item.type == ItemType.Key) {
                switch (item.id) {
                    // Silver Key
                    case 10:
                        this.userEntity.key += reward.itemAmount;
                        return new KeyValuePair<string, object>("key", this.userEntity.key);
                }
            } else if (item.type == ItemType.SkillBook) {
                int index = this.userEntity.skillBooks.FindIndex((e) => e.id == item.id);
                if (index >= 0) {
                    this.userEntity.skillBooks[index].amount += reward.itemAmount;
                } else {
                    SkillBook skillBook = new SkillBook(
                        id: item.id,
                        level: 1,
                        amount: reward.itemAmount - 1
                    );
                    this.userEntity.skillBooks.Add(skillBook);
                    index = this.userEntity.skillBooks.Count - 1;
                }
                return new KeyValuePair<string, object>($"skillBooks/{index}", this.userEntity.skillBooks[index].ToDictionary());
            } else if (item.type == ItemType.Part) {
                Model.Common.Part p = this.partRepository.FindById(item.id);
                Part part = new Part(
                    id: reward.id,
                    partId: item.id,
                    level: 1,
                    exp: p.upgradeSpecs[0].requiredExp
                );
                this.userEntity.parts.Add(part.id, part);
                return new KeyValuePair<string, object>($"parts/{part.id}", part.ToDictionary());
            }
            return null;
        }

        List<Reward> MergeRewards(List<Reward> rewards) {
            List<Reward> mergedRewards = new();
            Dictionary<int, int> others = new();
            foreach (var reward in rewards) {
                Model.Common.Item item = this.itemRepository.FindById(reward.itemId);
                if (item.type == ItemType.Part) {
                    mergedRewards.Add(reward);
                } else {
                    if (others.ContainsKey(item.id)) {
                        others[item.id] += reward.itemAmount;
                    } else {
                        others.Add(item.id, reward.itemAmount);
                    }
                }
            }
            foreach (var other in others) {
                mergedRewards.Add(new Reward(other.Key, other.Value));
            }
            return mergedRewards;
        }

        Dictionary<string, object> ReceiveReward(Reward reward) {
            Dictionary<string, object> results = new();
            Nullable<KeyValuePair<string, object>> result = this.EarnReward(reward);
            if (result.HasValue) {
                results.Add(result.Value.Key, result.Value.Value);
            }
            return results;
        }

        Dictionary<string, object> ReceiveRewards(List<Reward> rewards) {
            Dictionary<string, object> results = new();
            List<Reward> mergedRewards = this.MergeRewards(rewards);
            foreach (var mergedReward in mergedRewards) {
                Nullable<KeyValuePair<string, object>> result = this.EarnReward(mergedReward);
                if (result.HasValue) {
                    results.Add(result.Value.Key, result.Value.Value);
                }
            }
            return results;
        }

        List<Model.OutGame.ReceivedReward> ToReceivedRewards(List<Reward> rewards) {
            List<Model.OutGame.ReceivedReward> receivedRewards = new();
            foreach (var reward in rewards) {
                Model.OutGame.ReceivedReward receivedReward = this.ToReceivedReward(reward);
                receivedRewards.Add(receivedReward);
            }
            return receivedRewards;
        }

        Model.OutGame.ReceivedReward ToReceivedReward(Reward reward) {
            Model.Common.Item item = this.itemRepository.FindById(reward.itemId);
            if (item.type == ItemType.SkillBook) {
                SkillBook skillBook = this.userEntity.skillBooks.Find((e) => e.id == reward.itemId);
                Model.OutGame.InventorySkillBook inventorySkillBook = this.ToInventorySkillBook(skillBook);
                Model.OutGame.RewardSkillBook rewardSkillBook = new Model.OutGame.RewardSkillBook(item, reward.itemAmount, inventorySkillBook);
                return new Model.OutGame.ReceivedReward(rewardSkillBook);
            } else if (item.type == ItemType.Part) {
                Part part = this.userEntity.parts[reward.id];
                Model.OutGame.InventoryPart inventoryPart = this.ToInventoryPart(part);
                Model.OutGame.RewardPart rewardPart = new Model.OutGame.RewardPart(item, reward.itemAmount, inventoryPart);
                return new Model.OutGame.ReceivedReward(rewardPart);
            } else {
                Model.Common.RewardItem rewardItem = new Model.Common.RewardItem(item, reward.itemAmount);
                return new Model.OutGame.ReceivedReward(rewardItem);
            }
        }

        async Task<bool> TryResetSkillBookShop() {
            bool canReset = this.CanResetSkillBookShop();
            if (canReset == false) return false;
            await this.RefreshSkillBookShopItems();
            return true;
        }
        bool CanResetSkillBookShop() {
            var skillBookShop = this.userEntity.skillBookShop;
            TimeSpan timeSpan = DateTime.UtcNow.Date - skillBookShop.lastResetTime.Date;
            return timeSpan.TotalDays >= 1;
        }
        async Task RefreshSkillBookShopItems() {
            ShopItem[] shopItems = this.GetNewShopSkillBooks();
            SkillBookShop newSkillBookShop = new SkillBookShop(
                items: shopItems,
                maxResetCount: 3,
                remainResetCount: 3,
                lastResetTime: DateTime.UtcNow
            );
            this.userEntity.skillBookShop = newSkillBookShop;
            string rawJson = JsonConvert.SerializeObject(newSkillBookShop);
            await this.usersRef.Child(this.userEntity.id).Child("skillBookShop").SetRawJsonValueAsync(rawJson);
        }
        ShopItem[] GetNewShopSkillBooks() {
            List<ShopItem> shopItems = new();
            for (int i = 0; i < 3; i++) {
                var shopItemInfos = this.skillBookShopRepository.FindAllByIndex(i);
                float probability = Random.Range(0f, 100f);
                var shopItemInfo = Array.Find(shopItemInfos, (e) => e.probability >= probability);
                int shopItemId = shopItemInfo.itemIds[Random.Range(0, shopItemInfo.itemIds.Length)];
                Reward reward = new Reward(
                    itemId: shopItemId,
                    itemAmount: shopItemInfo.itemAmount
                );
                Cost cost = new Cost(
                    itemId: shopItemInfo.costId,
                    itemAmount: shopItemInfo.costAmount
                );
                shopItems.Add(
                    new ShopItem(
                        reward: reward,
                        cost: cost,
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    )
                );
            }
            return shopItems.ToArray();
        }

        async Task<bool> TryResetPartShop() {
            bool canReset = this.CanResetPartShop();
            if (canReset == false) return false;
            await this.RefreshPartShopItems();
            return true;
        }
        bool CanResetPartShop() {
            var partShop = this.userEntity.partShop;
            TimeSpan timeSpan = DateTime.UtcNow.Date - partShop.lastResetTime.Date;
            return timeSpan.TotalDays >= 1;
        }
        async Task RefreshPartShopItems() {
            ShopItem[] shopItems = this.GetNewShopParts();
            PartShop newPartShop = new PartShop(
                items: shopItems,
                maxResetCount: 3,
                remainResetCount: 3,
                lastResetTime: DateTime.UtcNow
            );
            this.userEntity.partShop = newPartShop;
            string rawJson = JsonConvert.SerializeObject(newPartShop);
            await this.usersRef.Child(this.userEntity.id).Child("partShop").SetRawJsonValueAsync(rawJson);
        }

        ShopItem[] GetNewShopParts() {
            List<ShopItem> shopItems = new();
            for (int i = 0; i < 3; i++) {
                var shopItemInfos = this.partShopRepository.FindAllByIndex(i);
                float probability = Random.Range(0f, 100f);
                var shopItemInfo = Array.Find(shopItemInfos, (e) => e.probability >= probability);
                int shopItemId = shopItemInfo.itemIds[Random.Range(0, shopItemInfo.itemIds.Length)];
                Reward reward = new Reward(
                    id: Guid.NewGuid().ToString(),
                    itemId: shopItemId,
                    itemAmount: shopItemInfo.itemAmount
                );
                Cost cost = new Cost(
                    itemId: shopItemInfo.costId,
                    itemAmount: shopItemInfo.costAmount
                );
                shopItems.Add(
                    new ShopItem(
                        reward: reward,
                        cost: cost,
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    )
                );
            }
            return shopItems.ToArray();
        }

        async Task<bool> TryResetGoldShop() {
            bool canReset = this.CanResetGoldShop();
            if (canReset == false) return false;
            await this.RefreshGoldShopItems();
            return true;
        }
        bool CanResetGoldShop() {
            var goldShop = this.userEntity.goldShop;
            TimeSpan timeSpan = DateTime.UtcNow.Date - goldShop.lastResetTime.Date;
            return timeSpan.TotalDays >= 1;
        }
        async Task RefreshGoldShopItems() {
            ShopItem[] shopItems = this.GetNewShopGolds();
            GoldShop newGoldShop = new GoldShop(
                items: shopItems,
                maxResetCount: 3,
                remainResetCount: 3,
                lastResetTime: DateTime.UtcNow
            );
            this.userEntity.goldShop = newGoldShop;
            string rawJson = JsonConvert.SerializeObject(newGoldShop);
            await this.usersRef.Child(this.userEntity.id).Child("goldShop").SetRawJsonValueAsync(rawJson);
        }

        ShopItem[] GetNewShopGolds() {
            List<ShopItem> shopItems = new();
            var shopItemInfos = this.goldShopRepository.goldShops;
            foreach (var shopItemInfo in shopItemInfos) {
                Reward reward = new Reward(
                    itemId: shopItemInfo.itemId,
                    itemAmount: shopItemInfo.itemAmount
                );
                Cost cost = new Cost(
                    itemId: shopItemInfo.costId,
                    itemAmount: shopItemInfo.costAmount
                );
                shopItems.Add(
                    new ShopItem(
                        reward: reward,
                        cost: cost,
                        maxBuyCount: shopItemInfo.buyCount,
                        remainBuyCount: shopItemInfo.buyCount
                    )
                );
            }
            return shopItems.ToArray();
        }

        async Task<bool> TryResetEnergyShop() {
            bool canReset = this.CanResetEnergyShop();
            if (canReset == false) return false;
            await this.RefreshEnergyShopItems();
            return true;
        }
        bool CanResetEnergyShop() {
            var energyShop = this.userEntity.energyShop;
            TimeSpan timeSpan = DateTime.UtcNow.Date - energyShop.lastResetTime.Date;
            return timeSpan.TotalDays >= 1;
        }
        async Task RefreshEnergyShopItems() {
            ShopItem[] shopItems = this.GetNewShopEnergies();
            EnergyShop newEnergyShop = new EnergyShop(
                items: shopItems,
                maxResetCount: 3,
                remainResetCount: 3,
                lastResetTime: DateTime.UtcNow
            );
            this.userEntity.energyShop = newEnergyShop;
            string rawJson = JsonConvert.SerializeObject(newEnergyShop);
            await this.usersRef.Child(this.userEntity.id).Child("energyShop").SetRawJsonValueAsync(rawJson);
        }

        ShopItem[] GetNewShopEnergies() {
            List<ShopItem> shopItems = new();
            var shopItemInfos = this.energyShopRepository.energyShops;
            foreach (var shopItemInfo in shopItemInfos) {
                Reward reward = new Reward(
                    itemId: shopItemInfo.itemId,
                    itemAmount: shopItemInfo.itemAmount
                );
                Cost cost = new Cost(
                    itemId: shopItemInfo.costId,
                    itemAmount: shopItemInfo.costAmount
                );
                shopItems.Add(
                    new ShopItem(
                        reward: reward,
                        cost: cost,
                        maxBuyCount: shopItemInfo.buyCount,
                        remainBuyCount: shopItemInfo.buyCount
                    )
                );
            }
            return shopItems.ToArray();
        }

        async Task ChargeEnergy() {
            Energy energy = this.userEntity.energy;
            if (30 > energy.amount) {
                int elapsedSeconds = ((int)(DateTime.UtcNow - energy.lastChargeTime).TotalSeconds);
                int chargeEnergy = elapsedSeconds / 300;
                if (chargeEnergy > 0) {
                    int remainingSeconds = elapsedSeconds % 300;
                    int amount = Mathf.Min(energy.amount + chargeEnergy, 30);
                    DateTime lastChargeTime = DateTime.UtcNow.AddSeconds(remainingSeconds * -1);
                    Energy newEnergy = new Energy(
                        amount: amount,
                        lastChargeTime: lastChargeTime
                    );
                    this.userEntity.energy = newEnergy;
                    string rawJson = JsonConvert.SerializeObject(newEnergy);
                    await this.usersRef.Child(this.userEntity.id).Child("energy").SetRawJsonValueAsync(rawJson);
                }
            }
        }

        async Task RefreshMails() {
            DataSnapshot systemMailsDataSnapshot = await this.systemMailRef.GetValueAsync();
            string systemMailsRawJson = systemMailsDataSnapshot.GetRawJsonValue();
            Dictionary<string, SystemMail> systemMails = systemMailsRawJson == null
                ? new()
                : JsonConvert.DeserializeObject<Dictionary<string, SystemMail>>(systemMailsRawJson);

            DataSnapshot systemMailsLogDataSnapshot = await this.systemMailsLogRef.Child(this.userEntity.id).GetValueAsync();
            string systemMailsLogRawJson = systemMailsLogDataSnapshot.GetRawJsonValue();
            Dictionary<string, string> systemMailsLog = systemMailsLogRawJson == null
                ? new()
                : JsonConvert.DeserializeObject<Dictionary<string, string>>(systemMailsLogRawJson);

            DateTime now = DateTime.UtcNow;
            List<Mail> mails = this.userEntity.mails.FindAll((e) => {
                int remainSeconds = ((int)(e.expiredAt - now).TotalSeconds);
                return remainSeconds > 0;
            });
            Dictionary<string, object> receivedSystemMails = new();
            foreach (var systemMail in systemMails.Values) {
                if (systemMailsLog.ContainsKey(systemMail.id)) continue;
                if (systemMail.CanReceive() == false) continue;
                Mail mail = new Mail(
                    id: systemMail.id,
                    titles: systemMail.titles,
                    createdAt: now,
                    expiredAt: now.AddDays(systemMail.storeDays),
                    receivedAt: now,
                    isReceived: false,
                    reward: new Reward(
                        systemMail.reward.id ?? DateTime.UtcNow.ToString("yyMMddssfff"),
                        systemMail.reward.itemId,
                        systemMail.reward.itemAmount
                    )
                );
                mails.Add(mail);
                receivedSystemMails.Add(systemMail.id, now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ffffffZ"));
            }
            this.userEntity.mails = mails;
            string rawJson = JsonConvert.SerializeObject(mails);
            await this.usersRef.Child(this.userEntity.id).Child("mails").SetRawJsonValueAsync(rawJson);
            await this.systemMailsLogRef.Child(this.userEntity.id).UpdateChildrenAsync(receivedSystemMails);
        }

        List<Reward> OpenChest(Model.OutGame.Chest chest, int openCount) {
            if (chest == null) throw new ValidationException();
            Dictionary<int, int> rewardsDict = new();
            for (int i = 0; i < openCount; i++) {
                foreach (var chestItem in chest.chestItems) {
                    float probability = Random.Range(0.0f, 100f);
                    if (probability > chestItem.probability) continue;
                    int itemIndex = Random.Range(0, chestItem.items.Length);
                    var item = chestItem.items[itemIndex];
                    if (rewardsDict.ContainsKey(item.id)) {
                        rewardsDict[item.id] += chestItem.amount;
                    } else {
                        rewardsDict[item.id] = chestItem.amount;
                    }
                }
            }
            List<Reward> rewards = new();
            foreach (var kv in rewardsDict) {
                Reward reward = new Reward(kv.Key, kv.Value);
                rewards.Add(reward);
            }
            return rewards;
        }

        async Task<Entity> CreateEntity(string userId) {
            Energy energy = new Energy(30, DateTime.UtcNow);
            Decks decks = new Decks(
                current: 0,
                list: new int[3][] {
                    new int[5] { 1001, 1002, 1003, 1004, 1005 },
                    new int[5] { 1001, 1002, 1003, 1004, 1005 },
                    new int[5] { 1001, 1002, 1003, 1004, 1005 }
                }
            );
            List<Mail> mails = new();
            // DateTime utcNow = DateTime.UtcNow;
            // int[] rewardPool = new int[] { 1, 3, 10 };
            // for (int i = 0; i < 10; i++) {
            //     DateTime createdAt = utcNow.AddDays(-i);
            //     DateTime expiredAt = createdAt.AddDays(15);
            //     bool isExpired = (utcNow - expiredAt).TotalSeconds > 0;
            //     mails.Add(new Mail(
            //         id: i,
            //         title: $"테스트 메일 {i}",
            //         createdAt: createdAt,
            //         expiredAt: expiredAt,
            //         receivedAt: createdAt,
            //         isReceived: false,
            //         reward: new Reward(rewardPool[Random.Range(0, rewardPool.Length)], 1000)
            //     ));
            // }
            List<SkillBook> skillBooks = new() {
                new SkillBook(id: 1001, level: 1, amount: 0),
                new SkillBook(id: 1002, level: 1, amount: 0),
                new SkillBook(id: 1003, level: 1, amount: 0),
                new SkillBook(id: 1004, level: 1, amount: 0),
                new SkillBook(id: 1005, level: 1, amount: 0),
            };
            EquippedParts equippedParts = new EquippedParts(
                weapon: "DEFAULT_1",
                armor: "DEFAULT_0",
                jewelry: "DEFAULT_2"
            );
            Dictionary<string, Part> parts = new Dictionary<string, Part>() {
                { "DEFAULT_0", new Part(id: "DEFAULT_0", partId: 2001, level: 1, exp: 50) },
                { "DEFAULT_1", new Part(id: "DEFAULT_1", partId: 2004, level: 1, exp: 50) },
                { "DEFAULT_2", new Part(id: "DEFAULT_2", partId: 2007, level: 1, exp: 50) },
            };
            SkillBookShop skillBookShop = new SkillBookShop(
                items: new ShopItem[3] {
                    new ShopItem(
                        reward: new Reward(itemId: 1001, itemAmount: 15),
                        cost: new Cost(itemId: 1, itemAmount: 1000),
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    ),
                    new ShopItem(
                        reward: new Reward(itemId: 1002, itemAmount: 15),
                        cost: new Cost(itemId: 1, itemAmount: 1000),
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    ),
                    new ShopItem(
                        reward: new Reward(itemId: 1003, itemAmount: 5),
                        cost: new Cost(itemId: 1, itemAmount: 3000),
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    )
                },
                maxResetCount: 3,
                remainResetCount: 3,
                lastResetTime: DateTime.UtcNow
            );
            PartShop partShop = new PartShop(
                items: new ShopItem[3] {
                    new ShopItem(
                        reward: new Reward(id: Guid.NewGuid().ToString(), itemId: 2001, itemAmount: 1),
                        cost: new Cost(itemId: 1, itemAmount: 1000),
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    ),
                    new ShopItem(
                        reward: new Reward(id: Guid.NewGuid().ToString(), itemId: 2004, itemAmount: 1),
                        cost: new Cost(itemId: 1, itemAmount: 1000),
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    ),
                    new ShopItem(
                        reward: new Reward(id: Guid.NewGuid().ToString(), itemId: 2007, itemAmount: 1),
                        cost: new Cost(itemId: 1, itemAmount: 1000),
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    )
                },
                maxResetCount: 3,
                remainResetCount: 3,
                lastResetTime: DateTime.UtcNow
            );
            GoldShop goldShop = new GoldShop(
                items: new ShopItem[2] {
                    new ShopItem(
                        reward: new Reward(itemId: 1, itemAmount: 3000),
                        cost: new Cost(itemId: 0, itemAmount: 0),
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    ),
                    new ShopItem(
                        reward: new Reward(itemId: 1, itemAmount: 5000),
                        cost: new Cost(itemId: 4, itemAmount: 0),
                        maxBuyCount: 3,
                        remainBuyCount: 3
                    ),
                },
                maxResetCount: 3,
                remainResetCount: 3,
                lastResetTime: DateTime.UtcNow
            );
            EnergyShop energyShop = new EnergyShop(
                items: new ShopItem[2] {
                    new ShopItem(
                        reward: new Reward(itemId: 3, itemAmount: 5),
                        cost: new Cost(itemId: 0, itemAmount: 0),
                        maxBuyCount: 1,
                        remainBuyCount: 1
                    ),
                    new ShopItem(
                        reward: new Reward(itemId: 3, itemAmount: 15),
                        cost: new Cost(itemId: 4, itemAmount: 0),
                        maxBuyCount: 3,
                        remainBuyCount: 3
                    ),
                },
                maxResetCount: 3,
                remainResetCount: 3,
                lastResetTime: DateTime.UtcNow
            );
            Entity entity = new Entity(
                id: userId,
                nickname: "GUEST",
                energy: energy,
                gold: 1000,
                key: 0,
                decks: decks,
                mails: mails,
                skillBooks: skillBooks,
                equippedParts: equippedParts,
                parts: parts,
                skillBookShop: skillBookShop,
                partShop: partShop,
                goldShop: goldShop,
                energyShop: energyShop
            );
            string rawJson = JsonConvert.SerializeObject(entity);
            await this.usersRef.Child(userId).SetRawJsonValueAsync(rawJson);
            return entity;
        }

        Model.OutGame.User ToUser(Entity userEntity) {
            string currentLanguage = this.localizationRepository.GetCurrentLanguage();
            Model.OutGame.User user = new Model.OutGame.User(
                id: userEntity.id,
                nickname: userEntity.nickname,
                energy: this.ToEnergy(userEntity.energy),
                gold: userEntity.gold,
                key: userEntity.key,
                mails: this.ToMails(userEntity.mails, currentLanguage),
                decks: this.ToDecks(userEntity.decks),
                inventorySkillBooks: this.ToInventorySkillBooks(userEntity.skillBooks),
                equippedParts: this.ToEquippedParts(userEntity.equippedParts),
                inventoryParts: this.ToInventoryParts(userEntity.parts),
                skillBookShop: this.ToSkillBookShop(userEntity.skillBookShop),
                partShop: this.ToPartShop(userEntity.partShop),
                goldShop: this.ToGoldShop(userEntity.goldShop),
                energyShop: this.ToEnergyShop(userEntity.energyShop)
            );
            return user;
        }

        Model.OutGame.Energy ToEnergy(Energy energy) {
            return new Model.OutGame.Energy(
                amount: energy.amount,
                lastChargeTime: energy.lastChargeTime
            );
        }
        List<Model.OutGame.Mail> ToMails(List<Mail> userMails, string language) {
            List<Model.OutGame.Mail> mails = new();
            foreach (var userMail in userMails) {
                mails.Add(this.ToMail(userMail, language));
            }
            return mails;
        }
        Model.OutGame.Mail ToMail(Mail mail, string language) {
            Model.Common.Item item = this.itemRepository.FindById(mail.reward.itemId);
            Model.Common.RewardItem reward = new Model.Common.RewardItem(
                item: item,
                amount: mail.reward.itemAmount
            );
            return new Model.OutGame.Mail(
                id: mail.id,
                title: mail.titles[language],
                createdAt: mail.createdAt,
                expiredAt: mail.expiredAt,
                isExpired: mail.isExpired,
                receivedAt: mail.receivedAt,
                isReceived: mail.isReceived,
                reward: reward
            );
        }
        Model.OutGame.Deck[] ToDecks(Decks userDecks) {
            Model.OutGame.Deck[] decks = new Model.OutGame.Deck[userDecks.list.Length];
            for (int i = 0; i < decks.Length; i++) {
                bool isUse = false;
                if (i == userDecks.current) {
                    isUse = true;
                }
                int[] list = userDecks.list[i];
                decks[i] = new Model.OutGame.Deck(isUse, list);
            }
            return decks;
        }
        Model.OutGame.InventorySkillBook[] ToInventorySkillBooks(List<SkillBook> userSkillBooks) {
            List<Model.OutGame.InventorySkillBook> inventorySkillBooks = new();
            foreach (var skillBook in this.skillBookRepository.skillBooks) {
                SkillBook userSkillBook = userSkillBooks.Find((e) => e.id == skillBook.id);
                if (userSkillBook == null) {
                    inventorySkillBooks.Add(this.ToInventorySkillBook(skillBook));
                } else {
                    inventorySkillBooks.Add(this.ToInventorySkillBook(userSkillBook));
                }
            }
            return inventorySkillBooks.ToArray();
        }
        Model.OutGame.InventorySkillBook ToInventorySkillBook(SkillBook userSkillBook) {
            Model.Common.SkillBook skillBook = this.skillBookRepository.FindById(userSkillBook.id);
            return new Model.OutGame.InventorySkillBook(
                skillBook: skillBook,
                level: userSkillBook.level,
                amount: userSkillBook.amount,
                isObtained: true
            );
        }
        Model.OutGame.InventorySkillBook ToInventorySkillBook(Model.Common.SkillBook skillBook) {
            return new Model.OutGame.InventorySkillBook(
                skillBook: skillBook,
                level: 1,
                amount: 0,
                isObtained: false
            );
        }
        Model.OutGame.EquippedParts ToEquippedParts(EquippedParts equippedParts) {
            return new Model.OutGame.EquippedParts(
                armor: equippedParts.armor,
                weapon: equippedParts.weapon,
                jewelry: equippedParts.jewelry
            );
        }
        List<Model.OutGame.InventoryPart> ToInventoryParts(Dictionary<string, Part> userParts) {
            List<Model.OutGame.InventoryPart> inventoryParts = new();
            foreach (var userPart in userParts.Values) {
                inventoryParts.Add(this.ToInventoryPart(userPart));
            }
            return inventoryParts;
        }
        Model.OutGame.InventoryPart ToInventoryPart(Part userPart) {
            Model.Common.Part part = this.partRepository.FindById(userPart.partId);
            return new Model.OutGame.InventoryPart(
                id: userPart.id,
                part: part,
                level: userPart.level,
                exp: userPart.exp
            );
        }
        Model.OutGame.ShopSkillBook ToShopSkillBook(int index, ShopItem userShopItem) {
            Model.Common.Item shopItem = this.itemRepository.FindById(userShopItem.reward.itemId);
            Model.Common.RewardItem rewardItem = new Model.Common.RewardItem(
                item: shopItem,
                amount: userShopItem.reward.itemAmount
            );
            Model.Common.Item costItem = this.itemRepository.FindById(userShopItem.cost.itemId);
            Model.Common.Cost cost = new Model.Common.Cost(
                item: costItem,
                amount: userShopItem.cost.itemAmount
            );
            Model.Common.SkillBook skillBook = this.skillBookRepository.FindById(shopItem.id);
            return new Model.OutGame.ShopSkillBook(
                skillBook: skillBook,
                item: rewardItem,
                index: index,
                cost: cost,
                maxBuyCount: userShopItem.maxBuyCount,
                remainBuyCount: userShopItem.remainBuyCount
            );
        }
        Model.OutGame.ShopSkillBook[] ToShopSkillBooks(ShopItem[] userShopItems) {
            List<Model.OutGame.ShopSkillBook> shopSkillBooks = new();
            for (int i = 0; i < userShopItems.Length; i++) {
                Model.OutGame.ShopSkillBook shopSkillBook = this.ToShopSkillBook(i, userShopItems[i]);
                shopSkillBooks.Add(shopSkillBook);
            }
            return shopSkillBooks.ToArray();
        }
        Model.OutGame.SkillBookShop ToSkillBookShop(SkillBookShop skillBookShop) {
            Model.OutGame.ShopSkillBook[] shopSkillBooks = this.ToShopSkillBooks(skillBookShop.items);
            return new Model.OutGame.SkillBookShop(
                shopItems: shopSkillBooks,
                maxResetCount: skillBookShop.maxResetCount,
                remainResetCount: skillBookShop.remainResetCount,
                lastResetTime: skillBookShop.lastResetTime
            );
        }
        Model.OutGame.ShopPart ToShopPart(int index, ShopItem userShopItem) {
            Model.Common.Item shopItem = this.itemRepository.FindById(userShopItem.reward.itemId);
            Model.Common.RewardItem rewardItem = new Model.Common.RewardItem(
                item: shopItem,
                amount: userShopItem.reward.itemAmount
            );
            Model.Common.Item costItem = this.itemRepository.FindById(userShopItem.cost.itemId);
            Model.Common.Cost cost = new Model.Common.Cost(
                item: costItem,
                amount: userShopItem.cost.itemAmount
            );
            Model.Common.Part part = this.partRepository.FindById(shopItem.id);
            return new Model.OutGame.ShopPart(
                part: part,
                item: rewardItem,
                index: index,
                cost: cost,
                maxBuyCount: userShopItem.maxBuyCount,
                remainBuyCount: userShopItem.remainBuyCount
            );
        }
        Model.OutGame.ShopPart[] ToShopParts(ShopItem[] userShopItems) {
            List<Model.OutGame.ShopPart> shopParts = new();
            for (int i = 0; i < userShopItems.Length; i++) {
                Model.OutGame.ShopPart shopPart = this.ToShopPart(i, userShopItems[i]);
                shopParts.Add(shopPart);
            }
            return shopParts.ToArray();
        }
        Model.OutGame.PartShop ToPartShop(PartShop partShop) {
            Model.OutGame.ShopPart[] shopParts = this.ToShopParts(partShop.items);
            return new Model.OutGame.PartShop(
                shopItems: shopParts,
                maxResetCount: partShop.maxResetCount,
                remainResetCount: partShop.remainResetCount,
                lastResetTime: partShop.lastResetTime
            );
        }
        Model.OutGame.ShopGold ToShopGold(int index, ShopItem userShopItem) {
            Model.Common.Item shopItem = this.itemRepository.FindById(userShopItem.reward.itemId);
            Model.Common.RewardItem rewardItem = new Model.Common.RewardItem(
                item: shopItem,
                amount: userShopItem.reward.itemAmount
            );
            Model.Common.Item costItem = this.itemRepository.FindById(userShopItem.cost.itemId);
            Model.Common.Cost cost = new Model.Common.Cost(
                item: costItem,
                amount: userShopItem.cost.itemAmount
            );
            return new Model.OutGame.ShopGold(
                gold: rewardItem,
                index: index,
                cost: cost,
                maxBuyCount: userShopItem.maxBuyCount,
                remainBuyCount: userShopItem.remainBuyCount
            );
        }
        Model.OutGame.ShopGold[] ToShopGolds(ShopItem[] userShopItems) {
            List<Model.OutGame.ShopGold> shopGolds = new();
            for (int i = 0; i < userShopItems.Length; i++) {
                Model.OutGame.ShopGold shopGold = this.ToShopGold(i, userShopItems[i]);
                shopGolds.Add(shopGold);
            }
            return shopGolds.ToArray();
        }
        Model.OutGame.GoldShop ToGoldShop(GoldShop goldShop) {
            Model.OutGame.ShopGold[] shopGolds = this.ToShopGolds(goldShop.items);
            return new Model.OutGame.GoldShop(
                shopItems: shopGolds,
                maxResetCount: goldShop.maxResetCount,
                remainResetCount: goldShop.remainResetCount,
                lastResetTime: goldShop.lastResetTime
            );
        }
        Model.OutGame.ShopEnergy ToShopEnergy(int index, ShopItem userShopItem) {
            Model.Common.Item shopItem = this.itemRepository.FindById(userShopItem.reward.itemId);
            Model.Common.RewardItem rewardItem = new Model.Common.RewardItem(
                item: shopItem,
                amount: userShopItem.reward.itemAmount
            );
            Model.Common.Item costItem = this.itemRepository.FindById(userShopItem.cost.itemId);
            Model.Common.Cost cost = new Model.Common.Cost(
                item: costItem,
                amount: userShopItem.cost.itemAmount
            );
            return new Model.OutGame.ShopEnergy(
                energy: rewardItem,
                index: index,
                cost: cost,
                maxBuyCount: userShopItem.maxBuyCount,
                remainBuyCount: userShopItem.remainBuyCount
            );
        }
        Model.OutGame.ShopEnergy[] ToShopEnergies(ShopItem[] userShopItems) {
            List<Model.OutGame.ShopEnergy> shopEnergies = new();
            for (int i = 0; i < userShopItems.Length; i++) {
                Model.OutGame.ShopEnergy shopEnergy = this.ToShopEnergy(i, userShopItems[i]);
                shopEnergies.Add(shopEnergy);
            }
            return shopEnergies.ToArray();
        }
        Model.OutGame.EnergyShop ToEnergyShop(EnergyShop energyShop) {
            Model.OutGame.ShopEnergy[] shopEnergies = this.ToShopEnergies(energyShop.items);
            return new Model.OutGame.EnergyShop(
                shopItems: shopEnergies,
                maxResetCount: energyShop.maxResetCount,
                remainResetCount: energyShop.remainResetCount,
                lastResetTime: energyShop.lastResetTime
            );
        }
    }
}