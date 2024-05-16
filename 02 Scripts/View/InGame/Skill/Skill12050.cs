using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill12050 : ISkill
    {
        [SerializeField]
        ParticleSystem spark;
        [SerializeField]
        ParticleSystem area;

        Coroutine attackCoroutine;

        IEnumerator Attack() {
            WaitForSeconds delay = new WaitForSeconds(this.skillSpec.delayPerHit);
            Debuff debuff = new Debuff(this.skillSpec.debuffType, this.skillSpec.debuffDuration);
            int currentHitCount = 0;
            while (this.skillSpec.hitCount > currentHitCount) {
                currentHitCount++;
                Collider[] colliders = Physics.OverlapBox(this.transform.position, new Vector3(2.5f, 1, 2.5f), Quaternion.identity, 1 << LayerMask.NameToLayer("Enemy"));
                Enemy.Instance[] enemyInstances = colliders.Select((e) => e.GetComponent<Enemy.Instance>()).Take(this.skillSpec.targetCount).ToArray();
                foreach (var enemyInstance in enemyInstances) {
                    if (enemyInstance == null || enemyInstance.isDead) continue;
                    int damage = this.GetDamage();
                    if (this.isCritical) {
                        enemyInstance.TakeCriticalDamage(damage, debuff);
                    } else {
                        enemyInstance.TakeDamage(damage, debuff);
                    }
                }
                yield return delay;
            }
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

            this.spark.Stop();
            this.area.Stop();
            var sparkMain = this.spark.main;
            sparkMain.duration = this.skillSpec.duration;
            var areamain = this.area.main;
            areamain.startLifetime = this.skillSpec.duration;
            this.spark.Play();
            this.area.Play();

            if (this.attackCoroutine != null) {
                StopCoroutine(this.attackCoroutine);
            }
            this.attackCoroutine = StartCoroutine(this.Attack());
        }

    }
}
