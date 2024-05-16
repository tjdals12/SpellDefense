using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill11040 : ISkill
    {
        [SerializeField]
        float speed;
        
        int currentHitCount = 0;

        #region Unity Method
        void Update() {
            this.transform.Translate(Vector3.forward * this.speed * Time.deltaTime);
        }
        #endregion

        #region Unity Method
        void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                Enemy.Instance enemyInstance = collider.GetComponent<Enemy.Instance>();
                this.Attack(enemyInstance);
            } else if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")) {
                this.onSuccess?.Invoke();
                this.gameObject.SetActive(false);
                // Destroy(this.gameObject);
            }
        }
        #endregion

        void Attack(Enemy.Instance enemyInstance) {
            if (enemyInstance == null || enemyInstance.isDead) return;
            this.currentHitCount++;
            Debuff debuff = new Debuff(this.skillSpec.debuffType, this.skillSpec.debuffDuration);
            int damage = this.GetDamage();
            if (this.isCritical) {
                enemyInstance.TakeCriticalDamage(damage, debuff);
            } else {
                enemyInstance.TakeDamage(damage, debuff);
            }
            if (this.currentHitCount >= this.skillSpec.hitCount) {
                this.onSuccess?.Invoke();
                this.gameObject.SetActive(false);
                // Destroy(this.gameObject);
            }
        }

        public override void Use()
        {
            this.currentHitCount = 0;
            if (this.target == null || this.target.isDead) {
                this.onCancel?.Invoke();
                this.gameObject.SetActive(false);
                // Destroy(this.gameObject);
            } else {
                this.transform.position = new Vector3(
                    x: this.target.transform.position.x,
                    y: 0,
                    z: this.transform.position.z
                );
            }
        }
    }
}
