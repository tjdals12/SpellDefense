using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Service.OutGame {
    using IPartRepository = Repository.Part.IRepository;
    using ISkillBookRepository = Repository.SkillBook.IRepository;
    using IUserRepository = Repository.User.IRepository;
    using ItemType = Repository.Item.ItemType;
    using Repository.User.Request;
    using Repository.User.Response;
    using Model.Common;
    using Model.OutGame;

    public class UserService : MonoBehaviour
    {
        #region Repository
        IPartRepository partRepository;
        ISkillBookRepository skillBookRepository;
        IUserRepository userRepository;
        #endregion

        #region Model
        UserModel userModel;
        #endregion

        #region Unity Method
        void Awake() {
            this.partRepository = GameObject.FindObjectOfType<IPartRepository>();
            this.skillBookRepository = GameObject.FindObjectOfType<ISkillBookRepository>();
            this.userRepository = GameObject.FindObjectOfType<IUserRepository>();
            this.userModel = GameObject.FindObjectOfType<UserModel>();
        }
        #endregion

        public async Task ReceiveMail(string mailId) {
            ReceiveMailRequest request = new ReceiveMailRequest(mailId);
            ReceiveMailResponse response = await this.userRepository.ReceiveMail(request);
            this.ReceiveReward(response.receivedReward);
            this.userModel.ReceiveMail(response.mail);
        }
        public async Task ReceiveAllMail() {
            ReceiveAllMailRequest request = new ReceiveAllMailRequest();
            ReceiveAllMailResponse response = await this.userRepository.ReceiveAllMail(request);
            Debug.Log(response);
            foreach (var receivedReward in response.receivedRewards) {
                this.ReceiveReward(receivedReward);
            }
            this.userModel.ReceiveMails(response.mails);
        }
        public async Task RefreshMails() {
            RefreshMailsRequest request = new RefreshMailsRequest();
            List<Mail> mails = await this.userRepository.RefreshMails(request);
            this.userModel.RefreshMails(mails);
        }
        public async Task UpgradeSkillBook(int skillBookId) {
            UpgradeSkillBookRequest request = new UpgradeSkillBookRequest(skillBookId);
            await this.userRepository.UpgradeSkillBook(request);
            this.userModel.UpgradeSkillBook(skillBookId);
        }
        public async Task EquipSkillBook(int index, int skillBookId) {
            EquipSkillBookRequest request = new EquipSkillBookRequest(index, skillBookId);
            await this.userRepository.EquipSkillBook(request);
            this.userModel.EquipSkillBook(index, skillBookId);
        }
        public async Task ChangeDeck(int index) {
            ChangeDeckRequest request = new ChangeDeckRequest(index);
            await this.userRepository.ChangeDeck(request);
            this.userModel.ChangeDeck(index);
        }
        public async Task UpgradePart(string id, string[] materialIds) {
            UpgradePartRequest request = new UpgradePartRequest(id, materialIds);
            await this.userRepository.UpgradePart(request);
            this.userModel.UpgradePart(id, materialIds);
        }
        public async Task EquipPart(string id) {
            EquipPartRequest request = new EquipPartRequest(id);
            await this.userRepository.EquipPart(request);
            this.userModel.EquipPart(id);
        }
        public async Task BuySkillBookInShop(int index) {
            BuySkillBookInShopRequest request = new BuySkillBookInShopRequest(index);
            BuySkillBookInShopResponse response = await this.userRepository.BuySkillBookInShop(request);
            this.ReceiveReward(response.receivedReward);
            this.userModel.BuySkillBookInShop(index);
        }
        public async Task ResetSkillBookShop() {
            ResetSkillBookShopRequest request = new ResetSkillBookShopRequest();
            SkillBookShop skillBookShop = await this.userRepository.ResetSkillBookShop(request);
            this.userModel.ResetSkillBookShop(skillBookShop);
        }
        public async Task ResetSkillBookShopByAds() {
            ResetSkillBookShopByAdsRequest request = new ResetSkillBookShopByAdsRequest();
            ShopSkillBook[] shopItems = await this.userRepository.ResetSkillBookShopByAds(request);
            this.userModel.ResetSkillBookShopByAds(shopItems);
        }
        public async Task BuyPartInShop(int index) {
            BuyPartInShopRequest request = new BuyPartInShopRequest(index);
            BuyPartInShopResponse response = await this.userRepository.BuyPartInShop(request);
            this.ReceiveReward(response.receivedReward);
            this.userModel.BuyPartInShop(index);
        }
        public async Task ResetPartShop() {
            ResetPartShopRequest request = new ResetPartShopRequest();
            PartShop partShop = await this.userRepository.ResetPartShop(request);
            this.userModel.ResetPartShop(partShop);
        }
        public async Task ResetPartShopByAds() {
            ResetPartShopByAdsRequest request = new ResetPartShopByAdsRequest();
            ShopPart[] shopItems = await this.userRepository.ResetPartShopByAds(request);
            this.userModel.ResetPartShopByAds(shopItems);
        }
        public async Task BuyGoldInShop(int index) {
            BuyGoldInShopRequest request = new BuyGoldInShopRequest(index);
            BuyGoldInShopResponse response = await this.userRepository.BuyGoldInShop(request);
            this.ReceiveReward(response.receivedReward);
            this.userModel.BuyGoldInShop(index);
        }
        public async Task ResetGoldShop() {
            ResetGoldShopRequest request = new ResetGoldShopRequest();
            GoldShop goldShop = await this.userRepository.ResetGoldShop(request);
            this.userModel.ResetGoldShop(goldShop);
        }
        public async Task ResetGoldShopByAds() {
            ResetGoldShopByAdsRequest request = new ResetGoldShopByAdsRequest();
            ShopGold[] shopItems = await this.userRepository.ResetGoldShopByAds(request);
            this.userModel.ResetGoldShopByAds(shopItems);
        }
        public async Task BuyEnergyInShop(int index) {
            BuyEnergyInShopRequest request = new BuyEnergyInShopRequest(index);
            BuyEnergyInShopResponse response = await this.userRepository.BuyEnergyInShop(request);
            this.ReceiveReward(response.receivedReward);
            this.userModel.BuyEnergyInShop(index);
        }
        public async Task ResetEnergyShop() {
            ResetEnergyShopRequest request = new ResetEnergyShopRequest();
            EnergyShop energyShop = await this.userRepository.ResetEnergyShop(request);
            this.userModel.ResetEnergyShop(energyShop);
        }
        public async Task ResetEnergyShopByAds() {
            ResetEnergyShopByAdsRequest request = new ResetEnergyShopByAdsRequest();
            ShopEnergy[] shopItems = await this.userRepository.ResetEnergyShopByAds(request);
            this.userModel.ResetEnergyShopByAds(shopItems);
        }

        public async Task OpenSilverChest(int count) {
            OpenSilverChestRequest request = new OpenSilverChestRequest(count);
            OpenSilverChestResponse response = await this.userRepository.OpenSilverChest(request);
            List<RewardItem> rewardItems = new();
            foreach (var receivedReward in response.receivedRewards) {
                this.ReceiveReward(receivedReward);
                if (receivedReward.rewardItem != null) {
                    rewardItems.Add(receivedReward.rewardItem);
                } else if (receivedReward.rewardSkillBook != null) {
                    rewardItems.Add(receivedReward.rewardSkillBook);
                } else if (receivedReward.rewardPart != null) {
                    rewardItems.Add(receivedReward.rewardPart);
                }
            }
            this.userModel.OpenChest(response.chest, response.count, rewardItems);
        }

        public async Task GameStart() {
            GameStartRequest request = new GameStartRequest();
            GameStartResponse response = await this.userRepository.GameStart(request);
            this.userModel.GameStart(response.energy);
        }
        
        void ReceiveReward(ReceivedReward receivedReward) {
            if (receivedReward.rewardItem != null) {
                RewardItem rewardItem = receivedReward.rewardItem;
                switch (rewardItem.item.type) {
                    case ItemType.Currency:
                        this.userModel.EarnCurrency(rewardItem);
                        break;
                    case ItemType.Key:
                        this.userModel.EarnKey(rewardItem);
                        break;
                }
            } else if (receivedReward.rewardSkillBook != null) {
                RewardSkillBook rewardSkillBook = receivedReward.rewardSkillBook;
                this.userModel.EarnSkillBook(rewardSkillBook.inventorySkillBook);
            } else if (receivedReward.rewardPart != null) {
                RewardPart rewardPart = receivedReward.rewardPart;
                this.userModel.EarnPart(rewardPart.inventoryPart);
            }
        }
    }
}