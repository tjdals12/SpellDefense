using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill12040 : ISkill
    {
        #region Unity Method
        void OnParticleCollision() {
            StartCoroutine(this.Attack());
        }
        #endregion

        IEnumerator Attack() {
            if (this.target == null || this.target.isDead) {
                this.onCancel?.Invoke();
                this.gameObject.SetActive(false);
                // Destroy(this.gameObject);
            } else {
                Debuff debuff = new Debuff(this.skillSpec.debuffType, this.skillSpec.debuffDuration);
                int damage = this.GetDamage();
                if (this.isCritical) {
                    this.target.TakeCriticalDamage(damage, debuff);
                } else {
                    this.target.TakeDamage(damage, debuff);
                }
                this.onSuccess?.Invoke();
                yield return new WaitForSeconds(1f);
                this.gameObject.SetActive(false);
            }
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
            }
        }
    }
}
