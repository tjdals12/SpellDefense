using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.InGame {
    public abstract class IPlayModel : MonoBehaviour {
        public DateTime startTime { get; protected set; }
        public DateTime endTime { get; protected set; }
        public int currentWaveNumber { get; protected set; }
        public Wave currentWave { get; protected set; }
        public int depeatedEnemy { get; protected set; }
        public int rewardGold { get; protected set; }
        public int rewardSilverKey { get; protected set; }

        public Action OnInitialize;
        public Action OnDepeatEnemy;
        public Action OnClearWave;
        public Action OnNextWave;
        public Action OnGameOver;
        public Action OnWatchAds;
    }

    public class PlayModel : IPlayModel
    {
        public void Initialize(Wave wave) {
            this.startTime = DateTime.UtcNow;
            this.currentWaveNumber = 1;
            this.currentWave = wave;
            this.rewardGold = 0;
            this.rewardSilverKey = 0;
            this.OnInitialize?.Invoke();
        }

        public void DepeatEnemy(int dropSp) {
            this.depeatedEnemy += 1;
            this.OnDepeatEnemy?.Invoke();
        }

        public void ClearWave() {
            foreach (var clearReward in this.currentWave.clearRewards) {
                switch (clearReward.item.id) {
                    // Gold
                    case 1:
                        this.rewardGold += clearReward.amount;
                        break;
                    // Silver Key
                    case 10:
                        this.rewardSilverKey += clearReward.amount;
                        break;
                }
            }
            this.OnClearWave?.Invoke();
        }

        public void NextWave(Wave wave) {
            this.currentWaveNumber += 1;
            this.currentWave = wave;
            this.OnNextWave?.Invoke();
        }

        public void GameOver() {
            this.endTime = DateTime.UtcNow;
            this.OnGameOver?.Invoke();
        }

        public void WatchAds() {
            this.OnWatchAds?.Invoke();
        }
    }
}
