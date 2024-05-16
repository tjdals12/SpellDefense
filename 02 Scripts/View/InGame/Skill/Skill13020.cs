using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill13020 : ISkill
    {
        [SerializeField]
        ParticleSystem orb;
        [SerializeField]
        ParticleSystem fog;
        [SerializeField]
        ParticleSystem bubble; 

        List<ParticleCollisionEvent> collisionEvents = new();

        Coroutine finishCoroutine;

        #region Unity Method
        void OnParticleCollision(GameObject other) {
            int numCollisionEvents = this.orb.GetCollisionEvents(other, this.collisionEvents);
            if (numCollisionEvents > 0) {
                Vector3 position = collisionEvents[0].intersection;
                StartCoroutine(this.Attack(position));
            }
        }
        #endregion

        IEnumerator Attack(Vector3 position) {
            WaitForSeconds delay = new WaitForSeconds(this.skillSpec.delayPerHit);
            int currentHitCount = 0;
            Debuff debuff = new Debuff(this.skillSpec.debuffType, this.skillSpec.debuffDuration);
            while (this.skillSpec.hitCount > currentHitCount) {
                currentHitCount++;
                Collider[] colliders = Physics.OverlapBox(position, new Vector3(1.5f, 1, 1.5f), Quaternion.identity, 1 << LayerMask.NameToLayer("Enemy"));
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

            this.orb.Stop();
            var fogMain = this.fog.main;
            fogMain.duration = this.skillSpec.duration;
            var bubbleMain = this.bubble.main;
            bubbleMain.duration = this.skillSpec.duration;
            this.orb.Play();
        }
    }
}
