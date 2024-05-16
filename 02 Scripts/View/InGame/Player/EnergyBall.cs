using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace View.InGame.Player {
    public class EnergyBall : MonoBehaviour
    {
        [SerializeField]
        float speed;

        int attackPower;
        int criticalRate;
        int criticalDamage;
        Action callback;
        Enemy.Instance target;

        Vector3? direction = null;
        bool isAttack = false;

        #region Unity Method
        void Update() {
            if (this.target != null && this.direction == null) {
                this.direction = ((this.target.transform.position + new Vector3(0, 0.5f, 0)) - this.transform.position).normalized;
                this.transform.rotation = Quaternion.LookRotation(direction.Value);
            }
            this.transform.Translate(Vector3.forward * this.speed * Time.deltaTime);
        }
        void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && this.isAttack == false) {
                Enemy.Instance enemyInstance = collider.GetComponent<Enemy.Instance>();
                if (enemyInstance != null && enemyInstance.isDead == false) {
                    this.isAttack = true;
                    this.direction = null;
                    this.Attack(collider.gameObject);
                }
            } else if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")) {
                this.direction = null;
                this.callback?.Invoke();
            }
        }
        #endregion

        public EnergyBall SetAttackPower(int attackPower) {
            this.attackPower = attackPower;
            return this;
        }
        public EnergyBall SetCriticalRate(int criticalRate) {
            this.criticalRate = criticalRate;
            return this;
        }
        public EnergyBall SetCriticalDamage(int criticalDamage) {
            this.criticalDamage = criticalDamage;
            return this;
        }
        public EnergyBall SetCallback(Action callback) {
            this.callback = callback;
            return this;
        }
        public void Move(Enemy.Instance target) {
            this.target = target;
        }
        void Attack(GameObject target) {
            if (target != null) {
                Enemy.Instance enemyInstance = target.GetComponent<Enemy.Instance>();
                bool isCritical = this.criticalRate > Random.Range(0, 100);
                if (isCritical) {
                    enemyInstance.TakeCriticalDamage((this.attackPower * this.criticalDamage) / 100);
                } else {
                    enemyInstance.TakeDamage(this.attackPower);
                }
            }
            this.isAttack = false;
            this.callback?.Invoke();
        }
    }
}
