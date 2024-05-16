using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Layout {
    using UserModel = Model.OutGame.IUserModel;

    public class HUD : MonoBehaviour
    {
        UserModel userModel;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        TextMeshProUGUI nicknameText;
        [SerializeField]
        TextMeshProUGUI goldText;
        [SerializeField]
        TextMeshProUGUI energyText;
        [SerializeField]
        TextMeshProUGUI energyChargeTimeText;
        [SerializeField]
        Button gameStartButton;

        Coroutine energyChargeTimer;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.nicknameText.text = this.userModel.user.nickname;
            this.OnChangeEnergy();
            this.OnChangeGold();
        }
        void OnEnable() {
            this.userModel.OnChangeGold += this.OnChangeGold;
            this.userModel.OnChangeEnergy += this.OnChangeEnergy;
        }
        void OnDisable() {
            this.userModel.OnChangeGold -= this.OnChangeGold;
            this.userModel.OnChangeEnergy -= this.OnChangeEnergy;
        }
        #endregion

        #region Event Listeners
        void OnChangeGold() {
            this.goldText.text = this.userModel.user.gold.ToString();
        }
        void OnChangeEnergy() {
            this.energyText.text = $"{this.userModel.user.energy.amount} / 30";
            if (30 > this.userModel.user.energy.amount) {
                if (this.energyChargeTimer != null) {
                    StopCoroutine(this.energyChargeTimer);
                    this.energyChargeTimer = null;
                }
                this.energyChargeTimer = StartCoroutine(this.StartChargeTimer());
                this.energyChargeTimeText.gameObject.SetActive(true);
            } else {
                if (this.energyChargeTimer != null) {
                    StopCoroutine(this.energyChargeTimer);
                }
                this.energyChargeTimeText.gameObject.SetActive(false);
            }
        }
        #endregion

        IEnumerator StartChargeTimer() {
            var energy = this.userModel.user.energy;
            int energyAmount = energy.amount;
            if (5 > energyAmount) {
                this.gameStartButton.enabled = false;
            }
            DateTime lastChargeTime = energy.lastChargeTime;
            DateTime now = DateTime.UtcNow;
            int chargedEnergyAmount = ((int)(now - lastChargeTime).TotalSeconds / 300);
            energyAmount = Mathf.Min(energyAmount + chargedEnergyAmount, 30);
            this.energyText.text = $"{energyAmount.ToString()} / 30";
            int elapsedSeconds = ((int)(now - lastChargeTime).TotalSeconds % 300);
            int remainingSeconds = 300 - elapsedSeconds;
            string minutes = (remainingSeconds / 60).ToString();
            string seconds = (remainingSeconds % 60).ToString().PadLeft(2, '0');
            this.energyChargeTimeText.text = $"{minutes}:{seconds}";
            WaitForSeconds delay = new WaitForSeconds(1f);
            while(30 > energyAmount) {
                if (energyAmount >= 5) {
                    this.gameStartButton.enabled = true;
                }
                yield return delay;
                remainingSeconds--;
                if (0 >= remainingSeconds) {
                    energyAmount++;
                    this.energyText.text = $"{energyAmount.ToString()} / 30";
                    remainingSeconds = 300;
                    lastChargeTime = DateTime.UtcNow;
                }
                minutes = (remainingSeconds / 60).ToString();
                seconds = (remainingSeconds % 60).ToString().PadLeft(2, '0');
                this.energyChargeTimeText.text = $"{minutes}:{seconds}";
            }
            this.energyChargeTimeText.gameObject.SetActive(false);
        }
    }
}
