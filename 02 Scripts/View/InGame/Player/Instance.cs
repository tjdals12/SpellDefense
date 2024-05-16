using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetKits.ParticleImage;

namespace View.InGame.Player {
    using SkillType = Repository.SkillBook.SkillType;
    using BoardModel = Model.InGame.BoardModel;
    using PlayModel = Model.InGame.PlayModel;
    using PlayerModel = Model.InGame.PlayerModel;
    using InGameController = Controller.InGame.InGameController;
    using IEquippedSkillBook = Model.InGame.IEquippedSkillBook;
    using SpawnedSkillBook = Model.InGame.SpawnedSkillBook;
    using CastingSkill = Model.InGame.CastingSkill;
    using Skill;

    public class Instance : MonoBehaviour
    {
        BoardModel boardModel;
        PlayModel playModel;
        PlayerModel playerModel;
        InGameController inGameController;

        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("Animation")]
        [Space(4)]
        [SerializeField]
        Animator animator;
        [SerializeField]
        float attackAnimationDelaySeconds;

        [Header("HP")]
        [Space(4)]
        [SerializeField]
        HpBar hpBar;

        [Header("Energy Ball")]
        [Space(4)]
        [SerializeField]
        EnergyBallPool energyBallPool;

        [Header("Attackable Area")]
        [Space(4)]
        [SerializeField]
        AttackableArea attackableArea;
        [SerializeField]
        AttackableArea skillAttackableArea;

        [Header("Skill Aim")]
        [Space(4)]
        [SerializeField]
        SkillAim[] skillAims;

        float attackDelay;
        float timeSinceAttack;
        bool isAttacking;
        WaitForSeconds attackAnimationDelay;

        Queue<int> pendingQueue;
        Queue<int> castingQueue;
        WaitForSeconds aimingDelay;
        int currentCastingCount;
        int maxConcurrenteCastingCount;
        Dictionary<int, ISkill> skillBySlot = new();
        Dictionary<int, int> skillIdBySlot = new();

        Coroutine attackCoroutine;
        Coroutine prepareCastingCoroutine;
        Coroutine castingCoroutine;

        #region Unity Method
        void Awake() {
            this.boardModel = GameObject.FindObjectOfType<BoardModel>();
            this.playModel = GameObject.FindObjectOfType<PlayModel>();
            this.playerModel = GameObject.FindObjectOfType<PlayerModel>();
            this.inGameController = GameObject.FindObjectOfType<InGameController>();
        }
        void Start() {
            this.hpBar.Initialize(this.playerModel.maxHp);

            // Attack
            this.attackableArea.Initialize();
            this.attackDelay = 2f / this.playerModel.attackSpeed;
            this.timeSinceAttack = 0f;
            this.attackAnimationDelay = new WaitForSeconds(this.attackAnimationDelaySeconds);

            // Cast
            this.skillAttackableArea.Initialize();
            this.pendingQueue = new();
            if (this.prepareCastingCoroutine == null) {
                this.prepareCastingCoroutine = StartCoroutine(this.PrepareCasting());
            }
            this.castingQueue = new();
            this.aimingDelay = new WaitForSeconds(1f);
            this.currentCastingCount = 0;
            this.maxConcurrenteCastingCount = 3;
            if (this.castingCoroutine == null) {
                this.castingCoroutine = StartCoroutine(this.Casting());
            }
        }
        void Update() {
            this.timeSinceAttack += Time.deltaTime;
            if (this.CanAttack()) {
                Enemy.Instance target = this.attackableArea.GetClosestTarget();
                if (target != null) {
                    this.attackCoroutine = StartCoroutine(this.Attack(target));
                }
            }
        }
        void OnEnable() {
            this.boardModel.OnCastSkillBook += this.OnCastSkillBook;
            this.playerModel.OnDamage += this.OnDamage;
            this.playerModel.OnDead += this.OnDead;
            this.playModel.OnNextWave += this.OnNextWave;
        }
        void OnDisable() {
            this.boardModel.OnCastSkillBook -= this.OnCastSkillBook;
            this.playerModel.OnDamage -= this.OnDamage;
            this.playerModel.OnDead -= this.OnDead;
            this.playModel.OnNextWave -= this.OnNextWave;
        }
        #endregion

        #region Event Listeners
        void OnCastSkillBook(int slot) {
            if (this.playerModel.isDead) return;
            this.pendingQueue.Enqueue(slot);
        }
        void OnDamage() {
            this.hpBar.UpdateView(this.playerModel.currentHp);
        }
        void OnDead() {
            if (this.attackCoroutine != null) {
                StopCoroutine(this.attackCoroutine);
            }
            if (this.prepareCastingCoroutine != null) {
                StopCoroutine(this.prepareCastingCoroutine);
            }
            if (this.castingCoroutine != null) {
                StopCoroutine(this.castingCoroutine);
            }
        }
        void OnNextWave() {
            this.attackableArea.Initialize();
            this.skillAttackableArea.Initialize();
        }
        #endregion

