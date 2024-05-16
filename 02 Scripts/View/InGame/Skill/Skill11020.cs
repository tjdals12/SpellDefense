using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill11020 : ISkill
    {
        Coroutine attackCoroutine;

        #region Unity Method
        void Update() {
            if (this.target == null) return;
            this.transform.position = this.target.hitPoint.position;
        }
        #endregion

        IEnumerator Attack() {
            WaitForSeconds delay = new WaitForSeconds(this.skillSpec.delayPerHit);
            Debuff debuff = new Debuff(this.skillSpec.debuffType, this.skillSpec.debuffDuration);
            int currentHitCount = 0;
            while (this.skillSpec.hitCount > currentHitCount) {
                if (this.target == null || this.target.isDead) break;
                currentHitCount++;
                int damage = this.GetDamage();
                if (this.isCritical) {
                    this.target.TakeCriticalDamage(damage, debuff);
                } else {
                    this.target.TakeDamage(damage, debuff);
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
            if (this.target == null || this.target.isDead) {
                this.onCancel?.Invoke();
                this.gameObject.SetActive(false);
                // Destroy(this.gameObject);
            } else {
                if (this.attackCoroutine != null) {
                    StopCoroutine(this.attackCoroutine);
                }
                this.attackCoroutine = StartCoroutine(this.Attack());
            }
        }
    }
}
