using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill13030 : ISkill
    {
        [SerializeField]
        ParticleSystem orb;
        [SerializeField]
        ParticleSystem arrow;
        [SerializeField]
        ParticleSystem bubble;

        [SerializeField]
        GameObject hitPrefab;

        bool isAttack;

        #region Unity Method
        void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                this.Attack();
            }
        }
        #endregion

        void Attack() {
            if (this.isAttack) return;
            this.isAttack = true;
            GameObject hit = Instantiate(hitPrefab, this.transform.position, Quaternion.identity);
            Destroy(hit, 0.5f);
            Collider[] colliders = Physics.OverlapBox(this.transform.position, new Vector3(3f, 1f, 3f), Quaternion.identity, 1 << LayerMask.NameToLayer("Enemy"));
            Enemy.Instance[] enemyInstances = colliders.Select((e) => e.GetComponent<Enemy.Instance>()).Take(this.skillSpec.targetCount).ToArray();
            Debuff debuff = new Debuff(this.skillSpec.debuffType, this.skillSpec.debuffDuration);
            foreach (var enemyInstance in enemyInstances) {
                if (enemyInstance == null || enemyInstance.isDead) continue;
                int damage = this.GetDamage();
                if (this.isCritical) {
                    enemyInstance.TakeCriticalDamage(damage, debuff);
                } else {
                    enemyInstance.TakeDamage(damage, debuff);
                }
            }
            this.gameObject.SetActive(false);
            // Destroy(this.gameObject);
        }

        public override void Use()
        {
            this.isAttack = false;
            this.transform.position = new Vector3(
                x: this.position.x,
                y: 0,
                z: this.position.z
            );

            this.orb.Stop();
            this.arrow.Stop();
            this.bubble.Stop();
            var orbMain = this.orb.main;
            orbMain.startLifetime = this.skillSpec.duration;
            var arrowMain = this.arrow.main;
            arrowMain.duration = this.skillSpec.duration;
            var bubbleMain = this.bubble.main;
            bubbleMain.duration = this.skillSpec.duration;
            this.orb.Play();
            this.arrow.Play();
            this.bubble.Play();

            this.onSuccess?.Invoke();
            StartCoroutine(this.Finish());
        }

        IEnumerator Finish() {
            yield return new WaitForSeconds(this.skillSpec.duration);
            this.gameObject.SetActive(false);
        }
    }
}
