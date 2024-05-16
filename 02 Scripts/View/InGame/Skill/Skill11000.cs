using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Skill {
    public class Skill11000 : ISkill
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
                int damage = this.GetDamage();
                if (this.isCritical) {
                    this.target.TakeCriticalDamage(damage);
                } else {
                    this.target.TakeDamage(damage);
                }
                this.onSuccess?.Invoke();
                yield return new WaitForSeconds(1f);
                this.gameObject.SetActive(false);
                // Destroy(this.gameObject, 1f);
            }
        }

        public override void Use() {
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
