using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Part {
    using UserModel = Model.OutGame.IUserModel;

    public class CharacterStats : MonoBehaviour
    {
        UserModel userModel;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        TextMeshProUGUI attackPowerText;
        [SerializeField]
        TextMeshProUGUI attackSpeedText;
        [SerializeField]
        TextMeshProUGUI criticalRateText;
        [SerializeField]
        TextMeshProUGUI criticalDamageText;

        int attackPower = 0;
        float attackSpeed = 0;
        int criticalRate = 0;
        int criticalDamage = 0;

        Coroutine lerpStatsCoroutine;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.ShowStats();
        }
        void OnEnable() {
            this.userModel.OnUpgradePart += this.OnUpgradePart;
            this.userModel.OnEquipPart += this.OnEquipPart;
        }
        void OnDisable() {
            this.userModel.OnUpgradePart -= this.OnUpgradePart;
            this.userModel.OnEquipPart -= this.OnEquipPart;
        }
        #endregion

        #region Event Listeners
        void OnUpgradePart(string id, string[] materialIds) {
            this.UpdateStats();
        }
        void OnEquipPart(string id) {
            this.UpdateStats();
        }
        #endregion

        void ShowStats() {
            var summaryStats = this.userModel.summaryStats;

            this.attackPower = summaryStats.attackPower;
            this.attackPowerText.text = this.attackPower.ToString();

            this.attackSpeed = summaryStats.attackSpeed;
            this.attackSpeedText.text = this.attackSpeed.ToString("0.00");

            this.criticalRate = summaryStats.criticalRate;
            this.criticalRateText.text = $"{this.criticalRate}%";

            this.criticalDamage = summaryStats.criticalDamage;
            this.criticalDamageText.text = $"{this.criticalDamage}%";
        }

        void UpdateStats() {
            var summaryStats = this.userModel.summaryStats;

            int prevAttackPower = this.attackPower;
            int deltaAttackPower = summaryStats.attackPower - prevAttackPower;
            this.attackPower = summaryStats.attackPower;

            float prevAttackSpeed = this.attackSpeed;
            float deltaAttackSpeed = summaryStats.attackSpeed - prevAttackSpeed;
            this.attackSpeed = summaryStats.attackSpeed;

            int prevCriticalRate = this.criticalRate;
            int deltaCriticalRate = summaryStats.criticalRate - prevCriticalRate;
            this.criticalRate = summaryStats.criticalRate;

            int prevCriticalDamage = this.criticalDamage;
            int deltaCriticalDamage = summaryStats.criticalDamage - prevCriticalDamage;
            this.criticalDamage = summaryStats.criticalDamage;

            if (this.lerpStatsCoroutine != null) {
                StopCoroutine(this.lerpStatsCoroutine);
            }
            this.lerpStatsCoroutine = StartCoroutine(LerpStats());
            IEnumerator LerpStats() {
                yield return null;
                float seconds = 0.5f;
                float elapsedSeconds = 0f;
                this.attackPowerText.color = deltaAttackPower == 0
                    ? Color.white
                    : deltaAttackPower > 0 ? Color.green : Color.red;
                this.attackSpeedText.color = deltaAttackSpeed == 0
                    ? Color.white
                    : deltaAttackSpeed > 0 ? Color.green : Color.red;
                this.criticalRateText.color = deltaCriticalRate == 0
                    ? Color.white
                    : deltaCriticalRate > 0 ? Color.green : Color.red;
                this.criticalDamageText.color = deltaCriticalDamage == 0
                    ? Color.white
                    : deltaCriticalDamage > 0 ? Color.green : Color.red;
                while (seconds > elapsedSeconds) {
                    float currentAttackPower =  Mathf.RoundToInt(Mathf.Lerp(prevAttackPower, this.attackPower, elapsedSeconds / seconds));
                    float currentAttackSpeed =  Mathf.Lerp(prevAttackSpeed, this.attackSpeed, elapsedSeconds / seconds);
                    float currentCriticalRate =  Mathf.RoundToInt(Mathf.Lerp(prevCriticalRate, this.criticalRate, elapsedSeconds / seconds));
                    float currentCriticalDamage =  Mathf.RoundToInt(Mathf.Lerp(prevCriticalDamage, this.criticalDamage, elapsedSeconds / seconds));

                    this.attackPowerText.text = currentAttackPower.ToString();
                    this.attackSpeedText.text = currentAttackSpeed.ToString("0.00");
                    this.criticalRateText.text = $"{currentCriticalRate}%";
                    this.criticalDamageText.text = $"{currentCriticalDamage}%";

                    elapsedSeconds += Time.deltaTime;

                    yield return null;
                }
                yield return new WaitForSeconds(0.5f);
                this.attackPowerText.color = Color.white;
                this.attackSpeedText.color = Color.white;
                this.criticalRateText.color = Color.white;
                this.criticalDamageText.color = Color.white;
            }
        }
    }
}