        public void TakeDamage(int amount) {
            this.inGameController.Damage(amount);
        }

        bool CanAttack() {
            if (this.attackDelay > this.timeSinceAttack) return false;
            if (this.isAttacking) return false;
            return true;
        }

        IEnumerator Attack(Enemy.Instance target) {
            this.isAttacking = true;
            this.animator.SetTrigger("Attack");
            yield return this.attackAnimationDelay;
            EnergyBall energyBall = this.energyBallPool.GetObject();
            energyBall.Move(target);
            this.timeSinceAttack = 0;
            this.isAttacking = false;
        }

        IEnumerator PrepareCasting() {
            while (true) {
                while (this.pendingQueue.Count > 0) {
                    if (this.castingQueue.Count >= this.maxConcurrenteCastingCount) break;
                    int slot = this.pendingQueue.Dequeue();
                    this.castingQueue.Enqueue(slot);
                }
                yield return null;
            }
        }

        IEnumerator Casting() {
            WaitForSeconds delay = new WaitForSeconds(0.5f);
            while (true) {
                for (int i = 0; i < this.maxConcurrenteCastingCount; i++) {
                    if (this.skillAttackableArea.hasTarget == false) break;
                    bool isRemain = this.castingQueue.TryDequeue(out int slot);
                    if (isRemain == false) break;
                    StartCoroutine(this.Cast(slot));
                    yield return delay;
                    this.currentCastingCount++;
                }
                while (this.currentCastingCount > 0) {
                    yield return null;
                }
                yield return null;
            }
        }

        IEnumerator Cast(int slot) {
            bool isExists = this.boardModel.usingSlots.TryGetValue(slot, out SpawnedSkillBook spawnedSkillBook);
            if (isExists == false || spawnedSkillBook == null) {
                this.currentCastingCount--;
                yield break;
            }
            IEquippedSkillBook equippedSkillBook = Array.Find(this.boardModel.skillBooks, (e) => e.skillBook.id == spawnedSkillBook.id);
            CastingSkill castingSkill = spawnedSkillBook.skill;
            Enemy.Instance target = null;
            Vector3 position = Vector3.zero;
            if (castingSkill.skillType == SkillType.Attack) {
                while (true) {
                    target = this.skillAttackableArea.GetRandomTarget();
                    if (target != null && target.isDead == false) {
                        break;
                    }
                    yield return null;
                }
                this.skillAims[slot].Aim(target.transform.position);
            } else if (castingSkill.skillType == SkillType.RangeAttack) {
                position = this.skillAttackableArea.GetRandomPosition();
                this.skillAims[slot].Aim(position);
            }
            yield return this.aimingDelay;
            ISkill skill = null;
            if (this.skillBySlot.ContainsKey(slot)) {
                if (this.skillIdBySlot[slot] == castingSkill.skillSpec.id) {
                    skill = this.skillBySlot[slot];
                    skill.gameObject.transform.position = Vector3.one;
                    skill.gameObject.SetActive(true);
                } else {
                    ISkill prevSkill = this.skillBySlot[slot];
                    Destroy(prevSkill.gameObject);
                    GameObject clone = Instantiate(castingSkill.prefab);
                    skill = clone.GetComponent<ISkill>();
                    this.skillBySlot[slot] = skill;
                    this.skillIdBySlot[slot] = castingSkill.skillSpec.id;
                }
            } else {
                GameObject clone = Instantiate(castingSkill.prefab);
                skill = clone.GetComponent<ISkill>();
                this.skillBySlot[slot] = skill;
                this.skillIdBySlot[slot] = castingSkill.skillSpec.id;
            }
            skill
                .SetTarget(target)
                .SetPosition(position)
                .SetLevel((equippedSkillBook.level - 1) + equippedSkillBook.upgradeLevel)
                .SetAttackPower(this.playerModel.attackPower)
                .SetCriticalRate(this.playerModel.criticalRate)
                .SetCriticalDamage(this.playerModel.criticalDamage)
                .SetSkillSpec(castingSkill.skillSpec)
                .OnSuccess(() => {
                    this.currentCastingCount = 0 >= (this.currentCastingCount - 1) ? 0 : (this.currentCastingCount - 1);
                    this.inGameController.CompleteCastingSkillBook(slot);
                })
                .OnCancel(() => {
                    this.currentCastingCount = 0 >= (this.currentCastingCount - 1) ? 0 : (this.currentCastingCount - 1);
                    this.castingQueue.Enqueue(slot);
                })
                .Use();
            this.soundManager.Cast();
        }
    }
}
