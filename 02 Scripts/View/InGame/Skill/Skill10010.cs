using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill10010 : ISkill
    {
        [SerializeField]
        float speed;
        [SerializeField]
        GameObject hitPrefab;

        #region Unity Method
        void Update() {
            this.transform.Translate(Vector3.forward * this.speed * Time.deltaTime);
        }
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
            GameObject hit = Instantiate(this.hitPrefab, this.transform.position, Quaternion.identity);
            Destroy(hit, 1f);
            int damage = this.GetDamage();
            Debuff debuff = new Debuff(this.skillSpec.debuffType, this.skillSpec.debuffDuration);
            if (this.isCritical) {
                enemyInstance.TakeCriticalDamage(damage, debuff);
            } else {
                enemyInstance.TakeDamage(damage, debuff);
            }
            this.onSuccess?.Invoke();
            this.gameObject.SetActive(false);
            // Destroy(this.gameObject);
        }

        public override void Use()
        {
            if (this.target == null || this.target.isDead) {
                this.onCancel?.Invoke();
                this.gameObject.SetActive(false);
                // Destroy(this.gameObject);
            } else {
                this.transform.position = new Vector3(
                    x: this.target.hitPoint.position.x,
                    y: this.target.hitPoint.position.y,
                    z: this.transform.position.z
                );
            }
        }
    }
}
