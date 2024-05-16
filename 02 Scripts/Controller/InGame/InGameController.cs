using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.InGame {
    using InGameService = Service.InGame.InGameService;

    public class InGameController : MonoBehaviour
    {
        InGameService inGameService;

        public event Action OnError;

        #region Unity Method
        void Awake() {
            this.inGameService = GameObject.FindObjectOfType<InGameService>();
        }
        #endregion

        public void SpawnSkillBook() {
            try {
                this.inGameService.SpawnSkillBook();
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public void MergeSkillBook(int slot1, int slot2) {
            try {
                this.inGameService.MergeSkillBook(slot1, slot2);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public void UpgradeSkillBook(int skillBookId) {
            try {
                this.inGameService.UpgradeSkillBook(skillBookId);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public void CastSkillBook(int slot) {
            try {
                this.inGameService.CastSkillBook(slot);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public void CompleteCastingSkillBook(int slot) {
            try {
                this.inGameService.CompleteCastingSkillBook(slot);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public void DepeatEnemy(int dropSp) {
            try {
                this.inGameService.DepeatEnemy(dropSp);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public void ClearWave() {
            try {
                this.inGameService.ClearWave();
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public void Damage(int amount) {
            try {
                this.inGameService.Damage(amount);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void GameOver() {
            try {
                await this.inGameService.GameOver();
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void WatchAds() {
            try {
                await this.inGameService.WatchAds();
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }
    }
}
