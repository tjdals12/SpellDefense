using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Player {
    public class AttackableArea : MonoBehaviour
    {
        List<Enemy.Instance> targets;

        public bool hasTarget {
            get => this.targets.Count > 0;
        }

        #region Unity Method
        void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                Enemy.Instance enemyInstance = collider.GetComponent<Enemy.Instance>();
                if (enemyInstance != null) {
                    this.targets.Add(enemyInstance);
                }
            }
        }
        #endregion

        public void Initialize() {
            this.targets = new();
        }

        public Enemy.Instance GetClosestTarget() {
            Enemy.Instance closestTarget = null;
            float closestDistance = 0;
            foreach (var target in this.targets) {
                if (target == null || target.isDead) continue;
                if (closestTarget == null) {
                    closestTarget = target;
                    closestDistance = Vector3.Distance(target.transform.position, Vector3.one);
                } else {
                    float distance = Vector3.Distance(target.transform.position, Vector3.one);
                    if (closestDistance > distance) {
                        closestTarget = target;
                        closestDistance = distance;
                    }
                }
            }
            return closestTarget;
        }

        public Enemy.Instance GetRandomTarget() {
            Enemy.Instance target = null;
            if (this.targets.Count > 0) {
                target = this.targets[Random.Range(0, this.targets.Count)];
            }
            return target;
        }

        public Vector3 GetRandomPosition() {
            Vector3 position = new Vector3(
                x: Random.Range(-7.5f, 7.5f),
                y: 0,
                z: Random.Range(5f, 25f)
            );
            return position;
        }
    }
}
