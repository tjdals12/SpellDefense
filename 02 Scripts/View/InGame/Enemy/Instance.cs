using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Enemy {
    using DebuffType = Repository.SkillBook.DebuffType;
    using Model.InGame;

    public class Instance : MonoBehaviour
    {
        [SerializeField]
        Transform _hitPoint;
        public Transform hitPoint { get { return this._hitPoint; } }
        [SerializeField]
        Animator animator;
        [SerializeField]
        float attackAnimationDelaySeconds;

        #region Stats
        string id;
        int enemyId;
        int maxHp;
        int currentHp;
        int attackPower;
        float attackSpeed;
        float moveSpeed;
        float currentMoveSpeed;
        int dropSp;
        #endregion

        bool isAttacking = false;
        Player.Instance playerInstance;
        float attackDelay;
        float timeSinceAttack;
        WaitForSeconds attackAnimationDelay;
        Action<string, int> OnAttack;

        WaitForSeconds hideHpBarDelay;
        HpBar hpBar;

        public bool isDead { get; private set; }
        Action<string, int, bool> OnDamage;
        Action<string, DebuffType, int> OnDebuffDamage;
        WaitForSeconds deadAnimationDelay;
        Action<string, int, int> OnDead;

        bool isSimulate;

        Coroutine hideHpBarCoroutine;
        Dictionary<DebuffType, Coroutine> debuffCoroutines = new();
        Coroutine attackCoroutine;
        Coroutine deadCoroutine;

        #region Unity Method
        void Update() {
            if (this.isDead) return;
            this.timeSinceAttack += Time.deltaTime;
            if (this.playerInstance == null) {
                this.Move();
            } else if (this.CanAttack()) {
                this.attackCoroutine = StartCoroutine(this.Attack());
            }
        }
        void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                this.playerInstance = collider.gameObject.GetComponent<Player.Instance>();
            }
        }
        void OnTriggerExit(Collider collider) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                this.isAttacking = false;
                this.playerInstance = null;
            }
        }
        #endregion

        public void Initialize(string id, int level, Enemy enemy, HpBar hpBar, Action<string, int> onAttack, Action<string, int, bool> onDamage, Action<string, DebuffType, int> onDebuffDamage, Action<string, int, int> onDead, bool isSimulate = false) {
            this.id = id;
            this.enemyId = enemy.id;
            this.maxHp = enemy.hp + (enemy.hpPerLevel * (level - 1));
            this.currentHp = this.maxHp;
            this.attackPower = enemy.attackPower + (enemy.attackPowerPerLevel * (level - 1));
            this.attackSpeed = enemy.attackSpeed;
            this.moveSpeed = enemy.moveSpeed;
            this.currentMoveSpeed = enemy.moveSpeed;
            this.dropSp = enemy.dropSp;
            this.attackDelay = 2 / this.attackSpeed;
            if (this.hideHpBarDelay == null) {
                this.hideHpBarDelay = new WaitForSeconds(3f);
            }
            this.hpBar = hpBar;
            this.playerInstance = null;
            this.attackAnimationDelay = new WaitForSeconds(this.attackAnimationDelaySeconds);
            this.OnAttack = onAttack;
            this.isDead = false;
            this.animator.SetBool("isDead", false);
            this.OnDamage = onDamage;
            this.OnDebuffDamage = onDebuffDamage;
            if (this.deadAnimationDelay == null) {
                this.deadAnimationDelay = new WaitForSeconds(1f);
            }
            this.OnDead = onDead;
            this.animator.speed = 1;
            if (this.hideHpBarCoroutine != null) {
                StopCoroutine(this.hideHpBarCoroutine);
                this.hideHpBarCoroutine = null;
            }
            foreach (var debuffCoroutine in debuffCoroutines.Values) {
                if (debuffCoroutine != null) {
                    StopCoroutine(debuffCoroutine);
                }
            }
            this.debuffCoroutines = new();
            if (this.attackCoroutine != null) {
                StopCoroutine(this.attackCoroutine);
            }
            if (deadCoroutine != null) {
                StopCoroutine(this.deadCoroutine);
                this.deadCoroutine = null;
            }
            this.isSimulate = isSimulate;
            if (this.isSimulate) {
                this.hpBar.gameObject.SetActive(false);
            }
        }
        public void TakeDamage(int amount) {
            this.OnDamage?.Invoke(this.id, amount, false);
            if (this.isSimulate) return;
            this.currentHp = Mathf.Clamp(this.currentHp - amount, 0, this.maxHp);
            this.hpBar.gameObject.SetActive(true);
            this.hpBar.UpdateView(this.transform, this.maxHp, this.currentHp);
            if (this.currentHp == 0) {
                if (this.deadCoroutine == null) {
                    this.deadCoroutine = StartCoroutine(this.Dead());
                }
            } else {
                if (this.hideHpBarCoroutine != null) {
                    StopCoroutine(this.hideHpBarCoroutine);
                }
                this.hideHpBarCoroutine = StartCoroutine(this.HideHpBar());
            }
        }
        public void TakeCriticalDamage(int amount) {
            this.OnDamage?.Invoke(this.id, amount, true);
            if (this.isSimulate) return;
            this.currentHp = Mathf.Clamp(this.currentHp - amount, 0, this.maxHp);
            this.hpBar.gameObject.SetActive(true);
            this.hpBar.UpdateView(this.transform, this.maxHp, this.currentHp);
            if (this.currentHp == 0) {
                if (this.deadCoroutine == null) {
                    this.deadCoroutine = StartCoroutine(this.Dead());
                }
            } else {
                if (this.hideHpBarCoroutine != null) {
                    StopCoroutine(this.hideHpBarCoroutine);
                }
                this.hideHpBarCoroutine = StartCoroutine(this.HideHpBar());
            }
        }
        public void TakeDebuffDamage(DebuffType debuffType, int amount) {
            this.OnDebuffDamage?.Invoke(this.id, debuffType, amount);
            if (this.isSimulate) return;
            this.currentHp = Mathf.Clamp(this.currentHp - amount, 0, this.maxHp);
            this.hpBar.gameObject.SetActive(true);
            this.hpBar.UpdateView(this.transform, this.maxHp, this.currentHp);
            if (this.currentHp == 0) {
                if (this.deadCoroutine == null) {
                    this.deadCoroutine = StartCoroutine(this.Dead());
                }
            } else {
                if (this.hideHpBarCoroutine != null) {
                    StopCoroutine(this.hideHpBarCoroutine);
                }
                this.hideHpBarCoroutine = StartCoroutine(this.HideHpBar());
            }
        }
        IEnumerator TakeDebuff(Skill.Debuff debuff) {
            if (debuff.type == DebuffType.Slow) {
                this.currentMoveSpeed = this.moveSpeed * 0.5f;
                yield return new WaitForSeconds(debuff.duration);
                this.currentMoveSpeed = this.moveSpeed;
            } else if (debuff.type == DebuffType.Freeze) {
                this.currentMoveSpeed = 0;
                this.animator.speed = 0;
                yield return new WaitForSeconds(debuff.duration);
                this.currentMoveSpeed = this.moveSpeed;
                this.animator.speed = 1;
            } else if (debuff.type == DebuffType.Poisoned) {
                WaitForSeconds delay = new WaitForSeconds(0.5f);
                int damage = Mathf.RoundToInt(this.maxHp * 0.1f);
                float currentDuration = 0f;
                while (currentDuration < debuff.duration) {
                    yield return delay;
                    currentDuration += 0.5f;
                    this.TakeDebuffDamage(debuff.type, damage);
                }
            } else if (debuff.type == DebuffType.Electrified) {
                this.currentMoveSpeed = this.moveSpeed * 0.5f;
                WaitForSeconds delay = new WaitForSeconds(0.5f);
                int damage = Mathf.RoundToInt(this.maxHp * 0.05f);
                float currentDuration = 0f;
                while (currentDuration < debuff.duration) {
                    yield return delay;
                    currentDuration += 0.5f;
                    this.TakeDebuffDamage(debuff.type, damage);
                }
                this.currentMoveSpeed = this.moveSpeed;
            }
        }
        public void TakeDamage(int amount, Skill.Debuff debuff) {
            if (this.debuffCoroutines.ContainsKey(debuff.type) && this.debuffCoroutines[debuff.type] != null) {
                StopCoroutine(this.debuffCoroutines[debuff.type]);
            }
            this.debuffCoroutines[debuff.type] = StartCoroutine(this.TakeDebuff(debuff));
            this.TakeDamage(amount);
        }

        public void TakeCriticalDamage(int amount, Skill.Debuff debuff) {
            if (this.debuffCoroutines.ContainsKey(debuff.type) && this.debuffCoroutines[debuff.type] != null) {
                StopCoroutine(this.debuffCoroutines[debuff.type]);
            }
            this.debuffCoroutines[debuff.type] = StartCoroutine(this.TakeDebuff(debuff));
            this.TakeCriticalDamage(amount);
        }
        IEnumerator HideHpBar() {
            yield return this.hideHpBarDelay;
            this.hpBar.gameObject.SetActive(false);
        }
        IEnumerator Dead() {
            this.isDead = true;
            this.animator.SetBool("isDead", true);
            this.hpBar.gameObject.SetActive(false);
            if (this.hideHpBarCoroutine != null) {
                StopCoroutine(this.hideHpBarCoroutine);
                this.hideHpBarCoroutine = null;
            }
            foreach (var kv in this.debuffCoroutines) {
                if (kv.Value != null) {
                    StopCoroutine(kv.Value);
                }
            }
            if (this.attackCoroutine != null) {
                StopCoroutine(this.attackCoroutine);
            }
            yield return this.deadAnimationDelay;
            this.OnDead?.Invoke(this.id, this.enemyId, this.dropSp);
        }
        void Move() {
            this.transform.rotation = Quaternion.LookRotation(Vector3.back);
            this.transform.Translate(Vector3.forward * this.currentMoveSpeed * Time.deltaTime);
            if (this.animator.GetBool("isMove") == false) {
                this.animator.SetBool("isMove", true);
            }
        }
        bool CanAttack() {
            if (this.isDead) return false;
            if (this.playerInstance == null) return false;
            if (this.attackDelay > this.timeSinceAttack) return false;
            if (this.isAttacking) return false;
            return true;
        }
        IEnumerator Attack() {
            if (this.animator.GetBool("isMove")) {
                this.animator.SetBool("isMove", false);
            }
            this.isAttacking = true;
            this.animator.SetTrigger("Attack");
            yield return this.attackAnimationDelay;
            while (this.animator.speed != 1) {
                yield return null;
            }
            this.OnAttack?.Invoke(this.id, this.attackPower);
            this.playerInstance.TakeDamage(this.attackPower);
            this.timeSinceAttack = 0;
            this.isAttacking = false;
        }
    }
}
