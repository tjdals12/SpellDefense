using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Player {
    using PlayerModel = Model.InGame.PlayerModel;

    public class EnergyBallPool : MonoBehaviour
    {
        PlayerModel playerModel;

        [SerializeField]
        int poolCount;
        [SerializeField]
        GameObject prefab;

        Queue<EnergyBall> pool = new();

        #region Unity Method
        void Awake() {
            this.playerModel = GameObject.FindObjectOfType<PlayerModel>();
        }
        void Start() {
            this.Initialize();
        }
        #endregion

        void Initialize() {
            for (int i = 0; i < poolCount; i++) {
                EnergyBall energyBall = this.CreateObject();
                energyBall.gameObject.SetActive(false);
                this.pool.Enqueue(energyBall);
            }
        }

        EnergyBall CreateObject() {
            GameObject clone = Instantiate(this.prefab, this.transform);
            EnergyBall energyBall = clone.GetComponent<EnergyBall>();
            energyBall
                .SetAttackPower(this.playerModel.attackPower)
                .SetCriticalRate(this.playerModel.criticalRate)
                .SetCriticalDamage(this.playerModel.criticalDamage)
                .SetCallback(() => this.ReturnObject(energyBall));
            return energyBall;
        }

        public EnergyBall GetObject() {
            EnergyBall energyBall = this.pool.Count > 0
                ? this.pool.Dequeue()
                : this.CreateObject();
            energyBall.gameObject.SetActive(true);
            return energyBall;
        }

        public void ReturnObject(EnergyBall energyBall) {
            energyBall.transform.position = this.transform.position;
            energyBall.gameObject.SetActive(false);
            this.pool.Enqueue(energyBall);
        }
    }
}
