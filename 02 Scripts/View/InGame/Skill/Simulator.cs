using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace View.InGame.Skill {
    using SkillType = Repository.SkillBook.SkillType;
    using DebuffType = Repository.SkillBook.DebuffType;

    public class Simulator : MonoBehaviour
    {
        [SerializeField]
        GameObject enemyPrefab;
        [SerializeField]
        Canvas canvas;
        [SerializeField]
        RectTransform canvasTransform;
        [SerializeField]
        GameObject hpBarPrefab;
        [SerializeField]
        DamageNumber damageNumber;
        [SerializeField]
        DamageNumber criticalDamageNumber;
        [SerializeField]
        DamageNumber poisonDamageNumber;
        [SerializeField]
        DamageNumber electricDamageNumber;
        [SerializeField]
        GameObject skillPrefab;

        Enemy.Instance target;
        Enemy.HpBar hpBar;

        Coroutine recoverCoroutine;

        #region Unity Method
        void Start() {
            GameObject enemyClone = Instantiate(this.enemyPrefab, new Vector3(0, 0, 15), Quaternion.AngleAxis(180, Vector3.up));
            Enemy.Instance enemy = enemyClone.GetComponent<Enemy.Instance>();
            GameObject hpBarClone = Instantiate(this.hpBarPrefab, this.canvas.transform);
            Enemy.HpBar hpBar = hpBarClone.GetComponent<Enemy.HpBar>();

            hpBar.Initialize(callback: () => {});
            this.hpBar = hpBar;

            enemy.Initialize(
                id: "Test",
                level: 1,
                enemy: new Model.InGame.Enemy(
                    id: 0,
                    name: "Test",
                    hp: 500,
                    hpPerLevel: 0,
                    attackPower: 0,
                    attackPowerPerLevel: 0,
                    attackSpeed: 0,
                    moveSpeed: 0,
                    dropSp: 0,
                    prefab: null
                ),
                hpBar: hpBar,
                onAttack: (_, _) => {},
                onDamage: this.OnDamage,
                onDebuffDamage: this.OnDebuffDamage,
                onDead: (_, _, _) => {}
            );
            this.target = enemy;
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                this.Simulate();
            }
        }
        #endregion

        void OnDamage(string id, int damage, bool isCritical) {
            Vector2 viewPoint = Camera.main.WorldToViewportPoint(this.target.transform.position);
            Vector2 position = new Vector2(viewPoint.x * this.canvasTransform.sizeDelta.x, viewPoint.y * this.canvasTransform.sizeDelta.y);
            if (isCritical) {
                this.criticalDamageNumber.SpawnGUI(this.canvasTransform, position, damage);
            } else {
                this.damageNumber.SpawnGUI(this.canvasTransform, position, damage);
            }
        }

        void OnDebuffDamage(string id, DebuffType debuffType, int damage) {
            Vector2 viewPoint = Camera.main.WorldToViewportPoint(this.target.transform.position);
            Vector2 position = new Vector2(viewPoint.x * this.canvasTransform.sizeDelta.x, viewPoint.y * this.canvasTransform.sizeDelta.y);
            DamageNumber debuffDamageNumber = this.damageNumber;
            switch (debuffType) {
                case DebuffType.Poisoned:
                    debuffDamageNumber = this.poisonDamageNumber;
                    break;
                case DebuffType.Electrified:
                    debuffDamageNumber = this.electricDamageNumber;
                    break;
            }
            debuffDamageNumber.SpawnGUI(this.canvasTransform, position, damage);
        }

        void Simulate() {
            GameObject clone = Instantiate(this.skillPrefab, Vector3.zero, Quaternion.identity);
            ISkill skill = clone.GetComponent<ISkill>();
            if (skill == null) {
                Debug.LogWarning("Skill Component is not availiable");
            } else {
                skill
                    .SetLevel(1)
                    .SetAttackPower(100)
                    .SetCriticalRate(50)
                    .SetCriticalDamage(100)
                    .SetTarget(this.target)
                    .SetPosition(this.target.transform.position - Vector3.back * 5f)
                    .SetSkillSpec(
                        new Model.InGame.CastingSkillSpec(
                            id: 0,
                            coolTime: 10,
                            duration: 3f,
                            targetCount: 1,
                            damage: 0.1f,
                            damagePerLevel: 0.1f,
                            hitCount: 1,
                            delayPerHit: 1f,
                            debuffType: DebuffType.None,
                            debuffDuration: 0f
                        )
                    )
                    .Use();
            }
        }
    }
}
