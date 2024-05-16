using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill13010 : ISkill
    {
        [SerializeField]
        ParticleSystem crystal;
        [SerializeField]
        ParticleSystem area;

        Coroutine finishCoroutine;

        #region Unity Method
        void OnParticleCollision() {
            this.Attack();
        }
        #endregion

        void Attack() {
            Collider[] colliders = Physics.OverlapBox(this.transform.position, new Vector3(2.5f, 1, 2.5f), Quaternion.identity, 1 << LayerMask.NameToLayer("Enemy"));
            Enemy.Instance[] enemyInstances = colliders.Select((e) => e.GetComponent<Enemy.Instance>()).ToArray();
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
            if (this.finishCoroutine != null) {
                StopCoroutine(this.finishCoroutine);
            }
            this.finishCoroutine = StartCoroutine(this.Finish());
        }

        IEnumerator Finish() {
            yield return new WaitForSeconds(1f);
            this.onSuccess?.Invoke();
            this.gameObject.SetActive(false);
            // Destroy(this.gameObject);
        }

        public override void Use()
        {
            this.transform.position = new Vector3(
                x: this.target.transform.position.x,
                y: 0,
                z: this.target.transform.position.z
            );

            this.crystal.Stop();
            this.area.Stop();
            float duration = this.skillSpec.hitCount * this.skillSpec.delayPerHit;
            var crystalMain = this.crystal.main;
            crystalMain.duration = duration;
            var crystalEmission = this.crystal.emission;
            crystalEmission.rateOverTime = 1f / this.skillSpec.delayPerHit;
            var areaMain = this.area.main;
            areaMain.startLifetime = duration;
            this.crystal.Play();
            this.area.Play();
        }
    }
}
