using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill12020 : ISkill
    {
        [SerializeField]
        ParticleSystem crystal;
        [SerializeField]
        ParticleSystem wind;
        [SerializeField]
        ParticleSystem area;

        Coroutine attackCoroutine;

        IEnumerator Attack() {
            WaitForSeconds delay = new WaitForSeconds(this.skillSpec.delayPerHit);
            int currentHitCount = 0;
            Debuff debuff = new Debuff(this.skillSpec.debuffType, this.skillSpec.debuffDuration);
            while (this.skillSpec.hitCount > currentHitCount) {
                currentHitCount++;
                Collider[] colliders = Physics.OverlapBox(this.transform.position, new Vector3(2.5f, 1, 2.5f), Quaternion.identity, 1 << LayerMask.NameToLayer("Enemy"));
                Enemy.Instance[] enemyInstances = colliders.Select((e) => e.GetComponent<Enemy.Instance>()).Take(this.skillSpec.targetCount).ToArray();
                foreach (var enemyInstance in enemyInstances) {
                    if (enemyInstance == null || enemyInstance.isDead) continue;
                    int damage = this.GetDamage();
                    if (this.isCritical) {
                        enemyInstance.TakeCriticalDamage(damage);
                    } else {
                        enemyInstance.TakeDamage(damage);
                    }
                }
                yield return delay;
            }
            this.onSuccess?.Invoke();
            yield return new WaitForSeconds(1f);
            this.gameObject.SetActive(false);
            // Destroy(this.gameObject, 1f);
        }

        public override void Use()
        {
            this.transform.position = new Vector3(
                x: this.target.transform.position.x,
                y: 0,
                z: this.target.transform.position.z
            );

            this.crystal.Stop();
            this.wind.Stop();
            this.area.Stop();
            var crystalMain = this.crystal.main;
            crystalMain.duration = this.skillSpec.duration;
            var windMain = this.wind.main;
            windMain.duration = this.skillSpec.duration;
            var areaMain = this.area.main;
            areaMain.startLifetime = this.skillSpec.duration;
            this.crystal.Play();
            this.wind.Play();
            this.area.Play();

            if (this.attackCoroutine != null) {
                StopCoroutine(this.attackCoroutine);
            }
            this.attackCoroutine = StartCoroutine(this.Attack());
        }

    }
}
