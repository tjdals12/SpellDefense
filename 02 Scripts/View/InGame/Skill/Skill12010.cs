using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill12010 : ISkill
    {
        [SerializeField]
        ParticleSystem crystal;
        [SerializeField]
        ParticleSystem smoke;
        [SerializeField]
        ParticleSystem area;

        Coroutine attackCoroutine;

        IEnumerator Attack() {
            Collider[] colliders = Physics.OverlapBox(this.transform.position, new Vector3(1.2f, 1, 1.2f), Quaternion.identity, 1 << LayerMask.NameToLayer("Enemy"));
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
            this.onSuccess?.Invoke();
            yield return new WaitForSeconds(this.skillSpec.duration);
            this.gameObject.SetActive(false);
            // Destroy(this.gameObject, this.skillSpec.duration);
        }

        public override void Use()
        {
            if (this.target == null || this.target.isDead) {
                this.onCancel?.Invoke();
                this.gameObject.SetActive(false);
                // Destroy(this.gameObject);
            } else {
                this.transform.position = new Vector3(
                    x: this.target.transform.position.x,
                    y: 0,
                    z: this.target.transform.position.z
                );


                this.crystal.Stop();
                this.smoke.Stop();
                this.area.Stop();
                var crystalMain = this.crystal.main;
                crystalMain.startLifetime = this.skillSpec.duration;
                var smokeMain = this.smoke.main;
                smokeMain.duration = this.skillSpec.duration;
                var areaMain = this.area.main;
                areaMain.startLifetime = this.skillSpec.duration;
                this.crystal.Play();
                this.smoke.Play();
                this.area.Play();

                if (this.attackCoroutine != null) {
                    StopCoroutine(this.attackCoroutine);
                }
                this.attackCoroutine = StartCoroutine(this.Attack());
            }
        }
    }
}
