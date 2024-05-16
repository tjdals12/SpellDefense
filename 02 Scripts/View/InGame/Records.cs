using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace View.InGame.Player {
    using Common;
    using PlayModel = Model.InGame.PlayModel;

    public class Records : MonoBehaviour
    {
        PlayModel playModel;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        LocalizationText currentWaveText;
        [SerializeField]
        TextMeshProUGUI rewardGoldText;
        [SerializeField]
        TextMeshProUGUI rewardSilverKeyText;
        [SerializeField]
        TextMeshProUGUI depeatedEnemyText;

        #region Unity Method
        void Awake() {
            this.playModel = GameObject.FindObjectOfType<PlayModel>();
        }
        void OnEnable() {
            this.playModel.OnInitialize += this.OnInitialize;
            this.playModel.OnDepeatEnemy += this.OnDepeatEnemy;
            this.playModel.OnNextWave += this.OnNextWave;
        }
        void OnDisable() {
            this.playModel.OnInitialize -= this.OnInitialize;
            this.playModel.OnDepeatEnemy -= this.OnDepeatEnemy;
            this.playModel.OnNextWave -= this.OnNextWave;
        }
        #endregion

        #region Event Listeners
        void OnInitialize() {
            this.currentWaveText.UpdateView(this.playModel.currentWaveNumber);
            this.rewardGoldText.text = this.playModel.rewardGold.ToString();
            this.rewardSilverKeyText.text = this.playModel.rewardSilverKey.ToString();
            this.depeatedEnemyText.text = this.playModel.depeatedEnemy.ToString();
        }
        void OnDepeatEnemy() {
            this.depeatedEnemyText.text = this.playModel.depeatedEnemy.ToString();
        }
        void OnNextWave() {
            this.currentWaveText.UpdateView(this.playModel.currentWaveNumber);
            this.rewardGoldText.text = this.playModel.rewardGold.ToString();
            this.rewardSilverKeyText.text = this.playModel.rewardSilverKey.ToString();
        }
        #endregion
    }
}