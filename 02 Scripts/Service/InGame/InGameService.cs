using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Service.InGame {
    using IUserRepository = Repository.User.IRepository;
    using IWaveRepository = Repository.Wave.IRepository;
    using ItemType = Repository.Item.ItemType;
    using Repository.User.Request;
    using Repository.User.Response;

    public class InGameService : MonoBehaviour
    {
        #region Repository
        IUserRepository userRepository;
        IWaveRepository waveRepository;
        #endregion

        #region Model
        Model.OutGame.UserModel userModel;
        Model.InGame.PlayerModel playerModel;
        Model.InGame.BoardModel boardModel;
        Model.InGame.PlayModel playModel;
        #endregion

        #region Unity Method
        void Awake() {
            this.userRepository = GameObject.FindObjectOfType<IUserRepository>();
            this.waveRepository = GameObject.FindObjectOfType<IWaveRepository>();
            this.userModel = GameObject.FindObjectOfType<Model.OutGame.UserModel>();
            this.playerModel = GameObject.FindObjectOfType<Model.InGame.PlayerModel>();
            this.boardModel = GameObject.FindObjectOfType<Model.InGame.BoardModel>();
            this.playModel = GameObject.FindObjectOfType<Model.InGame.PlayModel>();
        }
        async void Start() {
            Model.InGame.User user = await this.userRepository.GetInGameUser();
            this.playerModel.Initialize(user);
            this.boardModel.Initialize(user);

            Model.InGame.Wave wave = this.waveRepository.FindByWave(1);
            this.playModel.Initialize(wave);
        }
        #endregion

        public void SpawnSkillBook() {
            if (this.boardModel.HasEnoughSp() == false) return;
            if (this.boardModel.HasRemainingSlot() == false) return;
            this.boardModel.SpawnSkillBook();
        }

        public void MergeSkillBook(int slot1, int slot2) {
            if (this.boardModel.CanMerge(slot1, slot2) == false) return;
            this.boardModel.MergeSkillBook(slot1, slot2);
        }

        public void UpgradeSkillBook(int skillBookId) {
            if (this.boardModel.CanUpgrade(skillBookId) == false) return;
            this.boardModel.UpgradeSkillBook(skillBookId);
        }

        public void CastSkillBook(int slot) {
            this.boardModel.CastSkillBook(slot);
        }

        public void CompleteCastingSkillBook(int slot) {
            this.boardModel.CompleteCastingSkillBook(slot);
        }

        public void DepeatEnemy(int dropSp) {
            this.playModel.DepeatEnemy(dropSp);
            this.boardModel.EarnSp(dropSp);
        }

        public void ClearWave() {
            this.playModel.ClearWave();
            Model.InGame.Wave wave = this.waveRepository.FindByWave(this.playModel.currentWaveNumber + 1);
            if (wave == null) {
                this.playModel.GameOver();
            } else {
                this.playModel.NextWave(wave);
            }
        }

        public void Damage(int amount) {
            this.playerModel.Damage(amount);
        }

        public async Task GameOver() {
            GameOverRequest request = new GameOverRequest(this.playModel.currentWaveNumber - 1);
            GameOverResponse response = await this.userRepository.GameOver(request);
            foreach (var receivedReward in response.receivedRewards) {
                this.ReceiveReward(receivedReward);
            }
            this.playModel.GameOver();
        }

        public async Task WatchAds() {
            WatchAdsRequest request = new WatchAdsRequest(this.playModel.currentWaveNumber - 1);
            WatchAdsResponse response = await this.userRepository.WatchAds(request);
            foreach (var receiveReward in response.receivedRewards) {
                this.ReceiveReward(receiveReward);
            }
            this.playModel.WatchAds();
        }
        
        void ReceiveReward(Model.OutGame.ReceivedReward receivedReward) {
            if (receivedReward.rewardItem != null) {
                Model.Common.RewardItem rewardItem = receivedReward.rewardItem;
                switch (rewardItem.item.type) {
                    case ItemType.Currency:
                        this.userModel.EarnCurrency(rewardItem);
                        break;
                    case ItemType.Key:
                        this.userModel.EarnKey(rewardItem);
                        break;
                }
            } else if (receivedReward.rewardSkillBook != null) {
                Model.OutGame.RewardSkillBook rewardSkillBook = receivedReward.rewardSkillBook;
                this.userModel.EarnSkillBook(rewardSkillBook.inventorySkillBook);
            } else if (receivedReward.rewardPart != null) {
                Model.OutGame.RewardPart rewardPart = receivedReward.rewardPart;
                this.userModel.EarnPart(rewardPart.inventoryPart);
            }
        }
    }
}